using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Commands;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Windows.Input;

namespace KawaiiList.ViewModels
{
    public partial class HaderViewModel : BaseViewModel
    {
        public ICommand NavigateSingUpCommand { get; }

        private readonly UserStore _userStore;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private SearchViewModel _searchAnimeViewModel;

        [ObservableProperty]
        private string _nickname = "";

        [ObservableProperty]
        private bool _isOpenPopup = false;

        public bool IsLoggedIn => _userStore.IsLoggedIn;

        public HaderViewModel(SearchViewModel search, UserStore userStore, INavigationService singUpnavigationService, IAuthService authService)
        {
            NavigateSingUpCommand = new NavigateCommand(singUpnavigationService);
            SearchAnimeViewModel = search;
            _userStore = userStore;
            _authService = authService;

            _userStore.CurrentUserChanged += UserChanged;

            LoadUser();
        }

        private void LoadUser()
        {
            if (_userStore.CurrentUser != null)
            {
                Nickname = _userStore.CurrentUser.Nickname;
            }
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        private void UserChanged()
        {
            IsOpenPopup = false;
            LoadUser();
        }

        [RelayCommand]
        private async Task SignOut()
        {
            await _authService.SignOutAsync();
        }

        [RelayCommand]
        private void ToglePopup() => IsOpenPopup = !IsOpenPopup;
    }
}