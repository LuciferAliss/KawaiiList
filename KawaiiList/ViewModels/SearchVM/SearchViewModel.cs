using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using System.Diagnostics;
using System.DirectoryServices;
using System.Windows.Media;
using System.Xml.Linq;

namespace KawaiiList.ViewModels.SearchVM
{
    public partial class SearchViewModel : ObservableObject, ISearchViewModel
    {
        private readonly IApiService _apiService;
        private CancellationTokenSource _cts = new();

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private List<AnimeTitle> _animeList = [];

        [ObservableProperty]
        private string _watermark = "Поиск аниме...";

        [ObservableProperty]
        private Brush _textColor = new SolidColorBrush(Colors.LightGray);

        public SearchViewModel(IApiService apiService)
        {
            SearchText = "";
            _apiService = apiService;
        }

        partial void OnSearchTextChanged(string value)
        {
            if (value == "")
            {
                TextColor = new SolidColorBrush(Colors.LightGray);
            }
            else
            {
                TextColor = new SolidColorBrush(Colors.White);
            }

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
