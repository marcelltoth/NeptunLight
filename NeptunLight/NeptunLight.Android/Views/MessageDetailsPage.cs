using System;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using NeptunLight.ViewModels;
using ReactiveUI;
using ReactiveUI.AndroidSupport;

namespace NeptunLight.Droid.Views
{
    public class MessageDetailsPage : ReactiveUI.AndroidSupport.ReactiveFragment<MessageViewModel>, IActionBarProvider
    {
        private TextView TimeTextView { get; [UsedImplicitly] set; }
        private TextView SenderTextView { get; [UsedImplicitly] set; }
        private TextView SubjectTextView { get; [UsedImplicitly] set; }
        private TextView ContentTextView { get; [UsedImplicitly] set; }
        private TextView LetterBox { get; [UsedImplicitly] set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Analytics.TrackEvent("Message opened");
        }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
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
#pragma warning disable 618 // obsolete symbol
                ContentTextView.TextFormatted = Build.VERSION.SdkInt >= BuildVersionCodes.N ? Html.FromHtml(html, FromHtmlOptions.ModeLegacy) : Html.FromHtml(html);
#pragma warning restore 618
            });
            ContentTextView.MovementMethod = LinkMovementMethod.Instance;

            return layout;
        }
    }
}