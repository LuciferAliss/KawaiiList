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

        private CancellationTokenSource _cts1 = new();
        private CancellationTokenSource _cts2 = new();
        private CancellationTokenSource _cts3 = new();
        private List<AnilibriaTitle> _animeData = new();
        
        private int _page = 1;
        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                LoadingAnimeData = true;
                IsNext = false;


                if (SelectedYear.HasValue || SelectedGenres != "Любой")
                {
                    SortAnime();
                }
                else
                {
                    LoadAnime();
                }
            }
        }

        [ObservableProperty]
        private bool _loadingAnimeData = false;

        [ObservableProperty]
        private bool _isNext = false;

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

        public CatalogViewModel(IAnilibriaService anilibriaService, IShikimoriService shikimoriService, AnimeStore animeStore, INavigationService navigationService)
        {
            _anilibriaService = anilibriaService;
            _shikimoriService = shikimoriService;
            _navigationService = navigationService;
            _animeStore = animeStore;

            _loadingAnimeData = true;
            _ = LoadData();
        }

        partial void OnSelectedYearChanged(int? value)
        {
            _animeData = [];
            AnimeTitle = [];
            Page = 1;
        }

        partial void OnSelectedGenresChanged(string value)
        {
            _animeData = [];
            AnimeTitle = [];
            Page = 1;            
        }

        private async Task LoadData()
        {
            await LoadGenres();
            await LoadYears();
            await LoadAnime();
        }

        private async Task SortAnime()
        {
            _cts1.Cancel();
            _cts1.Dispose();
            _cts1 = new CancellationTokenSource();
            var token = _cts1.Token;
            LoadingAnimeData = true;

            try
            {
                List<AnilibriaTitle> result;

                (result, IsNext) = await _anilibriaService.GetSortTitlesAsync(Page, SelectedGenres, SelectedYear, token);

                if (token.IsCancellationRequested)
                    return;

                _animeData.AddRange(result);
                AnimeTitle = new List<AnilibriaTitle>(_animeData);
                LoadingAnimeData = false;
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task LoadAnime()
        {
            _cts1.Cancel();
            _cts1.Dispose();
            _cts1 = new CancellationTokenSource();
            var token = _cts1.Token;
            LoadingAnimeData = true;

            try
            {
                List<AnilibriaTitle> result;

                (result, IsNext) = await _anilibriaService.GetPageTitlesAsync(Page, token);

                if (token.IsCancellationRequested)
                    return;

                _animeData.AddRange(result);
                AnimeTitle = new List<AnilibriaTitle>(_animeData);
                LoadingAnimeData = false;
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task LoadGenres()
        {
            _cts2.Cancel();
            _cts2.Dispose();
            _cts2 = new CancellationTokenSource();
            var token = _cts2.Token;

            AnimeGenres = [];

            try
            {
                List<string> result = await _anilibriaService.GetGenresAsync(token);

                if (token.IsCancellationRequested)
                    return;

                if (result.Count == 0)
                {
                    await Task.Delay(1000);
                    await LoadGenres();
                    return;
                }

                result.Insert(0, "Любой");
                AnimeGenres = result ?? [];
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task LoadYears()
        {
            _cts3.Cancel();
            _cts3.Dispose();
            _cts3 = new CancellationTokenSource();
            var token = _cts3.Token;

            AnimeYears = [];

            try
            {
                var result = await _anilibriaService.GetYearsAsync(token);

                if (token.IsCancellationRequested)
                    return;

                if (result.Count == 0)
                {
                    await Task.Delay(1000);
                    await LoadYears();
                    return;
                }
                AnimeYears = result.Where(x => x <= DateTime.Now.Year).Reverse().ToList() ?? [];
            }
            catch (OperationCanceledException)
            {
            }
        }

        [RelayCommand]
        private void ClearYear()
        {
            SelectedYear = null;
        }

        [RelayCommand]
        private void NextPage() => Page++;

        [RelayCommand]
        private void ItemSelected(AnilibriaTitle selectedAnime)
        {
            _cts1.Cancel();
            _cts1.Dispose();
            _cts1 = new CancellationTokenSource();
            var token = _cts1.Token;

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

        public override void Dispose()
        {
            AnimeTitle = [];
            _animeData = [];

            _cts1.Dispose();
            _cts2.Dispose();
            _cts3.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
