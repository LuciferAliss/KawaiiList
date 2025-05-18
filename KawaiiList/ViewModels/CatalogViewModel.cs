using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KawaiiList.ViewModels
{
    public partial class CatalogViewModel : BaseViewModel
    {
        private readonly IAnilibriaService _anilibriaService;
        private readonly IShikimoriService _shikimoriService;
        private readonly INavigationService _navigationService;
        private readonly AnimeStore _animeStore;

        private CancellationTokenSource _cts = new();
        private bool _loadData = false;

        [ObservableProperty]
        private List<AnilibriaTitle> _animeTitle = [];

        [ObservableProperty]
        private List<string> _animeGenres = [];
        
        [ObservableProperty]
        private List<int> _animeYears = [];

        [ObservableProperty]
        private string _selectedGenres = "Любой";

        [ObservableProperty]
        private int? _selectedYear = null;

        [ObservableProperty]
        private int _currentPage;

        [ObservableProperty]
        private int _maxPage = 1;

        public CatalogViewModel(IAnilibriaService anilibriaService, IShikimoriService shikimoriService, AnimeStore animeStore, INavigationService navigationService)
        {
            _anilibriaService = anilibriaService;
            _shikimoriService = shikimoriService;
            _navigationService = navigationService;
            _animeStore = animeStore;

            CurrentPage = 1;
        }

        partial void OnCurrentPageChanged(int value)
        {
            CurrentPage = Math.Clamp(CurrentPage, 0, MaxPage);
            LoadPageAnime(value);
        }

        partial void OnAnimeTitleChanged(List<AnilibriaTitle> value)
        {
            if (value.Count != 0 && !_loadData)
            {
                LoadGenres();
                _loadData = true;
            }
        }

        partial void OnSelectedGenresChanged(string value)
        {
            if (!_loadData)
            {
                return;
            }
            LoadPageAnime(1);
        }

        private void LoadPageAnime(int page)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            AnimeTitle = [];

            Task.Run(async () =>
            {
                try
                {
                    var result = await _anilibriaService.GetPageAsync(page, SelectedGenres, SelectedYear, token);

                    if (token.IsCancellationRequested)
                        return;

                    if (result.Count == 0)
                    {
                        await Task.Delay(1000);
                        LoadPageAnime(page);
                        return;
                    }
                    AnimeTitle = result ?? [];
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        private void LoadGenres()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            AnimeGenres = [];

            Task.Run(async () =>
            {
                try
                {
                    List<string> result = new();
                    result.Add("Любой");

                    result.AddRange(await _anilibriaService.GetGenresAsync(token));

                    if (token.IsCancellationRequested)
                        return;

                    if (result.Count == 0)
                    {
                        await Task.Delay(1000);
                        LoadGenres();
                        return;
                    }
                    AnimeGenres = result ?? [];
                    LoadYears();
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        private void LoadYears()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            AnimeYears = [];

            Task.Run(async () =>
            {
                try
                {
                    var result = await _anilibriaService.GetYearsAsync(token);

                    if (token.IsCancellationRequested)
                        return;

                    if (result.Count == 0)
                    {
                        await Task.Delay(1000);
                        LoadYears();
                        return;
                    }
                    AnimeYears = result.Where(x => x <= DateTime.Now.Year).ToList() ?? [];
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        [RelayCommand]
        private void ItemSelected(AnilibriaTitle selectedAnime)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    var result = await _shikimoriService.GetInfoAsync(selectedAnime.Names?.En ?? "", token);

                    if (token.IsCancellationRequested)
                        return;

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
