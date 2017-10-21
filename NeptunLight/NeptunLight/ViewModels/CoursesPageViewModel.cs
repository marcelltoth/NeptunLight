using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class CoursesPageViewModel : PageViewModel
    {
        public override string Title { get; } = "Tárgyak";
        private readonly ObservableAsPropertyHelper<IReadOnlyList<CoursesTabViewModel>> _tabs;
        public IReadOnlyList<CoursesTabViewModel> Tabs => _tabs.Value;

        private IDataStorage DataStorage { get; }

        public CoursesPageViewModel(IDataStorage dataStorage, Func<Semester, CoursesTabViewModel> coursesTabVmFac)
        {
            DataStorage = dataStorage;
            this.WhenAnyValue(x => x.DataStorage.CurrentData.SubjectsPerSemester)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(coursesDict => coursesDict.Select(kvp => coursesTabVmFac(kvp.Key)).ToList())
                .ToProperty(this, x => x.Tabs, out _tabs);
        }
    }
}