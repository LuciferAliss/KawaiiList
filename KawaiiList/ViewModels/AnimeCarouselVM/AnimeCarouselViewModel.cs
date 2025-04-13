using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services.API;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace KawaiiList.ViewModels.AnimeCarouselVM
{
    public partial class AnimeCarouselViewModel : ObservableObject, IAnimeCarouselViewModel
    {
        private readonly IApiService _apiService;
        private IDisposable? _autoScrollSubscription;
        private IConnectableObservable<long>? _autoScrollObservable;

        [ObservableProperty]
        ObservableCollection<AnimeTitle> _animeTitle;

        [ObservableProperty]
        int _pageIndex;

        [ObservableProperty]
        string _buttonVisible;

        public AnimeCarouselViewModel(IApiService apiService)
        {
            ButtonVisible = "Hidden";
            PageIndex = -1;
            _animeTitle = [];
            _apiService = apiService;

            InitializeAutoScroll();
            _ = LoadAnime();
        }

        private void InitializeAutoScroll()
        {
            _autoScrollObservable = Observable.Interval(TimeSpan.FromSeconds(3)).StartWith(0L).Publish();

            _autoScrollSubscription = _autoScrollObservable.Subscribe(_ =>
            {
                PageIndex = (AnimeTitle?.Count - 4 > PageIndex) ? ++PageIndex : 0;
            });
        }

        private void StopAutoScroll()
        {
            _autoScrollSubscription?.Dispose();
            _autoScrollSubscription = null;
        }

        private async Task LoadAnime()
        {
            List<AnimeTitle> data = await _apiService.GetTitlesAsync(15);
            AnimeTitle = [.. data];

            ButtonVisible = "Visible";
            _autoScrollObservable?.Connect();
        }

        [RelayCommand]
        private void PageNext()
        {
            PageIndex = AnimeTitle.Count - 4 > PageIndex ? ++PageIndex : 0;
            Debug.WriteLine(PageIndex);
        }
        
        [RelayCommand]
        private void PagePrev()
        {
            PageIndex = PageIndex > 0 ? --PageIndex : PageIndex = AnimeTitle.Count - 4;
            Debug.WriteLine(PageIndex);
        }
    }
}
