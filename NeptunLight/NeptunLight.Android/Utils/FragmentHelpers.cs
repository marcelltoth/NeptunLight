using System.Linq;
using System.Reflection;
using Android.Support.V4.App;
using Android.Views;

namespace NeptunLight.Droid.Utils
{
    public static class FragmentHelpers
    {
        public static void MyWireUpControls(this Fragment target, View layout)
        {
            foreach (PropertyInfo viewProperty in target.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(p => typeof(View).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(View)))
            {
                string resourceName = viewProperty.Name;
                int resourceId = (int) typeof(Resource.Id).GetField(resourceName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Static).GetRawConstantValue();
                viewProperty.SetValue(target, layout.FindViewById(resourceId));
            }
        }
    }
}