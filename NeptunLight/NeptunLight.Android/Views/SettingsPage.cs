using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using Autofac;
using Microsoft.AppCenter.Analytics;
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
            Analytics.TrackEvent("Forced sync started");
            Toast.MakeText(Activity, "Frissítés folyamatban...", ToastLength.Long).Show();
            try
            {
                await App.Container.Resolve<IRefreshManager>().RefreshAsync();
                Toast.MakeText(Activity, "Frissítés sikeres!", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Toast.MakeText(Activity, "Hiba a frissítés során", ToastLength.Short).Show();
                Analytics.TrackEvent("Forced sync error", new Dictionary<string, string>{
                    {"Message", ex.Message},
                    { "Trace", ex.StackTrace.Substring(0,64)}
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