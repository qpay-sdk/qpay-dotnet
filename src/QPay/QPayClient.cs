using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using QPay.Models;

namespace QPay;

/// <summary>
/// QPay V2 API client with automatic token management.
/// </summary>
public sealed class QPayClient : IDisposable
{
    private const int TokenBufferSeconds = 30;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
    };

    private readonly QPayConfig _config;
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);

    private string _accessToken = string.Empty;
    private string _refreshToken = string.Empty;
    private long _expiresAt;
    private long _refreshExpiresAt;
    private bool _disposed;

    /// <summary>
    /// Creates a new QPay client with a default HttpClient.
    /// </summary>
    /// <param name="config">QPay configuration.</param>
    public QPayClient(QPayConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        _ownsHttpClient = true;
    }

    /// <summary>
    /// Creates a new QPay client with a provided HttpClient.
    /// </summary>
    /// <param name="config">QPay configuration.</param>
    /// <param name="httpClient">HttpClient instance to use for requests.</param>
    public QPayClient(QPayConfig config, HttpClient httpClient)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = false;
    }

    // --- Auth ---

    /// <summary>
    /// Authenticates with QPay using Basic Auth and returns a new token pair.
    /// POST /v2/auth/token
    /// </summary>
    public async Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var token = await GetTokenRequestAsync(cancellationToken).ConfigureAwait(false);

        await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            StoreToken(token);
        }
        finally
        {
            _tokenLock.Release();
        }

        return token;
    }

    /// <summary>
    /// Uses the current refresh token to obtain a new access token.
    /// POST /v2/auth/refresh
    /// </summary>
    public async Task<TokenResponse> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        string refreshTok;

        await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            refreshTok = _refreshToken;
        }
        finally
        {
            _tokenLock.Release();
        }

        var token = await DoRefreshTokenHttpAsync(refreshTok, cancellationToken).ConfigureAwait(false);

        await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            StoreToken(token);
        }
        finally
        {
            _tokenLock.Release();
        }

        return token;
    }

    // --- Invoice ---

    /// <summary>
    /// Creates a detailed invoice with full options.
    /// POST /v2/invoice
    /// </summary>
    public async Task<InvoiceResponse> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        return await DoRequestAsync<InvoiceResponse>(HttpMethod.Post, "/v2/invoice", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a simple invoice with minimal fields.
    /// POST /v2/invoice
    /// </summary>
    public async Task<InvoiceResponse> CreateSimpleInvoiceAsync(CreateSimpleInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        return await DoRequestAsync<InvoiceResponse>(HttpMethod.Post, "/v2/invoice", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates an invoice with ebarimt (tax) information.
    /// POST /v2/invoice
    /// </summary>
    public async Task<InvoiceResponse> CreateEbarimtInvoiceAsync(CreateEbarimtInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        return await DoRequestAsync<InvoiceResponse>(HttpMethod.Post, "/v2/invoice", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Cancels an existing invoice by ID.
    /// DELETE /v2/invoice/{id}
    /// </summary>
    public async Task CancelInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        await DoRequestAsync(HttpMethod.Delete, $"/v2/invoice/{invoiceId}", null, cancellationToken).ConfigureAwait(false);
    }

    // --- Payment ---

    /// <summary>
    /// Retrieves payment details by payment ID.
    /// GET /v2/payment/{id}
    /// </summary>
    public async Task<PaymentDetail> GetPaymentAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        return await DoRequestAsync<PaymentDetail>(HttpMethod.Get, $"/v2/payment/{paymentId}", null, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Checks if a payment has been made for an invoice.
    /// POST /v2/payment/check
    /// </summary>
    public async Task<PaymentCheckResponse> CheckPaymentAsync(PaymentCheckRequest request, CancellationToken cancellationToken = default)
    {
        return await DoRequestAsync<PaymentCheckResponse>(HttpMethod.Post, "/v2/payment/check", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Returns a list of payments matching the given criteria.
    /// POST /v2/payment/list
    /// </summary>
    public async Task<PaymentListResponse> ListPaymentsAsync(PaymentListRequest request, CancellationToken cancellationToken = default)
    {
        return await DoRequestAsync<PaymentListResponse>(HttpMethod.Post, "/v2/payment/list", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Cancels a payment (card transactions only).
    /// DELETE /v2/payment/cancel/{id}
    /// </summary>
    public async Task CancelPaymentAsync(string paymentId, PaymentCancelRequest? request = null, CancellationToken cancellationToken = default)
    {
        await DoRequestAsync(HttpMethod.Delete, $"/v2/payment/cancel/{paymentId}", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Refunds a payment (card transactions only).
    /// DELETE /v2/payment/refund/{id}
    /// </summary>
    public async Task RefundPaymentAsync(string paymentId, PaymentRefundRequest? request = null, CancellationToken cancellationToken = default)
    {
        await DoRequestAsync(HttpMethod.Delete, $"/v2/payment/refund/{paymentId}", request, cancellationToken).ConfigureAwait(false);
    }

    // --- Ebarimt ---

    /// <summary>
    /// Creates an ebarimt (electronic tax receipt) for a payment.
    /// POST /v2/ebarimt_v3/create
    /// </summary>
    public async Task<EbarimtResponse> CreateEbarimtAsync(CreateEbarimtRequest request, CancellationToken cancellationToken = default)
    {
        return await DoRequestAsync<EbarimtResponse>(HttpMethod.Post, "/v2/ebarimt_v3/create", request, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Cancels an ebarimt by payment ID.
    /// DELETE /v2/ebarimt_v3/{id}
    /// </summary>
    public async Task<EbarimtResponse> CancelEbarimtAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        return await DoRequestAsync<EbarimtResponse>(HttpMethod.Delete, $"/v2/ebarimt_v3/{paymentId}", null, cancellationToken).ConfigureAwait(false);
    }

    // --- Internal: Token Management ---

    private async Task EnsureTokenAsync(CancellationToken cancellationToken)
    {
        await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // Access token still valid
        if (!string.IsNullOrEmpty(_accessToken) && now < _expiresAt - TokenBufferSeconds)
        {
            _tokenLock.Release();
            return;
        }

        // Determine strategy: refresh or full auth
        bool canRefresh = !string.IsNullOrEmpty(_refreshToken) && now < _refreshExpiresAt - TokenBufferSeconds;
        string refreshTok = _refreshToken;
        _tokenLock.Release();

        // Access token expired, try refresh
        if (canRefresh)
        {
            try
            {
                var token = await DoRefreshTokenHttpAsync(refreshTok, cancellationToken).ConfigureAwait(false);
                await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    StoreToken(token);
                }
                finally
                {
                    _tokenLock.Release();
                }
                return;
            }
            catch
            {
                // Refresh failed, fall through to get new token
            }
        }

        // Both expired or no tokens, get new token
        var newToken = await GetTokenRequestAsync(cancellationToken).ConfigureAwait(false);

        await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            StoreToken(newToken);
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private async Task<TokenResponse> DoRefreshTokenHttpAsync(string refreshTok, CancellationToken cancellationToken)
    {
        var url = _config.BaseUrl + "/v2/auth/refresh";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshTok);

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            ThrowQPayException((int)response.StatusCode, responseBody);
        }

        return JsonSerializer.Deserialize<TokenResponse>(responseBody, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize token response");
    }

    private async Task<TokenResponse> GetTokenRequestAsync(CancellationToken cancellationToken)
    {
        return await DoBasicAuthRequestAsync<TokenResponse>(HttpMethod.Post, "/v2/auth/token", cancellationToken).ConfigureAwait(false);
    }

    private void StoreToken(TokenResponse token)
    {
        _accessToken = token.AccessToken;
        _refreshToken = token.RefreshToken;
        _expiresAt = token.ExpiresIn;
        _refreshExpiresAt = token.RefreshExpiresIn;
    }

    // --- Internal: HTTP ---

    private async Task DoRequestAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        await EnsureTokenAsync(cancellationToken).ConfigureAwait(false);

        var url = _config.BaseUrl + path;
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, body.GetType(), JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            ThrowQPayException((int)response.StatusCode, responseBody);
        }
    }

    private async Task<TResult> DoRequestAsync<TResult>(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        await EnsureTokenAsync(cancellationToken).ConfigureAwait(false);

        var url = _config.BaseUrl + path;
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, body.GetType(), JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            ThrowQPayException((int)response.StatusCode, responseBody);
        }

        if (string.IsNullOrEmpty(responseBody))
        {
            throw new InvalidOperationException("Empty response body when a result was expected");
        }

        return JsonSerializer.Deserialize<TResult>(responseBody, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    private async Task<TResult> DoBasicAuthRequestAsync<TResult>(HttpMethod method, string path, CancellationToken cancellationToken)
    {
        var url = _config.BaseUrl + path;
        using var request = new HttpRequestMessage(method, url);

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_config.Username}:{_config.Password}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            ThrowQPayException((int)response.StatusCode, responseBody);
        }

        if (string.IsNullOrEmpty(responseBody))
        {
            throw new InvalidOperationException("Empty response body when a result was expected");
        }

        return JsonSerializer.Deserialize<TResult>(responseBody, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    private static void ThrowQPayException(int statusCode, string responseBody)
    {
        string code = string.Empty;
        string message = string.Empty;

        try
        {
            var errorBody = JsonSerializer.Deserialize<QPayErrorBody>(responseBody, JsonOptions);
            if (errorBody is not null)
            {
                code = errorBody.Error ?? string.Empty;
                message = errorBody.Message ?? string.Empty;
            }
        }
        catch
        {
            // Failed to parse error body, use raw body as message
        }

        if (string.IsNullOrEmpty(code))
        {
            code = ReasonPhrase(statusCode);
        }

        if (string.IsNullOrEmpty(message))
        {
            message = responseBody;
        }

        throw new QPayException(statusCode, code, message, responseBody);
    }

    private static string ReasonPhrase(int statusCode) => statusCode switch
    {
        200 => "OK",
        201 => "Created",
        204 => "No Content",
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        429 => "Too Many Requests",
        500 => "Internal Server Error",
        502 => "Bad Gateway",
        503 => "Service Unavailable",
        _ => $"HTTP {statusCode}",
    };

    // --- Dispose ---

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }

        _tokenLock.Dispose();
    }
}
