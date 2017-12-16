using AcsApi.Models;

namespace AcsApi
{
    public struct LoginConfiguration
    {
        /// <summary>
        /// A Client Key provided by the ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string ClientKey { get; private set; }

        /// <summary>
        /// A Client Secret provided by the ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string ClientSecret { get; private set; }

        /// <summary>
        /// The ForeSee Services base URL.
        /// </summary>
		internal string ServicesBaseUrl { get; private set; }
		
        /// <summary>
        /// The SSO Client Identifier provided by the ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string SSOClientId { get; private set; }

        /// <summary>
        /// The SSO Client Secret provided by the ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string SSOClientSecret { get; private set; }

        /// <summary>
        /// The client's descriptor. This is used to determine the callback url for the SSO flow and is provided by the 
        /// ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string Consumer { get; private set; }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        /// <value>The environment.</value>
        internal LoginEnvironment Environment { get; private set; }

        /// <summary>
        /// Gets the authorization header.
        /// </summary>
        /// <value>The authorization header.</value>
        internal string BasicAuthorizationHeader => "Basic " +
            $"{ Utility.Base64Encode($"{ SSOClientId }:{ SSOClientSecret }") }";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcsApi.LoginConfiguration"/> struct.
        /// </summary>
        /// <param name="clientKey">Client key.</param>
        /// <param name="clientSecret">Client secret.</param>
        /// <param name="servicesBaseUrl">Services base URL.</param>
        /// <param name="ssoClientId">Sso client identifier.</param>
        /// <param name="ssoClientSecret">Sso client secret.</param>
        /// <param name="consumer">Consumer.</param>
        /// <param name="environment">Environment.</param>
        public LoginConfiguration(
            string clientKey,
            string clientSecret,
            string servicesBaseUrl,
            string ssoClientId,
            string ssoClientSecret,
            string consumer,
            LoginEnvironment environment
        ) {
            ClientKey = clientKey;
            ClientSecret = clientSecret;
			ServicesBaseUrl = servicesBaseUrl;
            SSOClientId = ssoClientId;
            SSOClientSecret = ssoClientSecret;
            Consumer = consumer;
            Environment = environment;
        }
    }
}
