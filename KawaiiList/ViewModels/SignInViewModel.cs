using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Services;
using System.ComponentModel;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace KawaiiList.ViewModels
{
    public partial class SignInViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly ICloseModalNavigationService _closeNavigationService;
        private readonly INavigationService _signpUpNavigationService;
        private readonly IAuthService _authService;

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
                }

                return null;
            }
        }

        public string Error => null;

        public bool IsFormValid =>
            string.IsNullOrEmpty(this[nameof(Username)]) &&
            string.IsNullOrEmpty(this[nameof(Password)]);

        public SignInViewModel(ICloseModalNavigationService closeNavigationService, INavigationService signpUpNavigationService, IAuthService authService)
        {
            _closeNavigationService = closeNavigationService;
            _signpUpNavigationService = signpUpNavigationService;
            _authService = authService;
        }

        partial void OnUsernameChanged(string value) =>
            OnPropertyChanged(nameof(IsFormValid));

        partial void OnPasswordChanged(string value) =>
            OnPropertyChanged(nameof(IsFormValid));

        [RelayCommand]
        private async Task Login()
        {
            if (await _authService.SignInAsync(Username, Password))
            {
                _closeNavigationService.Navigate();
            }
            else
            {
                ErrorMessage = "Не удалось зарегистрироваться. Проверьте данные и попробуйте снова.";
            }
        }

        [RelayCommand]
        private void SignUp() => _signpUpNavigationService.Navigate();

        [RelayCommand]
        private void CloseModal() => _closeNavigationService.Navigate();
    }
}
