using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Preferences;
using Autofac;
using JetBrains.Annotations;
using Microsoft.AppCenter.Analytics;
using NeptunLight.Droid.Services;

namespace NeptunLight.Droid
{
    [Service]
    public class RefreshService : Service
    {
        public const string REFRESH_WIFI_ONLY_KEY = "REFRESH_WIFI_ONLY";
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

        private readonly Timer _timer = new Timer() {Enabled = true, Interval = 2*3600*1000};

        public RefreshService()
        {
            _timer.Elapsed += TimerTick;
        }

        private async void TimerTick([CanBeNull] object sender, [CanBeNull] ElapsedEventArgs e)
        {
            TimeSpan timeDiff = DateTime.Now - new DateTime(Prefs.GetLong(LAST_REFRESH_TIME_PREF_KEY, 0));
            if (timeDiff.TotalSeconds > RefreshInterval)
            {
                // we should update 

                if (Prefs.GetBoolean(REFRESH_WIFI_ONLY_KEY, false))
                {
                    // check if there is wifi connection, abort if not
                    ConnectivityManager connnManager = (ConnectivityManager) GetSystemService(ConnectivityService);
                    if (connnManager.GetAllNetworks().All(net =>
                    {
                        NetworkInfo info = connnManager.GetNetworkInfo(net);
                        return info.Type != ConnectivityType.Wifi || !info.IsConnected;
                    }))
                    {
                        return;
                    }
                }
                await PerformUpdate();
            }
        }

        private async Task PerformUpdate()
        {
            _timer.Stop();

            Analytics.TrackEvent("Background sync started");

            try
            {
                RefreshManager rm = App.Container.Resolve<RefreshManager>();
                await rm.RefreshAsync();
                Prefs.Edit().PutLong(LAST_REFRESH_TIME_PREF_KEY, DateTime.Now.Ticks).Commit();
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


            _timer.Start();
        }

        public override void OnCreate()
        {
            base.OnCreate();
            _timer.Start();
            Android.Util.Log.Info("NEPTUN", "created");

            // start an update immediately if too much time has passed already
            TimerTick(null, null);
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