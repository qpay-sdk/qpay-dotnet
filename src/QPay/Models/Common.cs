using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Address information.
/// </summary>
public sealed class Address
{
    [JsonPropertyName("city")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? City { get; set; }

    [JsonPropertyName("district")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? District { get; set; }

    [JsonPropertyName("street")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Street { get; set; }

    [JsonPropertyName("building")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Building { get; set; }

    [JsonPropertyName("address")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AddressLine { get; set; }

    [JsonPropertyName("zipcode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Zipcode { get; set; }

    [JsonPropertyName("longitude")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Longitude { get; set; }

    [JsonPropertyName("latitude")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Latitude { get; set; }
}

/// <summary>
/// Sender branch data for invoice creation.
/// </summary>
public sealed class SenderBranchData
{
    [JsonPropertyName("register")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Register { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; set; }

    [JsonPropertyName("address")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Address? Address { get; set; }
}

/// <summary>
/// Sender staff data for invoice creation.
/// </summary>
public sealed class SenderStaffData
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; set; }
}

/// <summary>
/// Invoice receiver data.
/// </summary>
public sealed class InvoiceReceiverData
{
    [JsonPropertyName("register")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Register { get; set; }

    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }

    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Email { get; set; }

    [JsonPropertyName("phone")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Phone { get; set; }

    [JsonPropertyName("address")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Address? Address { get; set; }
}

/// <summary>
/// Bank account information for transactions.
/// </summary>
public sealed class Account
{
    [JsonPropertyName("account_bank_code")]
    public string AccountBankCode { get; set; } = string.Empty;

    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    [JsonPropertyName("iban_number")]
    public string IBANNumber { get; set; } = string.Empty;

    [JsonPropertyName("account_name")]
    public string AccountName { get; set; } = string.Empty;

    [JsonPropertyName("account_currency")]
    public string AccountCurrency { get; set; } = string.Empty;

    [JsonPropertyName("is_default")]
    public bool IsDefault { get; set; }
}

/// <summary>
/// Transaction information for invoice creation.
/// </summary>
public sealed class Transaction
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonPropertyName("accounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Account>? Accounts { get; set; }
}

/// <summary>
/// Invoice line item.
/// </summary>
public sealed class InvoiceLine
{
    [JsonPropertyName("tax_product_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TaxProductCode { get; set; }

    [JsonPropertyName("line_description")]
    public string LineDescription { get; set; } = string.Empty;

    [JsonPropertyName("line_quantity")]
    public string LineQuantity { get; set; } = string.Empty;

    [JsonPropertyName("line_unit_price")]
    public string LineUnitPrice { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; set; }

    [JsonPropertyName("discounts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaxEntry>? Discounts { get; set; }

    [JsonPropertyName("surcharges")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaxEntry>? Surcharges { get; set; }

    [JsonPropertyName("taxes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaxEntry>? Taxes { get; set; }
}

/// <summary>
/// Ebarimt invoice line item.
/// </summary>
public sealed class EbarimtInvoiceLine
{
    [JsonPropertyName("tax_product_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TaxProductCode { get; set; }

    [JsonPropertyName("line_description")]
    public string LineDescription { get; set; } = string.Empty;

    [JsonPropertyName("barcode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Barcode { get; set; }

    [JsonPropertyName("line_quantity")]
    public string LineQuantity { get; set; } = string.Empty;

    [JsonPropertyName("line_unit_price")]
    public string LineUnitPrice { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; set; }

    [JsonPropertyName("classification_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ClassificationCode { get; set; }

    [JsonPropertyName("taxes")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<TaxEntry>? Taxes { get; set; }
}

/// <summary>
/// Tax, discount, or surcharge entry.
/// </summary>
public sealed class TaxEntry
{
    [JsonPropertyName("tax_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TaxCode { get; set; }

    [JsonPropertyName("discount_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DiscountCode { get; set; }

    [JsonPropertyName("surcharge_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SurchargeCode { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    [JsonPropertyName("note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; set; }
}

/// <summary>
/// Deeplink for payment apps.
/// </summary>
public sealed class Deeplink
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("logo")]
    public string Logo { get; set; } = string.Empty;

    [JsonPropertyName("link")]
    public string Link { get; set; } = string.Empty;
}

/// <summary>
/// Pagination offset for list requests.
/// </summary>
public sealed class Offset
{
    [JsonPropertyName("page_number")]
    public int PageNumber { get; set; }

    [JsonPropertyName("page_limit")]
    public int PageLimit { get; set; }
}

/// <summary>
/// Card transaction details.
/// </summary>
public sealed class CardTransaction
{
    [JsonPropertyName("card_merchant_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CardMerchantCode { get; set; }

    [JsonPropertyName("card_terminal_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CardTerminalCode { get; set; }

    [JsonPropertyName("card_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CardNumber { get; set; }

    [JsonPropertyName("card_type")]
    public string CardType { get; set; } = string.Empty;

    [JsonPropertyName("is_cross_border")]
    public bool IsCrossBorder { get; set; }

    [JsonPropertyName("amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Amount { get; set; }

    [JsonPropertyName("transaction_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TransactionAmount { get; set; }

    [JsonPropertyName("currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Currency { get; set; }

    [JsonPropertyName("transaction_currency")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TransactionCurrency { get; set; }

    [JsonPropertyName("date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Date { get; set; }

    [JsonPropertyName("transaction_date")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TransactionDate { get; set; }

    [JsonPropertyName("status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Status { get; set; }

    [JsonPropertyName("transaction_status")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TransactionStatus { get; set; }

    [JsonPropertyName("settlement_status")]
    public string SettlementStatus { get; set; } = string.Empty;

    [JsonPropertyName("settlement_status_date")]
    public string SettlementStatusDate { get; set; } = string.Empty;
}

/// <summary>
/// P2P (peer-to-peer) transaction details.
/// </summary>
public sealed class P2PTransaction
{
    [JsonPropertyName("transaction_bank_code")]
    public string TransactionBankCode { get; set; } = string.Empty;

    [JsonPropertyName("account_bank_code")]
    public string AccountBankCode { get; set; } = string.Empty;

    [JsonPropertyName("account_bank_name")]
    public string AccountBankName { get; set; } = string.Empty;

    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("settlement_status")]
    public string SettlementStatus { get; set; } = string.Empty;
}

/// <summary>
/// Ebarimt item details.
/// </summary>
public sealed class EbarimtItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("barimt_id")]
    public string BarimtId { get; set; } = string.Empty;

    [JsonPropertyName("merchant_product_code")]
    public string? MerchantProductCode { get; set; }

    [JsonPropertyName("tax_product_code")]
    public string TaxProductCode { get; set; } = string.Empty;

    [JsonPropertyName("bar_code")]
    public string? BarCode { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("unit_price")]
    public string UnitPrice { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public string Quantity { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = string.Empty;

    [JsonPropertyName("city_tax_amount")]
    public string CityTaxAmount { get; set; } = string.Empty;

    [JsonPropertyName("vat_amount")]
    public string VatAmount { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    public string? Note { get; set; }

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
}

/// <summary>
/// Ebarimt history entry.
/// </summary>
public sealed class EbarimtHistory
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("barimt_id")]
    public string BarimtId { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_receiver_type")]
    public string EbarimtReceiverType { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_receiver")]
    public string EbarimtReceiver { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_register_no")]
    public string? EbarimtRegisterNo { get; set; }

    [JsonPropertyName("ebarimt_bill_id")]
    public string EbarimtBillId { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_date")]
    public string EbarimtDate { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_mac_address")]
    public string EbarimtMacAddress { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_internal_code")]
    public string EbarimtInternalCode { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_bill_type")]
    public string EbarimtBillType { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_qr_data")]
    public string EbarimtQRData { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_lottery")]
    public string EbarimtLottery { get; set; } = string.Empty;

    [JsonPropertyName("ebarimt_lottery_msg")]
    public string? EbarimtLotteryMsg { get; set; }

    [JsonPropertyName("ebarimt_error_code")]
    public string? EbarimtErrorCode { get; set; }

    [JsonPropertyName("ebarimt_error_msg")]
    public string? EbarimtErrorMsg { get; set; }

    [JsonPropertyName("ebarimt_response_code")]
    public string? EbarimtResponseCode { get; set; }

    [JsonPropertyName("ebarimt_response_msg")]
    public string? EbarimtResponseMsg { get; set; }

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
}
