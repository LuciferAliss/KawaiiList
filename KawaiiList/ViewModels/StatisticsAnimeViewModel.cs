using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Stores;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace KawaiiList.ViewModels
{
    public class PilotInfo : ObservableValue
    {
        public PilotInfo(string name, int value, SolidColorPaint paint)
        {
            Name = name;
            Paint = paint;
            Value = value;
        }

        public string Name { get; set; }
        public SolidColorPaint Paint { get; set; }
    }

    public partial class StatisticsAnimeViewModel : BaseViewModel
    {
        private readonly AnimeStore _animeStore;
        private readonly List<PilotInfo> _data = [];

        [ObservableProperty]
        private ISeries[] _seriesScores = [];

        [ObservableProperty]
        private IEnumerable<ISeries> _seriesListUser = [];

        [ObservableProperty]
        private Axis[] _xAxes = [];

        [ObservableProperty]
        private Axis[] _yAxes = [];

        public StatisticsAnimeViewModel(AnimeStore animeStore)
        {
            _animeStore = animeStore;

            CreatingFeedbackSchedule();
            CreatingUserListChart();
        }

        private void CreatingFeedbackSchedule()
        {
            var paints = Enumerable.Range(0, 10)
                .Select(i =>
                {
                    float startHue = 120f;
                    float endHue = 0f;

                    float hue = startHue + (endHue - startHue) * i / (10 - 1);

                    float saturation = 0.8f;
                    float lightness = 0.4f;

                    var color = SKColor.FromHsl(hue, saturation * 100, lightness * 100);
                    return new SolidColorPaint(color);
                })
                .ToArray();


            for (int i = 0; i < _animeStore.CurrentAnimeInfo.RatesScoresStats?.Count; i++)
            {
                _data.Add(new PilotInfo(_animeStore.CurrentAnimeInfo.RatesScoresStats[i].Name.ToString(), _animeStore.CurrentAnimeInfo.RatesScoresStats[i].Value, paints[i]));
            }

            _data.Reverse();

            var rowSeries = new RowSeries<PilotInfo>
            {
                Values = _data,
                Padding = 10,
            }
            .OnPointMeasured(point =>
            {
                if (point.Visual is null) return;
                point.Visual.Fill = point.Model!.Paint;
            });

            SeriesScores = [rowSeries];

            XAxes =
            [
                new Axis
                {
                    SeparatorsPaint = null,
                    LabelsPaint = null,
                    Position = AxisPosition.Start
                }
            ];

            YAxes =
            [
                new Axis
                {
                   Labels = _data.Select(d => d.Name + " (" + d.Value + ")").ToArray(),
                   LabelsPaint = new SolidColorPaint(SKColors.White) { FontFamily = "Montserrat" },
                   SeparatorsPaint = null,
                   LabelsRotation = 0,
                   TextSize = 14,
                   MinStep = 1,
                   ForceStepToMin = true,
                   Position = AxisPosition.Start
                }
            ];
        }

        private void CreatingUserListChart()
        {
            var stats = _animeStore.CurrentAnimeInfo.RatesStatusesStats ?? [];
            double total = stats.Sum(i => i.Value);

            SeriesListUser = stats.Select(item => new PieSeries<int>
            {
                Values = [item.Value],
                Name = item.Name,
                DataLabelsSize = 16,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                HoverPushout = 0,
                DataLabelsFormatter = (chartPoint) =>
                {
                    double percent = item.Value / total * 100;
                    return percent < 5 ? string.Empty : $"{item.Name}{Environment.NewLine}{item.Value}";
                }
            }).ToArray();
        }
    }
}
