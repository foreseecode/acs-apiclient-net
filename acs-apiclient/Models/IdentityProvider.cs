using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AcsApi.Models
{
    [JsonObject]
    public class IdentityProvider
    {
        [JsonProperty("identityProviderId")]
        internal string IdentityProviderId { get; set; }

        [JsonProperty("baseAuthRequestUrl")]
        internal string BaseAuthenticationRequestUrl { get; set; }

        [JsonProperty("clientId")]
        internal string ClientId { get; set; }
		
        [JsonProperty("clientName")]
        public string ClientName { get; set; }

        [JsonProperty("defaultParams")]
        internal string DefaultParameters { get; set; }

        [JsonProperty("defaultRedirectUrl")]
        internal string DefaultRedirectUrl { get; set; }
		
        [JsonProperty("ssoEnabled")]
		internal bool IsSSOEnabled { get; set; }

        [JsonIgnore]
        internal string state { get; private set; }

        [JsonIgnore]
        public string UrlString => $"{ BaseAuthenticationRequestUrl.TrimEnd('/') }" +
            $"?{ Utility.UrlEncode(DefaultParameters) }" +
            $"&redirect_uri={ DefaultRedirectUrl }" +
            $"&state={ state }";

        [OnDeserialized]
        internal void GenerateStateString(StreamingContext context) => state = Guid.NewGuid().ToString();
    }
}
