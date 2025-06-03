using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KawaiiList.ViewModels
{
    public partial class TitleAnimeListViewModel : BaseViewModel
    {
        private readonly ISupaBaseService<UserAnimeStatus> _userAnimeStatusService;
        private readonly IAnilibriaService _anilibriaService;
        private readonly IShikimoriService _shikimoriService;
        private readonly INavigationService _navigationAnimeInfoService;
        private readonly INavigationService _navigationEditingAnimeStatusTitleService;
        private readonly UserStore _userStore;
        private readonly AnimeStore _animeStore;

        private CancellationTokenSource _cts = new();
        private List<UserAnimeStatus> _userAnimeStatuses = [];
        private List<UserAnimeStatus> _filteredUserAnimeStatuses = [];
        private List<AnilibriaTitle> _animeTitle = [];

        [ObservableProperty]
        private AnimeTypeStatus _selectedTypeStatus = new();

        [ObservableProperty]
        private ObservableCollection<AnilibriaTitle> _filteredAnimeTitle = new();

        [ObservableProperty]
        private string _searchFilterAnime = "";

        [ObservableProperty]
        private bool _loadAnimeData = false;

        public ObservableCollection<AnimeTypeStatus> AnimeTypeStatuses { get; set; } = [];

        public TitleAnimeListViewModel(
            ISupaBaseService<UserAnimeStatus> userAnimeStatusService,
            UserStore userStore,
            AnimeStore animeStore,
            IAnilibriaService anilibriaService,
            IShikimoriService shikimoriService,
            INavigationService navigationAnimeInfoService,
            INavigationService navigationEditingAnimeStatusTitleService)
        {
            _userAnimeStatusService = userAnimeStatusService;
            _userStore = userStore;
            _animeStore = animeStore;
            _anilibriaService = anilibriaService;
            _shikimoriService = shikimoriService;
            _navigationAnimeInfoService = navigationAnimeInfoService;
            _navigationEditingAnimeStatusTitleService = navigationEditingAnimeStatusTitleService;

            LoadData();
        }

        private void UpdateAnime()
        {
            LoadAnimeData = false;
            _animeTitle.Clear();
            AnimeTypeStatuses.Clear();
            FilteredAnimeTitle.Clear();
            _filteredUserAnimeStatuses.Clear();
            _userAnimeStatuses.Clear();
            LoadData();
        }

        private async void LoadData()
        {

            List<FiltersQuery> filtersQuery = new List<FiltersQuery>()
            {
                new FiltersQuery()
                {
                    ColumnName = "user_id",
                    OperatorFilter = Supabase.Postgrest.Constants.Operator.Equals,
                    Value = _userStore.CurrentUser.Id
                },
                new FiltersQuery()
                {
                    ColumnName = "status",
                    OperatorFilter = Supabase.Postgrest.Constants.Operator.NotEqual,
                    Value = null
                }
            };

            var result = await _userAnimeStatusService.GetFilter("*", filtersQuery);

            if (result == null)
            {
                return;
            }

            List<string> nameStatuses = new List<string>()
            {
                "Все", "Запланировано", "Смотрю", "Просмотренно", "Отложенно", "Брошено", "Любимое"
            };

            _userAnimeStatuses = result.ToList();
            
            for (int i = 0; i < nameStatuses.Count; i++)
            {
                var statusName = nameStatuses[i];

                AnimeTypeStatuses.Add(new AnimeTypeStatus()
                {
                    StatusName = statusName,
                    AnimeCount = statusName == "Все"
                        ? _userAnimeStatuses.Count
                        : _userAnimeStatuses.Count(x => x.Status == statusName)
                });
            }

            await LoadAnime();

            _animeStore.CurrentAnimeChanged -= UpdateAnime;

            if (AnimeTypeStatuses.Any())
            {
                SelectedTypeStatus = AnimeTypeStatuses[0];
            }
        }

        private void FilteredAnimeStatuses()
        {
            switch (SelectedTypeStatus.StatusName)
            {
                case "Все":
                    _filteredUserAnimeStatuses = _userAnimeStatuses;
                    FilteredAnimeTitle = new ObservableCollection<AnilibriaTitle>(_animeTitle);
                    break;

                case "Запланировано":
                case "Смотрю":
                case "Просмотренно":
                case "Отложенно":
                case "Брошено":
                case "Любимое":
                    _filteredUserAnimeStatuses = _userAnimeStatuses
                        .Where(x => x.Status == SelectedTypeStatus.StatusName)
                        .ToList();

                    FilteredAnimeTitle = [];
                    foreach (var item in _filteredUserAnimeStatuses)
                    {
                        FilteredAnimeTitle.Add(_animeTitle.Where(x => x.Id == item.AnimeId).FirstOrDefault());
                    }

                    break;
                default:
                    _filteredUserAnimeStatuses = new List<UserAnimeStatus>();
                    break;
            }
        }

        private async Task LoadAnime()
        {
            foreach (var item in _userAnimeStatuses)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                await Task.Run(async () =>
                {
                    try
                    {
                        var result = await _anilibriaService.GetTitleIdAsync(item.AnimeId, token);

                        if (token.IsCancellationRequested)
                            return;

                        result.UploadedAt = item.UploadedAt;
                        _animeTitle.Add(result);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                });
            }
            LoadAnimeData = true;
        }

        private void FilterAnimeName(string value)
        {
            var query = value.ToLower().Trim();

            FilteredAnimeTitle = new ObservableCollection<AnilibriaTitle>(_animeTitle
                .Where(anime =>
                    anime.Names.Ru.ToLower().Contains(query) ||                
                    anime.Names.Ru.ToLower().StartsWith(query) ||
                    anime.Names.En.ToLower().Contains(query) ||                
                    anime.Names.En.ToLower().StartsWith(query))
                .OrderBy(anime => anime.Names.Ru.Length)                       
                .ToList());
        }

        partial void OnSelectedTypeStatusChanged(AnimeTypeStatus value)
        {
            SearchFilterAnime = "";
            FilteredAnimeStatuses();
        }

        partial void OnSearchFilterAnimeChanged(string value)
        {
            if (value.Length == 1)
            {
                SelectedTypeStatus = AnimeTypeStatuses[0];
            }
            FilterAnimeName(value);
        }

        [RelayCommand]
        private void OpenSetingsTitle(AnilibriaTitle anilibriaTitle)
        {
            _animeStore.CurrentAnime = anilibriaTitle;
            _navigationEditingAnimeStatusTitleService.Navigate();
            _animeStore.CurrentAnimeChanged += UpdateAnime;
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
                    _navigationAnimeInfoService.Navigate();
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        public override void Dispose()
        {
            _cts?.Cancel();
            _animeStore.CurrentAnimeChanged -= UpdateAnime;

            base.Dispose();
        }
    }
}
