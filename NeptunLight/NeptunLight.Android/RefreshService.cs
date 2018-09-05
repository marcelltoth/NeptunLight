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
using NeptunLight.Droid.Services;

namespace NeptunLight.Droid
{
    [Service]
    public class RefreshService : Service
    {
        private const int TIMER_INTERVAL = 1800*1000;
        public const string REFRESH_WIFI_ONLY_KEY = "REFRESH_WIFI_ONLY";

        private static ISharedPreferences Prefs => PreferenceManager.GetDefaultSharedPreferences(Application.Context);


        private readonly Timer _timer = new Timer() {Enabled = true, Interval = TIMER_INTERVAL};

        public RefreshService()
        {
            _timer.Elapsed += TimerTick;
        }

        private async void TimerTick([CanBeNull] object sender, [CanBeNull] ElapsedEventArgs e)
        {
            if (Prefs.GetBoolean(REFRESH_WIFI_ONLY_KEY, false))
            {
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

            RefreshManager rm = App.Container.Resolve<RefreshManager>();
            await rm.RefreshIfNeeded();
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