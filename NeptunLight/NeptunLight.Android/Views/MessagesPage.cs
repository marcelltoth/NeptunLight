using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class MessagesPage : ReactiveFragment<MessagesPageViewModel>, IActionBarProvider, SwipeRefreshLayout.IOnRefreshListener
    {
        private LayoutInflater _layoutInflater;
        public ListView MessageList { get; set; }
        private IDisposable _messageClickSubscription;

        public SwipeRefreshLayout SwipeRefresh { get; set; }

        public override View OnCreateView([NotNull] LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            _layoutInflater = inflater;
            View layout = inflater.Inflate(Resource.Layout.MessagesPage, container, false);

            this.WireUpControls(layout);

            SwipeRefresh.SetOnRefreshListener(this);

            return layout;
        }

        public override void OnStart()
        {
            base.OnStart();


            ViewModel.Messages.Changed
                .ObserveOn(Application.SynchronizationContext)
                .Subscribe(items => { RefreshAdapter(); });
            RefreshAdapter();

            this.WhenAnyValue(x => x.ViewModel).Subscribe(vm =>
            {
                vm.RefreshMessages.ThrownExceptions.Subscribe(ex =>
                {
                    AlertDialog.Builder dialog = new AlertDialog.Builder(Activity);
                    dialog.SetMessage("Hiba történt a frissítés során. Ellenőrizd az internetkapcsolatodat és próbáld újra.");
                    dialog.SetTitle("Hiba");
                    dialog.SetPositiveButton("Ok", (s, e) => {});
                    dialog.SetCancelable(true);
                    dialog.Create().Show();
                });
            });
            
            this.OneWayBind(ViewModel, x => x.IsRefreshing, x => x.SwipeRefresh.Refreshing);
        }

        private void RefreshAdapter()
        {
            var messageListAdapter = new ListAdapter<MessageViewModel>(_layoutInflater, ViewModel.Messages, Resource.Layout.MessageListItem, (itemView, model) =>
            {
                itemView.FindViewById<TextView>(Resource.Id.titleTextView).Text = model.Subject;
                itemView.FindViewById<TextView>(Resource.Id.senderTextView).Text = model.Sender;
                TextView letterBox = itemView.FindViewById<TextView>(Resource.Id.letterBox);
                letterBox.Text = model.SenderCode;
                GradientDrawable bgShape = (GradientDrawable) letterBox.Background;
                bgShape.SetColor(MessageColorPool.Instance[model.Sender]);
            });
            MessageList.Adapter = messageListAdapter;
            _messageClickSubscription?.Dispose();
            _messageClickSubscription = MessageList.Events().ItemClick.Select(args =>
            {
                return messageListAdapter[args.Position];
            }).InvokeCommand(this, x => x.ViewModel.OpenMessage);
        }

        public void OnRefresh()
        {
            ((ICommand)ViewModel.RefreshMessages).Execute(null);
        }

        public override void OnPause()
        {
            base.OnPause();
            
            SwipeRefresh.DestroyDrawingCache();
            SwipeRefresh.ClearAnimation();
        }
    }
}