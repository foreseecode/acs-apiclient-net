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
            var clientConfig = new AcsApiClientConfig(ConsumerKey, ConsumerSecret,
                "https://portal2.foreseeresults.com/services/", "***REMOVED***", "***REMOVED***");

            var foreseeClient = new AcsApiClient(clientConfig);
            bool hasToken = HasOAuthToken(foreseeClient);
            Console.WriteLine($"hasOAuthToken: {hasToken}");
        }

        static bool HasOAuthToken(IAcsApiClient client)
        {
            try
            {
                const string url = "https://portal2.foreseeresults.com/services/currentUser/";
                var uri = new Uri(url);
                var oauthtoken = client.GetAuthHeadersForRequestByType(url, "GET");
                if (!oauthtoken.StartsWith("OAuth"))
                {
                    throw new InvalidOperationException("OAuth header is incorrectly formatted");
                }

                var token = oauthtoken.Substring("OAuth ".Length);
                Console.WriteLine($"token={token}");

                return !string.IsNullOrEmpty(token);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
