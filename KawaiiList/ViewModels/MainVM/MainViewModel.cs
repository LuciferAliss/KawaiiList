using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using KawaiiList.Views.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KawaiiList.ViewModels.MainVm
{
    public partial class MainViewModel : ObservableObject, IMainViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        List<AnimeTitle>? _animeTitle;

        [ObservableProperty]
        AnimeCarouselControl _animeCarousel;

        [ObservableProperty]
        SearchControl _search;

        public MainViewModel(IApiService apiService, AnimeCarouselControl animeCarousel, SearchControl searchControl)
        {
            AnimeCarousel = animeCarousel;
            Search = searchControl;
            _apiService = apiService;
            _ = LoadAnime();
        }

        private async Task LoadAnime()
        {
            AnimeTitle = await _apiService.GetTitlesAsync(15);
        }
    }
}
