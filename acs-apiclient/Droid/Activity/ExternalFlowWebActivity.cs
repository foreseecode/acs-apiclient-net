using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using acs_apiclient.Droid.CustomViews;
using Android.Widget;
using System.Threading;
using AcsApi;

namespace acs_apiclient.Droid
{
    [Activity(Label = "ExternalFlowWebActivity", 
    Theme = "@android:style/Theme.NoTitleBar", 
    WindowSoftInputMode = SoftInput.AdjustResize)]
    public class ExternalFlowWebActivity : Activity
    {
        public static string UrlParamKey = "url";
        static Android.Graphics.Color SelectedButtonTintColor = Android.Graphics.Color.ParseColor(Colors.Ash);
        static Android.Graphics.Color UnselectedButtonTintColor = Android.Graphics.Color.ParseColor(Colors.Obsidian);
        WebView contentWebView;
        String urlString;
        Android.Net.Uri uri;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.activity_externalFlowWeb);
            InitUri();
            SetupTitle(uri.Host);
            SetupCloseButton();
            SetupSyncButton();
            SetupWebView();
        }

        private void InitUri()
        {
            this.urlString = this.Intent.GetStringExtra(UrlParamKey);
            if (String.IsNullOrEmpty(this.urlString))
            {
                throw new ArgumentException($"A URL must be provided for the '{UrlParamKey} key in the Intent to start {typeof(ExternalFlowWebActivity)}");
            }
            this.uri = Android.Net.Uri.Parse(this.urlString);
        }

        private void SetupTitle(string newTitle)
        {
            TextView title = FindViewById<TextView>(Resource.Id.textview_titlebarWithExitAndRefreshButtons_title);
            title.Text = newTitle;
        }

        private void SetupSyncButton()
        {
            EasyTintImageButton syncButton = FindViewById<EasyTintImageButton>(Resource.Id.easyTintImageButton_titlebarWithExitAndRefreshButtons_sync);
            syncButton.SelectedTintColor = SelectedButtonTintColor;
            syncButton.UnSelectedTintColor = UnselectedButtonTintColor;
            syncButton.Click += (object sender, EventArgs e) => 
            {
                this.contentWebView.LoadUrl(this.urlString);
            };
        }

        private void SetupCloseButton()
        {
            EasyTintImageButton closeButton = FindViewById<EasyTintImageButton>(Resource.Id.easyTintImageButton_titlebarWithExitAndRefreshButtons_close);
            closeButton.SelectedTintColor = SelectedButtonTintColor;
            closeButton.UnSelectedTintColor = UnselectedButtonTintColor;
            closeButton.Click += (object sender, EventArgs e) => 
            {
                LoginController.Instance.UserCancelledLogin();
                Finish();
            };
        }

        private void SetupWebView()
        {
            this.contentWebView = FindViewById<WebView>(Resource.Id.webview_externalFlowWeb_content);
            this.contentWebView.Settings.JavaScriptEnabled = true;
            this.contentWebView.Settings.DomStorageEnabled = true;
            this.contentWebView.SetWebViewClient(new ExternalFlowWebViewClient(this));
            this.contentWebView.LoadUrl(this.urlString);
        }
        
        public class ExternalFlowWebViewClient : WebViewClient
        {
            Activity activity;

            public ExternalFlowWebViewClient(Activity activity)
            {
                this.activity = activity;
            }
            
            /**
            * Use shouldOverrideUrlLoading(WebView, WebResourceRequest) instead when the Android app's min SDK is increased to >= 24
            * https://developer.android.com/reference/android/webkit/WebViewClient.html#shouldOverrideUrlLoading(android.webkit.WebView, android.webkit.WebResourceRequest)
            */
            public override bool ShouldOverrideUrlLoading(WebView webview, String url)
            {
                bool shouldOverrideUrl = false;
                try
                {
                    if (LoginController.Instance.ShouldInterceptRequest(url))
                    {
                        ThreadPool.QueueUserWorkItem((object state) =>
                        {
                            LoginController.Instance.RetrievedAuthCode();
                        });
                        shouldOverrideUrl = true;
                        this.activity.Finish();
                    }
                }
                catch (AcsApiException exception)
                {
                    this.activity.RunOnUiThread(() => LoginController.Instance.EncounteredError(exception.ErrorCode, exception.Message));
                }
                catch (Exception exception)
                {
                    this.activity.RunOnUiThread(() => LoginController.Instance.EncounteredError(AcsApiError.Other, exception.Message));
                }

                return shouldOverrideUrl;
            }                
        }
    }
}
