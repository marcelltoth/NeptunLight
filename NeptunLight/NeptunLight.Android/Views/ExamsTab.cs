using System;
using System.Globalization;
using Android.OS;
using Android.Views;
using Android.Widget;
using JetBrains.Annotations;
using NeptunLight.Droid.Utils;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class ExamsTab : ReactiveFragment<ExamsTabViewModel>
    {

        private ListView ExamList { get; [UsedImplicitly] set; }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.ExamsTab, container, false);

            this.WireUpControls(layout);

            this.WhenAnyValue(x => x.ViewModel.Exams).Subscribe(exams =>
            {
                ExamList.Adapter = new ListAdapter<ExamViewModel>(inflater, exams, Resource.Layout.ExamListItem, (itemView, model) =>
                {
                    itemView.FindViewById<TextView>(Resource.Id.subjectTextView).Text = model.Subject;
                    itemView.FindViewById<TextView>(Resource.Id.dateTextView).Text = model.StartTime.ToString(CultureInfo.CurrentCulture);
                    itemView.FindViewById<TextView>(Resource.Id.locationTextView).Text = model.Location;
                    itemView.FindViewById<TextView>(Resource.Id.resultTextView).Text = model.Result;
                });
            });

            return layout;
        }
    }
}