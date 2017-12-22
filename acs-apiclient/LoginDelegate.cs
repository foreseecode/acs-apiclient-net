using System;
using System.Collections.Generic;
using AcsApi.Models;

namespace AcsApi
{
    public interface LoginDelegate
    {
        /// <summary>
        /// Informs the delegate that multiple identity providers were found and that the selector should be shown.
        /// </summary>
        /// <param name="identityProviders">Identity providers.</param>
        void ShowIdentityProviderSelector(List<IdentityProvider> identityProviders);

        /// <summary>
        /// Informs the delegate that the password flow should be used. This method is typically used for displaying 
        /// the password field.
        /// </summary>
        void ShouldBeginPasswordFlow();

        /// <summary>
        /// Informs the delegate that the external flow should be used. This method is typically used to begin 
        /// displaying a loading view or indicator.
        /// </summary>
        /// <param name="identityProvider">Identity provider that should be used.</param>
        void ShouldBeginExternalFlow(IdentityProvider identityProvider);

        /// <summary>
        /// Informs the delegate that the login flow was complete and an API Client was successfully created.
        /// </summary>
        /// <param name="apiClient">An API client used to authorize requests.</param>
        void Authenticated(IAcsApiClient apiClient);

        /// <summary>
        /// Informs the delegate that an error was encountered during the login flow. This method should implement all 
        /// cases of errors in <c>AcsApiError</c>.
        /// </summary>
        /// <param name="error">An AcsApiError describing the error that was encountered.</param>
        /// <param name="message">A message associated with the error</param>
        /// <param name="code">The code associated with the error</param>
        void EncounteredError(AcsApiError error, string message, int code = 0);
    }
}
