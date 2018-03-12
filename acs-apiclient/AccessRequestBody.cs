using System;
using Newtonsoft.Json;

namespace acs_apiclient
{
    [JsonObject]
    public struct AccessRequestBody
    {
        [JsonProperty("consumerKey")]
        public String consumerKey;

        [JsonProperty("consumerSecret")]
        public String consumerSecret;

        [JsonProperty("username")]
        public String username;

        [JsonProperty("password")]
        public String password;

        public AccessRequestBody(String consumerKey, String consumerSecret, String username, String password)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.username = username;
            this.password = password;
        }
    }
}
