using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V13.App;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class ExamsPage : ReactiveFragment<ExamsPageViewModel>, IActionBarProvider
    {
        private ViewPager Pager { get; set; }

        private TabAdapter PagerAdapter { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.ExamsPage, container, false);

            this.WireUpControls(layout);

            this.WhenAnyValue(x => x.ViewModel.Tabs).Subscribe(tabs =>
            {
                PagerAdapter = new TabAdapter(FragmentManager, tabs);
                Pager.Adapter = PagerAdapter;
            });

            return layout;
        }

        private class TabAdapter : TabListAdapter<ExamsTabViewModel, ExamsTab>
        {
            public TabAdapter(FragmentManager fm, IEnumerable<ExamsTabViewModel> items) : base(fm, items)
            {
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(Items[position].SemesterName);
            }
        }
    }
}