using System;
using System.Threading.Tasks;
using NeptunLight.Models;

namespace NeptunLight.DataAccess
{
    public interface INeptunInterface
    {
        Task LoginAsync();

        Task<NeptunData> RefreshDataAsnyc(IProgress<string> progress = null);
    }
}