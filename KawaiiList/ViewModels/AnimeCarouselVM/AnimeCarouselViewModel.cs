using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KawaiiList.ViewModels.AnimeCarouselVM
{
    public partial class AnimeCarouselViewModel : ObservableObject, IAnimeCarouselViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        List<AnimeTitle>? _animeTitle;


        public AnimeCarouselViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _ = LoadAnime();
        }

        private async Task LoadAnime()
        {
            AnimeTitle = await _apiService.GetTitlesAsync(15);
        }
    }
}
