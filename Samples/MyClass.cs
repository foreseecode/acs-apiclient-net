using System;
using AcsApi;

namespace UnitTests
{
    public class MyClass
    {
        public const string ConsumerKey = "***REMOVED***";
        public const string ConsumerSecret = "***REMOVED***";

        static void Main(string[] args)
        {
            /*
            * Scenario: Get token from PROD
            * Expected Result: Successfully get token
            */
            string baseUrlForPROD = "https://services-edge.foresee.com/";
            GetToken(baseUrlForPROD, "***REMOVED***", "***REMOVED***");
            
             /*
            * Scenario: Get token from DEV
            * Expected Result: Successfully get token
            */
            string baseUrlForDEV = "https://services-edge-dev.foresee.com/";
            GetToken(baseUrlForDEV, "***REMOVED***", "***REMOVED***");
            
            /*
            * Scenario: Get token from Portal 2
            * Expected Result: AcsApiException(AcsApiError.CouldNotLogin) is thrown
            */
            string baseUrlForPortal2 = "https://portal2.foreseeresults.com/";
            GetToken(baseUrlForPortal2, "1", "1");
        }

        private static void GetToken(string baseUrl, string username, string password)
        {
            var clientConfig = new AcsApiClientConfig(ConsumerKey, ConsumerSecret,
                baseUrl, username, password);
            var foreseeClient = new AcsApiClient(clientConfig);
            bool hasToken = HasOAuthToken(foreseeClient, baseUrl);
            Console.WriteLine($"{baseUrl} hasOAuthToken: {hasToken}");
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
