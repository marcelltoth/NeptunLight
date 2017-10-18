using System;
using System.Globalization;
using Android.OS;
using Android.Views;
using Android.Widget;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class PeriodsPage : ReactiveFragment<PeriodsPageViewModel>, IActionBarContentProvider
    {
        public ListView PeriodList { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.PeriodsPage, container, false);

            this.WireUpControls(layout);

            this.WhenAnyValue(x => x.ViewModel.Periods).Subscribe(periods =>
            {
                PeriodList.Adapter = new ListAdapter<PeriodViewModel>(inflater, periods, Resource.Layout.PeriodsListItem, (itemView, model) =>
                {
                    itemView.FindViewById<TextView>(Resource.Id.categoryTextView).Text = model.Type;
                    itemView.FindViewById<TextView>(Resource.Id.titleTextView).Text = model.Name;
                    itemView.FindViewById<TextView>(Resource.Id.startDateTextView).Text = model.StartTime.ToString(CultureInfo.CurrentCulture);
                    itemView.FindViewById<TextView>(Resource.Id.endDateTextView).Text = model.EndTime.ToString(CultureInfo.CurrentCulture);
                });
            });

            return layout;
        }
    }
}