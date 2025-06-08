using AngleSharp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Stores;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace KawaiiList.ViewModels
{
    public partial class AnimeCharactersViewModel : BaseViewModel
    {
        private readonly AnimeStore _animeStore;

        [ObservableProperty]
        private ObservableCollection<AnimeCharacterAndPersonRole> _firstRoles = [];

        [ObservableProperty]
        private ObservableCollection<AnimeCharacterAndPersonRole> _secondRoles = [];

        public AnimeCharactersViewModel(AnimeStore animeStore) 
        {
            _animeStore = animeStore;

            LoadCharacters();
        }

        private async void LoadCharacters()
        {
            var character = _animeStore.CurrentAnimeInfo.AuthorAndCharacterInfo.Where(x => x.Character != null);

            foreach (var item in _animeStore.CurrentAnimeInfo.AuthorAndCharacterInfo.Where(x => x.Character != null))
            {
                item.Character.Image.Original =  await GetCharacterMainImageUrlAsync(item.Character.Id);
            }

            FirstRoles = new ObservableCollection<AnimeCharacterAndPersonRole>(character.Where(x => x.Roles[0] == "Main"));
            SecondRoles = new ObservableCollection<AnimeCharacterAndPersonRole>(character.Where(x => x.Roles[0] == "Supporting"));
        }

        public async Task<string> GetCharacterMainImageUrlAsync(int characterId)
        {
            var url = $"https://shikimori.one/characters/{characterId}";

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
            Process.Start(new ProcessStartInfo($"https://shikimori.one/characters/{anime.Character.Id}") { UseShellExecute = true });
        }
    }
}
