using CommunityToolkit.Mvvm.ComponentModel;

namespace KawaiiList.ViewModels
{
    public partial class HaderViewModel : BaseViewModel
    {
        [ObservableProperty]
        private SearchViewModel _searchAnimeViewModel;

        public HaderViewModel(SearchViewModel search)
        {
            SearchAnimeViewModel = search;
        }
    }
}