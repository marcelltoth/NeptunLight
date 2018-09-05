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
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.Droid.Services
{
    public class RefreshManager : ReactiveObject
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

        public RefreshManager(IDataStorage dataStorage, INeptunInterface client)
        {
            _dataStorage = dataStorage;
            _client = client;
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
                    LastRefreshTime = checkTime;
                    Analytics.TrackEvent("Background sync finished");
                }
                catch (Exception ex)
                {
                    Analytics.TrackEvent("Background sync error", new Dictionary<string, string>{
                        {"Message", ex.Message},
                        { "Trace", ex.StackTrace.Substring(0,64)}
                    });
                    // error refreshing, fail silently
                }
            }
        }

        public async Task RefreshAsync()
        {
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
            loadedData.Messages.Clear();
            loadedData.Messages.AddRange(messages);

            _dataStorage.CurrentData = loadedData;
            await _dataStorage.SaveDataAsync();

            IsRefreshing = false;
        }
    }
}