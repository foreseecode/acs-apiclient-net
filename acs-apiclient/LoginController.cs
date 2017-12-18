using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using AcsApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AcsApi
{
    public partial class LoginController: PlatformLoginController
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        LoginConfiguration configuration;

        /// <summary>
        /// The login delegate.
        /// </summary>
        LoginDelegate loginDelegate;

        /// <summary>
        /// Gets and sets the selected identity provider.
        /// </summary>
        /// <value>The selected identity provider.</value>
        internal IdentityProvider selectedIdentityProvider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcsApi.LoginController"/> class.
        /// </summary>
        /// <param name="configuration">A LoginConfiguration.</param>
        /// <param name="loginDelegate">A LoginDelegate, usually the calling class.</param>
        public LoginController(LoginConfiguration configuration, LoginDelegate loginDelegate)
        {
            this.configuration = configuration;
            this.loginDelegate = loginDelegate;
        }

        /// <summary>
        /// Initializes the identity.
        /// </summary>
        /// <param name="username">Username.</param>
        public void InitializeIdentity(string username)
        {
            configuration.Username = username;
            RunOnBackgroundThread(() => {
                var client = new RestClient(configuration.ServicesBaseUrl);
                var request = new RestRequest("/idps", Method.GET);

                request.AddHeader("Accept", "application/json");

                request.AddParameter("username", username);
                request.AddParameter("consumer", configuration.Consumer);

                var response = client.Execute(request);
                if (ReceivedErrorResponseCode(response.StatusCode)) { return; }

                var result = JsonConvert.DeserializeObject<JObject>(response.Content);

                var identityProviders = JsonConvert.DeserializeObject<List<IdentityProvider>>(
                    result["identityProviders"].ToString()
                );

                if (identityProviders.Count == 0)
                {
                    RunOnMainThread(() => loginDelegate.ShouldBeginPasswordFlow());
                }
                else if (identityProviders.Count == 1)
                {
                    RunOnMainThread(() => loginDelegate.ShouldBeginExternalFlow(identityProviders[0]));
                }
                else
                {
                    RunOnMainThread(() => loginDelegate.ShowIdentityProviderSelector(identityProviders));
                }
            });
        }

        public void BeginPasswordFlow(string password)
        {
            configuration.Password = password;
            RunOnBackgroundThread(() => {
                try
                {
                    var apiClient = new AcsApiClient(configuration.ApiClientConfiguration, true);
                    var uri = new Uri($"{ configuration.ServicesBaseUrl }/currentUser");
                    _ = apiClient.GetAuthHeadersForRequestByType(uri.AbsoluteUri, "GET");

                    RunOnMainThread(() => loginDelegate.DidLoginSuccessfully(apiClient));
                }
                catch (AcsApiException exception)
                {
                    RunOnMainThread(() => loginDelegate.DidEncounterError(exception.ErrorCode));
                }
                catch (Exception)
                {
                    RunOnMainThread(() => loginDelegate.DidEncounterError(AcsApiError.CouldNotLogin));
                }
            });
        }

        /// <summary>
        /// Exchanges the intercepted callback code for an Access Token.
        /// </summary>
        /// <param name="code">The intercepted callback code.</param>
        void BeginAccessTokenExchange(string code)
        {
            var client = new RestClient(configuration.Environment.AuthorizationServerBaseUrl());
            var request = new RestRequest("/oauth2/v1/token", Method.POST);

            request.AddHeader("Authorization", configuration.BasicAuthorizationHeader);
            request.AddHeader("Accept", "application/json");

            request.AddParameter("grant_type", "authorization_code", ParameterType.GetOrPost);
            request.AddParameter("redirect_uri", selectedIdentityProvider.DefaultRedirectUrl, ParameterType.GetOrPost);
            request.AddParameter("code", code, ParameterType.GetOrPost);

            var response = client.Execute(request);
            if (ReceivedErrorResponseCode(response.StatusCode)) { return; }

            var result = JsonConvert.DeserializeObject<JObject>(response.Content);

            var accessToken = result["access_token"].ToString();
            if (string.IsNullOrEmpty(accessToken))
            {
                RunOnMainThread(() => loginDelegate.DidEncounterError(AcsApiError.CouldNotExchangeToken));
            }
            BeginOAuthTokenExchange(accessToken);
        }

        /// <summary>
        /// Exchanges the AccessToken for an OAuth1 token and secret, then creates an AcsApiClient using the response.
        /// </summary>
        /// <param name="accessToken">Access token.</param>
        void BeginOAuthTokenExchange(string accessToken)
        {
            var client = new RestClient(configuration.ServicesBaseUrl);
            var request = new RestRequest("/token", Method.POST);

            var tokenExchangeRequestBody = new TokenExchangeRequestBody(
                accessToken,
                configuration.ClientKey,
                configuration.ClientSecret,
                configuration.Consumer
            );

            var jsonBody = JsonConvert.SerializeObject(tokenExchangeRequestBody);
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);

            var response = client.Execute(request);
            if (ReceivedErrorResponseCode(response.StatusCode)) { return; }

			var result = JsonConvert.DeserializeObject<JObject>(response.Content);

            var oauthToken = result["token"].ToString();
            var oauthSecret = result["secret"].ToString();

            if (string.IsNullOrEmpty(oauthToken) || string.IsNullOrEmpty(oauthSecret)) {
                RunOnMainThread(() => loginDelegate.DidEncounterError(AcsApiError.InvalidRequestToken));
                return;
            }

            var acsApiClientConfiguration = new AcsApiClientConfig(
                configuration.ClientKey,
                configuration.ClientSecret,
                configuration.ServicesBaseUrl
            );

            acsApiClientConfiguration.IsSSOClient = true;
            acsApiClientConfiguration.AccessToken = oauthToken;
            acsApiClientConfiguration.AccessTokenSecret = oauthSecret;

            try
            {
                var acsApiClient = new AcsApiClient(acsApiClientConfiguration, true);
                RunOnMainThread(() => loginDelegate.DidLoginSuccessfully(acsApiClient));
            }
            catch (AcsApiException exception)
            {
                RunOnMainThread(() => loginDelegate.DidEncounterError(exception.ErrorCode));
            }
            catch (Exception)
            {
                RunOnMainThread(() => loginDelegate.DidEncounterError(AcsApiError.CouldNotLogin));
            }
        }

        /// <summary>
        /// Processes the status code and informs the LoginDelegate if it is an error code, while terminating the flow.
        /// </summary>
        /// <returns><c>true</c>, if the status code is an error code, <c>false</c> otherwise.</returns>
        /// <param name="statusCode">Status code.</param>
        bool ReceivedErrorResponseCode(HttpStatusCode statusCode)
        {
            var responseCode = (int) statusCode;
            if (responseCode >= 500)
            {
                RunOnMainThread(() => loginDelegate.DidEncounterError(AcsApiError.ServerError));
                return true;
            }
            if (responseCode == 400)
            {
                loginDelegate.DidEncounterError(AcsApiError.BadRequest);
                return true;
            }
            if (responseCode > 400)
            {
                loginDelegate.DidEncounterError(AcsApiError.ClientError);
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Login controller implementation of methods that the platform web view should delegate to.
    /// </summary>
    public partial class LoginController : ExternalFlowDelegate
    {
        /// <summary>
        /// The SSO code returned in the intercepted callback.
        /// </summary>
        string ssoCode;

        /// <summary>
        /// Determines if the next request in the web view should be intercepted.
        /// </summary>
        /// <returns><c>true</c> if the request should be intercepted, <c>false</c> otherwise.</returns>
        /// <param name="urlString">URL string.</param>
        public bool ShouldInterceptRequest(string urlString)
        {
            var requestUri = new Uri(urlString);
            if (requestUri.GetLeftPart(UriPartial.Path).Equals(selectedIdentityProvider.DefaultRedirectUrl))
            {
                var requestParameters = HttpUtility.ParseQueryString(requestUri.Query);
                var state = requestParameters.Get("state");
                var code = requestParameters.Get("code");
                if (!String.IsNullOrEmpty(state) && !String.IsNullOrEmpty(code))
                {
                    if (selectedIdentityProvider.state.Equals(state))
                    {
                        ssoCode = code;
                        return true;
                    }
                    throw new AcsApiException(AcsApiError.StateMismatch);
                }
                throw new AcsApiException(AcsApiError.InvalidCode);
            }
            return false;
        }

        /// <summary>
        /// Informs the LoginDelegate that the user cancelled the login flow.
        /// </summary>
        public void DidCancelLoginProcess()
        {
            loginDelegate.DidEncounterError(AcsApiError.Interrupted);
        }

        /// <summary>
        /// Begins the Access Token exchange after successfully intercepting the callback and dismissing the web view.
        /// </summary>
        public void DidRetrieveCodeSuccessfully()
        {
            if (string.IsNullOrEmpty(ssoCode))
            {
                loginDelegate.DidEncounterError(AcsApiError.InvalidCode);
            }
            BeginAccessTokenExchange(ssoCode);
        }

        /// <summary>
        /// Passes an AcsApiError to the LoginDelegate.
        /// </summary>
        /// <param name="error">Error.</param>
        public void DidEncounterError(AcsApiError error)
        {
            loginDelegate.DidEncounterError(error);
        }
    }
}
