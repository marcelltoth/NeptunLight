using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class InitialSyncPageViewModel : PageViewModel
    {
        public enum RefreshStepState
        {
            Waiting = 0,
            Refreshing,
            Done
        }

        public InitialSyncPageViewModel(IDataStorage storage, INeptunInterface client, INavigator navigator)
        {
            if (!client.HasCredentials())
            {
                navigator.NavigateTo<LoginPageViewModel>();
            }

            PerformSync = ReactiveCommand.CreateFromTask(async () =>
            {
                LoadBasicDataStatus = RefreshStepState.Refreshing;
                NeptunData loadedData = new NeptunData();
                loadedData.BasicData = await client.RefreshBasicDataAsync();
                LoadBasicDataStatus = RefreshStepState.Done;
                LoadSemesterDataStatus = RefreshStepState.Refreshing;
                loadedData.SemesterInfo = await client.RefreshSemestersAsnyc();
                LoadSemesterDataStatus = RefreshStepState.Done;
                LoadCoursesStatus = RefreshStepState.Refreshing;
                loadedData.SubjectsPerSemester = await client.RefreshSubjectsAsnyc();
                LoadCoursesStatus = RefreshStepState.Done;
                LoadExamsStatus = RefreshStepState.Refreshing;
                loadedData.ExamsPerSemester = await client.RefreshExamsAsnyc();
                LoadExamsStatus = RefreshStepState.Done;
                LoadCalendarStatus = RefreshStepState.Refreshing;
                loadedData.Calendar = await client.RefreshCalendarAsnyc();
                LoadCalendarStatus = RefreshStepState.Done;
                LoadPeriodsStatus = RefreshStepState.Refreshing;
                loadedData.Periods = await client.RefreshPeriodsAsnyc();
                LoadPeriodsStatus = RefreshStepState.Done;

                IList<Mail> messages = await client.RefreshMessages(new Progress<MessageLoadingProgress>(progress =>
                {
                    LoadMessagesStatus = RefreshStepState.Refreshing;
                    MessageSyncProgress = progress.Current;
                    MessagesTotal = progress.Total;
                })).ToList();
                loadedData.Messages.Clear();
                loadedData.Messages.AddRange(messages);
                LoadMessagesStatus = RefreshStepState.Done;

                storage.CurrentData = loadedData;
                await storage.SaveDataAsync();
                navigator.NavigateTo<MenuPageViewModel>(false);
            });
        }

        public override string Title { get; } = "Első szinkronizáció";

        private ReactiveCommand PerformSync { get; }

        public IObservable<bool> IsSyncing => PerformSync.IsExecuting.StartWith(false);


        #region Step statuses

        private RefreshStepState _loadBasicDataStatus;

        public RefreshStepState LoadBasicDataStatus
        {
            get => _loadBasicDataStatus;
            set => this.RaiseAndSetIfChanged(ref _loadBasicDataStatus, value);
        }

        private RefreshStepState _loadSemesterDataStatus;

        public RefreshStepState LoadSemesterDataStatus
        {
            get => _loadSemesterDataStatus;
            set => this.RaiseAndSetIfChanged(ref _loadSemesterDataStatus, value);
        }

        private RefreshStepState _loadCoursesStatus;

        public RefreshStepState LoadCoursesStatus
        {
            get => _loadCoursesStatus;
            set => this.RaiseAndSetIfChanged(ref _loadCoursesStatus, value);
        }

        private RefreshStepState _loadExamsStatus;

        public RefreshStepState LoadExamsStatus
        {
            get => _loadExamsStatus;
            set => this.RaiseAndSetIfChanged(ref _loadExamsStatus, value);
        }

        private RefreshStepState _loadCalendarStatus;

        public RefreshStepState LoadCalendarStatus
        {
            get => _loadCalendarStatus;
            set => this.RaiseAndSetIfChanged(ref _loadCalendarStatus, value);
        }

        private RefreshStepState _loadPeriodsStatus;

        public RefreshStepState LoadPeriodsStatus
        {
            get => _loadPeriodsStatus;
            set => this.RaiseAndSetIfChanged(ref _loadPeriodsStatus, value);
        }

        private RefreshStepState _loadMessagesStatus;

        public RefreshStepState LoadMessagesStatus
        {
            get => _loadMessagesStatus;
            set => this.RaiseAndSetIfChanged(ref _loadMessagesStatus, value);
        }

        private int _messageSyncProgress = -1;

        public int MessageSyncProgress
        {
            get => _messageSyncProgress;
            set => this.RaiseAndSetIfChanged(ref _messageSyncProgress, value);
        }

        private int _messagesTotal;

        public int MessagesTotal
        {
            get => _messagesTotal;
            set => this.RaiseAndSetIfChanged(ref _messagesTotal, value);
        }

        #endregion
    }
}