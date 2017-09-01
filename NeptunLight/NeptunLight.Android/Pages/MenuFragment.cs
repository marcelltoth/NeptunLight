using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Pages
{
    public class MenuFragment : ReactiveFragment<MenuPageViewModel>
    {
        private ProgressDialog _loadingDialog;

        public ProgressDialog LoadingDialog
        {
            get => _loadingDialog;
            set => this.RaiseAndSetIfChanged(ref _loadingDialog, value);
        }

        public Button CalendarButton { get; set; }
        public Button MessagesButton { get; set; }
        public Button PeriodsButton { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.Menu, container, false);

            this.Activated.InvokeCommand(ViewModel.EnsureDataAccessible);
            this.WireUpControls(layout);

            LoadingDialog = new ProgressDialog(Activity);
            LoadingDialog.SetCancelable(false);
            LoadingDialog.SetTitle("Szinkronizáció");

            this.WhenAnyValue(x => x.ViewModel.LoadingDialogText).Subscribe(text => LoadingDialog.SetMessage(text));
            this.WhenAnyValue(x => x.ViewModel.LoadingDialogShown).Subscribe(shown =>
            {
                if(shown)
                    LoadingDialog.Show();
                else
                    LoadingDialog.Dismiss();
            });

            this.BindCommand(ViewModel, x => x.GoToCalendar, x => x.CalendarButton);
            this.BindCommand(ViewModel, x => x.GoToMessages, x => x.MessagesButton);
            this.BindCommand(ViewModel, x => x.GoToPeriods, x => x.PeriodsButton);

            return layout;
        }
    }
}