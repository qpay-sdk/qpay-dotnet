using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Response from ebarimt creation and cancellation endpoints.
/// </summary>
public sealed class EbarimtResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_by")]
    public string EbarimtBy { get; set; } = string.Empty;

    [JsonPropertyName("g_wallet_id")]
    public string GWalletId { get; set; } = string.Empty;

    [JsonPropertyName("g_wallet_customer_id")]
    public string GWalletCustomerId { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_receiver_type")]
    public string EbarimtReceiverType { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_receiver")]
    public string EbarimtReceiver { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_district_code")]
    public string EbarimtDistrictCode { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_bill_type")]
    public string EbarimtBillType { get; set; } = string.Empty;

    [JsonPropertyName("g_merchant_id")]
    public string GMerchantId { get; set; } = string.Empty;

    [JsonPropertyName("merchant_branch_code")]
    public string MerchantBranchCode { get; set; } = string.Empty;

    [JsonPropertyName("merchant_terminal_code")]
    public string? MerchantTerminalCode { get; set; }

    [JsonPropertyName("merchant_staff_code")]
    public string? MerchantStaffCode { get; set; }

    [JsonPropertyName("merchant_register_no")]
    public string MerchantRegisterNo { get; set; } = string.Empty;

    [JsonPropertyName("g_payment_id")]
    public string GPaymentId { get; set; } = string.Empty;

    [JsonPropertyName("paid_by")]
    public string PaidBy { get; set; } = string.Empty;

    [JsonPropertyName("object_type")]
    public string ObjectType { get; set; } = string.Empty;

    [JsonPropertyName("object_id")]
    public string ObjectId { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonPropertyName("vat_amount")]
    public string VatAmount { get; set; } = string.Empty;

    [JsonPropertyName("city_tax_amount")]
    public string CityTaxAmount { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_qr_data")]
    public string EbarimtQRData { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_lottery")]
    public string EbarimtLottery { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("barimt_status")]
    public string BarimtStatus { get; set; } = string.Empty;

    [JsonPropertyName("barimt_status_date")]
    public string BarimtStatusDate { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_sent_email")]
    public string? EbarimtSentEmail { get; set; }

    [JsonPropertyName("ebarimt_receiver_phone")]
    public string EbarimtReceiverPhone { get; set; } = string.Empty;

    [JsonPropertyName("tax_type")]
    public string TaxType { get; set; } = string.Empty;

    [JsonPropertyName("merchant_tin")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MerchantTIN { get; set; }

    [JsonPropertyName("ebarimt_receipt_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EbarimtReceiptId { get; set; }

    [JsonPropertyName("created_by")]
    public string CreatedBy { get; set; } = string.Empty;

    [JsonPropertyName("created_date")]
    public string CreatedDate { get; set; } = string.Empty;

    [JsonPropertyName("updated_by")]
    public string UpdatedBy { get; set; } = string.Empty;

    [JsonPropertyName("updated_date")]
    public string UpdatedDate { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public bool Status { get; set; }

    [JsonPropertyName("barimt_items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<EbarimtItem>? BarimtItems { get; set; }

    [JsonPropertyName("barimt_transactions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<object>? BarimtTransactions { get; set; }

    [JsonPropertyName("barimt_histories")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<EbarimtHistory>? BarimtHistories { get; set; }
}
