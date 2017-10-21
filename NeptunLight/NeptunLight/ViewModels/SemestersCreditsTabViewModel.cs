using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class SemestersCreditsTabViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<IReadOnlyList<BarDataPoint>> _barChartData;

        public SemestersCreditsTabViewModel(IDataStorage dataSource)
        {
            DataSource = dataSource;
            this.WhenAnyValue(x => x.DataSource.CurrentData.SemesterInfo).ObserveOn(RxApp.MainThreadScheduler).Select(sInf =>
                                                                                     sInf.OrderBy(sd => sd.Semester)
                                                                                         .Select(sd => new BarDataPoint(sd.CreditsTaken ?? 0, sd.CreditsAccomplished ?? 0))
                                                                                         .ToList()
                ).ToProperty(this, x => x.BarChartData, out _barChartData);

            this.WhenAnyValue(x => x.DataSource.CurrentData.SemesterInfo).ObserveOn(RxApp.MainThreadScheduler).Select(sInf =>
            {
                List<LineDataPoint> lineData = new List<LineDataPoint>() { new LineDataPoint(0, false) };
                int totalAccomplished = 0;

                foreach (SemesterData semesterData in sInf.OrderBy(sd => sd.Semester))
                {
                    if (semesterData.CreditsTaken.HasValue)
                    {
                        totalAccomplished += semesterData.CreditsAccomplished ?? semesterData.CreditsTaken.Value;
                    }
                    lineData.Add(new LineDataPoint(totalAccomplished, !semesterData.CreditsAccomplished.HasValue));
                }

                return lineData;
            }).ToProperty(this, x => x.LineChartData, out _lineChartData);
        }

        public IReadOnlyList<BarDataPoint> BarChartData => _barChartData.Value;

        private readonly ObservableAsPropertyHelper<IReadOnlyList<LineDataPoint>> _lineChartData;
        public IReadOnlyList<LineDataPoint> LineChartData => _lineChartData.Value;

        public IDataStorage DataSource { get; }

        public class BarDataPoint
        {
            public BarDataPoint(int taken, int accomplished)
            {
                Taken = taken;
                Accomplished = accomplished;
            }

            public int Taken { get; }
            public int Accomplished { get; }
        }

        public class LineDataPoint
        {
            public LineDataPoint(int cumulativeCredits, bool isPrediction)
            {
                CumulativeCredits = cumulativeCredits;
                IsPrediction = isPrediction;
            }

            public int CumulativeCredits { get; }

            public bool IsPrediction { get; }
        }
    }
}