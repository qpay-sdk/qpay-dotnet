using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Request to check payment status for an invoice.
/// POST /v2/payment/check
/// </summary>
public sealed class PaymentCheckRequest
{
    [JsonPropertyName("object_type")]
    public string ObjectType { get; set; } = string.Empty;

    [JsonPropertyName("object_id")]
    public string ObjectId { get; set; } = string.Empty;

    [JsonPropertyName("offset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Offset? Offset { get; set; }
}
