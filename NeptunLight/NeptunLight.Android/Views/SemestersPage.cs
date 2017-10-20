using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;
using ActionBar = Android.Support.V7.App.ActionBar;

namespace NeptunLight.Droid.Views
{
    public class SemestersPage : ReactiveFragment<SemestersPageViewModel>, IActionBarProvider
    {
        public ViewPager ViewPager { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.SemestersPage, container, false);

            this.WireUpControls(layout);

            this.WhenAny(x => x.ViewModel.CreditData, x => x.ViewModel.AveragesData, (cVm, aVm) => new TabsPagerAdapter(FragmentManager, cVm.Value, aVm.Value)).BindTo(this, x => x.ViewPager.Adapter);

            return layout;
        }

        private class TabsPagerAdapter : FragmentPagerAdapter
        {
            private readonly SemestersCreditsTab _creditsTab;
            private readonly SemestersAveragesTab _averagesTab;

            public TabsPagerAdapter(FragmentManager fm, SemestersCreditsTabViewModel creditsTabViewModel, SemestersAveragesTabViewModel averagesTabViewModel) : base(fm)
            {
                _creditsTab = new SemestersCreditsTab();
                _creditsTab.ViewModel = creditsTabViewModel;
                _averagesTab = new SemestersAveragesTab();
                _averagesTab.ViewModel = averagesTabViewModel;
            }

            public override int Count { get; } = 2;
            public override Fragment GetItem(int position)
            {
                switch (position)
                {
                    case 0:
                        return _creditsTab;
                    case 1:
                        return _averagesTab;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(position));
                }
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                switch (position)
                {
                    case 0:
                        return new Java.Lang.String("Kredit");
                    case 1:
                        return new Java.Lang.String("Átlagok");
                    default:
                        throw new ArgumentOutOfRangeException(nameof(position));
                }
            }
        }
        
    }
}