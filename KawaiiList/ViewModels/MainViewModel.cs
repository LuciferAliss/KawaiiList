using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Windows;

namespace KawaiiList.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modelNavigationStore;
        private readonly IMediaControlService _mediaService;

        [ObservableProperty]
        private BaseViewModel _currentViewModel;

        [ObservableProperty]
        private BaseViewModel _currentModalViewModel;

        [ObservableProperty]
        private NavigationBarViewModel _navigationBarViewModel;

        [ObservableProperty]
        private HaderViewModel _haderViewModel;

        [ObservableProperty]
        private bool _isOpenModalView;

        [ObservableProperty]
        private bool _isFullscreen = true;

        [ObservableProperty]
        private int _marginMainConent = 25;

        [ObservableProperty]
        private WindowState _windowState = WindowState.Normal;

        public MainViewModel(NavigationStore navigationStore,
            ModalNavigationStore modelNavigationStore,
            NavigationBarViewModel navigationBarViewModel,
            HaderViewModel hader,
            IMediaControlService mediaService)
        {
            _navigationStore = navigationStore;
            _modelNavigationStore = modelNavigationStore;
            NavigationBarViewModel = navigationBarViewModel;
            HaderViewModel = hader;
            _mediaService = mediaService;

            CurrentViewModel = _navigationStore.CurrentViewModel;
            CurrentModalViewModel = _modelNavigationStore.CurrentViewModel;
            IsOpenModalView = _modelNavigationStore.IsOpen;

            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modelNavigationStore.CurrentViewModelChanged += OnCurrentModalViewModelChanged;
            _mediaService.FullscreenModeChanged += OnFullscreenModeChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModel = _navigationStore.CurrentViewModel;
        }

        private void OnCurrentModalViewModelChanged()
        {
            CurrentModalViewModel = _modelNavigationStore.CurrentViewModel;
            IsOpenModalView = _modelNavigationStore.IsOpen;
        }

        private void OnFullscreenModeChanged()
        {
            IsFullscreen = _mediaService.IsFullscreen;
            MarginMainConent = IsFullscreen ? 25 : 4;
            WindowState = IsFullscreen ?  WindowState.Normal : WindowState.Maximized;
        }
    }
}