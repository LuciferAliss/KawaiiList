using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;

namespace KawaiiList.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly IAnilibriaService _anilibriaService;
        private readonly UserStore _userStore;
        private readonly INavigationService _navigationHomeService;
        private readonly INavigationService _navigationEditingProfileService;
       
        public User CurrentUser => _userStore.CurrentUser;

        [ObservableProperty]
        private string _nickname = "";

        [ObservableProperty]
        private string _avatarUrl = "";

        [ObservableProperty]
        private string _bannerUrl = "";

        [ObservableProperty]
        private int _selectViewModel = 0;

        [ObservableProperty]
        private TitleAnimeListViewModel _titleAnimeListViewModel;

        public ProfileViewModel(
            IAnilibriaService anilibriaService,
            UserStore userStore,
            INavigationService navigationHomeService,
            INavigationService navigationEditingProfileService,
            TitleAnimeListViewModel titleAnimeListViewModel)
        {
            _anilibriaService = anilibriaService;
            _userStore = userStore;
            _navigationHomeService = navigationHomeService;
            _navigationEditingProfileService = navigationEditingProfileService;

            TitleAnimeListViewModel = titleAnimeListViewModel;

            _userStore.CurrentUserChanged += LoadUser;

            LoadUser();
        }

        private void LoadUser()
        {
            if (_userStore.CurrentUser != null)
            {
                Nickname = _userStore.CurrentUser.Nickname;
                AvatarUrl = _userStore.CurrentUser.Images.AvatarUrl;
                BannerUrl = _userStore.CurrentUser.Images.BannerUrl;
            }
            else
            {
                _navigationHomeService.Navigate();
            }
        }

        [RelayCommand]
        private void OpenWindowEditingProfile()
        {
            _navigationEditingProfileService.Navigate();
        }

        public override void Dispose()
        {
            _userStore.CurrentUserChanged -= LoadUser;
            _titleAnimeListViewModel.Dispose();
            
            base.Dispose();
        }
    }
}
