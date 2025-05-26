using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Services;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace KawaiiList.ViewModels
{
    public partial class SignUpViewModel : BaseViewModel, IDataErrorInfo
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
        private string? _errorMessage;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Username):
                        if (string.IsNullOrWhiteSpace(Username))
                            return "Имя пользователя не может быть пустым";
                        if (Username.Length < 3)
                            return "Имя пользователя слишком короткое";
                        break;

                    case nameof(Password):
                        if (string.IsNullOrWhiteSpace(Password))
                            return "Пароль не может быть пустым";
                        if (Password.Length < 6)
                            return "Пароль должен быть не короче 6 символов";
                        if (!Regex.IsMatch(Password, @"[A-Z]"))
                            return "Пароль должен содержать хотя бы одну заглавную букву";
                        if (!Regex.IsMatch(Password, @"[0-9]"))
                            return "Пароль должен содержать хотя бы одну цифру";
                        break;

                    case nameof(Email):
                        if (string.IsNullOrWhiteSpace(Email))
                                return "Email не может быть пустым";
                        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                            return "Неверный формат email";
                        break;

                    case nameof(ConfirmPassword):
                        if (ConfirmPassword != Password)
                            return "Пароли не совпадают";
                        break;
                }

                return null;
            }
        }

        public string Error => null;

        public bool IsFormValid =>
            string.IsNullOrEmpty(this[nameof(Username)]) &&
            string.IsNullOrEmpty(this[nameof(Password)]) &&
            string.IsNullOrEmpty(this[nameof(ConfirmPassword)]) &&
            string.IsNullOrEmpty(this[nameof(Email)]);

        public SignUpViewModel(ICloseModalNavigationService closeNavigationService, INavigationService signInNavigationService, IAuthService authService)
        {
            _closeNavigationService = closeNavigationService;
            _signInNavigationService = signInNavigationService;
            _authService = authService;
        }


        partial void OnUsernameChanged(string value) =>
            OnPropertyChanged(nameof(IsFormValid));

        partial void OnPasswordChanged(string value) =>
            OnPropertyChanged(nameof(IsFormValid));

        partial void OnConfirmPasswordChanged(string value) =>
            OnPropertyChanged(nameof(IsFormValid));

        partial void OnEmailChanged(string value) =>
            OnPropertyChanged(nameof(IsFormValid));

        [RelayCommand]
        private async Task Register()
        {
            if (await _authService.SignUpAsync(Email, Password, Username, Username))
            {
                _signInNavigationService.Navigate();
            }
            else
            {
                ErrorMessage = "Не удалось зарегистрироваться. Проверьте данные и попробуйте снова.";
            }
        }

        [RelayCommand]
        private void SignIn() => _signInNavigationService.Navigate();

        [RelayCommand]
        private void CloseModal() => _closeNavigationService.Navigate();
    }
}