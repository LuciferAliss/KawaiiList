using CommunityToolkit.Mvvm.ComponentModel;

namespace KawaiiList.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private AnimeCarouselViewModel _animeCarousel;

        public HomeViewModel(AnimeCarouselViewModel animeCarousel)
        { 
            AnimeCarousel = animeCarousel;
        }
    }
}