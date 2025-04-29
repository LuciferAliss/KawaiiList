using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace KawaiiList.ViewModels
{
    public partial class AnimeInfoViewModel : BaseViewModel
    {
        private readonly AnimeStore _animeStore;
        private CancellationTokenSource _cts = new();

        [ObservableProperty]
        private BaseViewModel _currentComponent;

        [ObservableProperty]
        private Visibility _contentVisibility = Visibility.Hidden;

        [ObservableProperty]
        private AnimeTitle _anime;

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


        public AnimeInfoViewModel(AnimeStore animeStore, StatisticsAnimeViewModel statisticsAnimeViewModel)
        {
            _animeStore = animeStore;
            Anime = _animeStore.CurrentAnime;
            AnimeInfo = _animeStore.CurrentAnimeInfo;
            CurrentComponent = statisticsAnimeViewModel;

            CheckAndMarkIfNotEmpty();
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
    }
}