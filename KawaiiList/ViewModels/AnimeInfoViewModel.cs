using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using Newtonsoft.Json.Linq;
using System.Windows.Media;

namespace KawaiiList.ViewModels
{
    public partial class AnimeInfoViewModel : BaseViewModel
    {
        private readonly AnimeStore _animeStore;
        private readonly ShikimoriService _shikimoriService;
        private CancellationTokenSource _cts = new();

        [ObservableProperty]
        private AnimeTitle _anime;

        [ObservableProperty]
        private ShikimoriTitle _animeInfo;

        public AnimeInfoViewModel(AnimeStore animeStore, ShikimoriService shikimoriService)
        {
            _animeStore = animeStore;
            _shikimoriService = shikimoriService;
            Anime = _animeStore.CurrentAnime;
            AnimeInfo = new ShikimoriTitle();

            LoadInfo();
        }

        private void LoadInfo()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    var result = await _shikimoriService.GetInfoAsync(Anime.Names?.Ru ?? "", token);

                    if (token.IsCancellationRequested)
                        return;
    
                    if (result.Description != null)
                    {
                        Anime.Description = result.Description;
                    }

                    AnimeInfo = result ?? new ShikimoriTitle();
                }
                catch (OperationCanceledException)
                {
                }
            });
        }
    }
}