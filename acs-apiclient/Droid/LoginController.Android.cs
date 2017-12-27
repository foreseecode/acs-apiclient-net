using System;
using Android.Content;
using acs_apiclient.Droid;
using Android.App;
using Android.OS;
using System.Threading;
using System.Runtime.Serialization;
using Android.Runtime;

namespace AcsApi
{
    public partial class LoginController
    {
        public override void BeginLoginFlow<T>(T context, Models.IdentityProvider identityProvider)
        {
            if (!(context is Activity))
            {
                throw new ArgumentException(
                    $"{ typeof(T) } is not implemented, please make sure you are using a Activity for the " +
                    "context parameter."
                );
            }

            var loginActivity = context as Activity;
            selectedIdentityProvider = identityProvider;

            if (identityProvider.IsSSOEnabled)
            {
                var webViewActivityIntent = new Intent(loginActivity, typeof(ExternalFlowWebActivity));
                webViewActivityIntent.PutExtra(ExternalFlowWebActivity.UrlParamKey, identityProvider.UrlString);
                loginActivity.StartActivity(webViewActivityIntent);
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
            ThreadPool.QueueUserWorkItem(calback => action());
        }
    }
}
