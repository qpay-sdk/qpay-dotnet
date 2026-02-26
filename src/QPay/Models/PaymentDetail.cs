using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Detailed payment information.
/// GET /v2/payment/{id}
/// </summary>
public sealed class PaymentDetail
{
    [JsonPropertyName("payment_id")]
    public string PaymentId { get; set; } = string.Empty;

    [JsonPropertyName("payment_status")]
    public string PaymentStatus { get; set; } = string.Empty;

    [JsonPropertyName("payment_fee")]
    public string PaymentFee { get; set; } = string.Empty;

    [JsonPropertyName("payment_amount")]
    public string PaymentAmount { get; set; } = string.Empty;

    [JsonPropertyName("payment_currency")]
    public string PaymentCurrency { get; set; } = string.Empty;

    [JsonPropertyName("payment_date")]
    public string PaymentDate { get; set; } = string.Empty;

    [JsonPropertyName("payment_wallet")]
    public string PaymentWallet { get; set; } = string.Empty;

    [JsonPropertyName("transaction_type")]
    public string TransactionType { get; set; } = string.Empty;

    [JsonPropertyName("object_type")]
    public string ObjectType { get; set; } = string.Empty;

    [JsonPropertyName("object_id")]
    public string ObjectId { get; set; } = string.Empty;

    [JsonPropertyName("next_payment_date")]
    public string? NextPaymentDate { get; set; }

    [JsonPropertyName("next_payment_datetime")]
    public string? NextPaymentDatetime { get; set; }

    [JsonPropertyName("card_transactions")]
    public List<CardTransaction> CardTransactions { get; set; } = [];

    [JsonPropertyName("p2p_transactions")]
    public List<P2PTransaction> P2PTransactions { get; set; } = [];
}
