
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using acs_apiclient.Droid.CustomViews;
using Android.Widget;
using System.Threading;
using AcsApi;
using Android.Graphics;

namespace acs_apiclient.Droid
{
    [Activity(Label = "ExternalFlowWebActivity", Theme = "@android:style/Theme.NoTitleBar")]
    public class ExternalFlowWebActivity : Activity
    {
        public static string UrlParamKey = "url";
        private static Android.Graphics.Color Obsidian = Android.Graphics.Color.ParseColor("#222b3c");
        private static Android.Graphics.Color Ash = Android.Graphics.Color.ParseColor("#B7C0CC");
        private static Android.Graphics.Color SelectedButtonTintColor = Ash;
        private static Android.Graphics.Color UnselectedButtonTintColor = Obsidian;
        private WebView contentWebView;
        private String urlString;
        private Android.Net.Uri uri;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
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
                throw new ArgumentException($"'url' must be provided in the Intent to start {typeof(ExternalFlowWebActivity)}");
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
            this.contentWebView.SetWebViewClient(new ExternalFlowWebViewClient(this));
            this.contentWebView.LoadUrl(this.urlString);
        }
        
        public class ExternalFlowWebViewClient : WebViewClient
        {
            Activity activity;
            private string pendingUrl;

            public ExternalFlowWebViewClient(Activity activity)
            {
                this.activity = activity;
            }
            
            public override void OnPageStarted(WebView view, String url, Bitmap favicon)
            {
                if (pendingUrl == null) {
                    pendingUrl = url;
                }
            }
            
            public override void OnPageFinished(WebView view, String url)
            {
                bool urlRedirectDetected = !url.Equals(pendingUrl);
                if (urlRedirectDetected) 
                {
                    try
                    {
                        if (LoginController.Instance.ShouldInterceptRequest(url))
                        {
                            ThreadPool.QueueUserWorkItem((object state) =>
                            {
                                LoginController.Instance.RetrievedAuthCode();
                            });
                            
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
                    
                    pendingUrl = null;
                }
            }                  
        }
    }
}
