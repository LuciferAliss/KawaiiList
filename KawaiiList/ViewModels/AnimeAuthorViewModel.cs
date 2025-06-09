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

            Author = new ObservableCollection<AnimeCharacterAndPersonRole>(author);
            LoadingAnimeData = false;
        }

        [RelayCommand]
        private void ItemSelected(AnimeCharacterAndPersonRole anime)
        {
            Process.Start(new ProcessStartInfo($"https://shikimori.one/people/{anime.Person.Id}") { UseShellExecute = true });
        }
    }
}
