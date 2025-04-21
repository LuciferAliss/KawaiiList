using CommunityToolkit.Mvvm.ComponentModel;

namespace KawaiiList.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        [ObservableProperty]
        private AnimeCarouselViewModel _animeCarousel;

        public HomeViewModel(AnimeCarouselViewModel animeCarousel)
        { 
            AnimeCarousel = animeCarousel;
        }

        public override void Dispose() 
        {
            AnimeCarousel.Dispose();

            base.Dispose();
        }
    }
}