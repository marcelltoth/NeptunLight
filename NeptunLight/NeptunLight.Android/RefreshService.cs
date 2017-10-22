using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Autofac;
using JetBrains.Annotations;
using NeptunLight.DataAccess;
using NeptunLight.Models;
using NeptunLight.Services;

namespace NeptunLight.Droid
{
    [Service]
    public class RefreshService : Service
    {
        private const string LAST_REFRESH_TIME_PREF_KEY = "LAST_REFRESH_TIME";
        private const string REFRESH_INTERVAL_PREF_KEY = "REFRESH_INTERVAL_S";
        private const int DEFAULT_REFRESH_INTERVAL_S = 3600 * 12;

        [NotNull]
        private static ISharedPreferences Prefs => Application.Context.GetSharedPreferences("userPrimitives", FileCreationMode.Private);

        private readonly Timer _timer = new Timer() {Enabled = false};

        public RefreshService()
        {
            _timer.Elapsed += TimerTick;
        }

        private async void TimerTick(object sender, ElapsedEventArgs e)
        {
            await PerformUpdate();
        }

        private async Task PerformUpdate()
        {
            _timer.Stop();

            IDataStorage dataStorage = App.Container.Resolve<IDataStorage>();
            INeptunInterface client = App.Container.Resolve<INeptunInterface>();

            try
            {
                NeptunData loadedData = new NeptunData();
                loadedData.BasicData = await client.RefreshBasicDataAsync();
                loadedData.SemesterInfo = await client.RefreshSemestersAsnyc();
                loadedData.SubjectsPerSemester = await client.RefreshSubjectsAsnyc();
                loadedData.ExamsPerSemester = await client.RefreshExamsAsnyc();
                loadedData.Calendar = await client.RefreshCalendarAsnyc();
                loadedData.Periods = await client.RefreshPeriodsAsnyc();
                IList<Mail> messages = await client.RefreshMessages().ToList();
                loadedData.Messages.Clear();
                loadedData.Messages.AddRange(messages);

                dataStorage.CurrentData = loadedData;
                await dataStorage.SaveDataAsync();
                Prefs.Edit().PutLong(LAST_REFRESH_TIME_PREF_KEY, DateTime.Now.Ticks).Commit();
                Android.Util.Log.Info("NEPTUN", "refreshed");
            }
            catch (Exception ex)
            {
                Android.Util.Log.Info("NEPTUN", "Error:" + ex);
                // error refreshing, fail silently
            }

            _timer.Start();
        }

        public override async void OnCreate()
        {
            base.OnCreate();
            UpdateTimerInterval();
            _timer.Start();
            Android.Util.Log.Info("NEPTUN", "created");

            // start an update immediately if too much time has passed already
            if (Prefs.Contains(LAST_REFRESH_TIME_PREF_KEY))
            {
                TimeSpan timeDiff = DateTime.Now - new DateTime(Prefs.GetLong(LAST_REFRESH_TIME_PREF_KEY, 0));
                if (timeDiff.TotalSeconds > Prefs.GetInt(REFRESH_INTERVAL_PREF_KEY, DEFAULT_REFRESH_INTERVAL_S))
                {
                    await PerformUpdate();
                }
            }
        }

        private void UpdateTimerInterval()
        {
            _timer.Interval = Prefs.GetInt(REFRESH_INTERVAL_PREF_KEY, DEFAULT_REFRESH_INTERVAL_S) * 1000;
        }

        public override void OnDestroy()
        {
            Android.Util.Log.Info("NEPTUN", "destroyed");
            _timer.Stop();
            _timer.Dispose();
            base.OnDestroy();
        }

        
        [CanBeNull]
        public override IBinder OnBind([NotNull] Intent intent)
        {
            return null;
        }
    }
}