using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Response from payment list endpoint.
/// </summary>
public sealed class PaymentListResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("rows")]
    public List<PaymentListItem> Rows { get; set; } = [];
}

/// <summary>
/// Individual item from a payment list response.
/// </summary>
public sealed class PaymentListItem
{
    [JsonPropertyName("payment_id")]
    public string PaymentId { get; set; } = string.Empty;

    [JsonPropertyName("payment_date")]
    public string PaymentDate { get; set; } = string.Empty;

    [JsonPropertyName("payment_status")]
    public string PaymentStatus { get; set; } = string.Empty;

    [JsonPropertyName("payment_fee")]
    public string PaymentFee { get; set; } = string.Empty;

    [JsonPropertyName("payment_amount")]
    public string PaymentAmount { get; set; } = string.Empty;

    [JsonPropertyName("payment_currency")]
    public string PaymentCurrency { get; set; } = string.Empty;

    [JsonPropertyName("payment_wallet")]
    public string PaymentWallet { get; set; } = string.Empty;

    [JsonPropertyName("payment_name")]
    public string PaymentName { get; set; } = string.Empty;

    [JsonPropertyName("payment_description")]
    public string PaymentDescription { get; set; } = string.Empty;

    [JsonPropertyName("qr_code")]
    public string QRCode { get; set; } = string.Empty;

    [JsonPropertyName("paid_by")]
    public string PaidBy { get; set; } = string.Empty;

    [JsonPropertyName("object_type")]
    public string ObjectType { get; set; } = string.Empty;

    [JsonPropertyName("object_id")]
    public string ObjectId { get; set; } = string.Empty;
}
