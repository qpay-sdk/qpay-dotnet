namespace QPay;

/// <summary>
/// QPay V2 API error code constants.
/// </summary>
public static class ErrorCodes
{
    public const string AccountBankDuplicated = "ACCOUNT_BANK_DUPLICATED";
    public const string AccountSelectionInvalid = "ACCOUNT_SELECTION_INVALID";
    public const string AuthenticationFailed = "AUTHENTICATION_FAILED";
    public const string BankAccountNotFound = "BANK_ACCOUNT_NOTFOUND";
    public const string BankMCCAlreadyAdded = "BANK_MCC_ALREADY_ADDED";
    public const string BankMCCNotFound = "BANK_MCC_NOT_FOUND";
    public const string CardTerminalNotFound = "CARD_TERMINAL_NOTFOUND";
    public const string ClientNotFound = "CLIENT_NOTFOUND";
    public const string ClientUsernameDuplicated = "CLIENT_USERNAME_DUPLICATED";
    public const string CustomerDuplicate = "CUSTOMER_DUPLICATE";
    public const string CustomerNotFound = "CUSTOMER_NOTFOUND";
    public const string CustomerRegisterInvalid = "CUSTOMER_REGISTER_INVALID";
    public const string EbarimtCancelNotSupported = "EBARIMT_CANCEL_NOTSUPPERDED";
    public const string EbarimtNotRegistered = "EBARIMT_NOT_REGISTERED";
    public const string EbarimtQRCodeInvalid = "EBARIMT_QR_CODE_INVALID";
    public const string InformNotFound = "INFORM_NOTFOUND";
    public const string InputCodeRegistered = "INPUT_CODE_REGISTERED";
    public const string InputNotFound = "INPUT_NOTFOUND";
    public const string InvalidAmount = "INVALID_AMOUNT";
    public const string InvalidObjectType = "INVALID_OBJECT_TYPE";
    public const string InvoiceAlreadyCanceled = "INVOICE_ALREADY_CANCELED";
    public const string InvoiceCodeInvalid = "INVOICE_CODE_INVALID";
    public const string InvoiceCodeRegistered = "INVOICE_CODE_REGISTERED";
    public const string InvoiceLineRequired = "INVOICE_LINE_REQUIRED";
    public const string InvoiceNotFound = "INVOICE_NOTFOUND";
    public const string InvoicePaid = "INVOICE_PAID";
    public const string InvoiceReceiverDataAddressRequired = "INVOICE_RECEIVER_DATA_ADDRESS_REQUIRED";
    public const string InvoiceReceiverDataEmailRequired = "INVOICE_RECEIVER_DATA_EMAIL_REQUIRED";
    public const string InvoiceReceiverDataPhoneRequired = "INVOICE_RECEIVER_DATA_PHONE_REQUIRED";
    public const string InvoiceReceiverDataRequired = "INVOICE_RECEIVER_DATA_REQUIRED";
    public const string MaxAmountError = "MAX_AMOUNT_ERR";
    public const string MCCNotFound = "MCC_NOTFOUND";
    public const string MerchantAlreadyRegistered = "MERCHANT_ALREADY_REGISTERED";
    public const string MerchantInactive = "MERCHANT_INACTIVE";
    public const string MerchantNotFound = "MERCHANT_NOTFOUND";
    public const string MinAmountError = "MIN_AMOUNT_ERR";
    public const string NoCredentials = "NO_CREDENDIALS";
    public const string ObjectDataError = "OBJECT_DATA_ERROR";
    public const string P2PTerminalNotFound = "P2P_TERMINAL_NOTFOUND";
    public const string PaymentAlreadyCanceled = "PAYMENT_ALREADY_CANCELED";
    public const string PaymentNotPaid = "PAYMENT_NOT_PAID";
    public const string PaymentNotFound = "PAYMENT_NOTFOUND";
    public const string PermissionDenied = "PERMISSION_DENIED";
    public const string QRAccountInactive = "QRACCOUNT_INACTIVE";
    public const string QRAccountNotFound = "QRACCOUNT_NOTFOUND";
    public const string QRCodeNotFound = "QRCODE_NOTFOUND";
    public const string QRCodeUsed = "QRCODE_USED";
    public const string SenderBranchDataRequired = "SENDER_BRANCH_DATA_REQUIRED";
    public const string TaxLineRequired = "TAX_LINE_REQUIRED";
    public const string TaxProductCodeRequired = "TAX_PRODUCT_CODE_REQUIRED";
    public const string TransactionNotApproved = "TRANSACTION_NOT_APPROVED";
    public const string TransactionRequired = "TRANSACTION_REQUIRED";
}
