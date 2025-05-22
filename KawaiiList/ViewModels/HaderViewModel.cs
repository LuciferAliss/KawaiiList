using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Commands;
using KawaiiList.Services;
using System.Windows.Input;

namespace KawaiiList.ViewModels
{
    public partial class HaderViewModel : BaseViewModel
    {
        public ICommand NavigateSingUpCommand { get; }

        [ObservableProperty]
        private SearchViewModel _searchAnimeViewModel;


        public HaderViewModel(SearchViewModel search, INavigationService singUpnavigationService)
        {
            NavigateSingUpCommand = new NavigateCommand(singUpnavigationService);
            SearchAnimeViewModel = search;
        }
    }
}