using LibVLCSharp.Shared;

namespace KawaiiList.Services
{
    public interface IMediaControlService
    {
        public event Action FullscreenModeChanged;
        public event Action<long> TimeChanged;
        public event Action<long> EndTimeChanged;
        public event Action FinishAnimeChanged;

        public bool IsFullscreen { get; set; }
        public bool IsPlaying { get; set; }
        public int Volume { get; set; }
        public long TimeAnime { set; }
        public MediaPlayer AnimeMediaPlayer { get; }

        public void CreateMediaPlayer();
        public void DisposeMediaPlayer();
        public void ToggleEpisode(string url);
    }
}