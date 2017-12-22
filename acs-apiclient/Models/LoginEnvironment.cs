using System;

namespace AcsApi.Models
{
    public enum LoginEnvironment
    {
        Development,
        QualityAssurance,
        Staging,
        Production
    }

    public static class EnvironmentExtensions
    {
        public static string AuthorizationServerBaseUrl(this LoginEnvironment environment)
        {
            switch (environment)
            {
                case LoginEnvironment.Development:
                    return "https://foresee-dev.oktapreview.com";
                case LoginEnvironment.QualityAssurance:
                    return "https://foresee-qa.oktapreview.com";
                case LoginEnvironment.Staging:
                    return "https://foresee.oktapreview.com";
                case LoginEnvironment.Production:
                    return "https://foresee.okta.com";
            }
            throw new NotImplementedException(
                $"{ environment.ToString() } - This case is not handled."
            );
        }
    }
}
