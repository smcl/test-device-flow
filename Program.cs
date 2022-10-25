using Newtonsoft.Json;
using RestSharp;

const string clientId = "CLIENT_ID";
const string scope = "openid profile offline_access";
const string audience = "REQUIRED_AUDIENCE";
const string tenant = "TENANT_URL";

//-------------------------------------------------------------------------------------------------

// 1. retrieve the url the user needs to visit, authenticate and enter the device code
var deviceCodeResponse = GetDeviceCode(tenant, clientId, scope, audience);
Console.WriteLine($"Go to {deviceCodeResponse.VerificationUri} and enter the code {deviceCodeResponse.UserCode}");

// 2. poll Auth0 to see if the device code was confirmed, then retrieve the original access token + refresh token
TokenResponse tokenResponse;
do
{
    Thread.Sleep(deviceCodeResponse.Interval * 1000);
    tokenResponse = GetToken(tenant, clientId, deviceCodeResponse.DeviceCode, "urn:ietf:params:oauth:grant-type:device_code");
} while (tokenResponse == null);
Console.WriteLine($"Access token: {tokenResponse.AccessToken}");

// 3. use the refresh token to retrieve the access token
Console.WriteLine($"... requesting new access token using refresh token {tokenResponse.RefreshToken}");
var refreshedToken = GetToken(tenant, clientId, deviceCodeResponse.DeviceCode, "refresh_token", tokenResponse.RefreshToken);
Console.WriteLine($"New access token: {refreshedToken.AccessToken}");

//-------------------------------------------------------------------------------------------------
DeviceCodeResponse GetDeviceCode(string tenantUrl, string clientId, string scope, string audience)
{

    using (var client = new RestClient($"https://{tenantUrl}/oauth/device/code"))
    {
        var request = new RestRequest();

        request.Method = Method.Post;
        var requestBody = new DeviceCodeRequest
        {
            ClientId = clientId,
            Scope = scope,
            Audience = audience
        };
        request.AddJsonBody(JsonConvert.SerializeObject(requestBody));

        var response = client.Execute(request);

        return JsonConvert.DeserializeObject<DeviceCodeResponse>(response.Content);
    }
}

TokenResponse GetToken(string tenantUrl, string clientId, string deviceCode, string grantType, string refreshToken = null)
{
    using (var client = new RestClient($"https://{tenantUrl}/oauth/token"))
    {
        var request = new RestRequest();

        request.Method = Method.Post;
        var requestBody = new TokenRequest
        {
            ClientId = clientId,
            DeviceCode = deviceCode,
            GrantType = grantType,
            RefreshToken = refreshToken
        };

        request.AddJsonBody(JsonConvert.SerializeObject(requestBody));

        var response = client.Execute(request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return JsonConvert.DeserializeObject<TokenResponse>(response.Content);
    }
}