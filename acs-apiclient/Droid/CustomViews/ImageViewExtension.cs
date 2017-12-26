using System;
using Android.Graphics;
using Android.Widget;

namespace acs_apiclient.Droid.CustomViews
{
    public static class ImageViewExtension
    {
        public static Color NoTint = Color.Rgb(-1, -1, -1);

        public static void AdjustColorFilter(this ImageView imageView, Color newColor)
        {
            if (newColor.Equals(NoTint))
            {
                imageView.ClearColorFilter();
                return;
            }
    
            imageView.SetColorFilter(newColor, PorterDuff.Mode.SrcAtop);
        }
    }
}