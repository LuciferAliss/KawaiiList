using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace KawaiiList.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IShikimoriService _shikimoriService;

        private CancellationTokenSource _cts = new();

        [ObservableProperty]
        private ObservableCollection<ShikimoriTopic> _shikimoriTopics = [];

        [ObservableProperty]
        private AnimeCarouselViewModel _animeCarousel;

        public HomeViewModel(AnimeCarouselViewModel animeCarousel, IShikimoriService shikimoriService)
        { 
            _shikimoriService = shikimoriService;
            
            AnimeCarousel = animeCarousel;
            
            LoadAnime();
        }

        private async void LoadAnime()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            await Task.Run(async () =>
            {
                try
                {
                    var result = await _shikimoriService.GetNewsAsync(token);

                    if (token.IsCancellationRequested)
                        return;

                    if (result.Count == 0)
                    {
                        await Task.Delay(1000);
                        LoadAnime();
                        return;
                    }

                    ShikimoriTopics = new ObservableCollection<ShikimoriTopic>(result) ?? [];

                    foreach (var item in ShikimoriTopics)
                    {
                        item.ImageUrls = ParseHtmlFooter(item.HtmlFooter);
                    }
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        private List<MediaDisplay> ParseHtmlFooter(string html)
        {
            var mediaList = new List<MediaDisplay>();
            if (string.IsNullOrEmpty(html))
                return mediaList;

            string pattern = @"https:\/\/shikimori\.one\/[^""]+\.jpg|https:\/\/youtu\.be\/[a-zA-Z0-9_-]+|https:\/\/youtube\.com\/embed\/[a-zA-Z0-9_-]+|\/\/img\.youtube\.com\/vi\/[a-zA-Z0-9_-]+\/hqdefault\.jpg";
            var matches = Regex.Matches(html, pattern).Select(m => m.Value).ToList();

            matches = matches.Select(url => url.StartsWith("//") ? "https:" + url : url).ToList();

            var videoLinks = matches.Where(x => x.Contains("youtu.be")).Distinct();
            foreach (var videoLink in videoLinks)
            {
                string videoId = videoLink.Split('/').Last();
                mediaList.Add(new MediaDisplay
                {
                    ImageSource = $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg",
                    ClickUrl = $"https://youtu.be/{videoId}",
                    IsVideo = true
                });
            }

            var imageLinks = matches.Where(x => x.EndsWith(".jpg") && x.Contains("shikimori.one")).Distinct();
            foreach (var image in imageLinks)
            {
                mediaList.Add(new MediaDisplay
                {
                    ImageSource = image,
                    ClickUrl = image,
                    IsVideo = false
                });
            }

            return mediaList;
        }

        [RelayCommand]
        private void OpenNews(ShikimoriTopic anime)
        {
            Process.Start(new ProcessStartInfo("https://shikimori.one/forum/news/" + anime.Id) { UseShellExecute = true });
        }

        public override void Dispose() 
        {
            AnimeCarousel.Dispose();

            base.Dispose();
        }
    }
}