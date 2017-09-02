using System.Collections.Generic;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class CalendarPageViewModel : PageViewModel
    {
        private IReadOnlyCollection<CalendarEvent> _events;

        public IReadOnlyCollection<CalendarEvent> Events
        {
            get => _events;
            set => this.RaiseAndSetIfChanged(ref _events, value);
        }

        public CalendarPageViewModel(IDataStorage dataStorage)
        {
            Events = dataStorage.CurrentData.Calendar;
        }

        public override string Title { get; } = "Órarend";
    }
}