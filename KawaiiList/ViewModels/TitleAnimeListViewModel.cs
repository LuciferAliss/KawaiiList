using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models;
using KawaiiList.Services;
using System.Diagnostics;

namespace KawaiiList.ViewModels
{
    public partial class TitleAnimeListViewModel : BaseViewModel
    {
        private readonly ISupaBaseService<UserAnimeStatus> _userAnimeStatusService;

        [ObservableProperty]
        private string _selectedTypeStatus = "Все";

        public List<AnimeTypeStatus> animeTypeStatuses { get; set; }

        TitleAnimeListViewModel(ISupaBaseService<UserAnimeStatus> userAnimeStatusService)
        {
            _userAnimeStatusService = userAnimeStatusService;

            LoadData();
        }

        private void LoadData()
        {
            
        }

        partial void OnSelectedTypeStatusChanged(string value)
        {
            Debug.WriteLine(value);
        }
    }
}
