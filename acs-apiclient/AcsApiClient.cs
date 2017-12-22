// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Answers Cloud Services" file="AcsApiClient.cs">
// Copyright (c) 2015 Answers Cloud Services
// </copyright>
// <summary>
// The MIT License (MIT)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </summary>
// 
// --------------------------------------------------------------------------------------------------------------------
// ReSharper disable CheckNamespace

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Web;
using AcsApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuth;
using RestSharp;

//using RestSharp;

namespace AcsApi
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Client for connecting to the Answers Cloud Services (ForeSee in particular) web API in a headless manner. 
    /// You can use this to simplify connecting to the ACS API without requiring a browser or user interaction to grant access to a particular account.
    /// </summary>
    public class AcsApiClient : IAcsApiClient
    {
        /// <summary>
        /// Service layer connection info.
        /// </summary>
        private readonly AcsApiClientConfig serviceConfig;

        /// <summary>
        /// Collection of cookies used by authentication and required for subsequent requests.
        /// </summary>
        private readonly CookieCollection cookies;

        private const string GenericAuthorizationErrorMessage = "Could not authenticate. Please check you are using the correct credentials.";

        /// <summary>
        /// Key name used to refer to the API consumer type (in cookies and other collections.)
        /// </summary>
        private const string ConsumerTypeIdKey = "CONSUMER_TYPE";

        /// <summary>
        /// Type of API consumer that can use this module.
        /// </summary>
        private const string ConsumerTypeValue = "PUBLIC_API_TIER_1";

        /// <summary>
        /// Key name used to refer to the OAuth verifier (in url parameters and other collections.)
        /// </summary>
        private const string OauthVerifierKey = "oauth_verifier";

        /// <summary>
        /// Key name used to refer to the OAuth token (in url parameters and other collections.)
        /// </summary>
        private const string OauthTokenKey = "oauth_token";

        /// <summary>
        /// Key name used to refer to the OAuth token secret (in url parameters and other collections.)
        /// </summary>
        private const string OauthTokenSecretKey = "oauth_token_secret";

        private readonly bool _acsPortal = false;

        /// <summary>
        /// Key name of the session id cookie.
        /// </summary>
        private const string SessionIdKey = "jsessionid";

        public event Action<string> Log;

        private void InvokeLog(string str)
        {
            if (Log != null)
            {
                Log(str);
            }
            else
            {
                Console.WriteLine("INFO [{0}]", str);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AcsApiClient"/> class. 
        /// </summary>
        /// <param name="config"><see cref="AcsApiClient"/> instance containing configuration details.
        /// </param>
        public AcsApiClient(AcsApiClientConfig config, bool isAcsPortal = false)
        {
            if (!config.IsSSOClient 
               && (string.IsNullOrEmpty(config.PortalPassword) || string.IsNullOrEmpty(config.PortalUsername))
            ) {
                throw new AcsApiException(AcsApiError.InvalidCredentials);
            }

            serviceConfig = config;
            cookies = new CookieCollection();

            _acsPortal = isAcsPortal;
        }

        /// <summary>
        /// Get the authorization cookies that are required when issuing a request.
        /// </summary>
        /// <returns></returns>
        public Cookie[] GetAuthCookies()
        {
            if (cookies[SessionIdKey] != null && cookies[ConsumerTypeIdKey] != null)
            {
                return new[] { cookies[SessionIdKey], cookies[ConsumerTypeIdKey] };
            }

            return null;
        }

        public string GetAuthHeadersForRequest(string requestUrl)
        {
            return GetAuthHeadersForRequestByType(requestUrl, "POST");
        }

        /// <summary>
        /// Gets the Authorization Header for the supplied url.
        /// </summary>
        /// <param name="requestUrl">The request url for signing.</param>
        /// <returns>A string that can be used as the value for the "Authorization" header of an HTTP request.</returns>
        public string GetAuthHeadersForRequestByType(string requestUrl, string type)
        {
            if (string.IsNullOrEmpty(serviceConfig.OAuthToken))
            {
                try
                {
                    GetToken();
                }
                catch (AcsApiException)
                {
                    throw;
                }
                catch (Exception)
                {
                    // return non-specific error and destroy call stack
                    throw new Exception(GenericAuthorizationErrorMessage);
                }
            }

            // Creating a new instance directly
            var client = new OAuthRequest
            {
                Method = type,
                Type = OAuthRequestType.ProtectedResource,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = serviceConfig.ConsumerKey,
                ConsumerSecret = serviceConfig.ConsumerSecret,
                Token = serviceConfig.OAuthToken,
                TokenSecret = serviceConfig.OAuthSecret,
                RequestUrl = requestUrl
            };

            // Using HTTP header authorization
            return client.GetAuthorizationHeader(ParseParamCollection(new Uri(requestUrl)));
        }

        public IDictionary<string, string> ParseParamCollection(Uri path)
        {
            var paramsParsed = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(path.Query))
            {
                return paramsParsed;
            }

            var results = HttpUtility.UrlDecode(path.Query)?.TrimStart('?').Split('&');
            if (results == null)
            {
                return paramsParsed;
            }
            foreach (var item in results)
            {
                var pairs = item.Split('=');
                if (paramsParsed.ContainsKey(pairs[0]))
                {
                    throw new InvalidOperationException("Multiple parameters with the same name!");
                }

                paramsParsed.Add(pairs[0], Uri.EscapeDataString(pairs[1]));
            }
            return paramsParsed;
        }

        /// <summary>
        /// Renews the Oauth access token. Use this if your current token has expired and you are getting 401 responses.
        /// </summary>
        public void RenewAccessToken()
        {
            try
            {
                GetToken();
            }
            catch (AcsApiException)
            {
                throw;
            }
            catch (Exception)
            {
                // return non-specific error and destroy call stack
                throw new Exception(GenericAuthorizationErrorMessage);
            }
        }

        /// <summary>
        /// A helper method that can be used to add authorization headers and cookies to an existing HttpWebRequest.
        /// </summary>
        /// <param name="request">An HttpWebRequest that is ready to be sent and just requires OAuth headers and cookies to be added.</param>
        /// <returns>The response stream obtained by issuing the request</returns>
        public WebResponseWrapper MakeRequest(HttpWebRequest request)
        {
            request.Headers.Add("Authorization", GetAuthHeadersForRequestByType(request.RequestUri.ToString(), request.Method));

            if (request.CookieContainer == null)
            {
                request.CookieContainer = new CookieContainer();
            }

            if (cookies[SessionIdKey] != null)
            {
                request.CookieContainer.Add(cookies[SessionIdKey]);
            }

            if (cookies[ConsumerTypeIdKey] != null)
            {
                request.CookieContainer.Add(cookies[ConsumerTypeIdKey]);
            }

            try
            {
                return new WebResponseWrapper((HttpWebResponse)request.GetResponse());
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        if (response.StatusCode == HttpStatusCode.NotFound) { throw new AcsApiException(AcsApiError.UnknownEndpoint); }
                        if (response.StatusCode == HttpStatusCode.Forbidden) { throw new AcsApiException(AcsApiError.AccessDenied); }
                    }
                    else
                    {
                        // no http status code available
                    }
                }
                else
                {
                    // no http status code available
                }
                throw;
            }

        }

        private void RefreshCookies(CookieCollection responseCollection, HttpWebRequest request)
        {
            if (request.CookieContainer == null)
            {
                request.CookieContainer = new CookieContainer();
            }
            foreach (Cookie cookie in responseCollection)
            {
                request.CookieContainer.Add(cookie);
            }
        }

        public sealed class StreamWrapper : IDisposable
        {
            private readonly Stream _stream;

            public StreamWrapper(Stream stream)
            {
                _stream = stream;
            }

            public Stream Get()
            {
                return _stream;
            }

            public void Dispose()
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream.Dispose();
                }
            }
        }

        public sealed class WebResponseWrapper : IDisposable
        {
            private readonly HttpWebResponse _response;
            public WebResponseWrapper(HttpWebResponse response)
            {
                _response = response;
            }

            internal HttpWebResponse Get()
            {
                return _response;
            }

            /// <summary>
            /// ENSURE YOU DISPOSE THIS STREAM!
            /// </summary>
            /// <returns></returns>
            public StreamWrapper GetStream()
            {
                return new StreamWrapper(Get().GetResponseStream());
            }
            public void Dispose()
            {
                _response?.Close();
            }
        }

        // bbax: oAuth step 1: request_token
        private HttpWebResponse RequestToken(OAuthRequest client)
        {
            // Using HTTP header authorization
            var auth = client.GetAuthorizationHeader();
            var request = (HttpWebRequest)WebRequest.Create(client.RequestUrl);

            request.Headers.Add("Authorization", auth);
            var response = (HttpWebResponse)request.GetResponse();

            if (response == null)
            {
                throw new AcsApiException(AcsApiError.CouldNotAuthToken);
            }
            InvokeLog("Client token " + response);
            return response;
        }

        private string GetResponseFromWebResponseWrapped(WebResponseWrapper responseWrapper)
        {
            string responseText;
            using (var tokenResponseWrapper = new StreamWrapper(responseWrapper.Get().GetResponseStream()))
            {
                InvokeLog("Token Response: " + tokenResponseWrapper.Get());
                if (tokenResponseWrapper.Get() == null) { throw new AcsApiException(AcsApiError.CouldNotGetAccesstoken); }
                using (var reader = new StreamReader(tokenResponseWrapper.Get(), Encoding.ASCII))
                {
                    responseText = reader.ReadToEnd();
                    reader.Close();
                }
            }
            return responseText;
        }

        private HttpWebRequest Login(OAuthRequest client, string responseCookies, Hashtable fields)
        {
            client.Token = fields[OauthTokenKey].ToString();
            client.TokenSecret = fields[OauthTokenSecretKey].ToString();

            InvokeLog("Token and Sec: " + client.Token + " : " + client.TokenSecret);

            var ascii = new ASCIIEncoding();
            var postData =
                ascii.GetBytes("j_username=" + HttpUtility.UrlEncode(serviceConfig.PortalUsername) + "&j_password=" +
                               HttpUtility.UrlEncode(serviceConfig.PortalPassword));

            var myHttpWebRequest =
                (HttpWebRequest)WebRequest.Create(serviceConfig.ServerRoot + AcsApiClientConfig.ServicesLoginUrl);
            myHttpWebRequest.Method = "POST";
            myHttpWebRequest.AllowAutoRedirect = false;
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = postData.Length;
            myHttpWebRequest.CookieContainer = new CookieContainer();

            CookieCollection newRequiredCookies = new CookieCollection();
            try
            {
                newRequiredCookies = AcsApiSetCookieHeaderParser.GetAllCookiesFromHeader(responseCookies,
                    new Uri(serviceConfig.ServerRoot).Host);
            }
            catch (Exception)
            {
                throw new AcsApiException("Could not parse cookie for final request!");
            }
            RefreshCookies(newRequiredCookies, myHttpWebRequest);

            using (var requestStream = myHttpWebRequest.GetRequestStream())
            {
                requestStream.Write(postData, 0, postData.Length);
                requestStream.Close();
            }
            return myHttpWebRequest;
        }

        private HttpWebResponse ConsumerHandshake(OAuthRequest client, string jsessionid, string setCookieHeader, string domain)
        {
            var authReq =
                (HttpWebRequest)
                    WebRequest.Create(serviceConfig.ServerRoot + AcsApiClientConfig.AuthUrl + "?" + OauthTokenKey + "=" +
                                      HttpUtility.UrlEncode(client.Token));

            authReq.AllowAutoRedirect = false;
            authReq.CookieContainer = new CookieContainer();

            authReq.CookieContainer.Add(new Cookie(SessionIdKey.ToUpper(), jsessionid) { Domain = domain });
            authReq.CookieContainer.Add(new Cookie(ConsumerTypeIdKey, ConsumerTypeValue) { Domain = domain });

            cookies.Add(new Cookie(ConsumerTypeIdKey, ConsumerTypeValue, "/", domain));

            CookieCollection newRequiredCookies = new CookieCollection();
            try
            {
                newRequiredCookies = AcsApiSetCookieHeaderParser.GetAllCookiesFromHeader(setCookieHeader,
                    new Uri(serviceConfig.ServerRoot).Host);
            }
            catch (Exception)
            {
                throw new AcsApiException("Could not parse cookie for final request!");
            }
            RefreshCookies(newRequiredCookies, authReq);

            HttpWebResponse authReqResp;
            try
            {
                authReqResp = authReq.GetResponse() as HttpWebResponse;
            }
            catch (WebException wex)
            {
                throw new AcsApiException(AcsApiError.ServerUnreachable.ToString(), wex);
            }

            // bbax: response streams being closed while dealing with exceptions... yey..
            // todo: clean this up more...
            if (authReqResp?.Headers == null)
            {
                authReqResp?.Close();
                throw new AcsApiException(AcsApiError.CouldNotLogin);
            }
            return authReqResp;
        }

        private HttpWebResponse RequestFinal(OAuthRequest client, string accessToken, string oauthVerifier, string cdomain, string consumerCookies)
        {
            client.Token = accessToken;
            client.Type = OAuthRequestType.AccessToken;
            client.Verifier = oauthVerifier;
            client.RequestUrl = serviceConfig.ServerRoot + AcsApiClientConfig.AccessUrl;
            client.CallbackUrl = null;
            client.TokenSecret = HttpUtility.UrlDecode(client.TokenSecret);
            var auth = client.GetAuthorizationHeader();

            var accessTokenRequest = (HttpWebRequest)WebRequest.Create(serviceConfig.ServerRoot + AcsApiClientConfig.AccessUrl);
            accessTokenRequest.Method = "GET";
            accessTokenRequest.Headers.Add("Authorization", auth);
            accessTokenRequest.CookieContainer = new CookieContainer();
            //accessTokenRequest.CookieContainer.Add(new Cookie(ConsumerTypeIdKey, ConsumerTypeValue) { Domain = cdomain });

            CookieCollection newRequiredCookies = new CookieCollection();
            try
            {
                newRequiredCookies = AcsApiSetCookieHeaderParser.GetAllCookiesFromHeader(consumerCookies,
                    new Uri(serviceConfig.ServerRoot).Host);
            }
            catch (Exception)
            {
                throw new AcsApiException("Could not parse cookie for final request!");
            }
            RefreshCookies(newRequiredCookies, accessTokenRequest);

            accessTokenRequest.AllowAutoRedirect = false;

            HttpWebResponse accessTokenResponse = null;

            try
            {
                accessTokenResponse = (HttpWebResponse)accessTokenRequest.GetResponse();
            }
            catch (WebException wex)
            {
                throw new AcsApiException(AcsApiError.ServerUnreachable.ToString(), wex);
            }
            return accessTokenResponse;
        }

        /// <summary>
        /// Get an access token
        /// </summary>
        private void GetToken()
        {
            if (_acsPortal)
            {
                LoadAcsToken();
            }
            else
            {
                LoadFssToken();
            }
            
        }

        // bbax: case matters... 
        internal class LoginRequestToken
        {
            public string consumerKey { get; set; }
            public string consumerSecret { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

        // bbax: case matters... 
        internal class LoginRequestResponse
        {
            public string token { get; set; }
            public string secret { get; set; }
        }

        private void LoadAcsToken()
        {
            var requestDetails = new LoginRequestToken
            {
                consumerKey = serviceConfig.ConsumerKey,
                consumerSecret = serviceConfig.ConsumerSecret,
                password = serviceConfig.PortalPassword,
                username = serviceConfig.PortalUsername
            };

            var loginRequest = JsonConvert.SerializeObject(requestDetails);
            var client = new RestClient(serviceConfig.ServerRoot);
            var request = new RestRequest(AcsApiClientConfig.AcsServicesLoginUrl, Method.POST);
            request.AddParameter("application/json", loginRequest, ParameterType.RequestBody);

            var response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new AcsApiException(AcsApiError.CouldNotLogin);
            }

            var unmarshalledResponse = JsonConvert.DeserializeObject(response.Content, typeof (LoginRequestResponse)) as LoginRequestResponse;
            if (unmarshalledResponse == null)
            {
                throw new AcsApiException("Failed to unmarshall the login request!");
            }
            serviceConfig.OAuthSecret = unmarshalledResponse.secret;
            serviceConfig.OAuthToken = unmarshalledResponse.token;
        }

        private void LoadFssToken()
        {
            // Creating a new instance directly
            var client = new OAuthRequest
            {
                Method = "GET",
                Type = OAuthRequestType.RequestToken,
                SignatureMethod = OAuthSignatureMethod.HmacSha1,
                ConsumerKey = serviceConfig.ConsumerKey,
                ConsumerSecret = serviceConfig.ConsumerSecret,
                RequestUrl = serviceConfig.ServerRoot + AcsApiClientConfig.RequestUrl,
                CallbackUrl = serviceConfig.ServerRoot + "client"
            };


            //CookieCollection responseCookies = null;
            string setCookieHeader;
            var fields = new Hashtable();
            using (var responseWrapper = new WebResponseWrapper(RequestToken(client)))
            {
                var responseText = GetResponseFromWebResponseWrapped(responseWrapper);

                for (var i = 0; i < responseText.Split('&').Length; i++)
                {
                    var fieldInfo = responseText.Split('&')[i].Split('=');
                    fields[fieldInfo[0]] = fieldInfo[1];
                }

                //responseCookies = responseWrapper.Get().Cookies;
                setCookieHeader = responseWrapper.Get().Headers["Set-Cookie"];
            }

            var myHttpWebRequest = Login(client, setCookieHeader, fields);

            string jsessionid;
            setCookieHeader = null;
            try
            {
                using (var responseWrapper = new WebResponseWrapper((HttpWebResponse)myHttpWebRequest.GetResponse()))
                {
                    setCookieHeader = responseWrapper.Get().Headers["Set-Cookie"];
                    var sessionCookie = responseWrapper.Get().Cookies[SessionIdKey];
                    if (sessionCookie != null)
                    {
                        jsessionid = sessionCookie.Value;

                        InvokeLog("Jsesssion found at " + jsessionid);
                        cookies.Add(sessionCookie);
                    }
                    else
                    {
                        throw new InvalidOperationException("JSession Id returned null!");
                    }
                }
            }
            catch (WebException wex)
            {
                throw new AcsApiException(AcsApiError.ServerUnreachable.ToString(), wex);
            }

            var target = new Uri(serviceConfig.ServerRoot);
            var cdomain = target.Host;

            string[] locations;
            using (var responseWrapper = new WebResponseWrapper(ConsumerHandshake(client, jsessionid, setCookieHeader, cdomain)))
            {
                locations = responseWrapper.Get().Headers.GetValues("location");
                setCookieHeader = responseWrapper.Get().Headers["Set-Cookie"];
            }

            if (locations == null)
            {
                throw new AcsApiException(AcsApiError.CouldNotLogin);
            }

            var myUri = new Uri(locations.FirstOrDefault() ?? string.Empty);
            InvokeLog("Request for uri" + myUri);
            if (string.IsNullOrEmpty(myUri.Query))
            {
                // No oauth_token or oauth_verifier in location header, so didn't authenticate.
                throw new AcsApiException(AcsApiError.CouldNotLogin);
            }

            var accessToken = HttpUtility.ParseQueryString(myUri.Query).Get(OauthTokenKey);
            var oauthVerifier = HttpUtility.ParseQueryString(myUri.Query).Get(OauthVerifierKey);

            InvokeLog("Verifier response " + accessToken + " : " + oauthVerifier);

            if (string.IsNullOrEmpty(oauthVerifier))
            {
                throw new AcsApiException(AcsApiError.CouldNotFindVerifier);
            }

            using (var requestWrapper = new WebResponseWrapper(RequestFinal(client, accessToken, oauthVerifier, cdomain, setCookieHeader)))
            {
                using (var responseStream = new StreamWrapper(requestWrapper.Get().GetResponseStream()))
                {
                    if (responseStream.Get() == null)
                    {
                        throw new AcsApiException(AcsApiError.CouldNotGetAccesstoken);
                    }

                    string responseOutput;

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    using (var readStream = new StreamReader(responseStream.Get(), Encoding.UTF8))
                    {
                        responseOutput = readStream.ReadToEnd();
                    }

                    serviceConfig.OAuthToken = HttpUtility.ParseQueryString(responseOutput).Get(OauthTokenKey);
                    serviceConfig.OAuthSecret = HttpUtility.ParseQueryString(responseOutput).Get(OauthTokenSecretKey);

                    InvokeLog("Final Tokens: " + serviceConfig.OAuthToken + " : " + serviceConfig.OAuthSecret);
                }
            }
        }
    }
}