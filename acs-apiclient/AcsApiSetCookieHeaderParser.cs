//Code taken from: http://snipplr.com/view/4427/ideabubbling.com
//No license was attached to the snippet
using System;
using System.Collections;
using System.Net;

namespace acs_apiclient
{
    internal class AcsApiSetCookieHeaderParser
    {
        public static CookieCollection GetAllCookiesFromHeader(string strHeader, string strHost)
        {
            var al = new ArrayList();
            var cc = new CookieCollection();
            if (strHeader == string.Empty)
            {
                return cc;
            }
            al = ConvertCookieHeaderToArrayList(strHeader);
            cc = ConvertCookieArraysToCookieCollection(al, strHost);
            return cc;
        }


        private static ArrayList ConvertCookieHeaderToArrayList(string strCookHeader)
        {
            strCookHeader = strCookHeader.Replace("\r", "");
            strCookHeader = strCookHeader.Replace("\n", "");
            var strCookTemp = strCookHeader.Split(',');
            var al = new ArrayList();
            var i = 0;
            var n = strCookTemp.Length;
            while (i < n)
            {
                if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
                    i = i + 1;
                }
                else
                {
                    al.Add(strCookTemp[i]);
                }
                i = i + 1;
            }
            return al;
        }


        private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList al, string strHost)
        {
            var cc = new CookieCollection();

            var alcount = al.Count;
            for (var i = 0; i < alcount; i++)
            {
                var strEachCook = al[i].ToString();
                var strEachCookParts = strEachCook.Split(';');
                var intEachCookPartsCount = strEachCookParts.Length;
                var strDNameAndDValue = string.Empty;
                var cookTemp = new Cookie();

                for (var j = 0; j < intEachCookPartsCount; j++)
                {
                    if (j == 0)
                    {
                        var strCNameAndCValue = strEachCookParts[j];
                        if (strCNameAndCValue != string.Empty)
                        {
                            var firstEqual = strCNameAndCValue.IndexOf("=", StringComparison.Ordinal);
                            var firstName = strCNameAndCValue.Substring(0, firstEqual);
                            var allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }
                        continue;
                    }
                    string strPNameAndPValue;
                    string[] nameValuePairTemp;
                    if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            nameValuePairTemp = strPNameAndPValue.Split('=');
                            cookTemp.Path = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : "/";
                        }
                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    strPNameAndPValue = strEachCookParts[j];
                    if (strPNameAndPValue != string.Empty)
                    {
                        nameValuePairTemp = strPNameAndPValue.Split('=');

                        cookTemp.Domain = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : strHost;
                    }
                }

                if (cookTemp.Path == string.Empty)
                {
                    cookTemp.Path = "/";
                }
                if (cookTemp.Domain == string.Empty)
                {
                    cookTemp.Domain = strHost;
                }
                cc.Add(cookTemp);
            }
            return cc;
        } 
    }
}