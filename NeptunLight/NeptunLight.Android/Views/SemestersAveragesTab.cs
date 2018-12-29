using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using Android.Graphics;
using Android.OS;
using Android.Views;
using JetBrains.Annotations;
using MikePhil.Charting.Animation;
using MikePhil.Charting.Charts;
using MikePhil.Charting.Components;
using MikePhil.Charting.Data;
using MikePhil.Charting.Formatter;
using NeptunLight.ViewModels;
using ReactiveUI;

namespace NeptunLight.Droid.Views
{
    public class SemestersAveragesTab : ReactiveFragment<SemestersAveragesTabViewModel>
    {
        public LineChart LineChart { get; set; }

        public override View OnCreateView(LayoutInflater inflater, [CanBeNull] ViewGroup container, [CanBeNull] Bundle savedInstanceState)
        {
            View layout = inflater.Inflate(Resource.Layout.SemestersAveragesTab, container, false);

            this.WireUpControls(layout);

            LineChart.XAxis.SpaceMin = 0.1f;
            LineChart.XAxis.SpaceMax = 0.1f;
            LineChart.XAxis.Granularity = 1f;
            LineChart.XAxis.GranularityEnabled = true;
            LineChart.XAxis.ValueFormatter = new SemesterValueFormater();
            LineChart.AxisLeft.ValueFormatter = new DefaultAxisValueFormatter(2);
            LineChart.SetTouchEnabled(false);
            LineChart.Description.Enabled = false;
            this.WhenAnyValue(x => x.ViewModel.ChartData).Where(cd => cd.Count(s => s != null) >= 1).Subscribe(cd =>
            {
                List<Entry> pointEntries = new List<Entry>(cd.Count);
                List<Entry> cumulativeEntries = new List<Entry>(cd.Count);
                for (int i = 0; i < cd.Count; i++)
                {
                    if(cd[i] == null)
                        continue;
                    if (cd[i].PointAverage > 0)
                    {
                        pointEntries.Add(new Entry(i + 1, (float)cd[i].PointAverage));
                    }

                    if (cd[i].CumulativeAverage > 0)
                    {
                        cumulativeEntries.Add(new Entry(i + 1, (float)cd[i].CumulativeAverage));
                    }
                }

                LineDataSet pointDataSet = new LineDataSet(pointEntries, "Félévi átlag"){ Color = Color.ParseColor("#8bc34a"), LineWidth = 3f, CircleRadius = 5f, CircleHoleRadius = 3.5f, ValueTextSize = 10f, ValueFormatter = new DefaultValueFormatter(2) };
                LineDataSet cumulativeDataSet = new LineDataSet(cumulativeEntries, "Kumulatív átlag"){ Color = Color.ParseColor("#43a047"), LineWidth = 1.7f, CircleRadius = 3f, CircleHoleRadius = 2f, ValueTextSize = 10f, ValueFormatter = new DefaultValueFormatter(2) };
                LineChart.Data = new LineData(pointDataSet, cumulativeDataSet);
                LineChart.AnimateY(2000, Easing.EasingOption.EaseInOutCubic);
            });

            return layout;
        }

        private class SemesterValueFormater : Java.Lang.Object, IAxisValueFormatter
        {
            public string GetFormattedValue(float value, [NotNull] AxisBase axis)
            {
                return $"{value}.";
            }
        }
    }
}