using System;

namespace AcsApi
{
    public class AcsApiConfig
    {
        public string ConsumerKey;
        public string ConsumerSecret;
        public string ForeseeServicesUri = "https://portal2.foreseeresults.com/services/";

        public string Username;
        public string Password;

        public string AccessToken = null;
        public string AccessSecret = null;

        //"https://portal2.foreseeresults.com/services/client"

        // bbax: override these at your own risk.  Im pretty sure they don't change for Foresee...
        public string RequestTokenAction = "oauth/request_token";
        public string LoginAction = "login";
        public string UserAuthorizationAction = "oauth/user_authorization";
        public string AccessTokenAction = "oauth/access_token";
        public string CallbackAction = "client";

        public AcsApiConfig(string consumerKey, string consumerSecret, string username, string password)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
            Username = username;
            Password = password;
        }
    }
}