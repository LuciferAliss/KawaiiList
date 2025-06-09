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

        [ObservableProperty]
        private bool _loadingAnimeData = true;

        public AnimeCharactersViewModel(AnimeStore animeStore) 
        {
            _animeStore = animeStore;

            LoadCharacters();
        }

        private async void LoadCharacters()
        {
            var character = _animeStore.CurrentAnimeInfo.AuthorAndCharacterInfo.Where(x => x.Character != null);

            FirstRoles = new ObservableCollection<AnimeCharacterAndPersonRole>(character.Where(x => x.Roles[0] == "Main"));
            SecondRoles = new ObservableCollection<AnimeCharacterAndPersonRole>(character.Where(x => x.Roles[0] == "Supporting"));
            LoadingAnimeData = false;
        }
        
        [RelayCommand]
        private void ItemSelected(AnimeCharacterAndPersonRole anime)
        {
            Process.Start(new ProcessStartInfo($"https://shikimori.one/characters/{anime.Character.Id}") { UseShellExecute = true });
        }
    }
}
