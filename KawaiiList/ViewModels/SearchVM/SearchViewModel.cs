using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using System.Diagnostics;
using System.DirectoryServices;
using System.Xml.Linq;

namespace KawaiiList.ViewModels.SearchVM
{
    public partial class SearchViewModel : ObservableObject, ISearchViewModel
    {
        private readonly IApiService _apiService;
        private CancellationTokenSource _cts = new();

        [ObservableProperty]
        string _searchText;

        [ObservableProperty]
        List<AnimeTitle> _animeList = [];

        public SearchViewModel(IApiService apiService)
        {
            SearchText = "";
            _apiService = apiService;
        }

        partial void OnSearchTextChanged(string value)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            AnimeList = [];

            Task.Run(async () =>
            {
                try
                {
                    var result = await _apiService.SearchTitlesAsync(value, token);

                    if (token.IsCancellationRequested)
                        return;

                    AnimeList = result ?? new List<AnimeTitle>();
                }
                catch (OperationCanceledException)
                {
                }
            });
        }
    }
}
