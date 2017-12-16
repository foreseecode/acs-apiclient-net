//using System;

namespace AcsApi
{
    interface ExternalFlowDelegate
    {
        /// <summary>
        /// Determines if the URL should be intercepted.
        /// </summary>
        /// <returns><c>true</c>, if the request should be intercepted, <c>false</c> otherwise.</returns>
        /// <exception cref="AcsApiException">
        /// Throws an AcsApiException if an error is encoutered while determining if the request should be intercepted. 
        /// This is usually done to cancel the login flow.
        /// </exception>
        /// <param name="urlString">URL string.</param>
        bool ShouldInterceptRequest(string urlString);

        /// <summary>
        /// Used to inform the delegate that the user cancelled the login flow.
        /// </summary>
        void DidCancelLoginProcess();

        /// <summary>
        /// Used to inform the delegate that the callback was intercepted, and a code was successfully received.
        /// </summary>
        void DidRetrieveCodeSuccessfully();

        /// <summary>
        /// Used to inform the delegate of any errors encountered during the login flow.
        /// </summary>
        /// <param name="error">An AcsApiError describing the error that was encountered.</param>
        void DidEncounterError(AcsApiError error);
    }
}
