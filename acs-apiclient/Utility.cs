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

        internal static string Base64Decode(string stringToDecode)
        {
            var decodedBytes = Convert.FromBase64String(stringToDecode);
            return System.Text.Encoding.UTF8.GetString(decodedBytes);
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

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <returns><c>true</c>, if token is valid, <c>false</c> otherwise.</returns>
        /// <param name="tokenString">Token string.</param>
        internal static bool ValidateToken(string tokenString)
        {
            var tokens = tokenString?.Split('.') ?? new string[] { };
            if (tokens.Count() < Constants.NumberOfTokens) { return false; }
            try
            {
                var decodedDateString = Base64Decode(tokens[Constants.ExpirationDateIndex]);
                var expirationDate = DateTime.Parse(decodedDateString).ToUniversalTime();
                return !Expired(expirationDate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Checks if a datetime is expired.
        /// </summary>
        /// <returns><c>true</c></returns>
        /// <param name="expirationDate">Expiration date.</param>
        internal static bool Expired(DateTime expirationDate)
        {
            if (DateTime.Compare(expirationDate, DateTime.UtcNow) > 0)
            {
                return false;
            }
            return true;
        }
    }
}
