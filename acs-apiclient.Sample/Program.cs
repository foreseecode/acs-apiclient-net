using System;
using AcsApi;

namespace acsapiclient.Sample
{
    class MainClass
    {
        public const string ConsumerKey = "***REMOVED***";
        public const string ConsumerSecret = "***REMOVED***";
        
        public static void Main(string[] args)
        {
            Console.WriteLine($"Athentication on PROD was successful: True or false?{AuthenticateOnPROD()}");
            
            Console.WriteLine($"Athentication on DEV was successful: True or false?{AuthenticateOnDEV()}");
        }
        
        static bool AuthenticateOnPROD()
        {
            AcsApiClientConfig clientConfig
            = new AcsApiClientConfig(
            ConsumerKey,
            ConsumerSecret,
            "***REMOVED***",
            "***REMOVED***");
            string requestUrl = "https://portal2.foreseeresults.com/currentUser/";
            bool hasToken = HasToken(clientConfig, requestUrl);
            return hasToken;
        }
        
        static bool AuthenticateOnDEV()
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
            return hasToken;
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
                Console.WriteLine($"{requestUrl}: " + e);
                return false;
            }
        }
    }
}

