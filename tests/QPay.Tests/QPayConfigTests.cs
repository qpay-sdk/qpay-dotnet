using Xunit;

namespace QPay.Tests;

public class QPayConfigTests
{
    private static readonly string[] RequiredEnvVars =
    [
        "QPAY_BASE_URL",
        "QPAY_USERNAME",
        "QPAY_PASSWORD",
        "QPAY_INVOICE_CODE",
        "QPAY_CALLBACK_URL",
    ];

    private void ClearEnvVars()
    {
        foreach (var name in RequiredEnvVars)
        {
            Environment.SetEnvironmentVariable(name, null);
        }
    }

    private void SetAllEnvVars()
    {
        Environment.SetEnvironmentVariable("QPAY_BASE_URL", "https://merchant.qpay.mn");
        Environment.SetEnvironmentVariable("QPAY_USERNAME", "testuser");
        Environment.SetEnvironmentVariable("QPAY_PASSWORD", "testpass");
        Environment.SetEnvironmentVariable("QPAY_INVOICE_CODE", "INV001");
        Environment.SetEnvironmentVariable("QPAY_CALLBACK_URL", "https://example.com/callback");
    }

    [Fact]
    public void LoadFromEnvironment_AllSet_ReturnsConfig()
    {
        ClearEnvVars();
        SetAllEnvVars();

        var config = QPayConfig.LoadFromEnvironment();

        Assert.Equal("https://merchant.qpay.mn", config.BaseUrl);
        Assert.Equal("testuser", config.Username);
        Assert.Equal("testpass", config.Password);
        Assert.Equal("INV001", config.InvoiceCode);
        Assert.Equal("https://example.com/callback", config.CallbackUrl);

        ClearEnvVars();
    }

    [Theory]
    [InlineData("QPAY_BASE_URL")]
    [InlineData("QPAY_USERNAME")]
    [InlineData("QPAY_PASSWORD")]
    [InlineData("QPAY_INVOICE_CODE")]
    [InlineData("QPAY_CALLBACK_URL")]
    public void LoadFromEnvironment_MissingVar_ThrowsInvalidOperationException(string missingVar)
    {
        ClearEnvVars();
        SetAllEnvVars();
        Environment.SetEnvironmentVariable(missingVar, null);

        var ex = Assert.Throws<InvalidOperationException>(() => QPayConfig.LoadFromEnvironment());
        Assert.Contains(missingVar, ex.Message);

        ClearEnvVars();
    }

    [Theory]
    [InlineData("QPAY_BASE_URL")]
    [InlineData("QPAY_USERNAME")]
    [InlineData("QPAY_PASSWORD")]
    [InlineData("QPAY_INVOICE_CODE")]
    [InlineData("QPAY_CALLBACK_URL")]
    public void LoadFromEnvironment_EmptyVar_ThrowsInvalidOperationException(string emptyVar)
    {
        ClearEnvVars();
        SetAllEnvVars();
        Environment.SetEnvironmentVariable(emptyVar, "");

        var ex = Assert.Throws<InvalidOperationException>(() => QPayConfig.LoadFromEnvironment());
        Assert.Contains(emptyVar, ex.Message);

        ClearEnvVars();
    }

    [Fact]
    public void Config_RequiredProperties_AreSetViaInit()
    {
        var config = new QPayConfig
        {
            BaseUrl = "https://api.qpay.mn",
            Username = "user",
            Password = "pass",
            InvoiceCode = "CODE",
            CallbackUrl = "https://cb.example.com",
        };

        Assert.Equal("https://api.qpay.mn", config.BaseUrl);
        Assert.Equal("user", config.Username);
        Assert.Equal("pass", config.Password);
        Assert.Equal("CODE", config.InvoiceCode);
        Assert.Equal("https://cb.example.com", config.CallbackUrl);
    }
}
