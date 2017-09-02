using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.V13.App;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class ExamsPage : ReactiveFragment<ExamsPageViewModel>
    {
        private ViewPager Pager { get; set; }

        private TabAdapter PagerAdapter { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.Exams, container, false);

            this.WireUpControls(layout);

            this.WhenAnyValue(x => x.ViewModel.Tabs).Subscribe(tabs =>
            {
                PagerAdapter = new TabAdapter(FragmentManager, tabs);
                Pager.Adapter = PagerAdapter;
            });

            return layout;
        }

        private class TabAdapter : FragmentStatePagerAdapter
        {
            private IReadOnlyList<ExamsTabViewModel> Items { get; }

            public TabAdapter(FragmentManager fm, IEnumerable<ExamsTabViewModel> items) : base(fm)
            {
                Items = items.ToList();
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(Items[position].SemesterName);
            }

            public override int Count => Items.Count;
            public override Fragment GetItem(int position)
            {
                ExamsTab fragment = new ExamsTab();
                fragment.ViewModel = Items[position];
                return fragment;
            }
        }
    }
}