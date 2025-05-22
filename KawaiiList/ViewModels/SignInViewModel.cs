using CommunityToolkit.Mvvm.Input;
using KawaiiList.Services;

namespace KawaiiList.ViewModels
{
    public partial class SignInViewModel : BaseViewModel
    {
        private readonly ICloseModalNavigationService _closeNavigationService;
        private readonly INavigationService _signpUpNavigationService;

        public SignInViewModel(ICloseModalNavigationService closeNavigationService, INavigationService signpUpNavigationService)
        {
            _closeNavigationService = closeNavigationService;
            _signpUpNavigationService = signpUpNavigationService;
        }

        [RelayCommand]
        private void Login()
        {
            _closeNavigationService.Navigate();
        }

        [RelayCommand]
        private void SignUp() => _signpUpNavigationService.Navigate();

        [RelayCommand]
        private void CloseModal() => _closeNavigationService.Navigate();
    }
}
