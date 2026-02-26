using System.Text.Json.Serialization;

namespace QPay;

/// <summary>
/// Exception thrown when the QPay API returns an error response.
/// </summary>
public sealed class QPayException : Exception
{
    /// <summary>
    /// HTTP status code from the QPay API response.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// QPay error code (e.g., "INVOICE_NOTFOUND").
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Raw response body from the QPay API.
    /// </summary>
    public string RawBody { get; }

    public QPayException(int statusCode, string code, string message, string rawBody)
        : base($"qpay: {code} - {message} (status {statusCode})")
    {
        StatusCode = statusCode;
        Code = code;
        RawBody = rawBody;
    }

    /// <summary>
    /// Checks if the given exception is a <see cref="QPayException"/>.
    /// </summary>
    /// <param name="ex">The exception to check.</param>
    /// <param name="qpayException">The <see cref="QPayException"/> if the check succeeds.</param>
    /// <returns>True if the exception is a <see cref="QPayException"/>.</returns>
    public static bool IsQPayError(Exception? ex, out QPayException? qpayException)
    {
        qpayException = ex as QPayException;
        return qpayException is not null;
    }
}

/// <summary>
/// Internal model for deserializing QPay error responses.
/// </summary>
internal sealed class QPayErrorBody
{
    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
