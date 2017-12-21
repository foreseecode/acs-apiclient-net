using AcsApi.Models;

namespace AcsApi
{
    public struct LoginConfiguration
    {
        /// <summary>
        /// A Client Key provided by the ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string ConsumerKey { get; private set; }

        /// <summary>
        /// A Client Secret provided by the ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string ConsumerSecret { get; private set; }

        /// <summary>
        /// The ForeSee Services base URL.
        /// </summary>
		internal string ServicesBaseUrl { get; private set; }
		
        /// <summary>
        /// The SSO Client Identifier provided by the ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string OktaClientId { get; private set; }

        /// <summary>
        /// The SSO Client Secret provided by the ForeSee Support Team. This is a unique value for each client.
        /// </summary>
        internal string OktaClientSecret { get; private set; }

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
            $"{ Utility.Base64Encode($"{ OktaClientId }:{ OktaClientSecret }") }";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcsApi.LoginConfiguration"/> struct.
        /// </summary>
        /// <param name="consumerKey">OAuth consumer key.</param>
        /// <param name="consumerSecret">OAuth consumer secret.</param>
        /// <param name="servicesBaseUrl">Services base URL.</param>
        /// <param name="oktaClientId">Okta client identifier.</param>
        /// <param name="oktaClientSecret">Okta client secret.</param>
        /// <param name="consumer">Consumer.</param>
        /// <param name="environment">Environment.</param>
        public LoginConfiguration(
            string consumerKey,
            string consumerSecret,
            string servicesBaseUrl,
            string oktaClientId,
            string oktaClientSecret,
            string consumer,
            LoginEnvironment environment
        ) {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
			ServicesBaseUrl = servicesBaseUrl;
            OktaClientId = oktaClientId;
            OktaClientSecret = oktaClientSecret;
            Consumer = consumer;
            Environment = environment;
        }
    }
}
