using System;

namespace AcsApi.Models
{
    public enum Consumer
    {
        CxSuite,
        AnalyticsPortal,
        ExecutivePortal,
        iOS,
        Android
    }

    public static class ConsumerExtensions
    {
        public static string ToParameterString(this Consumer consumer)
        {
            switch (consumer)
            {
                case Consumer.CxSuite: 
                    return "CX_SUITE";
                case Consumer.AnalyticsPortal:
                    return "ANALYTIC_PORTAL";
                case Consumer.ExecutivePortal:
                    return "EXECUTIVE_PORTAL";
                case Consumer.iOS:
                    return "MOBILE_IOS";
                case Consumer.Android:
                    return "MOBILE_ANDROID";
            }
            throw new NotImplementedException(
                $"{ consumer.ToString() } - This case is not handled."
            );
        }
    }
}
