using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models.Anilibria;
using KawaiiList.Stores;

namespace KawaiiList.ViewModels
{
    public partial class AnimeInfoViewModel : BaseViewModel
    {
        private readonly AnimeStore _animeStore;

        [ObservableProperty]
        private AnimeTitle _anime;

        public AnimeInfoViewModel(AnimeStore animeStore)
        {
            _animeStore = animeStore;
            Anime = _animeStore.CurrentAnime;
        }
    }
}