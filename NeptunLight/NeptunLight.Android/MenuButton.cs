using System;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace NeptunLight.Droid
{
    public sealed class MenuButton : LinearLayout
    {
        public MenuButton(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Clickable = true;
            Focusable = true;
            Orientation = Orientation.Vertical;
            Background = Resources.GetDrawable(Resource.Drawable.menu_button_background, context.Theme);
            SetPadding(DpToPx(15), DpToPx(15), DpToPx(15), DpToPx(15));
            SetGravity(GravityFlags.Fill);

            FrameLayout iconHolder = new FrameLayout(context);
            ImageView icon = new ImageView(context);
            iconHolder.AddView(icon, DpToPx(40), DpToPx(40));
            icon.SetScaleType(ImageView.ScaleType.CenterInside);
            icon.SetImageResource(attrs.GetAttributeResourceValue("http://schemas.android.com/apk/res/android", "src", Android.Resource.Drawable.IcMenuAdd));
            FrameLayout.LayoutParams iconLayout = (FrameLayout.LayoutParams) icon.LayoutParameters;
            iconLayout.Gravity = GravityFlags.Left | GravityFlags.Top;
            AddView(iconHolder);

            

            TextView text = new TextView(context);
            text.Text = attrs.GetAttributeValue("http://schemas.android.com/apk/res/android", "text");
            text.Gravity = GravityFlags.Bottom | GravityFlags.Left;
            text.SetTextColor(Color.Black);
            AddView(text);
            LayoutParams param = (LayoutParams)text.LayoutParameters;
            param.Gravity = GravityFlags.Left;
            param.Weight = 1;
        }

        private int DpToPx(int dp)
        {
            return (int) Math.Round(dp * Resources.DisplayMetrics.Density);
        }
    }
}