using Android.App;
using Android.Content;
using Android.OS;
using JetBrains.Annotations;

namespace NeptunLight.Droid
{
    public class RefreshService : Service
    {
        [CanBeNull]
        public override IBinder OnBind([NotNull] Intent intent)
        {
            return null;
        }
    }
}