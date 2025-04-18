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
        SearchControl _search;

        [ObservableProperty]
        private HomeControl _homePage;

        public MainViewModel(SearchControl searchControl, HomeControl homeControl)
        {
            Search = searchControl;
            HomePage = homeControl;
        }
    }
}
