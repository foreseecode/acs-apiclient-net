using System;
using System.Collections.Generic;
using AcsApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AcsApi
{
    public class FSRLoginController
    {
        FSRLoginConfiguration configuration;

        FSRLoginDelegate loginDelegate;

        public FSRLoginController(FSRLoginConfiguration configuration, FSRLoginDelegate loginDelegate)
        {
            this.configuration = configuration;
            this.loginDelegate = loginDelegate;
        }

        public void InitializeIdentity(string username)
        {
            var client = new RestClient(configuration.BaseUrl);
            var request = new RestRequest("/idps", Method.GET);

            request.AddHeader("Accept", "application/json");

            request.AddParameter("username", username);
            request.AddParameter("consumer", configuration.Consumer.ToParameterString());

            var response = client.Execute(request);
            var results = JsonConvert.DeserializeObject<JObject>(response.Content);

            var identityProviders = JsonConvert
                .DeserializeObject<List<IdentityProvider>>(results["identityProviders"].ToString());
            
            if (identityProviders.Count == 0)
            {
                loginDelegate.ShouldBeginPasswordFlow();
            }
            else if (identityProviders.Count == 1)
            {
                loginDelegate.ShouldBeginExternalFlow();
            }
            else
            {
                loginDelegate.ShowIdentityProviderSelector(identityProviders);
            }

        }
    }
}
