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
        private readonly AnimeTitle _anime;

        [ObservableProperty]
        private MediaPlayer _mediaPlayer;

        public WatchAnimeViewModel(VlcService vlcService, AnimeStore animeStore)
        {
            _vlcService = vlcService;
            _anime = animeStore.CurrentAnime;
            MediaPlayer = _vlcService.CreatePlayer();
        }

        [RelayCommand]
        public void Play()
        {
            string url = "https://" + _anime.Player?.Host + _anime.Player?.List?["1"].Hls?.Hd ?? "";
            var media = _vlcService.CreateMedia(url);
            
            MediaPlayer.Play(media);
        }

        [RelayCommand]
        public void Pause()
        {
            MediaPlayer?.Pause();
        }
    }
}
