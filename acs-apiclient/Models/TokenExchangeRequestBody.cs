using Newtonsoft.Json;

namespace AcsApi.Models
{
    [JsonObject]
    public struct TokenExchangeRequestBody
    {
        [JsonProperty]
        string audience;

        [JsonProperty("subject_token")]
        string subjectToken;

        [JsonProperty("subject_token_type")]
        string subjectTokenType => "urn:ietf:params:oauth:token-type:access_token";

        [JsonProperty("grant_type")]
        string grantType => "urn:ietf:params:oauth:grant-type:token-exchange";

        [JsonProperty("actor_token")]
        string actorToken;

        [JsonProperty("actor_token_type")]
        string actorTokenType => "urn:ietf:params:oauth:grant_type:client_credentials";

        public TokenExchangeRequestBody(string token, string clientId, string clientSecret, string consumer)
        {
            audience = consumer;
            subjectToken = token;
            actorToken = Utility.Base64Encode($"{ clientId }:{ clientSecret }");
        }
    }
}
