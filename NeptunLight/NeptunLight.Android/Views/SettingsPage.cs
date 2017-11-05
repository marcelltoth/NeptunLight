using Android.OS;
using Android.Preferences;

namespace NeptunLight.Droid.Views
{
    public class SettingsPage : PreferenceFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(Resource.Layout.SettingsPage);
        }
    }
}