using System;
using System.Globalization;
using Android.Graphics;
using Android.Graphics.Drawables;
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
                    if (itemView.Background is LayerDrawable background)
                    {
                        GradientDrawable leftColor = (GradientDrawable)background.GetDrawable(1);
                        switch (model.ResultLevel)
                        {
                            case 5:
                                leftColor.SetColor(Color.ParseColor("#4caf50"));
                                break;
                            case 4:
                                leftColor.SetColor(Color.ParseColor("#66bb6a"));
                                break;
                            case 3:
                                leftColor.SetColor(Color.ParseColor("#81c784"));
                                break;
                            case 2:
                                leftColor.SetColor(Color.ParseColor("#a5d6a7"));
                                break;
                            case 1:
                                leftColor.SetColor(Color.ParseColor("#f44336"));
                                break;
                            default:
                                leftColor.SetColor(Color.ParseColor("#ff9100"));
                                break;
                        }
                    }
                });
            });

            return layout;
        }
    }
}