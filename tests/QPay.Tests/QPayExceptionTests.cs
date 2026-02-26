using Xunit;

namespace QPay.Tests;

public class QPayExceptionTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var ex = new QPayException(404, "INVOICE_NOTFOUND", "Invoice not found", "{\"error\":\"INVOICE_NOTFOUND\"}");

        Assert.Equal(404, ex.StatusCode);
        Assert.Equal("INVOICE_NOTFOUND", ex.Code);
        Assert.Equal("{\"error\":\"INVOICE_NOTFOUND\"}", ex.RawBody);
        Assert.Contains("INVOICE_NOTFOUND", ex.Message);
        Assert.Contains("Invoice not found", ex.Message);
        Assert.Contains("404", ex.Message);
    }

    [Fact]
    public void Message_ContainsCodeMessageAndStatus()
    {
        var ex = new QPayException(400, "INVALID_AMOUNT", "Amount is invalid", "{}");

        Assert.Equal("qpay: INVALID_AMOUNT - Amount is invalid (status 400)", ex.Message);
    }

    [Fact]
    public void IsQPayError_WithQPayException_ReturnsTrue()
    {
        var qpayEx = new QPayException(500, "INTERNAL", "Server error", "{}");

        var result = QPayException.IsQPayError(qpayEx, out var extracted);

        Assert.True(result);
        Assert.NotNull(extracted);
        Assert.Same(qpayEx, extracted);
        Assert.Equal(500, extracted!.StatusCode);
        Assert.Equal("INTERNAL", extracted.Code);
    }

    [Fact]
    public void IsQPayError_WithRegularException_ReturnsFalse()
    {
        var regularEx = new InvalidOperationException("something went wrong");

        var result = QPayException.IsQPayError(regularEx, out var extracted);

        Assert.False(result);
        Assert.Null(extracted);
    }

    [Fact]
    public void IsQPayError_WithNull_ReturnsFalse()
    {
        var result = QPayException.IsQPayError(null, out var extracted);

        Assert.False(result);
        Assert.Null(extracted);
    }

    [Fact]
    public void QPayException_IsException()
    {
        var ex = new QPayException(401, "AUTHENTICATION_FAILED", "Bad credentials", "{}");

        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void ErrorCodes_HasExpectedConstants()
    {
        Assert.Equal("AUTHENTICATION_FAILED", ErrorCodes.AuthenticationFailed);
        Assert.Equal("INVOICE_NOTFOUND", ErrorCodes.InvoiceNotFound);
        Assert.Equal("PAYMENT_NOTFOUND", ErrorCodes.PaymentNotFound);
        Assert.Equal("INVALID_AMOUNT", ErrorCodes.InvalidAmount);
        Assert.Equal("PERMISSION_DENIED", ErrorCodes.PermissionDenied);
        Assert.Equal("INVOICE_PAID", ErrorCodes.InvoicePaid);
        Assert.Equal("INVOICE_ALREADY_CANCELED", ErrorCodes.InvoiceAlreadyCanceled);
        Assert.Equal("PAYMENT_ALREADY_CANCELED", ErrorCodes.PaymentAlreadyCanceled);
        Assert.Equal("NO_CREDENDIALS", ErrorCodes.NoCredentials);
    }
}
