using AngleSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Stores;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace KawaiiList.ViewModels
{
    public partial class AnimeAuthorViewModel : BaseViewModel
    {
        private readonly AnimeStore _animeStore;

        [ObservableProperty]
        private ObservableCollection<AnimeCharacterAndPersonRole> _author = [];

        [ObservableProperty]
        private bool _loadingAnimeData = true;

        public AnimeAuthorViewModel(AnimeStore animeStore)
        {
            _animeStore = animeStore;

            LoadAuthor();
        }

        private async void LoadAuthor()
        {
            var author = _animeStore.CurrentAnimeInfo.AuthorAndCharacterInfo.Where(x => x.Person != null);

            foreach (var item in author)
            {
                item.Person.Image.Original = await GetCharacterMainImageUrlAsync(item.Person.Id);
            }

            Author = new ObservableCollection<AnimeCharacterAndPersonRole>(author);
            LoadingAnimeData = false;
        }

        public async Task<string> GetCharacterMainImageUrlAsync(int characterId)
        {
            var url = $"https://shikimori.one/people/{characterId}";

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            var metaImage = document.QuerySelector("meta[property='og:image']");
            if (metaImage != null)
            {
                var content = metaImage.GetAttribute("content");
                return content;
            }

            return "https://shikimori.one/assets/globals/missing/main.png";
        
        }

        [RelayCommand]
        private void ItemSelected(AnimeCharacterAndPersonRole anime)
        {
            Process.Start(new ProcessStartInfo($"https://shikimori.one/people/{anime.Person.Id}") { UseShellExecute = true });
        }
    }
}
