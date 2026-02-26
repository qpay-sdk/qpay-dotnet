using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Request to create an invoice with ebarimt (tax) information.
/// POST /v2/invoice
/// </summary>
public sealed class CreateEbarimtInvoiceRequest
{
    [JsonPropertyName("invoice_code")]
    public string InvoiceCode { get; set; } = string.Empty;

    [JsonPropertyName("sender_invoice_no")]
    public string SenderInvoiceNo { get; set; } = string.Empty;

    [JsonPropertyName("sender_branch_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SenderBranchCode { get; set; }

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

    [JsonPropertyName("tax_type")]
    public string TaxType { get; set; } = string.Empty;

    [JsonPropertyName("district_code")]
    public string DistrictCode { get; set; } = string.Empty;

    [JsonPropertyName("callback_url")]
    public string CallbackUrl { get; set; } = string.Empty;

    [JsonPropertyName("lines")]
    public List<EbarimtInvoiceLine> Lines { get; set; } = [];
}
