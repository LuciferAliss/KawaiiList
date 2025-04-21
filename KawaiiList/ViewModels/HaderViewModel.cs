using CommunityToolkit.Mvvm.ComponentModel;

namespace KawaiiList.ViewModels
{
    public partial class HaderViewModel : ObservableObject
    {
        [ObservableProperty]
        private SearchViewModel _search;

        public HaderViewModel(SearchViewModel search)
        {
            _search = search;
        }
    }
}