using CommunityToolkit.Mvvm.ComponentModel;

namespace KawaiiList.ViewModels
{
    public partial class HaderViewModel : BaseViewModel
    {
        [ObservableProperty]
        private SearchViewModel _search;

        public HaderViewModel(SearchViewModel search)
        {
            _search = search;
        }
    }
}