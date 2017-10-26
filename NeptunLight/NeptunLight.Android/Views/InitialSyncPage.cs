using Android.OS;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using NeptunLight.ViewModels;
using ReactiveUI;
using GridLayout = Android.Support.V7.Widget.GridLayout;

namespace NeptunLight.Droid.Views
{
    public class InitialSyncPage : ReactiveFragment<InitialSyncPageViewModel>
    {
        public GridLayout StatusPanel { get; set; }

        public Button StartButton { get; set; }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.InitialSyncPage, container, false);

            this.WireUpControls(layout);

            StartButton.Alpha = 1;
            StatusPanel.Alpha = 0;

            this.BindCommand(ViewModel, x => x.)

            return layout;
        }
    }
}