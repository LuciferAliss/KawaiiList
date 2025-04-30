using CommunityToolkit.Mvvm.ComponentModel;
using KawaiiList.Stores;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
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
        private Axis[] _xAxes;
            
        //    = 
        //[
        //    new Axis 
        //    {
        //        SeparatorsPaint = new SolidColorPaint(new SKColor(235, 235, 235)),
        //        LabelsPaint = new SolidColorPaint(SKColors.White),
        //        Labeler = value => value.ToString(), // Форматирование меток
        //        ForceStepToMin = false, // Принудительно использовать MinStep
        //        MinStep = 1,
        //    }
        //];

        [ObservableProperty]
        private Axis[] _yAxes;

        public StatisticsAnimeViewModel(AnimeStore animeStore)
        {
            _animeStore = animeStore;

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

            _series = [rowSeries];

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
    }
}
