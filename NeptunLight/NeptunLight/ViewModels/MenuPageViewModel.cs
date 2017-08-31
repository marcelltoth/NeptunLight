using System;
using System.Reactive.Linq;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class MenuPageViewModel : PageViewModel
    {
        public MenuPageViewModel(IDataStorage storage, INeptunInterface client, INavigator navigator, IMailContentCache mailContentCache)
        {
            EnsureDataAccessible = ReactiveCommand.CreateFromTask(async () =>
            {
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
                    storage.CurrentData.Messages = await client.RefreshMessagesAsnyc(mailContentCache, new Progress<MessageLoadingProgress>(progress =>
                    {
                        LoadingDialogText = $"Üzenetek betöltése ({progress.Current} / {progress.Total})... (első alkalommal több percet is igénybe vehet)";
                    }));

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

            GoToCalendar = ReactiveCommand.Create(() => navigator.NavigateTo<CalendarPageViewModel>(), this.WhenAnyValue(x => x.LoadingDialogShown).Select(x => !x));
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

        public ReactiveCommand GoToCalendar { get; }
    }
}