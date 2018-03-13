using System;
using AcsApi;

namespace UnitTests
{
    public class MyClass
    {
        public const string ConsumerKey = "***REMOVED***";
        public const string ConsumerSecret = "***REMOVED***";
        
        //https://services-edge.foresee.com (Prod)
        //https://services-edge-stg.foresee.com (Staging)
        //https://services-edge-qa.foresee.com (QA)
        //https://services-edge-dev.foresee.com (DEV)
        //https://portal2.foreseeresults.com (Prod)
        //https://portal2-stg.foreseeresults.com (Staging)
        //https://portal2-qa.foreseeresults.com (QA)
        //https://portal2-dev-aws.foreseeresults.com (DEV)
        
        static void Main(string[] args)
        {
            /*
            * Scenario: Get token from PROD
            * Expected Result: Successfully get token
            */
            GetToken("***REMOVED***", "***REMOVED***", "https://services-edge.foresee.com", "https://portal2.foreseeresults.com");

             /*
            * Scenario: Get token from DEV
            * Expected Result: Successfully get token
            */
            GetToken("***REMOVED***", "***REMOVED***", "https://services-edge-dev.foresee.com", "https://portal2.foreseeresults.com");
        }

        private static void GetToken(string username, string password, string authenticationBaseUrl, string serviceBaseUrl)
        {
            var clientConfig = new AcsApiClientConfig(ConsumerKey, ConsumerSecret, username, password, authenticationBaseUrl, serviceBaseUrl);
            var foreseeClient = new AcsApiClient(clientConfig);
            bool hasToken = HasOAuthToken(foreseeClient, authenticationBaseUrl);
            Console.WriteLine($"{authenticationBaseUrl} hasOAuthToken: {hasToken}");
        }

        static bool HasOAuthToken(IAcsApiClient client, string baseUrl)
        {
            try
            {
                string url = $"{baseUrl}" + "/currentUser/";
                var uri = new Uri(url);
                var oauthtoken = client.GetAuthHeadersForRequestByType(url, "GET");
                Console.WriteLine($"token={oauthtoken}");
                return !string.IsNullOrEmpty(oauthtoken);
            }
            catch (Exception e)
            {
                Console.WriteLine( $"{baseUrl}" + e);
                return false;
            }
        }
    }
}
