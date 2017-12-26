
using Android.App;
using Android.OS;
using Android.Views;

namespace acs_apiclient.Android
{
    [Activity(Label = "ExternalFlowWebActivity")]
    public class ExternalFlowWebActivity : Activity
    {
        public static string UrlParamKey = "url";
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            
        }
    }
}
