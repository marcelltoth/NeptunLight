using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using NeptunLight.Droid.Utils;
using NeptunLight.Models;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Pages
{
    public class MessagesFragment : ReactiveFragment<MessagesPageViewModel>
    {
        public ListView MessageList { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.Messages, container, false);

            this.WireUpControls(layout);

            this.WhenAnyValue(x => x.ViewModel.Messages).Subscribe(items =>
            {
                var messageListAdapter = new ListAdapter<MessageViewModel>(inflater, items, Resource.Layout.MessageListItem, (itemView, model) =>
                {
                    itemView.FindViewById<TextView>(Resource.Id.titleTextView).Text = model.Title;
                    itemView.FindViewById<TextView>(Resource.Id.senderTextView).Text = model.Sender;
                    itemView.FindViewById<TextView>(Resource.Id.letterBox).Text = model.SenderCode;
                });
                MessageList.Adapter = messageListAdapter;
                MessageList.Events().ItemClick.Select(args => messageListAdapter[args.Position]).InvokeCommand(this, x => x.ViewModel.OpenMessage);
            });

            return layout;
        }
    }
}