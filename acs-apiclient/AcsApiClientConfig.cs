// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Answers Cloud Services" file="AcsApiClientConfig.cs">
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

namespace AcsApi
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Client configuration for connecting to the Answers Cloud Services (ForeSee in particular) web API in a headless manner. 
    /// You can use this to simplify connecting to the ACS api without requiring a browser or user interaction to grant access to a particular account.
    /// </summary>
    public class AcsApiClientConfig
    {
        /// <summary>
        /// Request token URL
        /// </summary>
        internal const string RequestUrl = "oauth/request_token";

        /// <summary>
        /// Authorization URL
        /// </summary>
        internal const string AuthUrl = "oauth/user_authorization";

        /// <summary>
        /// Request access token URL
        /// </summary>
        internal const string AccessUrl = "oauth/access_token";

        /// <summary>
        /// The services login
        /// </summary>
        internal const string ServicesLoginUrl = "login";

        /// <summary>
        /// acs services login path
        /// </summary>
        internal const string AcsServicesLoginUrl = "access";


        /// <summary>
        /// The username
        /// </summary>
        internal readonly string PortalUsername;

        /// <summary>
        /// The password
        /// </summary>
        internal readonly string PortalPassword;

        /// <summary>
        /// Base url
        /// </summary>
        internal readonly string ServerRoot;

        /// <summary>
        /// OAuth Consumer Key
        /// </summary>
        internal readonly string ConsumerKey;

        /// <summary>
        /// OAuth Consumer Secret
        /// </summary>
        internal readonly string ConsumerSecret;

        /// <summary>
        /// A token received upon completing the entire authentication flow
        /// </summary>
        internal string OAuthToken { get; set; }

        /// <summary>
        /// A secret received upon completing the entire authentication flow
        /// </summary>
        internal string OAuthSecret { get; set; }

        /// <summary>
        /// Determines if the AcsApiClient was created from the SSO Flow
        /// </summary>
        internal bool IsSSOClient { get; set; } = false;

        /// <summary>
        /// The expiration date of the OAuthToken.
        /// </summary>
        internal DateTime ExpirationDate { get; set; }

        internal string EncodedTokenString => $"{ Utility.Base64Encode(OAuthToken) }" +
            $".{ Utility.Base64Encode(OAuthSecret) }" +
            $".{ Utility.Base64Encode(ExpirationDate.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")) }";

        /// <summary>
        /// Initializes a new instance of the <see cref="AcsApiClientConfig"/> class. 
        /// </summary>
        /// <param name="consumerKey">
        /// The OAuth consumer key.
        /// </param>
        /// <param name="consumerSecret">
        /// The OAuth consumer secret.
        /// </param>
        /// <param name="serverRoot">
        /// The server to use for authentication and service calls.
        /// </param>
        /// <param name="portalUsername">
        /// The username for ACS portal services.
        /// </param>
        /// <param name="portalPassword">
        /// The password for ACS portal services.
        /// </param>
        internal AcsApiClientConfig(
            string consumerKey, 
            string consumerSecret, 
            string serverRoot, 
            string portalUsername = "", 
            string portalPassword = ""
        ) {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.ServerRoot = serverRoot.EndsWith("/", StringComparison.CurrentCulture) ? serverRoot : serverRoot + "/";
            this.PortalUsername = portalUsername;
            this.PortalPassword = portalPassword;
        }
    }
}