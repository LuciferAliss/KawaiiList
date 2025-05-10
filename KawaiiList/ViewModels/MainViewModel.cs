using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace KawaiiList.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly NavigationStore _navigationStore;
        private readonly IMediaControlService _mediaService;

        [ObservableProperty]
        private ObservableObject _currentViewModel;

        [ObservableProperty]
        private NavigationBarViewModel _navigationBarViewModel;

        [ObservableProperty]
        private HaderViewModel _haderViewModel;

        [ObservableProperty]
        private bool _isFullscreen = true;

        [ObservableProperty]
        private int _marginMainConent = 25;

        [ObservableProperty]
        private WindowState _windowState = WindowState.Normal;

        public MainViewModel(NavigationStore navigationStore,
            NavigationBarViewModel navigationBarViewModel,
            HaderViewModel hader,
            IMediaControlService mediaService)
        {
            _navigationStore = navigationStore;
            NavigationBarViewModel = navigationBarViewModel;
            HaderViewModel = hader;
            _mediaService = mediaService;

            CurrentViewModel = _navigationStore.CurrentViewModel;
            
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _mediaService.FullscreenModeChanged += OnFullscreenModeChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModel = _navigationStore.CurrentViewModel;
        }

        private void OnFullscreenModeChanged()
        {
            IsFullscreen = _mediaService.IsFullscreen;
            MarginMainConent = IsFullscreen ? 25 : 4;
            WindowState = IsFullscreen ?  WindowState.Normal : WindowState.Maximized;
        }
    }
}