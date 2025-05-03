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
        private readonly VlcService _vlcService;

        [ObservableProperty]
        private AnimeTitle _anime;

        [ObservableProperty]
        private MediaPlayer _animeMediaPlayer;

        public WatchAnimeViewModel(VlcService vlcService, AnimeStore animeStore)
        {
            _vlcService = vlcService;
            AnimeMediaPlayer = _vlcService.AnimeMediaPlayer;
            Anime = animeStore.CurrentAnime;

            _vlcService.FullScreenModeChanged += UpdateFullScreenMode;
        }

        [RelayCommand]
        public void Play()
        {
            string url = _anime.Player?.List?["1"].Hls?.Hd ?? "";
            var media = _vlcService.CreateMedia(url);

            AnimeMediaPlayer.Play(media);
        }

        [RelayCommand]
        public void Pause()
        {
            AnimeMediaPlayer?.Pause();
        }

        override public void Dispose()
        {
            AnimeMediaPlayer?.Dispose();
            base.Dispose();
        }

        private void UpdateFullScreenMode()
        {
            AnimeMediaPlayer = _vlcService.AnimeMediaPlayer;
        }
    }
}
