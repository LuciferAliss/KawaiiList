using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using KawaiiList.Views.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KawaiiList.ViewModels.MainVm
{
    public partial class MainViewModel : ObservableObject, IMainViewModel
    {
        [ObservableProperty]
        private SearchControl _search;

        [ObservableProperty]
        private HomeControl _homePage;

        [ObservableProperty]
        private AnimeInfoControl _animeInfo;

        public MainViewModel(SearchControl searchControl, HomeControl homeControl, AnimeInfoControl animeInfo)
        {
            Search = searchControl;
            HomePage = homeControl;
            AnimeInfo = animeInfo;
        }
    }
}
