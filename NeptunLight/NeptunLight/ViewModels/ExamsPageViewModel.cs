using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class ExamsPageViewModel : PageViewModel
    {

        private readonly ObservableAsPropertyHelper<IEnumerable<ExamsTabViewModel>> _tabs;
        public IEnumerable<ExamsTabViewModel> Tabs => _tabs.Value;

        private IDataStorage DataStorage { get; }

        public ExamsPageViewModel(IDataStorage dataStorage, Func<Semester, ExamsTabViewModel> examsTabVmFac)
        {
            DataStorage = dataStorage;
            this.WhenAnyValue(x => x.DataStorage.CurrentData.ExamsPerSemester)
                .Select(examsDict => examsDict.Select(kvp => examsTabVmFac(kvp.Key)))
                .ToProperty(this, x => x.Tabs, out _tabs);
        }

        public override string Title { get; } = "Vizsgák";
    }
}