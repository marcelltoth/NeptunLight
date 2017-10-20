using System;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class MessageDetailsPage : ReactiveFragment<MessageViewModel>, IActionBarContentProvider
    {
        private TextView TimeTextView { get; set; }
        private TextView SenderTextView { get; set; }
        private TextView SubjectTextView { get; set; }
        private TextView ContentTextView { get; set; }
        private TextView LetterBox { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.MessageDetailsPage, container, false);

            this.WireUpControls(layout);

            this.OneWayBind(ViewModel, x => x.Date, x => x.TimeTextView.Text, dt => dt.ToString("g"));
            this.OneWayBind(ViewModel, x => x.Sender, x => x.SenderTextView.Text);
            this.OneWayBind(ViewModel, x => x.Subject, x => x.SubjectTextView.Text);
            this.OneWayBind(ViewModel, x => x.SenderCode, x => x.LetterBox.Text);
            this.WhenAnyValue(x => x.ViewModel.Sender).Subscribe(s =>
            {
                GradientDrawable bgShape = (GradientDrawable)LetterBox.Background;
                bgShape.SetColor(MessageColorPool.Instance[s]);
            });
            this.WhenAnyValue(x => x.ViewModel.HtmlContent).Subscribe(html =>
            {
                if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    ContentTextView.TextFormatted = Html.FromHtml(html, FromHtmlOptions.ModeLegacy);
                }
                else
                {
                    ContentTextView.TextFormatted = Html.FromHtml(html);
                }
            });
            ContentTextView.MovementMethod = LinkMovementMethod.Instance;

            return layout;
        }
    }
}