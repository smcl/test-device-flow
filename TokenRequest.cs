using Newtonsoft.Json;

class TokenRequest
{
    [JsonProperty("client_id")]
    public string ClientId { get; set; }

    [JsonProperty("device_code")]
    public string DeviceCode { get; set; }

    [JsonProperty("grant_type")]
    public string GrantType { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
}
