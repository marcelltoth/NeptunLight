using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.OS;
using Android.Support.V7.Preferences;
using Android.Widget;
using Autofac;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using NeptunLight.Services;

namespace NeptunLight.Droid.Views
{
    public class SettingsPage : PreferenceFragmentCompat
    {


        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            AddPreferencesFromResource(Resource.Layout.SettingsPage);
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Preference logout = FindPreference("logout_button");
            logout.PreferenceClick += Logout_PreferenceClick;

            Preference refresh = FindPreference("refresh_button");
            refresh.PreferenceClick += Refresh_PreferenceClick;

            Analytics.TrackEvent("Settings page shown");
        }

        private async void Refresh_PreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            Analytics.TrackEvent("Forced sync started", new Dictionary<string, string>
            {
                {"Source", "Menu"}
            });
            Toast.MakeText(Activity, "Frissítés folyamatban...", ToastLength.Long).Show();
            try
            {
                await App.Container.Resolve<IRefreshManager>().RefreshAsync();
                Analytics.TrackEvent("Forced sync finished", new Dictionary<string, string>
                {
                    {"Source", "Menu"}
                });
                if (Context != null)
                    Toast.MakeText(Context, "Frissítés sikeres!", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>{
                    {"Category", "Forced sync error"},
                    {"Source", "Menu"}
                });
                if (Context != null)
                    Toast.MakeText(Context, "Hiba a frissítés során", ToastLength.Short).Show();
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