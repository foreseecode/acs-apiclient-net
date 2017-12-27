
using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using acs_apiclient.Droid.CustomViews;
using Android.Widget;
using Android.Graphics;
using System.Drawing;
using AcsApi;
using Android.Content;

namespace acs_apiclient.Droid
{
    [Activity(Label = "ExternalFlowWebActivity", Theme = "@android:style/Theme.NoTitleBar")]
    public class ExternalFlowWebActivity : Activity
    {
        public static string UrlParamKey = "url";
        private static Android.Graphics.Color ObsidianGrey = Android.Graphics.Color.ParseColor("#222b3c");
        private WebView contentWebView;
        private String url;
        private ExternalFlowDelegate externalFlowDelegate;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            this.SetContentView(Resource.Layout.activity_externalFlowWeb);
            this.url = this.Intent.GetStringExtra(UrlParamKey);
            
            SetupTitle(url);
            SetupCloseButton();
            SetupSyncButton();
            SetupWebView();
        }

        private void SetupTitle(string newTitle)
        {
            TextView title = FindViewById<TextView>(Resource.Id.textview_toolbarContainer_title);
            title.Text = newTitle;
        }

        private void SetupSyncButton()
        {
            EasyTintImageButton syncButton = FindViewById<EasyTintImageButton>(Resource.Id.easyTintImageButton_titlebarWithExitAndRefereshButtons_sync);
            syncButton.SelectedTintColor = ObsidianGrey;
            syncButton.Click += (object sender, EventArgs e) => 
            {
                this.contentWebView.LoadUrl(this.url);
            };
        }

        private void SetupCloseButton()
        {
            EasyTintImageButton closeButton = FindViewById<EasyTintImageButton>(Resource.Id.easyTintImageButton_titlebarWithExitAndRefereshButtons_sync);
            closeButton.SelectedTintColor = ObsidianGrey;
            closeButton.Click += (object sender, EventArgs e) => 
            {
                externalFlowDelegate.UserCancelledLogin();
                Finish();
            };
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
