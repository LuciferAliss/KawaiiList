using KawaiiList.ViewModels;
using LibVLCSharp.Shared;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace KawaiiList.Services
{
    public interface IMediaControlService
    {
        public void SetMediaPlayer(MediaPlayer mediaPlayer, LibVLC libVLC);

        public void ToggleFullscreen(MediaPlayer player);
    }
}