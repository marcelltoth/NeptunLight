using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using NeptunLight.Droid.Views;

namespace NeptunLight.Droid
{
    [Activity(Label = "Beállítások", Theme = "@style/AppTheme", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SupportActionBar.Show();
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportFragmentManager.BeginTransaction().Replace(Android.Resource.Id.Content, new SettingsPage()).Commit();
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }
    }
}