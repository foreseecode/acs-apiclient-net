using System;
using Newtonsoft.Json;

namespace AcsApi.Models
{
    [JsonObject]
    public class IdentityProvider
    {
        [JsonProperty("identityProviderId")]
        public string IdentityProviderId { get; set; }

        [JsonProperty("baseAuthRequestUrl")]
        public string BaseAuthenticationRequestUrl { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }
		
        [JsonProperty("clientName")]
        public string ClientName { get; set; }

        [JsonProperty("defaultParams")]
        public string DefaultParameters { get; set; }

        [JsonProperty("defaultRedirectUrl")]
        public string DefaultRedirectUrl { get; set; }
		
        [JsonProperty("ssoEnabled")]
		public bool IsSSOEnabled { get; set; }
    }
}
