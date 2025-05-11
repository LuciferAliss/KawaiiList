using LibVLCSharp.Shared;

namespace KawaiiList.Services
{
    public interface IMediaControlService
    {
        public event Action FullscreenModeChanged;

        public bool IsFullscreen { get; set; }
        public bool IsPlaying { get; set; }
        public int Volume { get; set; }
        public MediaPlayer AnimeMediaPlayer { get; }

        public void CreateMediaPlayer();
        public void DisposeMediaPlayer();
        public void ToggleEpisode(string url);
    }
}