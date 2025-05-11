using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using LibVLCSharp.Shared;
using System.Windows.Threading;

namespace KawaiiList.ViewModels
{
    public partial class WatchAnimeViewModel : BaseViewModel
    {
        private readonly IMediaControlService _mediaService;
        private readonly IScreenService _screenService; 

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


        override public void Dispose()
        {
            _mediaService.DisposeMediaPlayer();

            base.Dispose();
        }
    }
}
