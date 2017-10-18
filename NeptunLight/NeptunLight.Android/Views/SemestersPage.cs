using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class SemestersPage : ReactiveFragment<SemestersPageViewModel>, IActionBarContentProvider
    {
        public ListView SemesterList { get; set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.SemestersPage, container, false);

            this.WireUpControls(layout);

            this.WhenAnyValue(x => x.ViewModel.Semesters).Subscribe(semesters =>
            {
                SemesterList.Adapter = new ListAdapter<SemesterViewModel>(inflater, semesters, Resource.Layout.SemestersListItem, (layoutView, model) =>
                {
                    layoutView.FindViewById<TextView>(Resource.Id.semesterNameTextView).Text = model.SemesterName;
                    layoutView.FindViewById<TextView>(Resource.Id.statusTextView).Text = model.Status;
                    layoutView.FindViewById<TextView>(Resource.Id.creditInfoTextView).Text = model.CreditInfo;
                    layoutView.FindViewById<TextView>(Resource.Id.totalCreditInfoTextView).Text = model.TotalCreditInfo;
                    layoutView.FindViewById<TextView>(Resource.Id.averageTextView).Text = model.Average;
                    layoutView.FindViewById<TextView>(Resource.Id.cumulativeAverageTextView).Text = model.CumulativeAverage;
                });
            });

            return layout;
        }
    }
}