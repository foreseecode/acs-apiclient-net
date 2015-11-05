Answers Cloud Services API Client Library for C#
===================
Helper library for connecting to the Answers Cloud Services (ForeSee in particular) web API in a headless manner from Java. You can use this to simplify connecting to the ACS api without requiring a browser or user interaction to grant access to a particular account.
###Installation
Logging Setup
AcsApiClient contains an event called Log to attach to.
```C#
var acsClient = new AcsApiClient(config);
acsClient.Log += Console.WriteLine;
```

###Simple Usage
```C#

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using AcsApi;

namespace acs_net40_console_example
{
    class Program
    {
        static int MakeCurrentUserRequest(IAcsApiClient client)
        {
            try
            {
                const string url = "https://portal2.foreseeresults.com/services/currentUser/";
                var uri = new Uri(url);
                var oauthtoken = client.GetAuthHeadersForRequestByType(url, "GET");
                if (!oauthtoken.StartsWith("OAuth"))
                {
                    throw new InvalidOperationException("OAuth header is incorrectly formatted");
                }

                var token = oauthtoken.Substring("OAuth ".Length);
                using (var httpHandler = new HttpClientHandler { AllowAutoRedirect = false })
                using (var httpClient = new HttpClient(httpHandler))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("OAuth", token);
                    var response = httpClient.GetAsync(uri);
                    response.Wait();

                    var body = response.Result.Content.ReadAsStringAsync();
                    body.Wait();

                    Console.WriteLine("Response: {0}", body.Result);
                    return (int)response.Result.StatusCode;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        static void Main(string[] args)
        {
            var input = "";
            var clientConfig = new AcsApiClientConfig(consumerKey, consumerSecret,
                "https://portal2.foreseeresults.com/services/", foreseeProvidedUserName, foreseeProvidedPassword);
            var foreseeClient = new AcsApiClient(clientConfig);
            while (input != "quit")
            {
                var result = MakeCurrentUserRequest(foreseeClient);
                if (result != (int)HttpStatusCode.OK)
                {
                    Console.WriteLine($"ERROR: Status Code was: {result}");
                }

                input = Console.ReadLine();
            }
            Console.WriteLine("Done");
            Console.ReadKey(true);
        }
    }
}

```

### Date Usage
```C#
namespace acs_apiclient_tests
{
    using AcsApi;
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests date examples from http://developer.answerscloud.com/docs-articles/foresee-products-services/public-api/date-resolution/
    /// </summary>
    [TestClass]
    public class DateRangeTests
    {
        [TestMethod]
        public void TestCustomDate()
        {
            var dateBuilder = new AcsApiDateRangeBuilder();
            
            // specify an explicit range from 15th Sep 1977 to 15th Sep 1997
            dateBuilder.SetCustomDateRange(new DateTime(1977, 09, 15), new DateTime(1997, 09, 15));

            var expectedJson =
                "{\"c\":\"G\",\"r\":\"CUSTOM\",\"f\":\"1977-09-15\",\"l\":\"1997-09-15\"}";
            Assert.AreEqual(expectedJson, dateBuilder.DateRange.ToAcsApiJson());
        }

        [TestMethod]
        public void TestCustomRelativeDate()
        {
            var dateBuilder = new AcsApiDateRangeBuilder(new DateTime(1977, 09, 15));

            // specify two days prior to the relative date of the range ie. 13th and 14th September 1977
            dateBuilder.SetAbsoluteDateRangeSequence(2, AcsApiDateRange.AcsApiRangeSequence.DAYS, AcsApiDateRange.AcsApiPeriodModifier.PRIOR);

            var expectedJson =
                "{\"a\":\"1977-09-15\",\"c\":\"G\",\"r\":\"DAYS\",\"n\":2,\"p\":\"PRIOR\"}";
            Assert.AreEqual(expectedJson, dateBuilder.DateRange.ToAcsApiJson());
        }

        [TestMethod]
        public void TestCalendarYearDefined()
        {
            var dateBuilder = new AcsApiDateRangeBuilder();

            // specify the calendar year 2013
            dateBuilder.SetAbsoluteDateRange(AcsApiDateRange.AcsApiRange.YEAR, 2013);

            var expectedJson =
                "{\"c\":\"G\",\"r\":\"YEAR\",\"n\":2013,\"p\":\"DEFINED\"}";
            Assert.AreEqual(expectedJson, dateBuilder.DateRange.ToAcsApiJson());
        }

        [TestMethod]
        public void TestTwoDaysPrior()
        {
            var dateBuilder = new AcsApiDateRangeBuilder();

            // specify two days prior ie. yesterday and the day before
            dateBuilder.SetAbsoluteDateRangeSequence(2, AcsApiDateRange.AcsApiRangeSequence.DAYS, AcsApiDateRange.AcsApiPeriodModifier.PRIOR);

            var expectedJson =
                "{\"c\":\"G\",\"r\":\"DAYS\",\"n\":2,\"p\":\"PRIOR\"}";
            Assert.AreEqual(expectedJson, dateBuilder.DateRange.ToAcsApiJson());
        }

        [TestMethod]
        public void TestThirdMonthPrior()
        {
            var dateBuilder = new AcsApiDateRangeBuilder();

            // specify the third month prior to the last full month eg. if it's currently mid-April, this will get January
            dateBuilder.SetAbsoluteDateRange(AcsApiDateRange.AcsApiRange.MONTH, 3, AcsApiDateRange.AcsApiPeriodModifier.PRIOR);

            var expectedJson =
                "{\"c\":\"G\",\"r\":\"MONTH\",\"n\":3,\"p\":\"PRIOR\"}";
            Assert.AreEqual(expectedJson, dateBuilder.DateRange.ToAcsApiJson());
        }

        [TestMethod]
        public void TestCurrentMonth()
        {
            var dateBuilder = new AcsApiDateRangeBuilder();

            // specify the current month
            dateBuilder.SetAbsoluteDateRange(AcsApiDateRange.AcsApiRange.MONTH, null, AcsApiDateRange.AcsApiPeriodModifier.CURRENT);

            var expectedJson =
                "{\"c\":\"G\",\"r\":\"MONTH\",\"p\":\"CURRENT\"}";
            Assert.AreEqual(expectedJson, dateBuilder.DateRange.ToAcsApiJson());
        }

        [TestMethod]
        public void TestPriorYear()
        {
            var dateBuilder = new AcsApiDateRangeBuilder();

            // specify the year prior to 2011 (strange use case for specifying 2010)
            dateBuilder.SetAbsoluteDateRange(AcsApiDateRange.AcsApiRange.YEAR, 2011, AcsApiDateRange.AcsApiPeriodModifier.PRIOR_YEAR);

            var expectedJson =
                "{\"c\":\"G\",\"r\":\"YEAR\",\"n\":2011,\"p\":\"PRIOR_YEAR\"}";
            Assert.AreEqual(expectedJson, dateBuilder.DateRange.ToAcsApiJson());
        }

        [TestMethod]
        public void TestCustomCalendar()
        {
            var dateBuilder = new AcsApiDateRangeBuilder();

            // specify the custom calendar
            dateBuilder.UseCustomCalendar("abc12345");

            // specify the current month
            dateBuilder.SetAbsoluteDateRange(AcsApiDateRange.AcsApiRange.MONTH, null, AcsApiDateRange.AcsApiPeriodModifier.CURRENT);

            var expectedJson = "{\"c\":\"G\",\"r\":\"MONTH\",\"k\":\"abc12345\",\"p\":\"CURRENT\"}";
            Assert.AreEqual(expectedJson, dateBuilder.DateRange.ToAcsApiJson());
        }

       [TestMethod]
       public void TestManualDateRange()
       {
           var dateRange = new AcsApiDateRange
                               {
                                   Range = "YR",
                                   CustomerKey = "abc12345",
                                   PeriodModifier = AcsApiDateRange.AcsApiPeriodModifier.CURRENT.ToString()
                               };

           var expectedJson = "{\"r\":\"YR\",\"k\":\"abc12345\",\"p\":\"CURRENT\"}";
           Assert.AreEqual(expectedJson, dateRange.ToAcsApiJson());
       }
    }
}
```