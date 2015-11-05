// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Answers Cloud Services" file="AcsApiError.cs">
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
    public enum AcsApiError
    {
        /// <summary>Could not get a request token. There may be something wrong with your consumer key or consumer secret.</summary>
        InvalidRequestToken,

        /// <summary>There was a problem with the login process. Probably not due to invalid credentials.</summary>
        CouldNotLogin,

        /// <summary>Could not log in with the provided credentials.</summary>
        InvalidCredentials,

        /// <summary>Could not authorize the token.</summary>
        CouldNotAuthToken,

        /// <summary>There was a problem with the authentication flow. Might be due to an invalid consumer_type, consumer_key or consumer_secret.</summary>
        CouldNotFindVerifier,

        /// <summary>There was a problem with the authentication flow. Might be due to an invalid consumer_type, consumer_key or consumer_secret.</summary>
        CouldNotGetAccesstoken,

        /// <summary>There was a problem with the authentication flow. Might be due to an invalid consumer_type, consumer_key or consumer_secret.</summary>
        CouldNotGetAccessTokenNull,

        /// <summary>You do not have access to that endpoint with those criteria.</summary>
        AccessDenied,

        /// <summary>Invalid endpoint</summary>
        UnknownEndpoint,

        /// <summary>The remote server could not be reached.</summary>
        ServerError
    }
}