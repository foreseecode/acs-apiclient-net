using System;
namespace AcsApi
{
    internal static class Utility
    {
        internal static string Base64Encode(string stringToEncode)
        {
            var bytesToEncode = System.Text.Encoding.UTF8.GetBytes(stringToEncode);
            return System.Convert.ToBase64String(bytesToEncode);
        }
    }
}
