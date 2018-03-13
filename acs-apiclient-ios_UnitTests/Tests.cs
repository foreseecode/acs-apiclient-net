
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
        
        [SetUp]
        public void Setup() { }


        [TearDown]
        public void Tear() { }

        [Test]
        public void AuthenticateOnPROD()
        {
            bool hasToken = HasToken("***REMOVED***", "***REMOVED***", "https://services-edge.foresee.com", "https://portal2.foreseeresults.com");
            Assert.True(hasToken);
        }

        [Test]
        public void AuthenticateOnDEV()
        {
            bool hasToken = HasToken("***REMOVED***", "***REMOVED***", "https://services-edge-dev.foresee.com", "https://portal2.foreseeresults.com");
            Assert.True(hasToken);
        }
        
        static bool HasToken(string username, string password, string authenticationBaseUrl, string serviceBaseUrl)
        {
            var clientConfig = new AcsApiClientConfig(ConsumerKey, ConsumerSecret, username, password, authenticationBaseUrl, serviceBaseUrl);
            var foreseeClient = new AcsApiClient(clientConfig);
            bool hasToken = HasOAuthToken(foreseeClient, serviceBaseUrl);
            Console.WriteLine($"{authenticationBaseUrl} hasOAuthToken: {hasToken}");
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
