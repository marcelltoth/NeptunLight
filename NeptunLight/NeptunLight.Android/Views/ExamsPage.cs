using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Java.Lang;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;
using ReactiveUI.AndroidSupport;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace NeptunLight.Droid.Views
{
    public class ExamsPage : ReactiveUI.AndroidSupport.ReactiveFragment<ExamsPageViewModel>, IActionBarProvider
    {
        public ExamsPage()
        {
            this.WhenActivated(cd =>
            {
                this.WhenAnyValue(x => x.ViewModel.Tabs).Subscribe(tabs =>
                {
                    PagerAdapter = new TabAdapter(ChildFragmentManager, tabs);
                    Pager.Adapter = PagerAdapter;
                }).DisposeWith(cd);
            });
        }

        private ViewPager Pager { get; [UsedImplicitly] set; }

        private TabAdapter PagerAdapter { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Analytics.TrackEvent("Exams page shown");
        }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.ExamsPage, container, false);

            this.WireUpControls(layout);

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