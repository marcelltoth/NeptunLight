using System;
using NeptunLight.ViewModels;

namespace NeptunLight.Services
{
    public interface INavigator
    {
        void NavigateTo<T>(bool addToStack = true) where T : PageViewModel;
        void NavigateTo(Type destinationVm, bool addToStack = true);
        void NavigateTo(PageViewModel destinationVm, bool addToStack = true);
        void NavigateUp<T>() where T : PageViewModel;
    }
}