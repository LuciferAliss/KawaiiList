﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Tools.Extension;
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

        public ObservableCollection<string> AnimeStatus { get; } = new ObservableCollection<string>()
        {
            "Смотрю", "Пересматриваю", "Запланировано", "Отложенно", "Брошено", "Просмотренно", "Любимое"
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
            AnimeStatus.Remove("Удалить");

            if(_userStore.CurrentUser != null)
            {
                LoadData();
            }
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
                }
            }
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

        partial void OnStatusStringChanged(string? value)
        {
            _ = HandleStatusStringChangedAsync(value);
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
                    Status = value.Value<string>(),
                    Score = null,
                    Progress = null
                };

                bool r = await _userAnimeStatusService.Upsert(anime, "user_id,anime_id");

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
    }
}