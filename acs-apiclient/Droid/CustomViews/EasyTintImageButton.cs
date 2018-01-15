using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace acs_apiclient.Droid.CustomViews
{
    /**
    * This class exists because android:tint requires a minimum of Android level 21. However, we are required to support Android level 19
    * at the moment. See https://developer.android.com/reference/android/widget/ImageView.html#attr_android:tint
    **/
    public class EasyTintImageButton : ImageButton
    {
        public static Color NoTint = ImageViewExtension.NoTint;
        private const bool TouchEventNotConsumed = false;
        private Color _selectedColor = NoTint;
        private Color _unselectedColor = NoTint;
        private View.IOnTouchListener touchListenerFromCaller;

        public EasyTintImageButton(Context context) : base(context)
        {
            Init();
        }

        public EasyTintImageButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Init();
        }

        public EasyTintImageButton(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            Init();
        }

        public Color SelectedTintColor
        {
            set
            {
                _selectedColor = value;
                AdjustTintToSelection();
            }
        }

        public Color UnSelectedTintColor
        {
            set
            {
                _unselectedColor = value;
                AdjustTintToSelection();
            }
        }

        public override bool Selected
        {
            get
            {
                return base.Selected;
            }
            set
            {
                base.Selected = value;
                AdjustTintToSelection();
            }
        }
        
        public override void SetOnTouchListener(View.IOnTouchListener listener)
        {
            this.touchListenerFromCaller = listener;
        }
        
        private void Init()
        {
            base.SetOnTouchListener(new ToggleOnSelectedListener());
            base.Clickable = true;
        }

        private void AdjustTintToSelection()
        {
            Color newColor = this.Selected ? _selectedColor : _unselectedColor;
            this.Background.AdjustColorFilter(newColor);
        }
        
        public class ToggleOnSelectedListener : Java.Lang.Object, View.IOnTouchListener {

            public bool OnTouch(View view, MotionEvent motionEvent)
            {
                bool isValidView = view != null && view is EasyTintImageButton;
                if (!isValidView)
                {
                    return TouchEventNotConsumed;
                }
    
                EasyTintImageButton button = view as EasyTintImageButton;
                button.Selected = (motionEvent.Action == MotionEventActions.Down);
                
                if(motionEvent.Action == MotionEventActions.Up)
                {
                    button.PerformClick();
                }
                
                if(button.touchListenerFromCaller == null)
                {
                    return TouchEventNotConsumed;
                }
                else
                {
                    return button.touchListenerFromCaller.OnTouch(view, motionEvent);
                }
            }
        }
    }
}