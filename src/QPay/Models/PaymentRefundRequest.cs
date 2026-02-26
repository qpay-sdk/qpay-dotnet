using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Request body for refunding a payment.
/// </summary>
public sealed class PaymentRefundRequest
{
    [JsonPropertyName("callback_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CallbackUrl { get; set; }

    [JsonPropertyName("note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; set; }
}
