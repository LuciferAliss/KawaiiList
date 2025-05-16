using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace KawaiiList.ViewModels
{
    public partial class SearchViewModel : BaseViewModel
    {
        private readonly AnilibriaService _apiAnilibriaService;
        private readonly ShikimoriService _apiShikimoriService;
        private readonly AnimeStore _animeStore;
        private readonly INavigationService _navigationService;
        private CancellationTokenSource _cts = new();

        [ObservableProperty]
        private bool _loadAnimeData = false;

        [ObservableProperty]
        private string _loadAnimeText = "Загрузка...";

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private List<AnimeTitle> _animeList = [];
        
        [ObservableProperty]
        private Brush _textColor = new SolidColorBrush(Colors.LightGray);

        public SearchViewModel(AnilibriaService anilibriaService, ShikimoriService shikimoriService, AnimeStore animeStore, INavigationService navigationService)
        {
            SearchText = "";
            _apiAnilibriaService = anilibriaService;
            _apiShikimoriService = shikimoriService;
            _navigationService = navigationService;
            _animeStore = animeStore;
        }

        partial void OnSearchTextChanged(string value)
        { 
            if (value == "")
            {
                TextColor = new SolidColorBrush(Colors.LightGray);
                return;
            }
            else
            {
                TextColor = new SolidColorBrush(Colors.White);
            }
            
            LoadAnime(value);
        }

        private void LoadAnime(string value)
        {
            LoadAnimeData = true;
            LoadAnimeText = "Загрузка...";

            Task.Delay(1000);

            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            AnimeList = [];

            Task.Run(async () =>
            {
                try
                {
                    var result = await _apiAnilibriaService.SearchTitlesAsync(value, token);

                    if (token.IsCancellationRequested)
                        return;

                    LoadAnimeData = false;
                    if (result.Count == 0)
                    {
                        LoadAnimeText = "Ничего не найдено";
                    }
                    AnimeList = result ?? [];
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        [RelayCommand]
        private void ItemSelected(AnimeTitle selectedAnime)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    var result = await _apiShikimoriService.GetInfoAsync(selectedAnime.Names?.En ?? "", token);

                    if (token.IsCancellationRequested)
                        return;

                    if (result.Id == -1)
                    {
                        ItemSelected(selectedAnime);
                        return;
                    }

                    if (result.Description != null)
                    {
                        string cleanedText = Regex.Replace(result.Description, @"\[character=\d+\]|\[\/character\]", "");
                        cleanedText = Regex.Replace(cleanedText, @"\[anime=\d+\]|\[\/anime\]", "");
                        selectedAnime.Description = cleanedText;
                        Debug.WriteLine(result.Description);
                        Debug.WriteLine(result.DescriptionSource);
                    }

                    ShikimoriTitle AnimeInfo = result ?? new ShikimoriTitle();

                    _animeStore.CurrentAnimeInfo = AnimeInfo;
                    _animeStore.CurrentAnime = selectedAnime;
                    _navigationService.Navigate();
                }
                catch (OperationCanceledException)
                {
                }
            });
        }
    }
}