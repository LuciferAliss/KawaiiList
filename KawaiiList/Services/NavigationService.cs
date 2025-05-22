using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Stores;
using KawaiiList.ViewModels;

namespace KawaiiList.Services
{
    public class NavigationService<TViewModel> : INavigationService 
        where TViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        private readonly Func<TViewModel> _createViewModel;

        public NavigationService(NavigationStore navigationStore, Func<TViewModel> createViewModel)
        {
            _navigationStore = navigationStore;
            _createViewModel = createViewModel;
        }

        public void Navigate()
        {
            if (_createViewModel == null)
                throw new InvalidOperationException("Create view model function is not initialized");

            _navigationStore.CurrentViewModel = _createViewModel();
        }
    }
}