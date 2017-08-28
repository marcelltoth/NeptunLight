using Android.OS;
using Android.Views;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Pages
{
    public class MenuFragment : ReactiveFragment<MenuPageViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.Menu, container, false);

            return layout;
        }
    }
}