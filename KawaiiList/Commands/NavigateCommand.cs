using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Services;

namespace KawaiiList.Commands
{
    public class NavigateCommand: CommandBase
    {
        private readonly INavigationService _navigationService;

        public NavigateCommand(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Execute(object parameter)
        {
            _navigationService.Navigate();
        }
    }
}