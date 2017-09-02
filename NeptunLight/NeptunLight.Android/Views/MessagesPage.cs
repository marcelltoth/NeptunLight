using System;
using System.Reactive.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class MessagesPage : ReactiveFragment<MessagesPageViewModel>
    {
        private LayoutInflater _layoutInflater;
        public ListView MessageList { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _layoutInflater = inflater;
            View layout = inflater.Inflate(Resource.Layout.Messages, container, false);

            this.WireUpControls(layout);

            return layout;
        }

        public override void OnStart()
        {
            base.OnStart();


            this.WhenAnyValue(x => x.ViewModel.Messages).Subscribe(items =>
            {
                var messageListAdapter = new ListAdapter<MessageViewModel>(_layoutInflater, items, Resource.Layout.MessageListItem, (itemView, model) =>
                {
                    itemView.FindViewById<TextView>(Resource.Id.titleTextView).Text = model.Subject;
                    itemView.FindViewById<TextView>(Resource.Id.senderTextView).Text = model.Sender;
                    itemView.FindViewById<TextView>(Resource.Id.letterBox).Text = model.SenderCode;
                });
                MessageList.Adapter = messageListAdapter;
                MessageList.Events().ItemClick.Select(args => messageListAdapter[args.Position]).InvokeCommand(this, x => x.ViewModel.OpenMessage);
            });
        }
    }
}