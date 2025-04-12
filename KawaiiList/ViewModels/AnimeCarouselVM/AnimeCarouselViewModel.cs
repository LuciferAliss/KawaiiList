using CommunityToolkit.Mvvm.ComponentModel;
using HandyControl.Controls;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KawaiiList.ViewModels.AnimeCarouselVM
{
    public partial class AnimeCarouselViewModel : ObservableObject, IAnimeCarouselViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        ObservableCollection<AnimeTitle>? _animeTitle;

        [ObservableProperty]
        DependencyProperty _indexPage;

        int _indexPageValue = 4;

        public AnimeCarouselViewModel(IApiService apiService)
        {
            IndexPage = DependencyProperty.Register("PageIndex", typeof(int), typeof(Carousel), new PropertyMetadata(_indexPageValue));
            _apiService = apiService;
            _ = LoadAnime();
        }

        private async Task LoadAnime()
        {
            List<AnimeTitle> data = await _apiService.GetTitlesAsync(15);
            AnimeTitle = [.. data];
        }
    }
}
