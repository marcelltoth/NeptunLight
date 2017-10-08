using System;
using System.Collections.Generic;
using Android.Content;
using NeptunLight.Services;

namespace NeptunLight.Droid.Services
{
    public class PrimitiveStorage : IPrimitiveStorage
    {
        private static ISharedPreferences Prefs => App.MainActivity.GetPreferences(FileCreationMode.Private);


        public void ClearValue(string key)
        {
            if (Prefs.Contains(key))
            {
                ISharedPreferencesEditor editor = Prefs.Edit();
                editor.Remove(key);
                editor.Apply();
            }
        }

        public bool ContainsKey(string key)
        {
            return Prefs.Contains(key);
        }

        public string GetString(string key)
        {
            if (!ContainsKey(key))
                throw new KeyNotFoundException();
            return Prefs.GetString(key, String.Empty);
        }

        public void PutString(string key, string value)
        {
            ISharedPreferencesEditor editor = Prefs.Edit();
            editor.PutString(key, value);
            editor.Apply();
        }

        public int GetInt(string key)
        {
            if (!ContainsKey(key))
                throw new KeyNotFoundException();
            return Prefs.GetInt(key, 0);
        }

        public void PutInt(string key, int value)
        {
            ISharedPreferencesEditor editor = Prefs.Edit();
            editor.PutInt(key, value);
            editor.Apply();
        }

        public double GetDouble(string key)
        {
            if (!ContainsKey(key))
                throw new KeyNotFoundException();
            return Prefs.GetFloat(key, Single.NaN);
        }

        public void PutDouble(string key, double value)
        {
            ISharedPreferencesEditor editor = Prefs.Edit();
            editor.PutFloat(key, (float) value);
            editor.Apply();
        }
    }
}