using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class SemestersPageViewModel : PageViewModel
    {
        public override string Title { get; } = "Féléves adatok";

        private readonly ObservableAsPropertyHelper<IReadOnlyList<SemesterViewModel>> _semesters;
        public IReadOnlyList<SemesterViewModel> Semesters => _semesters.Value;

        private IDataStorage DataStorage { get; }

        public SemestersPageViewModel(IDataStorage data, Func<SemesterData, SemesterViewModel> semesterVmFac)
        {
            DataStorage = data;
            this.WhenAnyValue(x => x.DataStorage.CurrentData.SemesterInfo).Select(semesters => semesters.Select(semesterVmFac).ToList()).ToProperty(this, x => x.Semesters, out _semesters);
        }
    }
}