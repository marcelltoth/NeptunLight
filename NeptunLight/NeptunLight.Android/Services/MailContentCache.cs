using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using NeptunLight.Models;
using NeptunLight.Services;
using Newtonsoft.Json;

namespace NeptunLight.Droid.Services
{
    public class MailContentCache : IMailContentCache
    {
        private Dictionary<MailHeader, Mail> _cacheObj = null;
        private static string FileLocation => Path.Combine(Application.Context.FilesDir.Path, "mailCache.json");

        private async Task LoadFile()
        {
            await Task.Run(() =>
            {
                if (File.Exists(FileLocation))
                {
                    string text = File.ReadAllText(FileLocation);
                    _cacheObj = JsonConvert.DeserializeObject<List<Mail>>(text).ToDictionary(m => (MailHeader)m, m => m);
                }
                else
                {
                    _cacheObj = new Dictionary<MailHeader, Mail>();
                }
            });
        }

        private async Task SaveFile()
        {
            await Task.Run(() =>
            {
                if (_cacheObj != null)
                    File.WriteAllText(FileLocation, JsonConvert.SerializeObject(_cacheObj.Values.ToList()));
            });
        }

        public async Task StoreAsync(MailHeader key, Mail value)
        {
            if (_cacheObj == null)
            {
                await LoadFile();
            }
            _cacheObj[key] = value;
            await SaveFile();
        }

        public async Task<Mail> TryRetrieveAsync(MailHeader key)
        {
            if (_cacheObj == null)
            {
                await LoadFile();
            }
            return _cacheObj.ContainsKey(key) ? _cacheObj[key] : null;
        }
    }
}