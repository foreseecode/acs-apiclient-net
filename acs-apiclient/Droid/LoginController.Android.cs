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
    public partial class LoginController : IParcelable
    {
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
                var webActivity = new Intent(loginFlowContext, typeof(ExternalFlowWebActivity));
                webActivity.PutExtra(ExternalFlowWebActivity.UrlParamKey, identityProvider.UrlString);
                webActivity.PutExtra(ExternalFlowWebActivity.DelegateParamKey, this);
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
            ThreadPool.QueueUserWorkItem(calback => action());
        }
        
        IntPtr IJavaObject.Handle => throw new NotImplementedException();

        public int DescribeContents()
        {
            return 0;
        }

        public void Dispose()
        {
        }

        public void WriteToParcel(Parcel dest, [GeneratedEnum] ParcelableWriteFlags flags)
        {
        }

        int IParcelable.DescribeContents()
        {
            return 0;
        }

        void IParcelable.WriteToParcel(Parcel dest, ParcelableWriteFlags flags)
        {
        }
    }
}
