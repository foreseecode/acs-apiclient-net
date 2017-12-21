using System;
using AcsApi.Models;

namespace AcsApi
{
    public abstract class PlatformLoginController
    {
        /// <summary>
        /// Platform specific implementation to launch and begin the external flow.
        /// </summary>
        /// <param name="context">The context from which the external flow will launch.</param>
        /// <param name="identityProvider">The identity provider to use.</param>
        /// <typeparam name="T">The context type. This is UIViewController on iOS and Activity on Android.</typeparam>
        public abstract void BeginLoginFlow<T>(T context, IdentityProvider identityProvider);

        /// <summary>
        /// Runs an action on the main thread.
        /// </summary>
        /// <param name="action">The work to be run on the main thread, wrapped in an Action.</param>
        protected abstract void RunOnMainThread(Action action);

        /// <summary>
        /// Runs an action on a background thread.
        /// </summary>
        /// <param name="action">The work to be run on a background thread, wrapped in an Action.</param>
        protected abstract void RunOnBackgroundThread(Action action);
    }
}
