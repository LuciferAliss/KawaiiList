using LibVLCSharp.Shared;
using System.Diagnostics;

namespace KawaiiList.Services
{
    public class MediaControlService : IMediaControlService
    {
        public event Action FullscreenModeChanged;
        public event Action<long, float> TimeChanged;
        public event Action<long> EndTimeChanged;
        public event Action FinishAnimeChanged;

        private readonly string host = "https://cache.libria.fun";
        public MediaPlayer AnimeMediaPlayer { get; private set; }
        private Media _media;
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

        private float _timeAnime;
        public float TimeAnime
        {
            private get => _timeAnime;
            set
            {
                _timeAnime = value;
                UpdateAnimeTime();
            }
        }

        private void UpdateAnimeTime()
        {
            AnimeMediaPlayer.Position = TimeAnime;
        }

        private void OnTimeChanged(object sender, EventArgs e)
        {
            TimeChanged?.Invoke(AnimeMediaPlayer.Time, AnimeMediaPlayer.Position);
        }

        private void OnLengthChanged(object sender, EventArgs e)
        {
            EndTimeChanged?.Invoke(AnimeMediaPlayer.Length);
        }

        public void CreateMediaPlayer()
        {
            _libVLC = new LibVLC(enableDebugLogs: true);
            AnimeMediaPlayer = new MediaPlayer(_libVLC);

            AnimeMediaPlayer.TimeChanged += OnTimeChanged!;
            AnimeMediaPlayer.LengthChanged += OnLengthChanged!;
            AnimeMediaPlayer.EndReached += OnMediaEnded!;
        }

        private void OnMediaEnded(object sender, EventArgs e)
        {
            Debug.WriteLine("end");
            IsPlaying = false;
            AnimeMediaPlayer.Stop();
            AnimeMediaPlayer.Media = _media;
            TimeAnime = 0;
            FinishAnimeChanged?.Invoke();
        }

        public void DisposeMediaPlayer()
        {
            _isPlaying = false;

            AnimeMediaPlayer.TimeChanged -= OnTimeChanged!;
            AnimeMediaPlayer.LengthChanged -= OnLengthChanged!;
            AnimeMediaPlayer.EndReached -= OnMediaEnded!;

            AnimeMediaPlayer?.Dispose();
            _libVLC?.Dispose();
        }

        public void ToggleEpisode(string url)
        {
            _media = new(_libVLC, host + url, FromType.FromLocation);
            _media.Parse(MediaParseOptions.ParseNetwork);
            AnimeMediaPlayer.Media = _media;
        }

        private void OnFullscreenModeChanged()
        {
            FullscreenModeChanged?.Invoke();
        }
    }
}