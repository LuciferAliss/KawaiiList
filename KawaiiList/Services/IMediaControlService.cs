using KawaiiList.ViewModels;
using System.Windows;
using System.Windows.Media;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace KawaiiList.Services
{
    public interface IMediaControlService
    {
        public void SetMediaPlayer(MediaPlayer mediaPlayer);

        public void ToggleFullscreen(BaseViewModel vm);
    }
}