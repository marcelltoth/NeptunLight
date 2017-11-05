using System.IO;
using Android.App;
using Android.OS;
using Android.Preferences;
using Autofac;
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