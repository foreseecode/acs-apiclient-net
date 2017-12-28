using System.Threading;
using Android.Webkit;
using AcsApi;

namespace acs_apiclient.Droid
{
    public class ExternalFlowWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
        
            // try
            //{
            //    InvokeInBackground(() => {
            //        if (webViewLoginDelegate.ShouldInterceptRequest(navigationAction.Request.Url.AbsoluteString))
            //        {
            //            InvokeOnMainThread(() => {
            //                decisionHandler(WKNavigationActionPolicy.Cancel);
            //                DismissViewController(true, () => {
            //                    InvokeInBackground(() => {
            //                        webViewLoginDelegate.RetrievedAuthCode();    
            //                    });
            //                });
            //            });
            //        }
            //        else
            //        {
            //            InvokeOnMainThread(() => decisionHandler(WKNavigationActionPolicy.Allow));
            //        } 
            //    });
            //}
            //catch (AcsApiException exception)
            //{
            //    InvokeOnMainThread(() => webViewLoginDelegate.EncounteredError(exception.ErrorCode, exception.Message));
            //}
            //catch (Exception exception)
            //{
            //    InvokeOnMainThread(() => webViewLoginDelegate.EncounteredError(AcsApiError.Other, exception.Message));
            //}
            
            ThreadPool.QueueUserWorkItem ((object state) => {
                if (LoginController.Instance.ShouldInterceptRequest(url))
                {
                    
                }
            });
            view.LoadUrl(url);
            return false;
        }
    }
}