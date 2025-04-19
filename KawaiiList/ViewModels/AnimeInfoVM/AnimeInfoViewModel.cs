using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models.Anilibria;

namespace KawaiiList.ViewModels.AnimeInfoVM
{
    public partial class AnimeInfoViewModel : ObservableObject, IAnimeInfoViewModel
    {
        [ObservableProperty]
        private AnimeTitle _anime = new();

        public AnimeInfoViewModel()
        {

        }
    }
}
