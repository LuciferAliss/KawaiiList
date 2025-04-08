using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KawaiiList.ViewModels.MainVm
{
    public partial class MainViewModel : ObservableObject, IMainViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        List<AnimeTitle>? _animeTitle;

        [ObservableProperty]
        AnimeTitle? _anime;

        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _ = LoadAnime();
        }

        private async Task LoadAnime()
        {
            AnimeTitle = await _apiService.GetTitlesAsync(3);
            Anime = AnimeTitle[0];
        }
    }
}
