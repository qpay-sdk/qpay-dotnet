using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Request to list payments.
/// POST /v2/payment/list
/// </summary>
public sealed class PaymentListRequest
{
    [JsonPropertyName("object_type")]
    public string ObjectType { get; set; } = string.Empty;

    [JsonPropertyName("object_id")]
    public string ObjectId { get; set; } = string.Empty;

    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("end_date")]
    public string EndDate { get; set; } = string.Empty;

    [JsonPropertyName("offset")]
    public Offset Offset { get; set; } = new();
}
