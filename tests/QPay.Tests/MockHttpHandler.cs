using System.Net;
using System.Text;

namespace QPay.Tests;

/// <summary>
/// A custom HttpMessageHandler that returns preconfigured responses for testing.
/// Captures sent requests for assertion purposes.
/// </summary>
public sealed class MockHttpHandler : HttpMessageHandler
{
    private readonly Queue<MockResponse> _responses = new();
    private readonly List<CapturedRequest> _capturedRequests = new();

    /// <summary>
    /// All HTTP requests that were sent through this handler.
    /// </summary>
    public IReadOnlyList<CapturedRequest> CapturedRequests => _capturedRequests;

    /// <summary>
    /// Enqueues a response to be returned on the next request.
    /// Responses are returned in FIFO order.
    /// </summary>
    public void EnqueueResponse(HttpStatusCode statusCode, string body, string contentType = "application/json")
    {
        _responses.Enqueue(new MockResponse(statusCode, body, contentType));
    }

    /// <summary>
    /// Enqueues a successful (200 OK) JSON response.
    /// </summary>
    public void EnqueueJsonResponse(string json)
    {
        EnqueueResponse(HttpStatusCode.OK, json);
    }

    /// <summary>
    /// Enqueues an error response with a QPay error body.
    /// </summary>
    public void EnqueueErrorResponse(HttpStatusCode statusCode, string errorCode, string message)
    {
        var body = $"{{\"error\":\"{errorCode}\",\"message\":\"{message}\"}}";
        EnqueueResponse(statusCode, body);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? requestBody = null;
        if (request.Content is not null)
        {
            requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
        }

        _capturedRequests.Add(new CapturedRequest(
            request.Method,
            request.RequestUri!,
            request.Headers.Authorization?.Scheme,
            request.Headers.Authorization?.Parameter,
            requestBody
        ));

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException(
                $"MockHttpHandler: no response enqueued for {request.Method} {request.RequestUri}. " +
                $"Call EnqueueResponse() before making the request.");
        }

        var mock = _responses.Dequeue();

        var response = new HttpResponseMessage(mock.StatusCode)
        {
            Content = new StringContent(mock.Body, Encoding.UTF8, mock.ContentType),
        };

        return response;
    }
}

/// <summary>
/// Represents a preconfigured mock HTTP response.
/// </summary>
public sealed record MockResponse(HttpStatusCode StatusCode, string Body, string ContentType);

/// <summary>
/// Represents a captured HTTP request for assertions.
/// </summary>
public sealed record CapturedRequest(
    HttpMethod Method,
    Uri RequestUri,
    string? AuthScheme,
    string? AuthParameter,
    string? Body
);
