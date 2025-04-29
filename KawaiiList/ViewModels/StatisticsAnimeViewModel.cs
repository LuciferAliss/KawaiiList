using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Stores;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
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
        private ISeries[] _series;

        [ObservableProperty]
        private Axis[] _xAxes = 
        [
            new Axis 
            { 
                SeparatorsPaint = new SolidColorPaint(new SKColor(235, 235, 235)),
                LabelsPaint = new SolidColorPaint(SKColors.White),
                CrosshairLabelsPaint = new SolidColorPaint(SKColors.White)
            }
        ];

        [ObservableProperty]
        private Axis[] _yAxes = [new Axis { IsVisible = false }];

        public StatisticsAnimeViewModel(AnimeStore animeStore)
        {
            _animeStore = animeStore;

            var paints = Enumerable.Range(0, 10)
                .Select(i =>
                {
                    float startHue = 120f;
                    float endHue = 0f;

                    float hue = startHue + (endHue - startHue) * i / (10 - 1);

                    float saturation = 1f;
                    float lightness = 0.5f;

                    var color = SKColor.FromHsl(hue, saturation * 100, lightness * 100);
                    return new SolidColorPaint(color);
                })
                .ToArray();


            for (int i = 0; i < _animeStore.CurrentAnimeInfo.RatesScoresStats?.Count; i++)
            {
                _data.Add(new PilotInfo(_animeStore.CurrentAnimeInfo.RatesScoresStats[i].Name.ToString(), _animeStore.CurrentAnimeInfo.RatesScoresStats[i].Value, paints[i]));
            }

            var rowSeries = new RowSeries<PilotInfo>
            {
                Values = _data,
                DataLabelsPaint = new SolidColorPaint(new SKColor(255, 255, 255)),
                DataLabelsPosition = DataLabelsPosition.Start,
                DataLabelsTranslate = new(0, 0),
                DataLabelsFormatter = point => $"{point.Model!.Name}",
                Ry = 3,
                MaxBarWidth = 70,
                Padding = 10,
            }
            .OnPointMeasured(point =>
            {
                // assign a different color to each point
                if (point.Visual is null) return;
                point.Visual.Fill = point.Model!.Paint;
            });

            _series = [rowSeries];
        }
    }
}
