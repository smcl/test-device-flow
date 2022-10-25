using Newtonsoft.Json;

class DeviceCodeRequest
{
    [JsonProperty("client_id")]
    public string ClientId { get; set; }
    
    [JsonProperty("scope")]
    public string Scope { get; set; }
    
    [JsonProperty("audience")]
    public string Audience { get; set; }
}
