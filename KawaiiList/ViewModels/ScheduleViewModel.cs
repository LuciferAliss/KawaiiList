using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Models;
using KawaiiList.Services;

namespace KawaiiList.ViewModels
{
    public partial class ScheduleViewModel : BaseViewModel
    {
        private readonly IAnilibriaService _anilibriaService;

        private CancellationTokenSource _cts = new();
        private List<ScheduleAnilibriaTitles> _animeData = [];

        [ObservableProperty]
        private List<AnilibriaTitle> _animeTitle = [];

        [ObservableProperty]
        private int _selectedDayIndex = 0;

        public ScheduleViewModel(IAnilibriaService anilibriaService)
        {
            _anilibriaService = anilibriaService;

            LoadAnime();
        }

        partial void OnSelectedDayIndexChanged(int value)
        {
            AnimeTitle = _animeData[value].List ?? [];
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
                    List<ScheduleAnilibriaTitles> data = await _anilibriaService.GetScheduleAsync(token);

                    if (token.IsCancellationRequested)
                        return;

                    if (data.Count == 0)
                    {
                        LoadAnime();
                        return;
                    }

                    _animeData = data;
                    var today = DateTime.Today.DayOfWeek;
                    SelectedDayIndex = today == DayOfWeek.Sunday ? 6 : ((int)today - 1);
                }
                catch (OperationCanceledException)
                {
                }
            });
        }
    }
}
