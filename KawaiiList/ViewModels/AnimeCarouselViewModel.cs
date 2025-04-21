using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models.Anilibria;
using KawaiiList.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace KawaiiList.ViewModels
{
    public partial class AnimeCarouselViewModel : ObservableObject
    {
        private readonly AnilibriaService _apiService;
        private IDisposable? _autoScrollSubscription;
        private IConnectableObservable<long>? _autoScrollObservable;
        private bool _loadData = false;

        [ObservableProperty]
        ObservableCollection<AnimeTitle> _animeTitle;

        [ObservableProperty]
        int _pageIndex;

        public AnimeCarouselViewModel(AnilibriaService apiService)
        {
            PageIndex = -1;
            _animeTitle = [];
            _apiService = apiService;

            InitializeAutoScroll(3, 0);
            _ = LoadAnime();
        }

        private void InitializeAutoScroll(int interval, int initialDelay)
        {
            _autoScrollObservable = Observable.Timer(
                    dueTime: TimeSpan.FromSeconds(initialDelay),
                    period: TimeSpan.FromSeconds(interval))
                .Publish();

            _autoScrollSubscription = _autoScrollObservable.Subscribe(_ =>
            {
                PageIndex = (AnimeTitle?.Count - 4 > PageIndex) ? ++PageIndex : 0;
                Debug.WriteLine($"Auto-scroll: {PageIndex}");
            });
        }

        private void StopAutoScroll()
        {
            _autoScrollSubscription?.Dispose();
            _autoScrollSubscription = null;
        }

        private void StartAutoScroll()
        {
            StopAutoScroll();
            InitializeAutoScroll(3, 3);
            _autoScrollObservable?.Connect();
        }

        private async Task LoadAnime()
        {
            try
            {
                List<AnimeTitle> data = await _apiService.GetTitlesAsync(15);

                if (data.Count == 0)
                {
                    await Task.Delay(1000);
                    await LoadAnime();
                    return;
                }

                AnimeTitle = [.. data];

                _autoScrollObservable?.Connect();
                _loadData = true;
            }
            catch (Exception)
            {
                await Task.Delay(1000);
                await LoadAnime();
            }
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

        [RelayCommand]
        private void MouseEnterCarousel()
        {
            if (!_loadData)
            {
                return;
            }

            StopAutoScroll();
        }

        [RelayCommand]
        private void MouseLeaveCarousel()
        {
            if (!_loadData)
            {
                return;
            }

            StartAutoScroll();
        }
    }
}