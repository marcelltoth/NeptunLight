using System;
using System.Globalization;
using Android.OS;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class PeriodsPage : ReactiveFragment<PeriodsPageViewModel>, IActionBarProvider
    {
        public ListView PeriodList { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Analytics.TrackEvent("Periods page shown");
        }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.PeriodsPage, container, false);

            this.WireUpControls(layout);

            this.WhenAnyValue(x => x.ViewModel.Periods).Subscribe(periods =>
            {
                PeriodList.Adapter = new ListAdapter<PeriodViewModel>(inflater, periods, Resource.Layout.PeriodsListItem, (itemView, model) =>
                {
                    itemView.FindViewById<TextView>(Resource.Id.categoryTextView).Text = model.Type;
                    itemView.FindViewById<TextView>(Resource.Id.titleTextView).Text = model.Name;
                    itemView.FindViewById<TextView>(Resource.Id.startDateTextView).Text = model.StartTime.ToString("g", CultureInfo.CurrentCulture);
                    itemView.FindViewById<TextView>(Resource.Id.endDateTextView).Text = model.EndTime.ToString("g", CultureInfo.CurrentCulture);
                });
            });

            return layout;
        }
    }
}