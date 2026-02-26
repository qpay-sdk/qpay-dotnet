using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Response from payment check endpoint.
/// </summary>
public sealed class PaymentCheckResponse
{
    [JsonPropertyName("count")]
    public int Count { get; set; }

    [JsonPropertyName("paid_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double PaidAmount { get; set; }

    [JsonPropertyName("rows")]
    public List<PaymentCheckRow> Rows { get; set; } = [];
}

/// <summary>
/// Individual row from a payment check response.
/// </summary>
public sealed class PaymentCheckRow
{
    [JsonPropertyName("payment_id")]
    public string PaymentId { get; set; } = string.Empty;

    [JsonPropertyName("payment_status")]
    public string PaymentStatus { get; set; } = string.Empty;

    [JsonPropertyName("payment_amount")]
    public string PaymentAmount { get; set; } = string.Empty;

    [JsonPropertyName("trx_fee")]
    public string TrxFee { get; set; } = string.Empty;

    [JsonPropertyName("payment_currency")]
    public string PaymentCurrency { get; set; } = string.Empty;

    [JsonPropertyName("payment_wallet")]
    public string PaymentWallet { get; set; } = string.Empty;

    [JsonPropertyName("payment_type")]
    public string PaymentType { get; set; } = string.Empty;

    [JsonPropertyName("next_payment_date")]
    public string? NextPaymentDate { get; set; }

    [JsonPropertyName("next_payment_datetime")]
    public string? NextPaymentDatetime { get; set; }

    [JsonPropertyName("card_transactions")]
    public List<CardTransaction> CardTransactions { get; set; } = [];

    [JsonPropertyName("p2p_transactions")]
    public List<P2PTransaction> P2PTransactions { get; set; } = [];
}
