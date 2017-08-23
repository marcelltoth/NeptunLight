using NeptunLight.DataAccess;

namespace NeptunLight.ViewModels
{
    public class MenuPageViewModel : PageViewModel
    {
        public MenuPageViewModel(INeptunInterface dataSource)
        {
            DataSource = dataSource;
        }

        public INeptunInterface DataSource { get; }
    }
}