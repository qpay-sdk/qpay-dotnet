using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Request to create an ebarimt (electronic tax receipt) for a payment.
/// POST /v2/ebarimt_v3/create
/// </summary>
public sealed class CreateEbarimtRequest
{
    [JsonPropertyName("payment_id")]
    public string PaymentId { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_receiver_type")]
    public string EbarimtReceiverType { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_receiver")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EbarimtReceiver { get; set; }

    [JsonPropertyName("district_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DistrictCode { get; set; }

    [JsonPropertyName("classification_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClassificationCode { get; set; }
}
