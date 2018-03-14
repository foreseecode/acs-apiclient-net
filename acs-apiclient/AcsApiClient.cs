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
using acs_apiclient;
using Newtonsoft.Json;
using OAuth;
using RestSharp;
using Newtonsoft.Json.Linq;

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
            if (string.IsNullOrEmpty(config.PortalPassword) || string.IsNullOrEmpty(config.PortalUsername))
            {
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
            if (string.IsNullOrEmpty(serviceConfig.AccessToken))
            {
                try
                {
                    LoadToken();
                }
                catch (AcsApiException ex)
                {
                    throw;
                }
                catch (Exception ex)
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
                Token = serviceConfig.AccessToken,
                TokenSecret = serviceConfig.AccessTokenSecret,
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
                LoadToken();
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

        private void LoadToken()
        {
            var requestDetails = new LoginRequestToken
            {
                consumerKey = serviceConfig.ConsumerKey,
                consumerSecret = serviceConfig.ConsumerSecret,
                password = serviceConfig.PortalPassword,
                username = serviceConfig.PortalUsername
            };

            var loginRequest = JsonConvert.SerializeObject(requestDetails);
            var client = new RestClient(serviceConfig.AuthServiceUri);
            var request = new RestRequest(AcsApiClientConfig.LoginUrl, Method.POST);
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
            serviceConfig.AccessTokenSecret = unmarshalledResponse.secret;
            serviceConfig.AccessToken = unmarshalledResponse.token;
        }
    }
}