
using System;
using AcsApi;
using NUnit.Framework;

namespace acsapiclientios_UnitTests
{
    [TestFixture]
    public class Tests
    {
        public const string ConsumerKey = "***REMOVED***";
        public const string ConsumerSecret = "***REMOVED***";
        
        [Test]
        public void AuthenticateOnPROD()
        {
            bool hasToken = HasToken("***REMOVED***", "***REMOVED***");
            Assert.True(hasToken);
        }

        [Test]
        public void AuthenticateOnDEV()
        {
            bool hasToken = HasToken("***REMOVED***", "***REMOVED***", "https://portal2.foreseeresults.com", ForeSeeEnvironment.Dev);
            Assert.True(hasToken);
        }
        
        static bool HasToken(string username, string password, string serviceBaseUrl = "", ForeSeeEnvironment environment = ForeSeeEnvironment.Prod)
        {
            AcsApiClientConfig clientConfig;
            if(!string.IsNullOrEmpty(serviceBaseUrl))
            {
                clientConfig = new AcsApiClientConfig(ConsumerKey, ConsumerSecret, username, password, serviceBaseUrl, environment);
            }
            else
            {
                clientConfig = new AcsApiClientConfig(ConsumerKey, ConsumerSecret, username, password, environment);
            }
            var foreseeClient = new AcsApiClient(clientConfig);
            bool hasToken = HasOAuthToken(foreseeClient, serviceBaseUrl);
            Console.WriteLine($"{environment.AuthServiceUri()} hasOAuthToken: {hasToken}");
            return hasToken;
        }

        static bool HasOAuthToken(IAcsApiClient client, string serviceBaseUrl)
        {
            try
            {
                string currentUserUrl = $"{serviceBaseUrl}" + "/currentUser/";
                var oauthtoken = client.GetAuthHeadersForRequestByType(currentUserUrl, "GET");
                Console.WriteLine($"token={oauthtoken}");
                return !string.IsNullOrEmpty(oauthtoken);
            }
            catch (Exception e)
            {
                Console.WriteLine( $"{serviceBaseUrl}" + e);
                return false;
            }
        }
    }
}
