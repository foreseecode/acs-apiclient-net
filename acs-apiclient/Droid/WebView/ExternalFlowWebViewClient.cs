using Android.Webkit;

namespace acs_apiclient.Droid
{
    public class ExternalFlowWebViewClient : WebViewClient
    {
        public override bool ShouldOverrideUrlLoading(WebView view, string url)
        {
            view.LoadUrl(url);
            return false;
        }
    }
}