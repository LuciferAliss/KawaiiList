using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using static Supabase.Postgrest.Constants;

namespace KawaiiList.ViewModels
{
    public partial class AnimeInfoViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly ISupaBaseService<UserAnimeStatus> _userAnimeStatusService;
        private readonly UserStore _userStore;

        [ObservableProperty]
        private BaseViewModel _currentComponent;

        [ObservableProperty]
        private Visibility _contentVisibility = Visibility.Hidden;

        [ObservableProperty]
        private AnilibriaTitle _anime;

        [ObservableProperty]
        private ShikimoriTitle _animeInfo;

        [ObservableProperty]
        private bool _isTypeVisible;

        [ObservableProperty]
        private bool _isDateStartVisible;

        [ObservableProperty]
        private bool _isDateEndVisible;

        [ObservableProperty]
        private bool _isStatusVisible;

        [ObservableProperty]
        private bool _isEpisodesLastVisible;

        [ObservableProperty]
        private bool _isStudioNameVisible;

        [ObservableProperty]
        private string? _statusString;

        [ObservableProperty]
        private int _userRating = 0;

        [ObservableProperty]
        private double? _rating = 0f;

        public bool IsLoggedIn => _userStore.IsLoggedIn;

        public ObservableCollection<string> AnimeStatus { get; } = new ObservableCollection<string>()
        {
            "Смотрю","Запланировано", "Отложенно", "Брошено", "Просмотренно", "Любимое"
        };

        public AnimeInfoViewModel(
            AnimeStore animeStore,
            UserStore userStore,
            StatisticsAnimeViewModel statisticsAnimeViewModel,
            INavigationService navigation,
            ISupaBaseService<UserAnimeStatus> userAnimeStatusService)
        {
            Anime = animeStore.CurrentAnime;
            AnimeInfo = animeStore.CurrentAnimeInfo;
            CurrentComponent = statisticsAnimeViewModel;
            _navigationService = navigation;
            _userAnimeStatusService = userAnimeStatusService;
            _userStore = userStore;

            _userStore.CurrentUserChanged += ClearenUserData;

            LoadData();
        }

        private void ClearenUserData()
        {
            StatusString = null;
            UserRating = 0;
            AnimeStatus.Remove("Удалить");

            if(_userStore.CurrentUser != null)
            {
                LoadData();
            }

            OnPropertyChanged(nameof(IsLoggedIn));
        }

        private async void LoadData()
        {
            CheckAndMarkIfNotEmpty();

            if (_userStore.CurrentUser != null)
            {
                List<FiltersQuery> filters = new List<FiltersQuery>()
                {
                    new FiltersQuery()
                    {
                        ColumnName = "user_id",
                        OperatorFilter = Operator.Equals,
                        Value = _userStore.CurrentUser.Id
                    },
                    new FiltersQuery()
                    {
                        ColumnName = "anime_id",
                        OperatorFilter = Operator.Equals,
                        Value = Anime.Id
                    }
                };

                var userAnimeStatusResult = await _userAnimeStatusService.GetFilter("*", filters);

                if (userAnimeStatusResult.Count() > 0)
                {
                    StatusString = userAnimeStatusResult.FirstOrDefault().Status ?? null;
                    UserRating = userAnimeStatusResult.FirstOrDefault().Score ?? 0;
                }
            }

            await UpdateRating();
        }

        private void CheckAndMarkIfNotEmpty()
        {
            IsTypeVisible = Anime.Type?.String != null;
            IsDateStartVisible = AnimeInfo?.DateStart != null;
            IsDateEndVisible = AnimeInfo?.DateEnd != null;
            IsStatusVisible = Anime?.Status?.String != null;
            IsEpisodesLastVisible = Anime?.Player?.Episodes?.Last != null;
            IsStudioNameVisible = AnimeInfo?.StudioText != "";

            ContentVisibility = Visibility.Visible;
        }

        private async Task HandleUserRatingChanged(int value)
        {
            var anime = new UserAnimeStatus()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = _userStore.CurrentUser.Id,
                AnimeId = Anime.Id,
                Status = StatusString,
                Score = value,
                UploadedAt = DateTime.Now 
            };

            await _userAnimeStatusService.Upsert(anime, "user_id,anime_id");

            await UpdateRating();
        }

        private async Task UpdateRating()
        {
            FiltersQuery filtersQuery = new FiltersQuery()
            {
                ColumnName = "anime_id",
                OperatorFilter = Operator.Equals,
                Value = Anime.Id
            };

            var result = await _userAnimeStatusService.GetFilter("*", filtersQuery);

            if (result.Count() > 0)
            {
                Rating = result.Average(x => x.Score);

                if (Rating == null)
                {
                    Rating = 0;
                }
            }
            else
            {
                Rating = 0;
            }
        }

        private async Task HandleStatusStringChangedAsync(string? value)
        {
            if (value != "Удалить" && value != "" && value != null)
            {
                var anime = new UserAnimeStatus()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = _userStore.CurrentUser.Id,
                    AnimeId = Anime.Id,
                    Status = value,
                    Score = UserRating == 0 ? null : UserRating,
                    UploadedAt = DateTime.Now
                };

                await _userAnimeStatusService.Upsert(anime, "user_id,anime_id");

                if (!AnimeStatus.Contains("Удалить"))
                {
                    AnimeStatus.Add("Удалить");
                }
            }
            else if (value == "Удалить")
            {
                await _userAnimeStatusService.Delete(x => x.AnimeId == Anime.Id);
                AnimeStatus.Remove("Удалить");
                StatusString = null;
            }
        }

        partial void OnStatusStringChanged(string? value)
        {
            _ = HandleStatusStringChangedAsync(value);
        }

        partial void OnUserRatingChanged(int value)
        {
            _ = HandleUserRatingChanged(value);
        }

        [RelayCommand]
        private void OpenUrl()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://shikimori.one/" + AnimeInfo.Url?.Substring(1),
                UseShellExecute = true
            });
        }

        [RelayCommand]
        private void OpenWatchAnimeView()
        {
            _navigationService.Navigate();
        }

        public override void Dispose()
        {
            _userStore.CurrentUserChanged -= ClearenUserData;
        }
    }
}