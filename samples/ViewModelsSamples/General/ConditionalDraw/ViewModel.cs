using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;

namespace ViewModelsSamples.General.ConditionalDraw;

public class ViewModel
{
    private readonly ObservableCollection<ObservableValue> _values = [];

    public ViewModel()
    {
        var dangerPaint = new SolidColorPaint(SKColors.Red);

        _values = [
            new(2),
            new(8),
            new(4)
        ];

        var series = new ColumnSeries<ObservableValue>
        {
            Name = "Mary",
            Values = _values
        };

        series
            .OnPointMeasured(point =>
            {
                if (point.Visual is null) return;

                var isDanger = point.Model?.Value > 5;

                point.Visual.Fill = isDanger
                    ? dangerPaint
                    : null; // when null, the series fill is used // mark
            });

        Series = [series];

        Randomize();
    }

    public ISeries[] Series { get; set; }

    public RectangularSection[] Sections { get; set; } = [
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
    ];

    public ICartesianAxis[] Y { get; set; } = [
        new Axis { MinLimit = 0 }
    ];

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
