using System;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace acs_apiclient.Android.CustomViews
{
    public static class DrawableExtension
    {
        public static Color NO_TINT = Color.Rgb(-1, -1, -1);
        
        public static void AdjustColorFilter(this Drawable drawable, Color newColor)
        {
            if (newColor.Equals(NO_TINT))
            {
                drawable.ClearColorFilter();
                return;
            }
    
            drawable.SetColorFilter(newColor, PorterDuff.Mode.SrcAtop);
        }
    }
}