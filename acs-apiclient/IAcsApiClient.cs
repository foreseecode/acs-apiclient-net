// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Answers Cloud Services" file="IAcsApiClient.cs">
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
namespace AcsApi
// ReSharper restore CheckNamespace
{
    using System.IO;
    using System.Net;

    /// <summary>
    /// Client for connecting to the Answers Cloud Services (ForeSee in particular) web API in a headless manner. 
    /// You can use this to simplify connecting to the ACS API without requiring a browser or user interaction to grant access to a particular account.
    /// </summary>
    public interface IAcsApiClient
    {
        /// <summary>
        /// Get the authorization cookies that are required when issuing a request.
        /// </summary>
        /// <returns>A collection of HttpCookies.</returns>
        Cookie[] GetAuthCookies();

        /// <summary>
        /// Gets the Authorization Header for the supplied url.
        /// </summary>
        /// <param name="requestUrl">The request url for signing.</param>
        /// <returns>A string that can be used as the value for the "Authorization" header of an HTTP request.</returns>
        string GetAuthHeadersForRequest(string requestUrl);

        /// <summary>
        /// Gets the authorization header for supplied url by HTTP method... GET/POST/PUT/etc
        /// </summary>
        /// <param name="requestUrl">The request url for signing.</param>
        /// <param name="type">HTTP Method</param>
        /// <returns>A string that can be used as the value for the "Authorization" header of an HTTP request.</returns>
        string GetAuthHeadersForRequestByType(string requestUrl, string type);

        /// <summary>
        /// Renews the Oauth access token. Use this if your current token has expired and you are getting a 401 response.
        /// </summary>
        void RenewAccessToken();

        /// <summary>
        /// A helper method that can be used to add authorization headers and cookies to an existing HttpWebRequest.
        /// </summary>
        /// <param name="request">An HttpWebRequest that is ready to be sent and just requires OAuth headers and cookies to be added.</param>
        /// <returns>The response stream obtained by issuing the request</returns>
        AcsApiClient.WebResponseWrapper MakeRequest(HttpWebRequest request);
    }
}