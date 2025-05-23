using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Commands;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Windows;
using System.Windows.Input;

namespace KawaiiList.ViewModels
{
    public partial class HaderViewModel : BaseViewModel
    {
        public ICommand NavigateSingUpCommand { get; }

        private readonly UserStore _userStore;

        [ObservableProperty]
        private SearchViewModel _searchAnimeViewModel;

        [ObservableProperty]
        private string _nickname = "";

        public bool IsLoggedIn => _userStore.IsLoggedIn;

        public HaderViewModel(SearchViewModel search, UserStore userStore, INavigationService singUpnavigationService)
        {
            NavigateSingUpCommand = new NavigateCommand(singUpnavigationService);
            SearchAnimeViewModel = search;
            _userStore = userStore;

            _userStore.CurrentUserChanged += UserChanged;

            LoadUser();
        }

        private void LoadUser()
        {
            if (_userStore.CurrentUser != null)
            {
                Nickname = _userStore.CurrentUser.Nickname;
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        private void UserChanged()
        {
            LoadUser();
        }
    }
}