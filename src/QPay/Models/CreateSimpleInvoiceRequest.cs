using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Request to create a simple invoice with minimal fields.
/// POST /v2/invoice
/// </summary>
public sealed class CreateSimpleInvoiceRequest
{
    [JsonPropertyName("invoice_code")]
    public string InvoiceCode { get; set; } = string.Empty;

    [JsonPropertyName("sender_invoice_no")]
    public string SenderInvoiceNo { get; set; } = string.Empty;

    [JsonPropertyName("invoice_receiver_code")]
    public string InvoiceReceiverCode { get; set; } = string.Empty;

    [JsonPropertyName("invoice_description")]
    public string InvoiceDescription { get; set; } = string.Empty;

    [JsonPropertyName("sender_branch_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SenderBranchCode { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; }

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; } = string.Empty;
}
