using LibVLCSharp.Shared;
using System.Threading;

namespace KawaiiList.Services
{
    public class MediaControlService : IMediaControlService
    {
        public event Action FullscreenModeChanged;
        public event Action<long> TimeChanged;
        public event Action<long> EndTimeChanged;
        public event Action FinishAnimeChanged;

        private readonly string host = "https://cache.libria.fun";
        public MediaPlayer? AnimeMediaPlayer { get; private set; }
        private Media? _media;
        private LibVLC? _libVLC;

        private bool _mute = false;
        public bool Mute
        {
            get => _mute;
            set
            {
                _mute = value;
                AnimeMediaPlayer!.Mute = _mute;
            }
        }

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
                    AnimeMediaPlayer?.Play();
                }
            }
        }

        public int Volume
        {
            get => AnimeMediaPlayer!.Volume;
            set
            {
                AnimeMediaPlayer!.Volume = value;
            }
        }

        private long _timeAnime;
        public long TimeAnime
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
            AnimeMediaPlayer!.Time = TimeAnime;
        }

        private void OnTimeChanged(object sender, EventArgs e)
        {
            TimeChanged?.Invoke(AnimeMediaPlayer!.Time);

            if (AnimeMediaPlayer?.Length - 1000 <= AnimeMediaPlayer?.Time)
            {
                AnimeMediaPlayer.Position = 0;
                IsPlaying = false;

                FinishAnimeChanged?.Invoke();
            }
        }

        private void OnLengthChanged(object sender, EventArgs e)
        {
            EndTimeChanged?.Invoke(AnimeMediaPlayer!.Length - 1000);
        }

        public void CreateMediaPlayer()
        {
            _libVLC = new LibVLC();
            AnimeMediaPlayer = new MediaPlayer(_libVLC);

            AnimeMediaPlayer.TimeChanged += OnTimeChanged!;
            AnimeMediaPlayer.LengthChanged += OnLengthChanged!;
        }

        public void DisposeMediaPlayer()
        {
            _isPlaying = false;

            AnimeMediaPlayer!.TimeChanged -= OnTimeChanged!;
            AnimeMediaPlayer!.LengthChanged -= OnLengthChanged!;
            _timeAnime = 0;

            AnimeMediaPlayer?.Dispose();
            _libVLC?.Dispose();

            AnimeMediaPlayer = null;
            _libVLC = null;
        }

        public void ToggleEpisode(string url)
        {
            _media = new(_libVLC!, host + url, FromType.FromLocation);
            _media.Parse(MediaParseOptions.ParseNetwork);
            AnimeMediaPlayer!.Media = _media;
            
            AnimeMediaPlayer.Volume = Volume;
            AnimeMediaPlayer.Mute = Mute;
        }
        
        public void ToggleSelectedResolution(string url)
        {
            float time = AnimeMediaPlayer!.Position;
            bool isPlaing = IsPlaying;

            IsPlaying = false;
            _media = new(_libVLC!, host + url, FromType.FromLocation);
            _media.Parse(MediaParseOptions.ParseNetwork);
            AnimeMediaPlayer!.Media = _media;
            IsPlaying = isPlaing;
            AnimeMediaPlayer!.Position = time;
            AnimeMediaPlayer.Volume = Volume;
            AnimeMediaPlayer.Mute = Mute;
        }

        private void OnFullscreenModeChanged()
        {
            FullscreenModeChanged?.Invoke();
        }
    }
}