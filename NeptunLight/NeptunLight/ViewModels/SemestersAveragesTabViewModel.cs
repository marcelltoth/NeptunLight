using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using JetBrains.Annotations;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class SemestersAveragesTabViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<IReadOnlyList<ChartDataPoint>> _chartData;

        public SemestersAveragesTabViewModel(IDataStorage dataSource)
        {
            DataSource = dataSource;
            this.WhenAnyValue(x => x.DataSource.CurrentData.SemesterInfo).ObserveOn(RxApp.MainThreadScheduler).Select(sInf =>
            {
                List<ChartDataPoint> ret =  new List<ChartDataPoint>();
                foreach (SemesterData semesterData in sInf.OrderBy(sd => sd.Semester))
                {
                    if (semesterData.Average.HasValue && semesterData.CumulativeAverage.HasValue)
                    {
                        ret.Add(new ChartDataPoint(semesterData.Average.Value, semesterData.CumulativeAverage.Value));
                    }
                    else
                    {
                        ret.Add(null);
                    }
                }
                return ret;
            }).ToProperty(this, x => x.ChartData, out _chartData);
        }

        [ItemCanBeNull]
        public IReadOnlyList<ChartDataPoint> ChartData => _chartData.Value;

        public IDataStorage DataSource { get; }

        public class ChartDataPoint
        {
            public ChartDataPoint(double pointAverage, double cumulativeAverage)
            {
                PointAverage = pointAverage;
                CumulativeAverage = cumulativeAverage;
            }

            public double PointAverage { get; }
            public double CumulativeAverage { get; }
        }
    }
}