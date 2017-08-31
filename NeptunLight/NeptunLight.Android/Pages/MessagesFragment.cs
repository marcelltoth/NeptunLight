using Android.OS;
using Android.Views;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Pages
{
    public class MessagesFragment : ReactiveFragment<MessagesPageViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.Messages, container, false);

            this.WireUpControls(container);

            return layout;
        }
    }
}