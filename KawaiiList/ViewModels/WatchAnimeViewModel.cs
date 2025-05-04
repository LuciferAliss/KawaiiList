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
        private readonly string host;
        public LibVLC LibVLC { get; }

        [ObservableProperty]
        private AnimeTitle _anime;

        [ObservableProperty]
        private MediaPlayer _animeMediaPlayer;

        public WatchAnimeViewModel(AnimeStore animeStore)
        {
            Anime = animeStore.CurrentAnime;

            Core.Initialize();
            LibVLC = new LibVLC(enableDebugLogs: true);
            host = "https://cache.libria.fun";

            string url = _anime.Player?.List?["1"].Hls?.Hd ?? "";
            var media = new Media(LibVLC, host + url, FromType.FromLocation);
            AnimeMediaPlayer = new MediaPlayer(LibVLC);

            AnimeMediaPlayer.Media = media;
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
            AnimeMediaPlayer.Fullscreen = !AnimeMediaPlayer.Fullscreen;
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
