using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Response from invoice creation endpoints.
/// </summary>
public sealed class InvoiceResponse
{
    [JsonPropertyName("invoice_id")]
    public string InvoiceId { get; set; } = string.Empty;

    [JsonPropertyName("qr_text")]
    public string QRText { get; set; } = string.Empty;

    [JsonPropertyName("qr_image")]
    public string QRImage { get; set; } = string.Empty;

    [JsonPropertyName("qPay_shortUrl")]
    public string QPayShortUrl { get; set; } = string.Empty;

    [JsonPropertyName("urls")]
    public List<Deeplink> Urls { get; set; } = [];
}
