using LibVLCSharp.Shared;

namespace KawaiiList.Services
{
    public class MediaControlService : IMediaControlService
    {
        public event Action FullscreenModeChanged;

        private readonly string host = "https://cache.libria.fun";
        public MediaPlayer AnimeMediaPlayer { get; private set; }
        private LibVLC _libVLC;

        private bool _isFullscreen = true;
        public bool IsFullscreen
        {
            get => _isFullscreen;
            set
            {
                _isFullscreen = value;
                OnFullscreenModeChanged();
            }
        }

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                if (!_isPlaying)
                {
                    AnimeMediaPlayer?.Pause();
                }
                else
                {
                    AnimeMediaPlayer.Play();
                }
            }
        }

        public int Volume
        {
            get => AnimeMediaPlayer.Volume;
            set
            {
                AnimeMediaPlayer.Volume = value;
            }
        }

        public void CreateMediaPlayer()
        {
            _libVLC = new LibVLC(enableDebugLogs: true);
            AnimeMediaPlayer = new MediaPlayer(_libVLC);
        }

        public void DisposeMediaPlayer()
        {
            _isPlaying = false;
            AnimeMediaPlayer?.Dispose();
            _libVLC?.Dispose();
        }

        public void ToggleEpisode(string url)
        {
            Media media = new(_libVLC, host + url, FromType.FromLocation);
            AnimeMediaPlayer.Media = media;
        }

        private void OnFullscreenModeChanged()
        {
            FullscreenModeChanged?.Invoke();
        }
    }
}