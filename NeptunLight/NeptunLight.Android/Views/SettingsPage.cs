using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using Autofac;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NeptunLight.Droid.Services;
using NeptunLight.Services;

namespace NeptunLight.Droid.Views
{
    public class SettingsPage : PreferenceFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Layout.SettingsPage);

            Preference logout = FindPreference("logout_button");
            logout.PreferenceClick += Logout_PreferenceClick;

            Preference refresh = FindPreference("refresh_button");
            refresh.PreferenceClick += Refresh_PreferenceClick;

            Analytics.TrackEvent("Settings page shown");
        }

        private async void Refresh_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            Analytics.TrackEvent("Forced sync started", new Dictionary<string, string>()
            {
                {"Source", "Menu"}
            });
            Toast.MakeText(Activity, "Frissítés folyamatban...", ToastLength.Long).Show();
            try
            {
                await App.Container.Resolve<IRefreshManager>().RefreshAsync();
                Toast.MakeText(Activity, "Frissítés sikeres!", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, "Hiba a frissítés során", ToastLength.Short).Show();
                Crashes.TrackError(ex, new Dictionary<string, string>{
                    {"Category", "Forced sync error"},
                    {"Source", "Menu"}
                });
            }
        }

        private async void Logout_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            PreferenceManager.GetDefaultSharedPreferences(Application.Context).Edit().Clear().Commit();
            await App.Container.Resolve<IDataStorage>().ClearDataAsync();

            DirectoryInfo filesDir = new DirectoryInfo(Application.Context.FilesDir.Path);
            foreach (FileInfo dataFile in filesDir.GetFiles())
            {
                dataFile.Delete();
            }

            e.Handled = true;
            Activity.Finish();
        }
    }
}