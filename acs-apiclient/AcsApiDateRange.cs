// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Answers Cloud Services" file="AcsApiDateRange.cs">
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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Date Range object used for date resolution in ACS API requests.
    /// </summary>
    public class AcsApiDateRange
    {
        /// <summary>
        /// The type of calendar.
        /// </summary>
        public enum AcsApiCalendarType
        {
            /// <summary>
            /// Gregorian (terse) – All starting points (Year, Month, Week) are according to the Gregorian calendar.
            /// </summary>
            G = 0,

            /// <summary>
            /// Gregorian – All starting points (Year, Month, Week) are according to the Gregorian calendar.
            /// </summary>
            GREGORIAN,

            /// <summary>
            /// Fiscal – All starting points must be specified.
            /// </summary>
            FISCAL
        }

        /// <summary>
        /// Type of date range period.
        /// </summary>
        public enum AcsApiRange
        {
            /// <summary>
            /// The numbered day of the year.
            /// </summary>
            DAY, 

            /// <summary>
            /// The numbered week of the year.
            /// </summary>
            WEEK, 

            /// <summary>
            /// The numbered month of the year.
            /// </summary>
            MONTH, 

            /// <summary>
            /// The numbered three-month period of the year.
            /// </summary>
            QUARTER, 

            /// <summary>
            /// The specific four-character year.
            /// </summary>
            YEAR
        }

        /// <summary>
        /// Type of date range sequence.
        /// </summary>
        public enum AcsApiRangeSequence
        {
            /// <summary>
            /// The number of full days.
            /// </summary>
            DAYS,

            /// <summary>
            /// The number of full weeks.
            /// </summary>
            WEEKS,

            /// <summary>
            /// The number of full months.
            /// </summary>
            MONTHS,
            
            /// <summary>
            /// The number of full three-month periods.
            /// </summary>
            QUARTERS,

            /// <summary>
            /// The number of full years.
            /// </summary>
            YEARS,

            /// <summary>
            /// A specific period is specified using start and end dates.
            /// </summary>
            CUSTOM
        }

        /// <summary>
        /// Modifier for the date range period.
        /// </summary>
        public enum AcsApiPeriodModifier
        {
            /// <summary>
            /// Use the start and end dates as defined.
            /// </summary>
            DEFINED,

            /// <summary>
            /// Move the start and end dates to a previous range.
            /// </summary>
            PRIOR,

            /// <summary>
            /// Move the end date to the end of the range type.
            /// </summary>
            CURRENT,

            /// <summary>
            /// Move the start and end dates to a following range.
            /// </summary>
            NEXT,

            /// <summary>
            /// Move the start and end dates to a previous year.
            /// </summary>
            PRIOR_YEAR
        }

        /// <summary>
        /// Gets the as-of/relative date.
        /// </summary>
        [JsonProperty(PropertyName = "a")]
        public string AsOfDate { get; set; }

        /// <summary>
        /// Gets the calendar type. See <see cref="AcsApiCalendarType"/> for possible values (correct at time of writing.)
        /// </summary>
        [JsonProperty(PropertyName = "c")]
        public string CalendarType { get; set; }

        /// <summary>
        /// Gets the range. See <see cref="AcsApiRange"/> or <see cref="AcsApiRangeSequence"/> for possible values (correct at time of writing.)
        /// </summary>
        [JsonProperty(PropertyName = "r")]
        public string Range { get; set; }

        /// <summary>
        /// Gets the customer key.
        /// </summary>
        [JsonProperty(PropertyName = "k")]
        public string CustomerKey { get; set; }

        /// <summary>
        /// Gets the range number.
        /// </summary>
        [JsonProperty(PropertyName = "n")]
        public int? RangeNumber { get; set; }

        /// <summary>
        /// Gets the period modifier. See <see cref="AcsApiPeriodModifier"/> for possible values (correct at time of writing.)
        /// </summary>
        [JsonProperty(PropertyName = "p")]
        public string PeriodModifier { get; set; }

        /// <summary>
        /// Gets the from date.
        /// </summary>
        [JsonProperty(PropertyName = "f")]
        public string FromDate { get; set; }

        /// <summary>
        /// Gets the to date.
        /// </summary>
        [JsonProperty(PropertyName = "l")]
        public string ToDate { get; set; }

        /// <summary>
        /// Returns a json string of the DateRange object that can be used in ACS API requests.
        /// </summary>
        /// <returns></returns>
        public string ToAcsApiJson()
        {
            // serialize this object, omitting nulls
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            return JsonConvert.SerializeObject(this, Formatting.None, settings);
        }
    }
}