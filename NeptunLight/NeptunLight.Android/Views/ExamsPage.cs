using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;
using JetBrains.Annotations;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class ExamsPage : ReactiveFragment<ExamsPageViewModel>, IActionBarProvider
    {
        private ViewPager Pager { get; [UsedImplicitly] set; }

        private TabAdapter PagerAdapter { get; set; }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.ExamsPage, container, false);

            this.WireUpControls(layout);

            this.WhenAnyValue(x => x.ViewModel.Tabs).Subscribe(tabs =>
            {
                PagerAdapter = new TabAdapter(ChildFragmentManager, tabs);
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