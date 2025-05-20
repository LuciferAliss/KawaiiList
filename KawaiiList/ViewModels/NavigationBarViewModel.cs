using KawaiiList.Commands;
using KawaiiList.Services;
using System.Windows.Input;

namespace KawaiiList.ViewModels
{
    public partial class NavigationBarViewModel : BaseViewModel
    {
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCatalogCommand { get; }
        public ICommand NavigateScheduleCommand { get; }

        public NavigationBarViewModel(INavigationService homeNavigationService,
            INavigationService catalogNavigationService,
            INavigationService scheduleNavigationService)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(homeNavigationService);
            NavigateCatalogCommand = new NavigateCommand<CatalogViewModel>(catalogNavigationService);
            NavigateScheduleCommand = new NavigateCommand<ScheduleViewModel>(scheduleNavigationService);
        }
    }
}