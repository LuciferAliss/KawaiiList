using CommunityToolkit.Mvvm.Input;
using KawaiiList.Services;

namespace KawaiiList.ViewModels
{
    public partial class SignUpViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;

        public SignUpViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        private void Login()
        {
            _navigationService.Navigate();
        }
    }
}