using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using LibVLCSharp.Shared;

namespace KawaiiList.ViewModels
{
    public partial class WatchAnimeViewModel : BaseViewModel
    {
        private readonly IMediaControlService _mediaService;
        private readonly IScreenService _screenService;

        private HlsLinks _videoResolution = new();
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
        private string _audioIconKind = "VolumeHigh";

        [ObservableProperty]
        private List<string> _episodesText = [];

        [ObservableProperty]
        private string _selectedEpisode = "";

        [ObservableProperty]
        private int _episode;

        [ObservableProperty]
        private List<string> _videoResolutionText = [];

        [ObservableProperty]
        private string _selectedResolution = "";

        [ObservableProperty]
        private string _nameAnime = "";

        [ObservableProperty]
        private string _nameEpisode = "";

        [ObservableProperty]
        private string _currentTimeEpisode = "00:00";

        [ObservableProperty]
        private string _endTimeEpisode = "24:00";

        [ObservableProperty]
        private long _valueTimeSlider;

        [ObservableProperty]
        private long _valueEndTimeSlider = 1;

        [ObservableProperty]
        private bool _isCheckedOpenPopue = false;

        public WatchAnimeViewModel(AnimeStore animeStore, IMediaControlService mediaService, IScreenService screenService)
        {
            Anime = animeStore.CurrentAnime;
            _mediaService = mediaService;
            _screenService = screenService;

            ScreenHeight = 540;
            ScreenWidth = 960;

            _mediaService.CreateMediaPlayer();
            NameAnime = _anime.Names?.Ru ?? _anime.Names?.En ?? "";
            Volume = 60;
            Episode = (int)(_anime.Player!.List!.Keys!.Min());

            foreach (var item in _anime.Player!.List!.Keys)
            {
                string str = $"Серия {item}";
                EpisodesText.Add(str);
            }

            AnimeMediaPlayer = _mediaService.AnimeMediaPlayer; 
        }

        partial void OnVolumeChanged(int value)
        {
            _mediaService.Volume = value;
        }

        partial void OnSelectedEpisodeChanged(string value)
        {
            _mediaService.IsPlaying = false;
            PlayIconKind = _mediaService.IsPlaying ? "Pause" : "Play";

            Episode = int.Parse(value.Split(" ")[1]);
        }

        partial void OnEpisodeChanged(int value)
        {
            VideoResolutionText.Clear();

            NameEpisode = _anime.Player?.List?[Episode]?.Name ?? "";
            _videoResolution = _anime.Player?.List?[Episode].Hls ?? new HlsLinks();

            string? bestResolutionLink = _videoResolution.Fhd
                          ?? _videoResolution.Hd
                          ?? _videoResolution.Sd;

            if (_videoResolution.Fhd != null)
            {
                VideoResolutionText.Add("1080p");
            }
            if (_videoResolution.Hd != null)
            {
                VideoResolutionText.Add("720p");
            }
            if (_videoResolution.Sd != null)
            {
                VideoResolutionText.Add("480p");
            }

            SelectedResolution = VideoResolutionText[0];

            _mediaService.ToggleEpisode(bestResolutionLink ?? "");

            _mediaService.TimeChanged += OnTimeChanged;
            _mediaService.EndTimeChanged += OnLengthAnimeChanged;
            _mediaService.FinishAnimeChanged += FinishAnimeChanged;
        }

        partial void OnSelectedResolutionChanged(string value)
        {
            switch (SelectedResolution)
            {
                case "1080p":
                    _mediaService.ToggleSelectedResolution(_videoResolution.Fhd ?? "");
                    break;
                case "720p":
                    _mediaService.ToggleSelectedResolution(_videoResolution.Hd ?? "");
                    break;
                case "480p":
                    _mediaService.ToggleSelectedResolution(_videoResolution.Sd ?? "");
                    break;
            }
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
        public void ToggleSettings()
        {
            IsCheckedOpenPopue = !IsCheckedOpenPopue;
        }

        [RelayCommand]
        public void TogglePlaying()
        {
            _mediaService.IsPlaying = !_mediaService.IsPlaying;
            PlayIconKind = _mediaService.IsPlaying ? "Pause" : "Play";
        }

        [RelayCommand]
        private void ToggleMuteAudio()
        {
            _mediaService.Mute = !_mediaService.Mute;
            AudioIconKind = _mediaService.Mute ? "VolumeMute" : "VolumeHigh";
        }

        [RelayCommand]
        public void ToggleFullscreen()
        {
            _mediaService.IsFullscreen = !_mediaService.IsFullscreen;

            ScreenHeight = _mediaService.IsFullscreen ? 540 : _screenService.GetScreenHeight() ;
            ScreenWidth = _mediaService.IsFullscreen ? 960 : _screenService.GetScreenWidth() ;
        }

        private void OnTimeChanged(long time)
        {
            CurrentTimeEpisode = FormatTime(time);

            if (!_isDraggingSlider)
            {
                ValueTimeSlider = time;
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
            ValueEndTimeSlider = time;
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
