namespace QPay;

/// <summary>
/// Configuration for the QPay client.
/// </summary>
public sealed class QPayConfig
{
    /// <summary>
    /// QPay API base URL (e.g., "https://merchant.qpay.mn").
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// QPay merchant username.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// QPay merchant password.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Default invoice code.
    /// </summary>
    public required string InvoiceCode { get; init; }

    /// <summary>
    /// Payment callback URL.
    /// </summary>
    public required string CallbackUrl { get; init; }

    /// <summary>
    /// Loads configuration from environment variables.
    /// Required: QPAY_BASE_URL, QPAY_USERNAME, QPAY_PASSWORD, QPAY_INVOICE_CODE, QPAY_CALLBACK_URL.
    /// </summary>
    /// <returns>A validated <see cref="QPayConfig"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a required environment variable is not set.</exception>
    public static QPayConfig LoadFromEnvironment()
    {
        var baseUrl = GetRequiredEnv("QPAY_BASE_URL");
        var username = GetRequiredEnv("QPAY_USERNAME");
        var password = GetRequiredEnv("QPAY_PASSWORD");
        var invoiceCode = GetRequiredEnv("QPAY_INVOICE_CODE");
        var callbackUrl = GetRequiredEnv("QPAY_CALLBACK_URL");

        return new QPayConfig
        {
            BaseUrl = baseUrl,
            Username = username,
            Password = password,
            InvoiceCode = invoiceCode,
            CallbackUrl = callbackUrl,
        };
    }

    private static string GetRequiredEnv(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Required environment variable {name} is not set");
        }
        return value;
    }
}
