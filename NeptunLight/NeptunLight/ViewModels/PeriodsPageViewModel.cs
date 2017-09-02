using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class PeriodsPageViewModel : PageViewModel
    {
        private readonly ObservableAsPropertyHelper<IEnumerable<PeriodViewModel>> _periods;

        public PeriodsPageViewModel(IDataStorage dataStorage, Func<Period, PeriodViewModel> periodVmFac)
        {
            DataStorage = dataStorage;
            this.WhenAnyValue(x => x.DataStorage.CurrentData.Periods).Select(x => x.Select(periodVmFac)).ToProperty(this, x => x.Periods, out _periods);
        }

        public IEnumerable<PeriodViewModel> Periods => _periods.Value;
        private IDataStorage DataStorage { get; }

        public override string Title { get; } = "Időszakok";
    }
}