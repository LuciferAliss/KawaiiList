using CommunityToolkit.Mvvm.Input;
using KawaiiList.Commands;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace KawaiiList.ViewModels
{
    public partial class NavigationBarViewModel : BaseViewModel
    {
        private readonly IAnilibriaService _anilibriaService;
        private readonly IShikimoriService _apiShikimoriService;
        private readonly INavigationService _navigationService;
        private readonly AnimeStore _animeStore;

        private CancellationTokenSource _cts = new();

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCatalogCommand { get; }
        public ICommand NavigateScheduleCommand { get; }

        public NavigationBarViewModel(INavigationService homeNavigationService,
            INavigationService catalogNavigationService,
            INavigationService scheduleNavigationService,
            INavigationService navigationService,
            AnimeStore animeStore,
            IAnilibriaService anilibriaService,
            IShikimoriService apiShikimoriService)
        {
            NavigateHomeCommand = new NavigateCommand(homeNavigationService);
            NavigateCatalogCommand = new NavigateCommand(catalogNavigationService);
            NavigateScheduleCommand = new NavigateCommand(scheduleNavigationService);

            _navigationService = navigationService;
            _animeStore = animeStore;
            _anilibriaService = anilibriaService;
            _apiShikimoriService = apiShikimoriService;
        }

        [RelayCommand]
        private void RandomAnime()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    AnilibriaTitle data = await _anilibriaService.GetRandomAsync(token);

                    if (token.IsCancellationRequested)
                        return;

                    if (data.Names?.Ru == null)
                    {
                        RandomAnime();
                        return;
                    }

                    ItemSelected(data);
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

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