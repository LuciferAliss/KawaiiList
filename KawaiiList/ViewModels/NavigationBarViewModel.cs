using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Commands;
using KawaiiList.Services;
using System.Windows.Input;

namespace KawaiiList.ViewModels
{
    public partial class NavigationBarViewModel : ObservableObject
    {
        public ICommand NavigateHomeCommand { get; }

        public NavigationBarViewModel(INavigationService homeNavigationService)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(homeNavigationService);
        }
    }
}