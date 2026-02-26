using System.Net;
using System.Text.Json;
using QPay.Models;
using Xunit;

namespace QPay.Tests;

public class QPayClientTests : IDisposable
{
    private readonly MockHttpHandler _handler;
    private readonly HttpClient _httpClient;
    private readonly QPayConfig _config;
    private readonly QPayClient _client;

    private static readonly string TokenJson = JsonSerializer.Serialize(new
    {
        token_type = "bearer",
        refresh_expires_in = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds(),
        refresh_token = "mock-refresh-token",
        access_token = "mock-access-token",
        expires_in = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds(),
        scope = "default",
        not_before_policy = "0",
        session_state = "session-123",
    });

    public QPayClientTests()
    {
        _handler = new MockHttpHandler();
        _httpClient = new HttpClient(_handler);
        _config = new QPayConfig
        {
            BaseUrl = "https://merchant.qpay.mn",
            Username = "testuser",
            Password = "testpass",
            InvoiceCode = "TEST_INVOICE",
            CallbackUrl = "https://example.com/callback",
        };
        _client = new QPayClient(_config, _httpClient);
    }

    public void Dispose()
    {
        _client.Dispose();
        _httpClient.Dispose();
    }

    // --- Helper ---

    /// <summary>
    /// Enqueues a token response so that EnsureTokenAsync succeeds.
    /// Must be called before any authenticated API call.
    /// </summary>
    private void EnqueueTokenResponse()
    {
        _handler.EnqueueJsonResponse(TokenJson);
    }

    // =============================================
    // Auth Tests
    // =============================================

    [Fact]
    public async Task GetTokenAsync_SendsBasicAuth_ReturnsToken()
    {
        EnqueueTokenResponse();

        var token = await _client.GetTokenAsync();

        Assert.Equal("mock-access-token", token.AccessToken);
        Assert.Equal("mock-refresh-token", token.RefreshToken);
        Assert.Equal("bearer", token.TokenType);

        var req = _handler.CapturedRequests[0];
        Assert.Equal(HttpMethod.Post, req.Method);
        Assert.Equal("https://merchant.qpay.mn/v2/auth/token", req.RequestUri.ToString());
        Assert.Equal("Basic", req.AuthScheme);
        // Basic auth = base64("testuser:testpass")
        var expectedCreds = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("testuser:testpass"));
        Assert.Equal(expectedCreds, req.AuthParameter);
    }

    [Fact]
    public async Task GetTokenAsync_ApiError_ThrowsQPayException()
    {
        _handler.EnqueueErrorResponse(HttpStatusCode.Unauthorized, "AUTHENTICATION_FAILED", "Bad credentials");

        var ex = await Assert.ThrowsAsync<QPayException>(() => _client.GetTokenAsync());

        Assert.Equal(401, ex.StatusCode);
        Assert.Equal("AUTHENTICATION_FAILED", ex.Code);
    }

    [Fact]
    public async Task RefreshTokenAsync_SendsBearerRefreshToken()
    {
        // First get a token so we have a refresh token stored
        EnqueueTokenResponse();
        await _client.GetTokenAsync();

        // Now enqueue response for refresh
        _handler.EnqueueJsonResponse(TokenJson);

        var token = await _client.RefreshTokenAsync();

        Assert.Equal("mock-access-token", token.AccessToken);

        var refreshReq = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Post, refreshReq.Method);
        Assert.Equal("https://merchant.qpay.mn/v2/auth/refresh", refreshReq.RequestUri.ToString());
        Assert.Equal("Bearer", refreshReq.AuthScheme);
        Assert.Equal("mock-refresh-token", refreshReq.AuthParameter);
    }

    // =============================================
    // Invoice Tests
    // =============================================

    [Fact]
    public async Task CreateInvoiceAsync_SendsCorrectRequest()
    {
        EnqueueTokenResponse();

        var invoiceResponse = new
        {
            invoice_id = "inv-123",
            qr_text = "qr-text-data",
            qr_image = "base64-image",
            qPay_shortUrl = "https://qpay.mn/i/abc",
            urls = new[]
            {
                new { name = "Khan Bank", description = "Khan Bank app", logo = "logo.png", link = "khanbank://pay?q=abc" }
            }
        };
        _handler.EnqueueJsonResponse(JsonSerializer.Serialize(invoiceResponse));

        var request = new CreateInvoiceRequest
        {
            InvoiceCode = "TEST_INVOICE",
            SenderInvoiceNo = "INV-001",
            InvoiceReceiverCode = "receiver",
            InvoiceDescription = "Test invoice",
            Amount = 50000,
            CallbackUrl = "https://example.com/callback",
        };

        var result = await _client.CreateInvoiceAsync(request);

        Assert.Equal("inv-123", result.InvoiceId);
        Assert.Equal("qr-text-data", result.QRText);
        Assert.Equal("base64-image", result.QRImage);
        Assert.Equal("https://qpay.mn/i/abc", result.QPayShortUrl);
        Assert.Single(result.Urls);
        Assert.Equal("Khan Bank", result.Urls[0].Name);

        // First request is auth, second is the invoice request
        var invoiceReq = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Post, invoiceReq.Method);
        Assert.Equal("https://merchant.qpay.mn/v2/invoice", invoiceReq.RequestUri.ToString());
        Assert.Equal("Bearer", invoiceReq.AuthScheme);
        Assert.Equal("mock-access-token", invoiceReq.AuthParameter);
        Assert.NotNull(invoiceReq.Body);
        Assert.Contains("\"invoice_code\"", invoiceReq.Body);
        Assert.Contains("TEST_INVOICE", invoiceReq.Body);
    }

    [Fact]
    public async Task CreateSimpleInvoiceAsync_SendsCorrectRequest()
    {
        EnqueueTokenResponse();

        var invoiceResponse = new
        {
            invoice_id = "inv-simple-456",
            qr_text = "simple-qr",
            qr_image = "simple-img",
            qPay_shortUrl = "https://qpay.mn/i/simple",
            urls = Array.Empty<object>()
        };
        _handler.EnqueueJsonResponse(JsonSerializer.Serialize(invoiceResponse));

        var request = new CreateSimpleInvoiceRequest
        {
            InvoiceCode = "TEST_INVOICE",
            SenderInvoiceNo = "SINV-001",
            InvoiceReceiverCode = "receiver",
            InvoiceDescription = "Simple test",
            Amount = 10000,
            CallbackUrl = "https://example.com/callback",
        };

        var result = await _client.CreateSimpleInvoiceAsync(request);

        Assert.Equal("inv-simple-456", result.InvoiceId);

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Post, req.Method);
        Assert.Contains("/v2/invoice", req.RequestUri.ToString());
    }

    [Fact]
    public async Task CreateEbarimtInvoiceAsync_SendsCorrectRequest()
    {
        EnqueueTokenResponse();

        var invoiceResponse = new
        {
            invoice_id = "inv-ebarimt-789",
            qr_text = "eb-qr",
            qr_image = "eb-img",
            qPay_shortUrl = "https://qpay.mn/i/eb",
            urls = Array.Empty<object>()
        };
        _handler.EnqueueJsonResponse(JsonSerializer.Serialize(invoiceResponse));

        var request = new CreateEbarimtInvoiceRequest
        {
            InvoiceCode = "TEST_INVOICE",
            SenderInvoiceNo = "EBI-001",
            InvoiceReceiverCode = "receiver",
            InvoiceDescription = "Ebarimt invoice",
            TaxType = "1",
            DistrictCode = "34",
            CallbackUrl = "https://example.com/callback",
            Lines =
            [
                new EbarimtInvoiceLine
                {
                    TaxProductCode = "TAX001",
                    LineDescription = "Product A",
                    LineQuantity = "1",
                    LineUnitPrice = "50000",
                }
            ],
        };

        var result = await _client.CreateEbarimtInvoiceAsync(request);

        Assert.Equal("inv-ebarimt-789", result.InvoiceId);
    }

    [Fact]
    public async Task CancelInvoiceAsync_SendsDeleteRequest()
    {
        EnqueueTokenResponse();
        _handler.EnqueueResponse(HttpStatusCode.OK, "{}");

        await _client.CancelInvoiceAsync("inv-123");

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Delete, req.Method);
        Assert.Equal("https://merchant.qpay.mn/v2/invoice/inv-123", req.RequestUri.ToString());
    }

    [Fact]
    public async Task CancelInvoiceAsync_NotFound_ThrowsQPayException()
    {
        EnqueueTokenResponse();
        _handler.EnqueueErrorResponse(HttpStatusCode.NotFound, "INVOICE_NOTFOUND", "Invoice not found");

        var ex = await Assert.ThrowsAsync<QPayException>(() => _client.CancelInvoiceAsync("nonexistent"));

        Assert.Equal(404, ex.StatusCode);
        Assert.Equal("INVOICE_NOTFOUND", ex.Code);
    }

    // =============================================
    // Payment Tests
    // =============================================

    [Fact]
    public async Task GetPaymentAsync_ReturnsPaymentDetail()
    {
        EnqueueTokenResponse();

        var paymentJson = JsonSerializer.Serialize(new
        {
            payment_id = "pay-123",
            payment_status = "PAID",
            payment_fee = "100",
            payment_amount = "50000",
            payment_currency = "MNT",
            payment_date = "2025-01-15",
            payment_wallet = "Khan Bank",
            transaction_type = "P2P",
            object_type = "INVOICE",
            object_id = "inv-123",
            card_transactions = Array.Empty<object>(),
            p2p_transactions = new[]
            {
                new
                {
                    transaction_bank_code = "050",
                    account_bank_code = "050",
                    account_bank_name = "Khan Bank",
                    account_number = "1234567890",
                    status = "SUCCESS",
                    amount = "50000",
                    currency = "MNT",
                    settlement_status = "SETTLED"
                }
            }
        });
        _handler.EnqueueJsonResponse(paymentJson);

        var result = await _client.GetPaymentAsync("pay-123");

        Assert.Equal("pay-123", result.PaymentId);
        Assert.Equal("PAID", result.PaymentStatus);
        Assert.Equal("50000", result.PaymentAmount);
        Assert.Equal("MNT", result.PaymentCurrency);
        Assert.Single(result.P2PTransactions);
        Assert.Equal("Khan Bank", result.P2PTransactions[0].AccountBankName);

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Get, req.Method);
        Assert.Equal("https://merchant.qpay.mn/v2/payment/pay-123", req.RequestUri.ToString());
    }

    [Fact]
    public async Task CheckPaymentAsync_ReturnsCheckResponse()
    {
        EnqueueTokenResponse();

        var checkJson = JsonSerializer.Serialize(new
        {
            count = 1,
            paid_amount = 50000.0,
            rows = new[]
            {
                new
                {
                    payment_id = "pay-123",
                    payment_status = "PAID",
                    payment_amount = "50000",
                    trx_fee = "100",
                    payment_currency = "MNT",
                    payment_wallet = "Khan Bank",
                    payment_type = "P2P",
                    card_transactions = Array.Empty<object>(),
                    p2p_transactions = Array.Empty<object>()
                }
            }
        });
        _handler.EnqueueJsonResponse(checkJson);

        var request = new PaymentCheckRequest
        {
            ObjectType = "INVOICE",
            ObjectId = "inv-123",
        };

        var result = await _client.CheckPaymentAsync(request);

        Assert.Equal(1, result.Count);
        Assert.Equal(50000.0, result.PaidAmount);
        Assert.Single(result.Rows);
        Assert.Equal("pay-123", result.Rows[0].PaymentId);

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Post, req.Method);
        Assert.Contains("/v2/payment/check", req.RequestUri.ToString());
    }

    [Fact]
    public async Task ListPaymentsAsync_ReturnsListResponse()
    {
        EnqueueTokenResponse();

        var listJson = JsonSerializer.Serialize(new
        {
            count = 2,
            rows = new[]
            {
                new
                {
                    payment_id = "pay-001",
                    payment_date = "2025-01-10",
                    payment_status = "PAID",
                    payment_fee = "50",
                    payment_amount = "25000",
                    payment_currency = "MNT",
                    payment_wallet = "Khan Bank",
                    payment_name = "Order #1",
                    payment_description = "First payment",
                    qr_code = "qr1",
                    paid_by = "user1",
                    object_type = "INVOICE",
                    object_id = "inv-001"
                },
                new
                {
                    payment_id = "pay-002",
                    payment_date = "2025-01-11",
                    payment_status = "PAID",
                    payment_fee = "80",
                    payment_amount = "40000",
                    payment_currency = "MNT",
                    payment_wallet = "Golomt",
                    payment_name = "Order #2",
                    payment_description = "Second payment",
                    qr_code = "qr2",
                    paid_by = "user2",
                    object_type = "INVOICE",
                    object_id = "inv-002"
                }
            }
        });
        _handler.EnqueueJsonResponse(listJson);

        var request = new PaymentListRequest
        {
            ObjectType = "INVOICE",
            ObjectId = "inv-001",
            StartDate = "2025-01-01",
            EndDate = "2025-01-31",
            Offset = new Offset { PageNumber = 1, PageLimit = 10 },
        };

        var result = await _client.ListPaymentsAsync(request);

        Assert.Equal(2, result.Count);
        Assert.Equal(2, result.Rows.Count);
        Assert.Equal("pay-001", result.Rows[0].PaymentId);
        Assert.Equal("pay-002", result.Rows[1].PaymentId);
    }

    [Fact]
    public async Task CancelPaymentAsync_SendsDeleteRequest()
    {
        EnqueueTokenResponse();
        _handler.EnqueueResponse(HttpStatusCode.OK, "{}");

        await _client.CancelPaymentAsync("pay-123", new PaymentCancelRequest
        {
            CallbackUrl = "https://example.com/cancel-cb",
            Note = "Customer requested cancel",
        });

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Delete, req.Method);
        Assert.Equal("https://merchant.qpay.mn/v2/payment/cancel/pay-123", req.RequestUri.ToString());
        Assert.NotNull(req.Body);
        Assert.Contains("cancel-cb", req.Body);
    }

    [Fact]
    public async Task CancelPaymentAsync_WithoutBody_SendsDeleteRequest()
    {
        EnqueueTokenResponse();
        _handler.EnqueueResponse(HttpStatusCode.OK, "{}");

        await _client.CancelPaymentAsync("pay-123");

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Delete, req.Method);
        Assert.Null(req.Body);
    }

    [Fact]
    public async Task RefundPaymentAsync_SendsDeleteRequest()
    {
        EnqueueTokenResponse();
        _handler.EnqueueResponse(HttpStatusCode.OK, "{}");

        await _client.RefundPaymentAsync("pay-456", new PaymentRefundRequest
        {
            CallbackUrl = "https://example.com/refund-cb",
            Note = "Refund for defective item",
        });

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Delete, req.Method);
        Assert.Equal("https://merchant.qpay.mn/v2/payment/refund/pay-456", req.RequestUri.ToString());
        Assert.NotNull(req.Body);
        Assert.Contains("refund-cb", req.Body);
    }

    [Fact]
    public async Task RefundPaymentAsync_WithoutBody_SendsDeleteRequest()
    {
        EnqueueTokenResponse();
        _handler.EnqueueResponse(HttpStatusCode.OK, "{}");

        await _client.RefundPaymentAsync("pay-456");

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Delete, req.Method);
        Assert.Null(req.Body);
    }

    // =============================================
    // Ebarimt Tests
    // =============================================

    [Fact]
    public async Task CreateEbarimtAsync_ReturnsEbarimtResponse()
    {
        EnqueueTokenResponse();

        var ebarimtJson = JsonSerializer.Serialize(new
        {
            id = "eb-123",
            ebarimt_by = "merchant",
            g_wallet_id = "w-1",
            g_wallet_customer_id = "c-1",
            ebarimt_receiver_type = "CITIZEN",
            ebarimt_receiver = "AA12345678",
            ebarimt_district_code = "34",
            ebarimt_bill_type = "1",
            g_merchant_id = "m-1",
            merchant_branch_code = "branch-01",
            merchant_register_no = "REG001",
            g_payment_id = "pay-123",
            paid_by = "user1",
            object_type = "INVOICE",
            object_id = "inv-123",
            amount = "50000",
            vat_amount = "5000",
            city_tax_amount = "500",
            ebarimt_qr_data = "qr-data",
            ebarimt_lottery = "ABC123",
            barimt_status = "CREATED",
            barimt_status_date = "2025-01-15",
            ebarimt_receiver_phone = "99001122",
            tax_type = "1",
            created_by = "system",
            created_date = "2025-01-15",
            updated_by = "system",
            updated_date = "2025-01-15",
            status = true,
        });
        _handler.EnqueueJsonResponse(ebarimtJson);

        var request = new CreateEbarimtRequest
        {
            PaymentId = "pay-123",
            EbarimtReceiverType = "CITIZEN",
            EbarimtReceiver = "AA12345678",
            DistrictCode = "34",
        };

        var result = await _client.CreateEbarimtAsync(request);

        Assert.Equal("eb-123", result.Id);
        Assert.Equal("CITIZEN", result.EbarimtReceiverType);
        Assert.Equal("50000", result.Amount);
        Assert.Equal("ABC123", result.EbarimtLottery);
        Assert.True(result.Status);

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Post, req.Method);
        Assert.Contains("/v2/ebarimt_v3/create", req.RequestUri.ToString());
    }

    [Fact]
    public async Task CancelEbarimtAsync_SendsDeleteRequest()
    {
        EnqueueTokenResponse();

        var ebarimtJson = JsonSerializer.Serialize(new
        {
            id = "eb-123",
            ebarimt_by = "merchant",
            g_wallet_id = "w-1",
            g_wallet_customer_id = "c-1",
            ebarimt_receiver_type = "CITIZEN",
            ebarimt_receiver = "AA12345678",
            ebarimt_district_code = "34",
            ebarimt_bill_type = "1",
            g_merchant_id = "m-1",
            merchant_branch_code = "branch-01",
            merchant_register_no = "REG001",
            g_payment_id = "pay-123",
            paid_by = "user1",
            object_type = "INVOICE",
            object_id = "inv-123",
            amount = "50000",
            vat_amount = "5000",
            city_tax_amount = "500",
            ebarimt_qr_data = "qr-data",
            ebarimt_lottery = "ABC123",
            barimt_status = "CANCELED",
            barimt_status_date = "2025-01-16",
            ebarimt_receiver_phone = "99001122",
            tax_type = "1",
            created_by = "system",
            created_date = "2025-01-15",
            updated_by = "system",
            updated_date = "2025-01-16",
            status = false,
        });
        _handler.EnqueueJsonResponse(ebarimtJson);

        var result = await _client.CancelEbarimtAsync("pay-123");

        Assert.Equal("eb-123", result.Id);
        Assert.Equal("CANCELED", result.BarimtStatus);
        Assert.False(result.Status);

        var req = _handler.CapturedRequests[1];
        Assert.Equal(HttpMethod.Delete, req.Method);
        Assert.Equal("https://merchant.qpay.mn/v2/ebarimt_v3/pay-123", req.RequestUri.ToString());
    }

    // =============================================
    // Token Auto-Management Tests
    // =============================================

    [Fact]
    public async Task AutoToken_FirstRequest_GetsNewToken()
    {
        // Enqueue token + invoice response
        EnqueueTokenResponse();
        var invoiceJson = JsonSerializer.Serialize(new
        {
            invoice_id = "inv-auto",
            qr_text = "",
            qr_image = "",
            qPay_shortUrl = "",
            urls = Array.Empty<object>()
        });
        _handler.EnqueueJsonResponse(invoiceJson);

        var request = new CreateSimpleInvoiceRequest
        {
            InvoiceCode = "TEST",
            SenderInvoiceNo = "AUTO-001",
            InvoiceReceiverCode = "rcv",
            InvoiceDescription = "Auto token test",
            Amount = 1000,
            CallbackUrl = "https://example.com/cb",
        };

        await _client.CreateSimpleInvoiceAsync(request);

        // Should have 2 requests: token + invoice
        Assert.Equal(2, _handler.CapturedRequests.Count);
        Assert.Equal("Basic", _handler.CapturedRequests[0].AuthScheme);
        Assert.Equal("Bearer", _handler.CapturedRequests[1].AuthScheme);
    }

    [Fact]
    public async Task AutoToken_ReusesCachedToken()
    {
        // Get token first
        EnqueueTokenResponse();
        await _client.GetTokenAsync();

        // Now make two API calls -- should NOT request a new token
        var invoiceJson = JsonSerializer.Serialize(new
        {
            invoice_id = "inv-1",
            qr_text = "",
            qr_image = "",
            qPay_shortUrl = "",
            urls = Array.Empty<object>()
        });
        _handler.EnqueueJsonResponse(invoiceJson);
        _handler.EnqueueJsonResponse(invoiceJson);

        await _client.CreateSimpleInvoiceAsync(new CreateSimpleInvoiceRequest
        {
            InvoiceCode = "TEST",
            SenderInvoiceNo = "C-001",
            InvoiceReceiverCode = "rcv",
            InvoiceDescription = "Cached token 1",
            Amount = 1000,
            CallbackUrl = "https://example.com/cb",
        });

        await _client.CreateSimpleInvoiceAsync(new CreateSimpleInvoiceRequest
        {
            InvoiceCode = "TEST",
            SenderInvoiceNo = "C-002",
            InvoiceReceiverCode = "rcv",
            InvoiceDescription = "Cached token 2",
            Amount = 2000,
            CallbackUrl = "https://example.com/cb",
        });

        // 1 token request + 2 invoice requests = 3 total
        Assert.Equal(3, _handler.CapturedRequests.Count);
        Assert.Equal("Basic", _handler.CapturedRequests[0].AuthScheme);
        Assert.Equal("Bearer", _handler.CapturedRequests[1].AuthScheme);
        Assert.Equal("Bearer", _handler.CapturedRequests[2].AuthScheme);
    }

    // =============================================
    // Error Handling Tests
    // =============================================

    [Fact]
    public async Task ApiError_ParsesQPayErrorBody()
    {
        EnqueueTokenResponse();
        _handler.EnqueueErrorResponse(HttpStatusCode.BadRequest, "INVALID_AMOUNT", "Amount must be positive");

        var ex = await Assert.ThrowsAsync<QPayException>(() =>
            _client.CreateSimpleInvoiceAsync(new CreateSimpleInvoiceRequest
            {
                InvoiceCode = "TEST",
                SenderInvoiceNo = "ERR-001",
                InvoiceReceiverCode = "rcv",
                InvoiceDescription = "Error test",
                Amount = -100,
                CallbackUrl = "https://example.com/cb",
            }));

        Assert.Equal(400, ex.StatusCode);
        Assert.Equal("INVALID_AMOUNT", ex.Code);
        Assert.Contains("Amount must be positive", ex.Message);
    }

    [Fact]
    public async Task ApiError_NonJsonBody_UsesRawBody()
    {
        EnqueueTokenResponse();
        _handler.EnqueueResponse(HttpStatusCode.InternalServerError, "Internal Server Error", "text/plain");

        var ex = await Assert.ThrowsAsync<QPayException>(() =>
            _client.GetPaymentAsync("pay-999"));

        Assert.Equal(500, ex.StatusCode);
        Assert.Equal("Internal Server Error", ex.RawBody);
    }

    [Fact]
    public async Task ApiError_PaymentNotFound()
    {
        EnqueueTokenResponse();
        _handler.EnqueueErrorResponse(HttpStatusCode.NotFound, "PAYMENT_NOTFOUND", "Payment not found");

        var ex = await Assert.ThrowsAsync<QPayException>(() =>
            _client.GetPaymentAsync("nonexistent"));

        Assert.Equal(404, ex.StatusCode);
        Assert.Equal("PAYMENT_NOTFOUND", ex.Code);
    }

    // =============================================
    // Constructor Tests
    // =============================================

    [Fact]
    public void Constructor_NullConfig_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new QPayClient(null!));
    }

    [Fact]
    public void Constructor_NullHttpClient_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new QPayClient(_config, null!));
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        var client = new QPayClient(_config, _httpClient);

        client.Dispose();
        client.Dispose(); // Should not throw
    }

    // =============================================
    // CheckPayment with Offset
    // =============================================

    [Fact]
    public async Task CheckPaymentAsync_WithOffset_SerializesCorrectly()
    {
        EnqueueTokenResponse();

        var checkJson = JsonSerializer.Serialize(new
        {
            count = 0,
            rows = Array.Empty<object>()
        });
        _handler.EnqueueJsonResponse(checkJson);

        var request = new PaymentCheckRequest
        {
            ObjectType = "INVOICE",
            ObjectId = "inv-500",
            Offset = new Offset { PageNumber = 2, PageLimit = 25 },
        };

        await _client.CheckPaymentAsync(request);

        var req = _handler.CapturedRequests[1];
        Assert.NotNull(req.Body);
        Assert.Contains("\"page_number\"", req.Body);
        Assert.Contains("\"page_limit\"", req.Body);
    }
}
