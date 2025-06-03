using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KawaiiList.ViewModels
{
    public partial class ScheduleViewModel : BaseViewModel
    {
        private readonly IAnilibriaService _anilibriaService;
        private readonly IShikimoriService _shikimoriService;
        private readonly INavigationService _navigationService;
        private readonly AnimeStore _animeStore;

        private CancellationTokenSource _cts = new();
        private List<ScheduleAnilibriaTitles> _animeData = [];

        [ObservableProperty]
        private List<AnilibriaTitle> _animeTitle = [];

        [ObservableProperty]
        private int _selectedDayIndex = 0;

        public ScheduleViewModel(
            IAnilibriaService anilibriaService,
            IShikimoriService shikimoriService,
            INavigationService navigationService,
            AnimeStore animeStore)
        {
            _anilibriaService = anilibriaService;
            _shikimoriService = shikimoriService;
            _navigationService = navigationService;
            _animeStore = animeStore;

            LoadAnime();
        }

        partial void OnSelectedDayIndexChanged(int value)
        {
            AnimeTitle = _animeData[value].List ?? [];
        }

        private void LoadAnime()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    List<ScheduleAnilibriaTitles> data = await _anilibriaService.GetScheduleAsync(token);

                    if (token.IsCancellationRequested)
                        return;

                    if (data.Count == 0)
                    {
                        LoadAnime();
                        return;
                    }

                    _animeData = data;
                    var today = DateTime.Today.DayOfWeek;
                    SelectedDayIndex = today == DayOfWeek.Sunday ? 6 : ((int)today - 1);
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
