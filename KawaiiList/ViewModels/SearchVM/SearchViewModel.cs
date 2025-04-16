using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Services.API;
using System.Diagnostics;

namespace KawaiiList.ViewModels.SearchVM
{
    public partial class SearchViewModel : ObservableObject, ISearchViewModel
    {
        private readonly IApiService _apiService;

        [ObservableProperty]
        string _searchText;

        public SearchViewModel(IApiService apiService)
        {
            SearchText = "";
            _apiService = apiService;
        }

        partial void OnSearchTextChanged(string value)
        {
            Debug.WriteLine(value);
        }
    }
}
