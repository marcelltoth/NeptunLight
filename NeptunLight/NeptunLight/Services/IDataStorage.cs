using System.Threading.Tasks;
using NeptunLight.Models;

namespace NeptunLight.Services
{
    public interface IDataStorage
    {
        Task<NeptunData> LoadDataAsync();

        Task SaveDataAsync(NeptunData data);
    }
}