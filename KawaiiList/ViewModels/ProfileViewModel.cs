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

        public ProfileViewModel(IAnilibriaService anilibriaService, UserStore userStore)
        {
            _anilibriaService = anilibriaService;
            _userStore = userStore;
        }
    }
}
