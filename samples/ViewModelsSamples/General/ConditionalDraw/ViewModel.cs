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
        _values = new ObservableCollection<ObservableValue>
        {
            new(2), new(5), new(4), new(6), new(8), new(3), new(2), new(4), new(6)
        };

        var series1 = new LineSeries<ObservableValue>
        {
            Name = "Mary",
            Values = _values
        }
        .WithConditionalPaint(new SolidColorPaint(SKColors.Red))
        .When(point => point.Model?.Value > 5);

        Series = new ISeries[] { series1 };

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
