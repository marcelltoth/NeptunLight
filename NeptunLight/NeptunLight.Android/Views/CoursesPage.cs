using System;
using System.Collections.Generic;
using Android.App;
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
    public class CoursesPage : ReactiveUI.AndroidSupport.ReactiveFragment<CoursesPageViewModel>, IActionBarProvider
    {
        private ViewPager Pager { get; [UsedImplicitly] set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Analytics.TrackEvent("Courses page shown");

            this.WhenActivated(cd =>
            {
                cd.Add(this.WhenAnyValue(x => x.ViewModel.Tabs).Subscribe(tabs =>
                {
                    Pager.Adapter = new TabAdapter(ChildFragmentManager, tabs);
                }));
            });
        }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.CoursesPage, container, false);

            this.MyWireUpControls(layout);

            return layout;
        }

        private class TabAdapter : TabListAdapter<CoursesTabViewModel, CoursesTab>
        {
            public TabAdapter(FragmentManager fm, IEnumerable<CoursesTabViewModel> items) : base(fm, items)
            {
            }

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new Java.Lang.String(Items[position].SemesterName);
            }
        }
    }
}