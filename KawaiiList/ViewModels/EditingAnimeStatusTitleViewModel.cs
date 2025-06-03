using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Collections.ObjectModel;
using static Supabase.Postgrest.Constants;

namespace KawaiiList.ViewModels
{
    public partial class EditingAnimeStatusTitleViewModel : BaseViewModel
    {
        private readonly AnimeStore _animeStore;
        private readonly UserStore _userStore;
        private readonly ICloseModalNavigationService _closeNavigationService;
        private readonly ISupaBaseService<UserAnimeStatus> _userAnimeStatusService;

        private string _originalStatus;

        [ObservableProperty]
        private string _selectedAnimeStatus = "";

        [ObservableProperty]
        private int? _userRating;

        public ObservableCollection<string> AnimeStatus { get; } = new ObservableCollection<string>()
        {
            "Смотрю","Запланировано", "Отложенно", "Брошено", "Просмотренно", "Любимое", "Удалить"
        };

        public EditingAnimeStatusTitleViewModel(
            AnimeStore animeStore,
            UserStore userStore,
            ICloseModalNavigationService closeNavigationService,
            ISupaBaseService<UserAnimeStatus> userAnimeStatusService)
        {
            _animeStore = animeStore;
            _userStore = userStore;
            _closeNavigationService = closeNavigationService;
            _userAnimeStatusService = userAnimeStatusService;

            LoadData();
        }

        private async void LoadData()
        {
            List<FiltersQuery> filters = new List<FiltersQuery>()
            {
                new FiltersQuery()
                {
                    ColumnName = "anime_id",
                    OperatorFilter = Operator.Equals,
                    Value = _animeStore.CurrentAnime.Id
                },
                new FiltersQuery()
                {
                    ColumnName = "user_id",
                    OperatorFilter = Operator.Equals,
                    Value = _userStore.CurrentUser.Id
                }
            };

            var userAnimeStatusResult = await _userAnimeStatusService.GetFilter("*", filters);

            if (userAnimeStatusResult.Count() > 0)
            {
                SelectedAnimeStatus = userAnimeStatusResult.FirstOrDefault().Status;
                _originalStatus = SelectedAnimeStatus;
                UserRating = userAnimeStatusResult.FirstOrDefault().Score;
            }
        }

        [RelayCommand]
        private void CloseModalWindow()
        {
            _closeNavigationService.Navigate();
            _animeStore.CurrentAnime = _animeStore.CurrentAnime;
        }

        [RelayCommand]
        private async void RemoveAnimeFromList()
        {
            await _userAnimeStatusService.Delete(x => x.AnimeId == _animeStore.CurrentAnime.Id && x.UserId == _userStore.CurrentUser.Id);
            _closeNavigationService.Navigate();
            _animeStore.CurrentAnime = _animeStore.CurrentAnime;
        }

        [RelayCommand]
        private async void UpdateAnimeData()
        {
            if (SelectedAnimeStatus != "Удалить")
            {
                var anime = new UserAnimeStatus()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = _userStore.CurrentUser.Id,
                    AnimeId = _animeStore.CurrentAnime.Id,
                    Status = SelectedAnimeStatus,
                    Score = UserRating,
                    UploadedAt = DateTime.Now
                };

                bool r = await _userAnimeStatusService.Upsert(anime, "user_id,anime_id");

                if (!AnimeStatus.Contains("Удалить"))
                {
                    AnimeStatus.Add("Удалить");
                }
            }
            else if (SelectedAnimeStatus == "Удалить")
            {
                await _userAnimeStatusService.Delete(x => x.AnimeId == _animeStore.CurrentAnime.Id);
                AnimeStatus.Remove("Удалить");
            }

            _animeStore.CurrentAnime = _animeStore.CurrentAnime;
            _closeNavigationService.Navigate();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
