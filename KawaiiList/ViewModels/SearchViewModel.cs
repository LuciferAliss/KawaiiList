using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Windows.Media;

namespace KawaiiList.ViewModels
{
    public partial class SearchViewModel : BaseViewModel
    {
        private readonly AnilibriaService _apiService;
        private readonly AnimeStore _animeStore;
        private readonly INavigationService _navigationService;
        private CancellationTokenSource _cts = new();

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private List<AnimeTitle> _animeList = [];
        
        [ObservableProperty]
        private Brush _textColor = new SolidColorBrush(Colors.LightGray);

        public SearchViewModel(AnilibriaService apiService, AnimeStore animeStore, INavigationService navigationService)
        {
            SearchText = "";
            _apiService = apiService;
            _navigationService = navigationService;
            _animeStore = animeStore;
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

        [RelayCommand]
        private void ItemSelected(AnimeTitle selectedAnime)
        {
            _animeStore.CurrentAnime = selectedAnime;
            _navigationService.Navigate();
        }
    }
}