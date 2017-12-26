using System;
using Android.Content;
using acs_apiclient.Android;
using Android.App;
using Android.OS;
using System.Threading;

namespace AcsApi
{
	public partial class LoginController
	{
        //        /// <summary>
        ///// Initiates the login flow from the specified UIViewController if SSO is enabled. Otherwise, 
        ///// the LoginController will inform its delegate to show the password field for the normal flow.
        ///// </summary>
        ///// <param name="context">
        ///// The UIViewController which will handle presenting the ExternalFlowWebViewController
        ///// </param>
        ///// <param name="identityProvider">The chosen identity provider.</param>
        //public override void BeginLoginFlow<T>(T context, IdentityProvider identityProvider)
        //{
        //    if (!(context is UIViewController))
        //    {
        //        throw new ArgumentException(
        //            $"{ typeof(T) } is not implemented, please make sure you are using a UIViewController for the " +
        //            "context parameter."
        //        );
        //    }
        //    var loginFlowContext = context as UIViewController;
        //    selectedIdentityProvider = identityProvider;
        //    if (identityProvider.IsSSOEnabled)
        //    {
        //        var externalLoginViewController = new ExternalFlowWebViewController(identityProvider.UrlString, this);
        //        var navigationController = new UINavigationController(externalLoginViewController);
        //        loginFlowContext.PresentViewController(navigationController, true, null);
        //    }
        //    else 
        //    {
        //        loginDelegate.ShouldBeginPasswordFlow();
        //    }
        //}
        
        public override void BeginLoginFlow<T>(T context, Models.IdentityProvider identityProvider)
        {
            if (!(context is Context))
            {
                throw new ArgumentException(
                    $"{ typeof(T) } is not implemented, please make sure you are using a Context for the " +
                    "context parameter."
                );
            }
            var loginFlowContext = context as Context;
            
            selectedIdentityProvider = identityProvider;
            if (identityProvider.IsSSOEnabled)
            {
                var webActivity = new Intent (loginFlowContext, typeof(ExternalFlowWebActivity));
                webActivity.PutExtra(ExternalFlowWebActivity.UrlParamKey, identityProvider.UrlString);
                loginFlowContext.StartActivity(webActivity);
            }
            else 
            {
                loginDelegate.ShouldBeginPasswordFlow();
            }
        }
        
        protected override void RunOnMainThread(Action action)
        {
            var handlerOnMainThread = new Handler(Looper.MainLooper);
            handlerOnMainThread.Post(() => { action(); });
        }
        
        protected override void RunOnBackgroundThread(Action action)
        {
            ThreadPool.QueueUserWorkItem(o => action());
        }
	}
}
