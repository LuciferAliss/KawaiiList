using KawaiiList.Components;
using KawaiiList.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace KawaiiList.Services
{
    public class MediaControlService : IMediaControlService
    {
        private MediaPlayer? _mediaPlayer;
        
        private readonly Func<FullscreenPlayerWindow> _windowFactory;
        private Window? _fullscreenWindow;

        public MediaControlService(Func<FullscreenPlayerWindow> windowFactory)
        {
            _windowFactory = windowFactory;
        }

        public void SetMediaPlayer(MediaPlayer player)
        {
            _mediaPlayer = player;
        }

        public void ToggleFullscreen(BaseViewModel vm)
        {
            if (_fullscreenWindow == null)
            {
                _fullscreenWindow = _windowFactory();
                _fullscreenWindow.DataContext = vm;
                _fullscreenWindow.Show();
            }
            else
            {
                _fullscreenWindow.Close();
                _fullscreenWindow = null;
            }
        }
    }
}
