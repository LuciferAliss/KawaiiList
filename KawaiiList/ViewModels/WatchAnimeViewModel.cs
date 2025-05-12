using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using LibVLCSharp.Shared;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace KawaiiList.ViewModels
{
    public partial class WatchAnimeViewModel : BaseViewModel
    {
        private readonly IMediaControlService _mediaService;
        private readonly IScreenService _screenService;

        private bool _isDraggingSlider = false;

        [ObservableProperty]
        private AnimeTitle _anime;

        [ObservableProperty]
        private MediaPlayer _animeMediaPlayer;

        [ObservableProperty]
        private double _screenWidth;

        [ObservableProperty]
        private double _screenHeight;

        [ObservableProperty]
        private int _volume;

        [ObservableProperty]
        private string _playIconKind = "Play";

        [ObservableProperty]
        private int _episode;

        [ObservableProperty]
        private string _nameEpisode = "";

        [ObservableProperty]
        private string _currentTimeEpisode = "00:00";

        [ObservableProperty]
        private string _endTimeEpisode = "24:00";

        [ObservableProperty]
        private float _valueTimeSlider;

        public WatchAnimeViewModel(AnimeStore animeStore, IMediaControlService mediaService, IScreenService screenService)
        {
            Anime = animeStore.CurrentAnime;
            _mediaService = mediaService;
            _screenService = screenService;

            ScreenHeight = 504;
            ScreenWidth = 896;

            _mediaService.CreateMediaPlayer();
            Volume = 60;
            Episode = 1;

            AnimeMediaPlayer = _mediaService.AnimeMediaPlayer; 
        }

        partial void OnVolumeChanged(int value)
        {
            _mediaService.Volume = value;
        }

        partial void OnEpisodeChanged(int value)
        {
            NameEpisode = _anime.Player?.List?[$"{Episode}"]?.Name ?? "";
            _mediaService.ToggleEpisode(_anime.Player?.List?[$"{Episode}"].Hls?.Fhd ?? "");

            _mediaService.TimeChanged += OnTimeChanged;
            _mediaService.EndTimeChanged += OnLengthAnimeChanged;
            _mediaService.FinishAnimeChanged += FinishAnimeChanged;
        }

        [RelayCommand]
        private void SliderReleased()
        {
            _mediaService.TimeAnime = ValueTimeSlider;
            _isDraggingSlider = false;
        }

        [RelayCommand]
        private void SliderDragStarted()
        {
            _isDraggingSlider = true;
        }

        [RelayCommand]
        public void TogglePlaying()
        {
            _mediaService.IsPlaying = !_mediaService.IsPlaying;
            PlayIconKind = _mediaService.IsPlaying ? "Pause" : "Play";
        }

        [RelayCommand]
        public void ToggleFullscreen()
        {
            _mediaService.IsFullscreen = !_mediaService.IsFullscreen;

            ScreenHeight = _mediaService.IsFullscreen ? 504 : _screenService.GetScreenHeight() ;
            ScreenWidth = _mediaService.IsFullscreen ? 896 : _screenService.GetScreenWidth() ;
        }

        private void OnTimeChanged(long time, float position)
        {
            CurrentTimeEpisode = FormatTime(time);

            if (!_isDraggingSlider)
            {
                ValueTimeSlider = position;
            }
        }

        private void FinishAnimeChanged()
        {
            PlayIconKind = _mediaService.IsPlaying ? "Pause" : "Play";
            CurrentTimeEpisode = "00:00";
            ValueTimeSlider = 0;
        }

        private void OnLengthAnimeChanged(long time)
        {
            EndTimeEpisode = FormatTime(time);
        }

        private string FormatTime(long milliseconds)
        {
            var time = TimeSpan.FromMilliseconds(milliseconds);
            return time.ToString(@"mm\:ss");
        }

        override public void Dispose()
        {
            _mediaService.DisposeMediaPlayer();
            _mediaService.TimeChanged -= OnTimeChanged;
            _mediaService.EndTimeChanged -= OnLengthAnimeChanged;
            _mediaService.FinishAnimeChanged -= FinishAnimeChanged;

            base.Dispose();
        }
    }
}
