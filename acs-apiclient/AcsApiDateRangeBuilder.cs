// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Answers Cloud Services" file="AcsApiDateRangeBuilder.cs">
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
    using System;

    /// <summary>
    /// A helper class that can be used to create a date range filter for ACS API requests.
    /// </summary>
    public class AcsApiDateRangeBuilder
    {
        public readonly AcsApiDateRange DateRange;

        private const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// Initializes a new instance of the <see cref="AcsApiDateRangeBuilder"/> class.
        /// </summary>
        /// <param name="relativeDate">
        /// A start date.  If supplied, all date ranges will be relative to this date.
        /// </param>
        /// <param name="calendar">
        /// The type of calendar to use. If using FISCAL calendar, all start points (day, month, year) must be defined. 
        /// Otherwise, start points are relative to the Gregorian calendar.
        /// </param>
        public AcsApiDateRangeBuilder(DateTime? relativeDate = null, AcsApiDateRange.AcsApiCalendarType calendar = AcsApiDateRange.AcsApiCalendarType.G)
        {
            this.DateRange = new AcsApiDateRange();

            if (relativeDate.HasValue)
            {
                this.DateRange.AsOfDate = relativeDate.Value.ToString(DateFormat);
            }

            this.DateRange.CalendarType = calendar.ToString();
        }

        /// <summary>
        /// Set a custom date range with explicit start and end dates.
        /// </summary>
        /// <param name="start">Start date.</param>
        /// <param name="end">End date.</param>
        public void SetCustomDateRange(DateTime start, DateTime end)
        {
            this.DateRange.Range = AcsApiDateRange.AcsApiRangeSequence.CUSTOM.ToString();
            this.DateRange.FromDate = start.ToString(DateFormat);
            this.DateRange.ToDate = end.ToString(DateFormat);
        }

        /// <summary>
        /// Set an absolute date range by specifying the type of the range and the value, eg. YEAR 2015 
        /// </summary>
        /// <param name="rangeType">Type of range eg. Year.</param>
        /// <param name="value">Numeric value for the range.</param>
        /// <param name="modifier">A modifier to offset the date range.</param>
        public void SetAbsoluteDateRange(AcsApiDateRange.AcsApiRange rangeType, int? value = null, AcsApiDateRange.AcsApiPeriodModifier modifier = AcsApiDateRange.AcsApiPeriodModifier.DEFINED)
        {
            this.DateRange.Range = rangeType.ToString();
            this.DateRange.RangeNumber = value;
            this.DateRange.PeriodModifier = modifier.ToString();
        }

        /// <summary>
        /// Set an absolute date range sequence by specifying the type of the sequence and the value, eg. 2 DAYS 
        /// </summary>
        /// <param name="value">Numeric value for the range.</param>
        /// <param name="rangeType">Type of range eg. Days.</param>
        /// <param name="modifier">A modifier to offset the date range.</param>
        public void SetAbsoluteDateRangeSequence(int value,  AcsApiDateRange.AcsApiRangeSequence rangeType, AcsApiDateRange.AcsApiPeriodModifier modifier = AcsApiDateRange.AcsApiPeriodModifier.DEFINED)
        {
            this.DateRange.Range = rangeType.ToString();
            this.DateRange.RangeNumber = value;
            this.DateRange.PeriodModifier = modifier.ToString();
        }

        /// <summary>
        /// Use a custom calendar for this date range.
        /// </summary>
        /// <param name="calendarKey"></param>
        public void UseCustomCalendar(string calendarKey)
        {
            this.DateRange.CustomerKey = calendarKey;
        }
    }
}