using System.Threading.Tasks;
using NeptunLight.Models;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.Droid.Services
{
    public class InstanceDataStorage : ReactiveObject, IDataStorage
    {
        private NeptunData _currentData;

        public NeptunData CurrentData
        {
            get => _currentData;
            set => this.RaiseAndSetIfChanged(ref _currentData, value);
        }

        public Task LoadDataAsync(bool forceReload = false)
        {
            return Task.CompletedTask;
        }

        public Task SaveDataAsync()
        {
            return Task.CompletedTask;
        }
    }
}