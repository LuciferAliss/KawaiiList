using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;

namespace KawaiiList.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly IAnilibriaService _anilibriaService;
        private readonly UserStore _userStore;

        public User CurrentUser => _userStore.CurrentUser;

        [ObservableProperty]
        private string _nickname = "";

        [ObservableProperty]
        private string _avatarUrl = "";

        [ObservableProperty]
        private string _bannerUrl = "";

        public ProfileViewModel(IAnilibriaService anilibriaService, UserStore userStore)
        {
            _anilibriaService = anilibriaService;
            _userStore = userStore;

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
        }
    }
}
