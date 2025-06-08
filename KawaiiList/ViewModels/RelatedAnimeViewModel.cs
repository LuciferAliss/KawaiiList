using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Navigation;

namespace KawaiiList.ViewModels
{
    public partial class RelatedAnimeViewModel : BaseViewModel
    {
        private readonly IAnilibriaService _anilibriaService;
        private readonly IShikimoriService _shikimoriService;
        private readonly INavigationService _navigationService;
        private readonly AnimeStore _animeStore;

        private CancellationTokenSource _cts = new();

        [ObservableProperty]
        private ObservableCollection<AnilibriaTitle> _animeTitle = [ null ];

        [ObservableProperty]
        private AnilibriaTitle _selectedTitle = new(); 

        [ObservableProperty]
        private bool _loadingAnimeData = true;

        public RelatedAnimeViewModel(IAnilibriaService anilibriaService, IShikimoriService shikimoriService, INavigationService navigationService, AnimeStore animeStore)
        {
            _anilibriaService = anilibriaService;
            _shikimoriService = shikimoriService;
            _navigationService = navigationService;
            _animeStore = animeStore;

            AnimeLoad();
        }

        private async void AnimeLoad()
        {
            if (_animeStore.CurrentAnime.Franchises.Count() == 0)
            {
                AnimeTitle = [];
                LoadingAnimeData = false;
                return;
            }

            List<AnilibriaTitle> _anime = [];

            foreach (var item in _animeStore.CurrentAnime.Franchises[0].Releases)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                await Task.Run(async () =>
                {
                    try
                    {
                        var result = await _anilibriaService.GetTitleIdAsync(item.Id, token);

                        if (token.IsCancellationRequested)
                            return;

                        _anime.Add(result);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                });
            }
            AnimeTitle = new ObservableCollection<AnilibriaTitle>(_anime);
            SelectedTitle = AnimeTitle.FirstOrDefault(a => a.Id == _animeStore.CurrentAnime.Id);
            LoadingAnimeData = false;
        }

        partial void OnSelectedTitleChanged(AnilibriaTitle value)
        {
            if (LoadingAnimeData)
            {
                return;
            }

            ItemSelected(value);
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