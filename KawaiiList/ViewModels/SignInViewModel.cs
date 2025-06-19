using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Services;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace KawaiiList.ViewModels
{
    public partial class SignInViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly ICloseModalNavigationService _closeNavigationService;
        private readonly INavigationService _signpUpNavigationService;
        private readonly IAuthService _authService;

        private bool _usernameTouched;
        private bool _passwordTouched;

        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string? _errorMessage;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Username):
                        if (!_usernameTouched) return null;
                        if (string.IsNullOrWhiteSpace(Username))
                            return "Имя пользователя не может быть пустым";
                        if (Regex.IsMatch(Username, @"[а-яА-Я]"))
                            return "Пароль не должен содержать русских символов";
                        if (Username.Length < 3)
                            return "Имя пользователя слишком короткое";
                        break;

                    case nameof(Password):
                        if (!_passwordTouched) return null;
                        if (string.IsNullOrWhiteSpace(Password))
                            return "Пароль не может быть пустым";
                        if (Regex.IsMatch(Password, @"[а-яА-Я]"))
                            return "Пароль не должен содержать русских символов";
                        if (Password.Length < 6)
                            return "Пароль должен быть не короче 6 символов";
                        if (!Regex.IsMatch(Password, @"[A-Z]"))
                            return "Пароль должен содержать хотя бы одну заглавную букву";
                        if (!Regex.IsMatch(Password, @"[0-9]"))
                            return "Пароль должен содержать хотя бы одну цифру";
                        break;
                }

                return string.Empty;
            }
        }

        public string Error => string.Empty;

        public bool IsFormValid =>
            string.IsNullOrEmpty(this[nameof(Username)]) &&
            string.IsNullOrEmpty(this[nameof(Password)]);

        public SignInViewModel(ICloseModalNavigationService closeNavigationService, INavigationService signpUpNavigationService, IAuthService authService)
        {
            _closeNavigationService = closeNavigationService;
            _signpUpNavigationService = signpUpNavigationService;
            _authService = authService;
        }

        partial void OnUsernameChanged(string value)
        {
            _usernameTouched = true;
            OnPropertyChanged(nameof(IsFormValid));
            OnPropertyChanged("Item[]");
        }

        partial void OnPasswordChanged(string value)
        {
            _passwordTouched = true;
            OnPropertyChanged(nameof(IsFormValid));
            OnPropertyChanged("Item[]");
        }

        [RelayCommand]
        private async Task Login()
        {
            bool isLogin;
            string message;
            (isLogin, message) = await _authService.SignInAsync(Username, Password);

            if (isLogin)
            {
                _closeNavigationService.Navigate();
            }
            else
            {
                ErrorMessage = message;
            }
        }

        [RelayCommand]
        private void SignUp() => _signpUpNavigationService.Navigate();

        [RelayCommand]
        private void CloseModal() => _closeNavigationService.Navigate();
    }
}
