using System;
using System.Threading.Tasks;
using ReactiveUI;

namespace NeptunLight.Services
{
    public interface IRefreshManager : IReactiveObject
    {
        bool IsRefreshing { get; set; }
        DateTime LastRefreshTime { get; set; }

        Task RefreshAsync();
        Task RefreshIfNeeded();
    }
}