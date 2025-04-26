using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

namespace KawaiiList.ViewModels
{
    public partial class AnimeInfoViewModel : BaseViewModel
    {
        private readonly AnimeStore _animeStore;
        private readonly ShikimoriService _shikimoriService;
        private CancellationTokenSource _cts = new();

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
                    var result = await _shikimoriService.GetInfoAsync(Anime.Names?.En ?? "", token);

                    if (token.IsCancellationRequested)
                        return;
    
                    if (result.Description != null)
                    {
                        string cleanedText = Regex.Replace(result.Description, @"\[character=\d+\]|\[\/character\]", "");
                        Anime.Description = cleanedText;
                        Debug.WriteLine(result.Description);
                        Debug.WriteLine(result.DescriptionSource);
                    }

                    AnimeInfo = result ?? new ShikimoriTitle();

                    CheckAndMarkIfNotEmpty();

                    ContentVisibility = Visibility.Visible;
                }
                catch (OperationCanceledException)
                {   
                }
            });
        }

        private void CheckAndMarkIfNotEmpty()
        {
            IsTypeVisible = Anime.Type?.String != null;
            IsDateStartVisible = AnimeInfo?.DateStart != null;
            IsDateEndVisible = AnimeInfo?.DateEnd != null;
            IsStatusVisible = Anime?.Status?.String != null;
            IsEpisodesLastVisible = Anime?.Player?.Episodes?.Last != null;
            IsStudioNameVisible = AnimeInfo?.StudioText != "";
        }
    }
}