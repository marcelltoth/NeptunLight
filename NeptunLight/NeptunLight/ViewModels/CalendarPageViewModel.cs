using System.Collections.Generic;
using System.Reactive.Linq;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class CalendarPageViewModel : PageViewModel
    {
        private IDataStorage DataStorage { get; }

        private readonly ObservableAsPropertyHelper<IReadOnlyCollection<CalendarEvent>> _events;
        public IReadOnlyCollection<CalendarEvent> Events => _events.Value;

        public CalendarPageViewModel(IDataStorage dataStorage)
        {
            DataStorage = dataStorage;
            this.WhenAnyValue(x => x.DataStorage.CurrentData.Calendar).ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.Events, out _events);
        }

        public override string Title { get; } = "Órarend";
    }
}