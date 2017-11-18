using System;
using Android.App;
using Android.OS;
using Android.Support.V13.App;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class SemestersPage : ReactiveFragment<SemestersPageViewModel>, IActionBarProvider
    {
        public ViewPager ViewPager { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Analytics.TrackEvent("Semesters page shown");
        }

        public override View OnCreateView([NotNull] LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.SemestersPage, container, false);

            this.WireUpControls(layout);

            this.WhenAny(x => x.ViewModel.CreditData, x => x.ViewModel.AveragesData, (cVm, aVm) => new TabsPagerAdapter(ChildFragmentManager, cVm.Value, aVm.Value)).Subscribe(adapter =>
            {
                ViewPager.Adapter = adapter;
                ViewPager.Adapter.NotifyDataSetChanged();
            });

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