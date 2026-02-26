using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Request to create a detailed invoice with full options.
/// POST /v2/invoice
/// </summary>
public sealed class CreateInvoiceRequest
{
    [JsonPropertyName("invoice_code")]
    public string InvoiceCode { get; set; } = string.Empty;

    [JsonPropertyName("sender_invoice_no")]
    public string SenderInvoiceNo { get; set; } = string.Empty;

    [JsonPropertyName("sender_branch_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SenderBranchCode { get; set; }

    [JsonPropertyName("sender_branch_data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SenderBranchData? SenderBranchData { get; set; }

    [JsonPropertyName("sender_staff_data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SenderStaffData? SenderStaffData { get; set; }

    [JsonPropertyName("sender_staff_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SenderStaffCode { get; set; }

    [JsonPropertyName("invoice_receiver_code")]
    public string InvoiceReceiverCode { get; set; } = string.Empty;

    [JsonPropertyName("invoice_receiver_data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public InvoiceReceiverData? InvoiceReceiverData { get; set; }

    [JsonPropertyName("invoice_description")]
    public string InvoiceDescription { get; set; } = string.Empty;

    [JsonPropertyName("enable_expiry")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EnableExpiry { get; set; }

    [JsonPropertyName("allow_partial")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowPartial { get; set; }

    [JsonPropertyName("minimum_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? MinimumAmount { get; set; }

    [JsonPropertyName("allow_exceed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowExceed { get; set; }

    [JsonPropertyName("maximum_amount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? MaximumAmount { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; } = string.Empty;

    [JsonPropertyName("sender_terminal_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SenderTerminalCode { get; set; }

    [JsonPropertyName("sender_terminal_data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? SenderTerminalData { get; set; }

    [JsonPropertyName("allow_subscribe")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AllowSubscribe { get; set; }

    [JsonPropertyName("subscription_interval")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SubscriptionInterval { get; set; }

    [JsonPropertyName("subscription_webhook")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SubscriptionWebhook { get; set; }

    [JsonPropertyName("note")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Note { get; set; }

    [JsonPropertyName("transactions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Transaction>? Transactions { get; set; }

    [JsonPropertyName("lines")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<InvoiceLine>? Lines { get; set; }
}
