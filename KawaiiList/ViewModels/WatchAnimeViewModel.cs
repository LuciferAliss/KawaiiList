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

        private readonly string host;
        public LibVLC LibVLC { get; }

        [ObservableProperty]
        private AnimeTitle _anime;

        [ObservableProperty]
        private MediaPlayer _animeMediaPlayer;

        [ObservableProperty]
        private double _screenWidth;

        [ObservableProperty]
        private double _screenHeight;

        public WatchAnimeViewModel(AnimeStore animeStore, IMediaControlService mediaService, IScreenService screenService)
        {
            Anime = animeStore.CurrentAnime;
            _mediaService = mediaService;
            _screenService = screenService;

            ScreenHeight = 504;
            ScreenWidth = 896;

            Core.Initialize();
            LibVLC = new LibVLC(enableDebugLogs: true);
            host = "https://cache.libria.fun";

            string url = host + _anime.Player?.List?["1"].Hls?.Fhd ?? "";

            var media = new Media(LibVLC, url, FromType.FromLocation);
            AnimeMediaPlayer = new MediaPlayer(LibVLC)
            {
                Media = media
            };
        }

        [RelayCommand]
        public void ManageStartAndStopVideo()
        {
            if (AnimeMediaPlayer.IsPlaying)
            {
                AnimeMediaPlayer?.Pause();
            }
            else
            {
                AnimeMediaPlayer.Play();
            }
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
            AnimeMediaPlayer?.Stop();
            AnimeMediaPlayer?.Dispose();
            LibVLC?.Dispose();

            base.Dispose();
        }
    }
}
