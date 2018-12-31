using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Preferences;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.Droid.Services
{
    public class RefreshManager : ReactiveObject, IRefreshManager
    {
        
        public const string LAST_REFRESH_TIME_PREF_KEY = "LAST_REFRESH_TIME";
        public const string REFRESH_INTERVAL_PREF_KEY = "REFRESH_INTERVAL_S";
        public const int DEFAULT_REFRESH_INTERVAL_S = 3600 * 12;

        [NotNull]
        private static ISharedPreferences Prefs => PreferenceManager.GetDefaultSharedPreferences(Application.Context);

        private static int RefreshInterval
        {
            get
            {
                string s = Prefs.GetString(REFRESH_INTERVAL_PREF_KEY, "");
                if (int.TryParse(s, out int i))
                {
                    return i;
                }
                return DEFAULT_REFRESH_INTERVAL_S;
            }
        }

        private readonly IDataStorage _dataStorage;
        private readonly INeptunInterface _client;
        private readonly IMailNotificationDispatcher _mailNotificationService;

        public RefreshManager(IDataStorage dataStorage, INeptunInterface client, IMailNotificationDispatcher mailNotificationService)
        {
            _dataStorage = dataStorage;
            _client = client;
            _mailNotificationService = mailNotificationService;
        }

        public DateTime LastRefreshTime
        {
            get => new DateTime(Prefs.GetLong(LAST_REFRESH_TIME_PREF_KEY, 0));
            set
            {
                this.RaisePropertyChanging();
                Prefs.Edit().PutLong(LAST_REFRESH_TIME_PREF_KEY, value.Ticks).Commit();
                this.RaisePropertyChanged();
            }
        }

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => this.RaiseAndSetIfChanged(ref _isRefreshing, value);
        }

        public async Task RefreshIfNeeded()
        {
            DateTime checkTime = DateTime.Now;
            TimeSpan timeDiff = checkTime - LastRefreshTime;
            if (timeDiff.TotalSeconds > RefreshInterval)
            {
                // we should update 
                Analytics.TrackEvent("Background sync started");

                try
                {
                    await RefreshAsync();
                    Analytics.TrackEvent("Background sync finished");
                }
                catch (Exception ex)
                {
                    Crashes.TrackError(ex, new Dictionary<string, string>{
                        {"Category", "Background sync error"}
                    });
                    // error refreshing, fail silently
                }
            }
        }

        public async Task RefreshAsync()
        {
            try
            {
                DateTime startTime = DateTime.Now;
                if (IsRefreshing)
                    return;
                IsRefreshing = true;

                if (!_client.HasCredentials())
                    return;

                NeptunData loadedData = new NeptunData();
                loadedData.BasicData = await _client.RefreshBasicDataAsync();
                loadedData.SemesterInfo = await _client.RefreshSemestersAsnyc();
                loadedData.SubjectsPerSemester = await _client.RefreshSubjectsAsnyc();
                loadedData.ExamsPerSemester = await _client.RefreshExamsAsnyc();
                loadedData.Calendar = await _client.RefreshCalendarAsnyc();
                loadedData.Periods = await _client.RefreshPeriodsAsnyc();
                IList<Mail> messages = await _client.RefreshMessages().ToList();

                // If there are new mails, notify the user
                var newMails = messages.Where(m => m.IsNew).ToList();
                if (newMails.Count == 1)
                {
                    _mailNotificationService.NotifyNewMail(newMails.First());
                }
                else
                {
                    _mailNotificationService.NotifyMultipleNewMails(newMails);
                }

                loadedData.Messages.Clear();
                loadedData.Messages.AddRange(messages);

                _dataStorage.CurrentData = loadedData;
                await _dataStorage.SaveDataAsync();
                LastRefreshTime = startTime;
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}