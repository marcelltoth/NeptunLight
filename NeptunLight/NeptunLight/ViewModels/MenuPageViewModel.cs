using System;
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
                await storage.LoadDataAsync();
                // see if there is saved data available
                if (storage.CurrentData == null)
                {
                    // need to refresh
                    try
                    {
                        await client.LoginAsync();
                    }
                    catch (Exception)
                    {
                        // invalid credentials
                        navigator.NavigateTo<LoginPageViewModel>();
                        return;
                    }

                    LoadingDialogShown = true;
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
                    /*LoadingDialogText = "Üzenetek betöltése... (első alkalommal több percet is igénybe vehet)";
                    storage.CurrentData.Messages = await client.RefreshMessagesAsnyc();*/

                    LoadingDialogText = "A szinkronizáció sikeres.";
                    await storage.SaveDataAsync();
                    LoadingDialogShown = false;
                }
            });
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
    }
}