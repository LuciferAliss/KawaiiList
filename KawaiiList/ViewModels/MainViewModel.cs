using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Stores;

namespace KawaiiList.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly NavigationStore _navigationStore;

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        [ObservableProperty]
        private NavigationBarViewModel _navigationBarViewModel;

        [ObservableProperty]
        private HaderViewModel _haderViewModel;

        public MainViewModel(NavigationStore navigationStore, NavigationBarViewModel navigationBarViewModel, HaderViewModel hader)
        {
            _navigationStore = navigationStore;
            NavigationBarViewModel = navigationBarViewModel;
            HaderViewModel = hader;

            CurrentViewModel = _navigationStore.CurrentViewModel;
            
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModel = _navigationStore.CurrentViewModel;
        }
    }
}