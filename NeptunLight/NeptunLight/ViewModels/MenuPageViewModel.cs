using NeptunLight.DataAccess;
using NeptunLight.Services;
using ReactiveUI;

namespace NeptunLight.ViewModels
{
    public class MenuPageViewModel : PageViewModel
    {
        public MenuPageViewModel(IDataStorage storage, INeptunInterface client)
        {
            
        }

        private bool _loadingDialogShown;

        public bool LoadingDialogShown
        {
            get => _loadingDialogShown;
            set => this.RaiseAndSetIfChanged(ref _loadingDialogShown, value);
        }

        private string _loadingDialogText;

        public string LoadingDialogText
        {
            get => _loadingDialogText;
            set => this.RaiseAndSetIfChanged(ref _loadingDialogText, value);
        }
    }
}