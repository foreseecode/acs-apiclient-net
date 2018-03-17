using System;
using System.IO;
using AcsApi;
using Newtonsoft.Json;

namespace acsapiclient.Sample
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ClientConfigs configs = ReadClientConfigs();
            Console.WriteLine($"Athentication on PROD was successful: True or false?{AuthenticateOnPROD(configs)}");
            Console.WriteLine($"Athentication on DEV was successful: True or false?{AuthenticateOnDEV(configs)}");
        }
        
        static bool AuthenticateOnPROD(ClientConfigs configs)
        {
            AcsApiClientConfig clientConfig
            = new AcsApiClientConfig(
            configs.ConsumerKey,
            configs.ConsumerSecret,
            configs.UsernameOnProd,
            configs.PasswordOnProd);
            string requestUrl = "https://portal2.foreseeresults.com/currentUser/";
            bool hasToken = HasToken(clientConfig, requestUrl);
            return hasToken;
        }
        
        static bool AuthenticateOnDEV(ClientConfigs configs)
        {
            AcsApiClientConfig clientConfig
            = new AcsApiClientConfig(
            configs.ConsumerKey,
            configs.ConsumerSecret,
            configs.UsernameOnDev,
            configs.PasswordOnDev,
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
        
        public static ClientConfigs ReadClientConfigs()
        {
            string filePath = System.IO.Path.GetFullPath("client_config.json");
            using (StreamReader r = new StreamReader(filePath))
            {
               string json = r.ReadToEnd();
               ClientConfigs configs = JsonConvert.DeserializeObject<ClientConfigs>(json);
               return configs;
            }
        }
        
        public class ClientConfigs
        {
            public string ConsumerKey { get; set; }
            public string ConsumerSecret { get; set; }
            public string UsernameOnProd { get; set; }
            public string PasswordOnProd { get; set; }  
            public string UsernameOnDev { get; set; }
            public string PasswordOnDev { get; set; }
        }
    }
}

