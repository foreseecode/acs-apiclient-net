
using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace acs_apiclient.Android
{
    [Activity(Label = "ExternalFlowWebActivity", Theme = "@android:style/Theme.NoTitleBar")]
    public class ExternalFlowWebActivity : Activity
    {
        public static string UrlParamKey = "url";
        
        private WebView contentWebView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            this.SetContentView(Resource.Layout.activity_externalFlowWeb);
            SetupWebView();
        }

        private void SetupWebView()
        {
            contentWebView = FindViewById<WebView>(Resource.Id.webview_externalFlowWeb_content);
            contentWebView.Settings.JavaScriptEnabled = true;
            contentWebView.SetWebViewClient(new ExternalFlowWebViewClient());
            contentWebView.LoadUrl("http://www.google.com");
        }
    }
}
