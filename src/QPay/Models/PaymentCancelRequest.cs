using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Request body for cancelling a payment.
/// </summary>
public sealed class PaymentCancelRequest
{
    [JsonPropertyName("callback_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CallbackUrl { get; set; }

    [JsonPropertyName("note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; set; }
}
