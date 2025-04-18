using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.ViewModels.AnimeCarouselVM;
using KawaiiList.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KawaiiList.ViewModels.HomeVM
{
    public partial class HomeViewModel : ObservableObject, IHomeViewModel
    {
        [ObservableProperty]
        private AnimeCarouselControl _animeCarousel;

        

        public HomeViewModel(AnimeCarouselControl animeCarousel) 
        {
            AnimeCarousel = animeCarousel;   
        }
    }
}
