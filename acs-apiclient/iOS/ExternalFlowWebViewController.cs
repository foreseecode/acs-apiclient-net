using System;
using CoreGraphics;
using Foundation;
using UIKit;
using WebKit;

namespace AcsApi.iOS
{
    partial class ExternalFlowWebViewController : UIViewController
    {
        UIBarButtonItem dismissButton;
        UIBarButtonItem refreshButton;
        WKWebView webView;

        /// <summary>
        /// Gets the navigation bar of the view controller's navigation controller.
        /// </summary>
        /// <value>The navigation bar for this view controller.</value>
        UINavigationBar navigationBar => NavigationController.NavigationBar;

        /// <summary>
        /// The URL to load into the web view.
        /// </summary>
        NSUrl urlToLoad;

        /// <summary>
        /// The web view login delegate. This should always be the LoginController.
        /// </summary>
        ExternalFlowDelegate webViewLoginDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AcsApi.iOS.LoginFlowWebViewController"/> class.
        /// </summary>
        /// <param name="urlString">URL string.</param>
        /// <param name="webViewLoginDelegate">The calling LoginController.</param>
        public ExternalFlowWebViewController(string urlString, ExternalFlowDelegate webViewLoginDelegate)
        {
            urlToLoad = new NSUrl(urlString);
            this.webViewLoginDelegate = webViewLoginDelegate;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PrepareView();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            webView.LoadRequest(new NSUrlRequest(urlToLoad));
        }

        #region - View Preparation

        /// <summary>
        /// Prepares the view and its subviews.
        /// </summary>
        void PrepareView()
        {
            View.BackgroundColor = UIColor.White;

            PrepareNavigationBar();
            PrepareBackButton();
            PrepareRefreshButton();
            PrepareNavigationItem();
            PrepareWebView();
        }

        void PrepareNavigationBar()
        {
            var obsidianColor = new UIColor(34f / 255f, 43f / 255f, 60f / 255f, 1f);
            navigationBar.TintColor = obsidianColor;
            navigationBar.BackgroundColor = UIColor.White;
            navigationBar.Translucent = false;
            navigationBar.TitleTextAttributes = new UIStringAttributes {
                ForegroundColor = obsidianColor
            };
        }

        void PrepareBackButton()
        {
            var closeIcon = UIImage.FromFile("iOS/Images/ic_close-lib.png");
            dismissButton = new UIBarButtonItem(closeIcon, UIBarButtonItemStyle.Plain, (sender, e) => {
                webViewLoginDelegate.UserCancelledLogin();
                DismissViewController(true, null);
            });
        }

        void PrepareRefreshButton()
        {
            var refreshIcon = UIImage.FromBundle("iOS/Images/ic_sync-lib.png");
            refreshButton = new UIBarButtonItem(refreshIcon, UIBarButtonItemStyle.Plain, (sender, e) => {
				webView.LoadRequest(new NSUrlRequest(urlToLoad));            
            });
        }

        void PrepareNavigationItem()
        {
            NavigationItem.Title = urlToLoad.Host;
            NavigationItem.LeftBarButtonItem = dismissButton;
            NavigationItem.RightBarButtonItem = refreshButton;
        }

        void PrepareWebView()
        {
            webView = new WKWebView(CGRect.Empty, new WKWebViewConfiguration());
            webView.AllowsBackForwardNavigationGestures = false;
            webView.BackgroundColor = UIColor.White;
            webView.TranslatesAutoresizingMaskIntoConstraints = false;
            webView.NavigationDelegate = this;

            View.AddSubview(webView);

            webView.TopAnchor.ConstraintEqualTo(View.TopAnchor).Active = true;
            webView.LeftAnchor.ConstraintEqualTo(View.LeftAnchor).Active = true;
            webView.BottomAnchor.ConstraintEqualTo(View.BottomAnchor).Active = true;
            webView.RightAnchor.ConstraintEqualTo(View.RightAnchor).Active = true;

            webView.LayoutIfNeeded();
        }

        #endregion
    }

    partial class ExternalFlowWebViewController: IWKNavigationDelegate
    {
        /// <summary>
        /// Decides the policy to use for a new navigation action.
        /// </summary>
        /// <param name="webView">The web view which is requesting the navigation action.</param>
        /// <param name="navigationAction">The navigation action.</param>
        /// <param name="decisionHandler">The decision callback.</param>
        [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        public void DecidePolicy(
            WKWebView webView,
            WKNavigationAction navigationAction,
            Action<WKNavigationActionPolicy> decisionHandler
        ) {
            try
            {
                InvokeInBackground(() => {
					if (webViewLoginDelegate.ShouldInterceptRequest(navigationAction.Request.Url.AbsoluteString))
					{
						InvokeOnMainThread(() => {
                            decisionHandler(WKNavigationActionPolicy.Cancel);
                            DismissViewController(true, () => {
                                InvokeInBackground(() => {
                                    webViewLoginDelegate.RetrievedAuthCode();    
                                });
                            });
						});
                    }
					else
					{
                        InvokeOnMainThread(() => decisionHandler(WKNavigationActionPolicy.Allow));
					} 
                });
			}
            catch (AcsApiException exception)
            {
                InvokeOnMainThread(() => webViewLoginDelegate.EncounteredError(exception.ErrorCode, exception.Message));
            }
            catch (Exception exception)
            {
                InvokeOnMainThread(() => webViewLoginDelegate.EncounteredError(AcsApiError.Other, exception.Message));
            }
        }
    }
}
