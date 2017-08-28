using System.IO;
using System.Threading.Tasks;
using Android.App;
using NeptunLight.Models;
using NeptunLight.Services;
using Newtonsoft.Json;
using ReactiveUI;

namespace NeptunLight.Droid.Services
{
    public class FileDataStorage : ReactiveObject, IDataStorage
    {
        private static string FileLocation => Path.Combine(Application.Context.FilesDir.Path, "neptunData.json");

        private NeptunData _currentData;

        public NeptunData CurrentData
        {
            get => _currentData;
            private set => this.RaiseAndSetIfChanged(ref _currentData, value);
        }
        public async Task LoadDataAsync(bool forceReload = false)
        {
            if (File.Exists(FileLocation))
            {
                NeptunData deserializedData = null;
                await Task.Run(() =>
                {
                    string text = File.ReadAllText(FileLocation);
                    deserializedData = JsonConvert.DeserializeObject<NeptunData>(text);
                });
                CurrentData = deserializedData;
            }
        }

        public async Task SaveDataAsync()
        {
            await Task.Run(() =>
            {
                File.WriteAllText(FileLocation, JsonConvert.SerializeObject(CurrentData));
            });
        }
    }
}