using System.Threading.Tasks;
using NeptunLight.Models;
using ReactiveUI;

namespace NeptunLight.Services
{
    public interface IDataStorage : IReactiveObject
    {
        NeptunData CurrentData { get; set; }

        Task LoadDataAsync(bool forceReload = false);

        Task SaveDataAsync();

        Task ClearDataAsync();
    }
}