using System;
using System.Linq;
using System.Web;

namespace AcsApi
{
    static class Utility
    {
        internal static string Base64Encode(string stringToEncode)
        {
            var bytesToEncode = System.Text.Encoding.UTF8.GetBytes(stringToEncode);
            return Convert.ToBase64String(bytesToEncode);
        }

        internal static string UrlEncode(string queryString)
        {
            var parameters = HttpUtility.ParseQueryString(queryString);
            var encodedParameters = (
                from parameterKey in parameters.AllKeys
                from parameterValue in parameters.GetValues(parameterKey)
                select $"{ HttpUtility.UrlEncode(parameterKey) }={ HttpUtility.UrlEncode(parameterValue) }"
            ).ToArray();
            return string.Join("&", encodedParameters);
        }
    }
}
