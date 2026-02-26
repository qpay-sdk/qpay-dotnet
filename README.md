# QPay .NET SDK

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

QPay V2 API client for .NET. Provides automatic token management, full async/await support, and covers all QPay V2 endpoints including invoices, payments, and ebarimt (electronic tax receipts).

## Installation

### NuGet Package Manager

```
Install-Package QPay
```

### .NET CLI

```bash
dotnet add package QPay
```

### PackageReference

```xml
<PackageReference Include="QPay" Version="1.0.0" />
```

## Quick Start

```csharp
using QPay;
using QPay.Models;

// Configure from environment variables
var config = QPayConfig.LoadFromEnvironment();
using var client = new QPayClient(config);

// Create a simple invoice
var invoice = await client.CreateSimpleInvoiceAsync(new CreateSimpleInvoiceRequest
{
    InvoiceCode = "YOUR_INVOICE_CODE",
    SenderInvoiceNo = "ORDER-001",
    InvoiceReceiverCode = "terminal",
    InvoiceDescription = "Payment for Order #001",
    Amount = 50000,
    CallbackUrl = "https://yoursite.com/api/qpay/callback",
});

Console.WriteLine($"Invoice ID: {invoice.InvoiceId}");
Console.WriteLine($"QR Image: {invoice.QRImage}");
Console.WriteLine($"Short URL: {invoice.QPayShortUrl}");
```

## Configuration

### Environment Variables

| Variable | Description |
|---|---|
| `QPAY_BASE_URL` | QPay API base URL (e.g., `https://merchant.qpay.mn`) |
| `QPAY_USERNAME` | QPay merchant username |
| `QPAY_PASSWORD` | QPay merchant password |
| `QPAY_INVOICE_CODE` | Default invoice code |
| `QPAY_CALLBACK_URL` | Payment callback URL |

### Load from Environment

```csharp
var config = QPayConfig.LoadFromEnvironment();
```

### Manual Configuration

```csharp
var config = new QPayConfig
{
    BaseUrl = "https://merchant.qpay.mn",
    Username = "your_username",
    Password = "your_password",
    InvoiceCode = "YOUR_INVOICE_CODE",
    CallbackUrl = "https://yoursite.com/api/qpay/callback",
};
```

### Custom HttpClient

You can provide your own `HttpClient` instance (useful for dependency injection or custom handlers):

```csharp
var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
using var client = new QPayClient(config, httpClient);
```

## Usage

### Authentication

The client manages tokens automatically. You do not need to call `GetTokenAsync` or `RefreshTokenAsync` manually -- the client handles token acquisition and refresh before each request.

If you need explicit control:

```csharp
// Get a new token pair
var token = await client.GetTokenAsync();
Console.WriteLine($"Access Token: {token.AccessToken}");
Console.WriteLine($"Expires In: {token.ExpiresIn}");

// Refresh the current token
var refreshed = await client.RefreshTokenAsync();
```

### Create Invoice (Full Options)

```csharp
var invoice = await client.CreateInvoiceAsync(new CreateInvoiceRequest
{
    InvoiceCode = "YOUR_INVOICE_CODE",
    SenderInvoiceNo = "ORDER-001",
    InvoiceReceiverCode = "terminal",
    InvoiceDescription = "Detailed invoice",
    Amount = 100000,
    CallbackUrl = "https://yoursite.com/api/qpay/callback",
    AllowPartial = false,
    AllowExceed = false,
    Note = "Special order",
    Lines =
    [
        new InvoiceLine
        {
            TaxProductCode = "TAX001",
            LineDescription = "Product A",
            LineQuantity = "2",
            LineUnitPrice = "50000",
        }
    ],
    Transactions =
    [
        new Transaction
        {
            Description = "Payment transaction",
            Amount = "100000",
        }
    ],
});
```

### Create Simple Invoice

```csharp
var invoice = await client.CreateSimpleInvoiceAsync(new CreateSimpleInvoiceRequest
{
    InvoiceCode = "YOUR_INVOICE_CODE",
    SenderInvoiceNo = "ORDER-002",
    InvoiceReceiverCode = "terminal",
    InvoiceDescription = "Simple invoice",
    Amount = 25000,
    CallbackUrl = "https://yoursite.com/api/qpay/callback",
});
```

### Create Invoice with Ebarimt

```csharp
var invoice = await client.CreateEbarimtInvoiceAsync(new CreateEbarimtInvoiceRequest
{
    InvoiceCode = "YOUR_INVOICE_CODE",
    SenderInvoiceNo = "ORDER-003",
    InvoiceReceiverCode = "terminal",
    InvoiceDescription = "Invoice with tax",
    TaxType = "1",
    DistrictCode = "34",
    CallbackUrl = "https://yoursite.com/api/qpay/callback",
    Lines =
    [
        new EbarimtInvoiceLine
        {
            TaxProductCode = "TAX001",
            LineDescription = "Taxable Product",
            LineQuantity = "1",
            LineUnitPrice = "75000",
        }
    ],
});
```

### Cancel Invoice

```csharp
await client.CancelInvoiceAsync("invoice-id-here");
```

### Get Payment

```csharp
var payment = await client.GetPaymentAsync("payment-id-here");

Console.WriteLine($"Status: {payment.PaymentStatus}");
Console.WriteLine($"Amount: {payment.PaymentAmount} {payment.PaymentCurrency}");
Console.WriteLine($"Wallet: {payment.PaymentWallet}");

foreach (var p2p in payment.P2PTransactions)
{
    Console.WriteLine($"  Bank: {p2p.AccountBankName}, Amount: {p2p.Amount}");
}

foreach (var card in payment.CardTransactions)
{
    Console.WriteLine($"  Card: {card.CardType}, Amount: {card.Amount}");
}
```

### Check Payment

```csharp
var check = await client.CheckPaymentAsync(new PaymentCheckRequest
{
    ObjectType = "INVOICE",
    ObjectId = "invoice-id-here",
});

Console.WriteLine($"Paid: {check.Count > 0}");
Console.WriteLine($"Total Paid: {check.PaidAmount}");

foreach (var row in check.Rows)
{
    Console.WriteLine($"  Payment {row.PaymentId}: {row.PaymentStatus} ({row.PaymentAmount} {row.PaymentCurrency})");
}
```

### List Payments

```csharp
var list = await client.ListPaymentsAsync(new PaymentListRequest
{
    ObjectType = "INVOICE",
    ObjectId = "invoice-id-here",
    StartDate = "2025-01-01",
    EndDate = "2025-12-31",
    Offset = new Offset { PageNumber = 1, PageLimit = 20 },
});

Console.WriteLine($"Total: {list.Count}");
foreach (var item in list.Rows)
{
    Console.WriteLine($"  {item.PaymentId}: {item.PaymentAmount} {item.PaymentCurrency} - {item.PaymentStatus}");
}
```

### Cancel Payment

Card transactions only:

```csharp
await client.CancelPaymentAsync("payment-id-here", new PaymentCancelRequest
{
    CallbackUrl = "https://yoursite.com/api/qpay/cancel-callback",
    Note = "Cancelled by customer request",
});

// Or without a request body:
await client.CancelPaymentAsync("payment-id-here");
```

### Refund Payment

Card transactions only:

```csharp
await client.RefundPaymentAsync("payment-id-here", new PaymentRefundRequest
{
    CallbackUrl = "https://yoursite.com/api/qpay/refund-callback",
    Note = "Refund for defective item",
});

// Or without a request body:
await client.RefundPaymentAsync("payment-id-here");
```

### Create Ebarimt

```csharp
var ebarimt = await client.CreateEbarimtAsync(new CreateEbarimtRequest
{
    PaymentId = "payment-id-here",
    EbarimtReceiverType = "CITIZEN",
    EbarimtReceiver = "AA12345678",
    DistrictCode = "34",
});

Console.WriteLine($"Ebarimt ID: {ebarimt.Id}");
Console.WriteLine($"Amount: {ebarimt.Amount}");
Console.WriteLine($"VAT: {ebarimt.VatAmount}");
Console.WriteLine($"QR: {ebarimt.EbarimtQRData}");
Console.WriteLine($"Lottery: {ebarimt.EbarimtLottery}");
```

### Cancel Ebarimt

```csharp
var cancelled = await client.CancelEbarimtAsync("payment-id-here");

Console.WriteLine($"Status: {cancelled.BarimtStatus}");
```

## Error Handling

All API errors throw `QPayException` with structured error information:

```csharp
try
{
    await client.CancelInvoiceAsync("nonexistent-id");
}
catch (QPayException ex)
{
    Console.WriteLine($"Status Code: {ex.StatusCode}");   // e.g., 404
    Console.WriteLine($"Error Code: {ex.Code}");           // e.g., "INVOICE_NOTFOUND"
    Console.WriteLine($"Message: {ex.Message}");           // Full error message
    Console.WriteLine($"Raw Body: {ex.RawBody}");          // Raw JSON response
}
```

### Check if an Exception is a QPay Error

```csharp
try
{
    await client.GetPaymentAsync("some-id");
}
catch (Exception ex)
{
    if (QPayException.IsQPayError(ex, out var qpayEx))
    {
        // Handle QPay-specific error
        Console.WriteLine($"QPay error: {qpayEx!.Code}");
    }
    else
    {
        // Handle other errors (network, serialization, etc.)
        throw;
    }
}
```

### Error Code Constants

Use the `ErrorCodes` class for comparing error codes:

```csharp
catch (QPayException ex) when (ex.Code == ErrorCodes.InvoiceNotFound)
{
    Console.WriteLine("Invoice does not exist");
}
catch (QPayException ex) when (ex.Code == ErrorCodes.InvoicePaid)
{
    Console.WriteLine("Invoice has already been paid");
}
catch (QPayException ex) when (ex.Code == ErrorCodes.AuthenticationFailed)
{
    Console.WriteLine("Invalid credentials");
}
```

Available error codes include:

| Constant | Value |
|---|---|
| `ErrorCodes.AuthenticationFailed` | `AUTHENTICATION_FAILED` |
| `ErrorCodes.InvoiceNotFound` | `INVOICE_NOTFOUND` |
| `ErrorCodes.InvoicePaid` | `INVOICE_PAID` |
| `ErrorCodes.InvoiceAlreadyCanceled` | `INVOICE_ALREADY_CANCELED` |
| `ErrorCodes.InvalidAmount` | `INVALID_AMOUNT` |
| `ErrorCodes.PaymentNotFound` | `PAYMENT_NOTFOUND` |
| `ErrorCodes.PaymentNotPaid` | `PAYMENT_NOT_PAID` |
| `ErrorCodes.PaymentAlreadyCanceled` | `PAYMENT_ALREADY_CANCELED` |
| `ErrorCodes.PermissionDenied` | `PERMISSION_DENIED` |
| `ErrorCodes.NoCredentials` | `NO_CREDENDIALS` |
| `ErrorCodes.InvoiceCodeInvalid` | `INVOICE_CODE_INVALID` |
| `ErrorCodes.InvoiceLineRequired` | `INVOICE_LINE_REQUIRED` |
| `ErrorCodes.MerchantNotFound` | `MERCHANT_NOTFOUND` |
| `ErrorCodes.MerchantInactive` | `MERCHANT_INACTIVE` |

See `ErrorCodes.cs` for the full list.

## API Reference

### QPayClient Methods

| Method | Description | HTTP |
|---|---|---|
| `GetTokenAsync()` | Authenticate and get token pair | `POST /v2/auth/token` |
| `RefreshTokenAsync()` | Refresh access token | `POST /v2/auth/refresh` |
| `CreateInvoiceAsync(request)` | Create detailed invoice | `POST /v2/invoice` |
| `CreateSimpleInvoiceAsync(request)` | Create simple invoice | `POST /v2/invoice` |
| `CreateEbarimtInvoiceAsync(request)` | Create invoice with ebarimt | `POST /v2/invoice` |
| `CancelInvoiceAsync(invoiceId)` | Cancel invoice | `DELETE /v2/invoice/{id}` |
| `GetPaymentAsync(paymentId)` | Get payment details | `GET /v2/payment/{id}` |
| `CheckPaymentAsync(request)` | Check payment status | `POST /v2/payment/check` |
| `ListPaymentsAsync(request)` | List payments | `POST /v2/payment/list` |
| `CancelPaymentAsync(paymentId, request?)` | Cancel payment (card) | `DELETE /v2/payment/cancel/{id}` |
| `RefundPaymentAsync(paymentId, request?)` | Refund payment (card) | `DELETE /v2/payment/refund/{id}` |
| `CreateEbarimtAsync(request)` | Create ebarimt | `POST /v2/ebarimt_v3/create` |
| `CancelEbarimtAsync(paymentId)` | Cancel ebarimt | `DELETE /v2/ebarimt_v3/{id}` |

All methods accept an optional `CancellationToken` parameter.

### Models

**Request models** (`QPay.Models` namespace):

- `CreateInvoiceRequest` -- Full invoice creation with all options
- `CreateSimpleInvoiceRequest` -- Minimal invoice creation
- `CreateEbarimtInvoiceRequest` -- Invoice with tax information
- `PaymentCheckRequest` -- Check payment status
- `PaymentListRequest` -- List payments with date range and pagination
- `PaymentCancelRequest` -- Cancel payment (optional body)
- `PaymentRefundRequest` -- Refund payment (optional body)
- `CreateEbarimtRequest` -- Create electronic tax receipt

**Response models** (`QPay.Models` namespace):

- `TokenResponse` -- Authentication token pair
- `InvoiceResponse` -- Invoice with QR code and deeplinks
- `PaymentDetail` -- Full payment information
- `PaymentCheckResponse` -- Payment check result with rows
- `PaymentListResponse` -- Paginated payment list
- `EbarimtResponse` -- Ebarimt receipt details

## Running Tests

```bash
dotnet test
```

## Building

```bash
dotnet build
```

## License

MIT License. See [LICENSE](LICENSE) for details.
