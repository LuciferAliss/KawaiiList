using KawaiiList.Stores;
using KawaiiList.ViewModels;

namespace KawaiiList.Services
{
    public class ModalNavigationService<TViewModel> : INavigationService
        where TViewModel : BaseViewModel
    {
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly Func<TViewModel> _createViewModel;

        public ModalNavigationService(ModalNavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _modalNavigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void Navigate()
        {
            _modalNavigationStore.CurrentViewModel = _createViewModel();
        }
    }
}
