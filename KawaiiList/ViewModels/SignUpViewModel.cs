using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Services;
using System.Text.RegularExpressions;

namespace KawaiiList.ViewModels
{
    public partial class SignUpViewModel : BaseViewModel
    {
        private readonly ICloseModalNavigationService _closeNavigationService;
        private readonly INavigationService _signInNavigationService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _email = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _confirmPassword = "";

        [ObservableProperty]
        private string _errorMessage = "";

        public SignUpViewModel(ICloseModalNavigationService closeNavigationService, INavigationService signInNavigationService, IAuthService authService)
        {
            _closeNavigationService = closeNavigationService;
            _signInNavigationService = signInNavigationService;
            _authService = authService;
        }

        partial void OnPasswordChanged(string value)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9!@#\$_%&*-]");

            var filtered = regex.Replace(value, "");

            if (filtered != value)
            {
                Password = filtered;
            }
        }

        [RelayCommand]
        private void Register()
        {
            if (Password == ConfirmPassword)

            _closeNavigationService.Navigate();
        }

        [RelayCommand]
        private void SignIn() => _signInNavigationService.Navigate();

        [RelayCommand]
        private void CloseModal() => _closeNavigationService.Navigate();
    }
}