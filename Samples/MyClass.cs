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
            string baseUrlDoesNotProvideToken = "https://portal2.foreseeresults.com/services/";
            string baseUrlForPROD = "https://services-edge.foresee.com/";
            var clientConfig = new AcsApiClientConfig(ConsumerKey, ConsumerSecret,
                baseUrlForPROD, "***REMOVED***", "***REMOVED***");

            var foreseeClient = new AcsApiClient(clientConfig);
            bool hasToken = HasOAuthToken(foreseeClient);
            Console.WriteLine($"hasOAuthToken: {hasToken}");
            
            //foreseeClient.GetAuthHeadersForRequest();
        }

        static bool HasOAuthToken(IAcsApiClient client)
        {
            try
            {
                const string UrlDoesNotProvideToken = "https://portal2.foreseeresults.com/services/currentUser/";
                const string urlForPROD = "https://services-edge.foresee.com/currentUser/";
                var uri = new Uri(urlForPROD);
                var oauthtoken = client.GetAuthHeadersForRequestByType(urlForPROD, "GET");
                Console.WriteLine($"token={oauthtoken}");
                return !string.IsNullOrEmpty(oauthtoken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
