using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Windows;

namespace KawaiiList.ViewModels
{
    public partial class AnimeCarouselViewModel : BaseViewModel
    {
        private readonly IAnilibriaService _anilibriaService;
        private readonly IShikimoriService _shikimoriService;
        private readonly INavigationService _navigationService;
        private readonly AnimeStore _animeStore;

        private IDisposable? _autoScrollSubscription;
        private IConnectableObservable<long>? _autoScrollObservable;
        private CancellationTokenSource _cts = new();
        private bool _stop = false;

        [ObservableProperty]
        private List<AnilibriaTitle> _animeTitle;

        [ObservableProperty]
        private int _pageIndex;

        [ObservableProperty]
        private Visibility _contentVisibility = Visibility.Hidden;

        public AnimeCarouselViewModel(IAnilibriaService anilibriaService, IShikimoriService shikimoriService, AnimeStore animeStore, INavigationService navigationService)
        {
            PageIndex = 0;
            _animeTitle = [];
            _shikimoriService = shikimoriService;
            _anilibriaService = anilibriaService;
            _navigationService = navigationService;
            _animeStore = animeStore;

            InitializeAutoScroll(3, 0);
            LoadAnime();
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

        private void LoadAnime()
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    List<AnilibriaTitle> data = await _anilibriaService.GetTitlesAsync(15, token);

                    if (token.IsCancellationRequested)
                        return;
                    
                    if (data.Count == 0)
                    {
                        await Task.Delay(1000);
                        LoadAnime();
                        return;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        AnimeTitle = data;
                        PageIndex = -1;
                        ContentVisibility = Visibility.Visible;
                    });

                    _autoScrollObservable?.Connect();
                }
                catch (OperationCanceledException)
                {
                }
            });
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
            if (_stop)
            {
                return;
            }

            StopAutoScroll();
        }

        [RelayCommand]
        private void MouseLeaveCarousel()
        {
            if (_stop)
            {
                return;
            }

            StartAutoScroll();
        }

        [RelayCommand]
        private void ItemSelected(AnilibriaTitle selectedAnime)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            Task.Run(async () =>
            {
                try
                {
                    var result = await _shikimoriService.GetInfoAsync(selectedAnime.Names?.En ?? "", token);

                    if (token.IsCancellationRequested)
                        return;

                    if (result.Description != null)
                    {
                        string cleanedText = Regex.Replace(result.Description, @"\[character=\d+\]|\[\/character\]", "");
                        cleanedText = Regex.Replace(cleanedText, @"\[anime=\d+\]|\[\/anime\]", "");
                        selectedAnime.Description = cleanedText;
                        Debug.WriteLine(result.Description);
                        Debug.WriteLine(result.DescriptionSource);
                    }

                    ShikimoriTitle AnimeInfo = result ?? new ShikimoriTitle();

                    _animeStore.CurrentAnimeInfo = AnimeInfo;
                    _animeStore.CurrentAnime = selectedAnime;
                    _navigationService.Navigate();
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        public override void Dispose()
        {
            _stop = true;
            StopAutoScroll();
            _cts.Cancel();

            base.Dispose();
        }
    }
}