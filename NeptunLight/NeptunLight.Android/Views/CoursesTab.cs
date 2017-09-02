using Android.OS;
using Android.Views;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class CoursesTab : ReactiveFragment<CoursesTabViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.CoursesTab, container, false);

            this.WireUpControls(layout);

            return layout;
        }
    }
}