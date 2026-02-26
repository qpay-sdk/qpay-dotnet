using System.Text.Json.Serialization;

namespace QPay.Models;

/// <summary>
/// Token response from QPay authentication endpoints.
/// </summary>
public sealed class TokenResponse
{
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("refresh_expires_in")]
    public long RefreshExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; } = string.Empty;

    [JsonPropertyName("not-before-policy")]
    public string NotBeforePolicy { get; set; } = string.Empty;

    [JsonPropertyName("session_state")]
    public string SessionState { get; set; } = string.Empty;
}
