using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.ConditionalDraw;

public partial class ViewModel : ObservableObject
{
    private readonly ObservableCollection<ObservableValue> _values = new();

    public ViewModel()
    {
        var dangerPaint = new SolidColorPaint(SKColors.Red);

        _values = new ObservableCollection<ObservableValue>
        {
            new(2), new(5), new(4), new(6), new(8), new(3), new(2), new(4), new(6)
        };

        var series = new ColumnSeries<ObservableValue>
        {
            Name = "Mary",
            Values = _values
        }
        .OnPointMeasured(point =>
        {
            if (point.Visual is null) return;

            var isDanger = point.Model?.Value > 5;

            point.Visual.Fill = isDanger
                ? dangerPaint
                : null; // when null, the series fill is used // mark
        });

        Series = new ISeries[] { series };

        Randomize();
    }

    public ISeries[] Series { get; set; }

    public RectangularSection[] Sections { get; set; } =
    {
        new RectangularSection
        {
            Label = "Danger zone!",
            LabelSize = 15,
            LabelPaint = new SolidColorPaint(SKColors.Red)
            {
                SKTypeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
            },
            Yj = 5,
            Fill = new SolidColorPaint(SKColors.Red.WithAlpha(50))
        }
    };

    public Axis[] Y { get; set; } =
    {
        new Axis { MinLimit = 0 }
    };

    private async void Randomize()
    {
        var r = new Random();

        while (true)
        {
            await Task.Delay(3000);

            foreach (var item in _values)
            {
                item.Value = r.Next(0, 10);
            }
        }
    }
}
