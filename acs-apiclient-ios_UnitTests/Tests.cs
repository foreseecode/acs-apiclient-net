
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
            AcsApiClientConfig clientConfig 
            = new AcsApiClientConfig(
            ConsumerKey, 
            ConsumerSecret, 
            "***REMOVED***", 
            "***REMOVED***");
            string requestUrl = "https://portal2.foreseeresults.com/currentUser/";
            bool hasToken = HasToken(clientConfig, requestUrl);
            Assert.True(hasToken);
        }

        [Test]
        public void AuthenticateOnDEV()
        {
            AcsApiClientConfig clientConfig 
            = new AcsApiClientConfig(
            ConsumerKey, 
            ConsumerSecret, 
            "***REMOVED***", 
            "***REMOVED***", 
            ForeSeeEnvironment.Dev);
            string requestUrl = "https://portal2-dev-aws.foreseeresults.com/currentUser/";
            bool hasToken = HasToken(clientConfig, requestUrl);
            Assert.True(hasToken);
        }
        
        static bool HasToken(AcsApiClientConfig clientConfig, string requestUrl)
        {
            var client = new AcsApiClient(clientConfig);
            try
            {
                var oauthtoken = client.GetAuthHeadersForRequestByType(requestUrl, "GET");
                return !string.IsNullOrEmpty(oauthtoken);
            }
            catch (Exception e)
            {
                Console.WriteLine( $"{requestUrl}: " + e);
                return false;
            }
        }
    }
}
