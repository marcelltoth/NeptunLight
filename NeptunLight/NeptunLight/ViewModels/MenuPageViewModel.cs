using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class MenuPageViewModel : PageViewModel
    {
        public MenuPageViewModel(IDataStorage storage, INeptunInterface client, INavigator navigator)
        {
            EnsureDataAccessible = ReactiveCommand.CreateFromTask(async () =>
            {
                if (storage.CurrentData == null)
                    await storage.LoadDataAsync();
                // see if there is saved data available
                if (storage.CurrentData == null)
                {
                    // need to refresh

                    LoadingDialogShown = true;
                    LoadingDialogText = "Belépés...";
                    await client.LoginAsync();
                    storage.CurrentData = new NeptunData();
                    LoadingDialogText = "Féléves adatok betöltése...";
                    storage.CurrentData.SemesterInfo = await client.RefreshSemestersAsnyc();
                    LoadingDialogText = "Felvett tantárgyak betöltése...";
                    storage.CurrentData.SubjectsPerSemester = await client.RefreshSubjectsAsnyc();
                    LoadingDialogText = "Felvett vizsgák betöltése...";
                    storage.CurrentData.ExamsPerSemester = await client.RefreshExamsAsnyc();
                    LoadingDialogText = "Naptár betöltése...";
                    storage.CurrentData.Calendar = await client.RefreshCalendarAsnyc();
                    LoadingDialogText = "Időszakok betöltése...";
                    storage.CurrentData.Periods = await client.RefreshPeriodsAsnyc();
                    LoadingDialogText = "Üzenetek betöltése... (első alkalommal több percet is igénybe vehet)";
                    IList<Mail> messages = await client.RefreshMessages(new Progress<MessageLoadingProgress>(progress =>
                    {
                        LoadingDialogText = $"Üzenetek betöltése ({progress.Current} / {progress.Total})... (első alkalommal több percet is igénybe vehet)";
                    })).ToList();
                    storage.CurrentData.Messages.Clear();
                    storage.CurrentData.Messages.AddRange(messages);

                    LoadingDialogText = "A szinkronizáció sikeres.";
                    await storage.SaveDataAsync();
                    LoadingDialogShown = false;
                }
            });

            EnsureDataAccessible.ThrownExceptions.Subscribe(_ =>
            {
                LoadingDialogShown = false;
                navigator.NavigateTo<LoginPageViewModel>();
            });

            IObservable<bool> menuAvailable = EnsureDataAccessible.IsExecuting.Select(x => !x);
            GoToMessages = ReactiveCommand.Create(() => navigator.NavigateTo<MessagesPageViewModel>(), menuAvailable);
            GoToCalendar = ReactiveCommand.Create(() => navigator.NavigateTo<CalendarPageViewModel>(), menuAvailable);
            GoToCourses = ReactiveCommand.Create(() => navigator.NavigateTo<CoursesPageViewModel>(), menuAvailable);
            GoToExams = ReactiveCommand.Create(() => navigator.NavigateTo<ExamsPageViewModel>(), menuAvailable);
            GoToSemesters = ReactiveCommand.Create(() => navigator.NavigateTo<SemestersPageViewModel>(), menuAvailable);
            GoToPeriods = ReactiveCommand.Create(() => navigator.NavigateTo<PeriodsPageViewModel>(), menuAvailable);
        }

        private bool _loadingDialogShown;

        public bool LoadingDialogShown
        {
            get => _loadingDialogShown;
            set => this.RaiseAndSetIfChanged(ref _loadingDialogShown, value);
        }

        private string _loadingDialogText;

        public string LoadingDialogText
        {
            get => _loadingDialogText;
            set => this.RaiseAndSetIfChanged(ref _loadingDialogText, value);
        }

        public ReactiveCommand EnsureDataAccessible { get; }

        public ReactiveCommand GoToMessages { get; }
        public ReactiveCommand GoToCalendar { get; }
        public ReactiveCommand GoToCourses { get; }
        public ReactiveCommand GoToExams { get; }
        public ReactiveCommand GoToSemesters { get; }
        public ReactiveCommand GoToPeriods { get; }

        public override string Title { get; } = "Neptun";
    }
}