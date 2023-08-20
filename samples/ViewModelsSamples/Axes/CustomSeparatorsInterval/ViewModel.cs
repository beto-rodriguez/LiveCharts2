using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.Axes.CustomSeparatorsInterval;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new LineSeries<int> { Values = new[] { 10, 55, 45, 68, 60, 70, 75, 78 } }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            // We can specify a custom separator collection
            // the library will use this separators instead of
            // calculating them based on the date of the chart
            CustomSeparators = new double[] { 0, 10, 25, 50, 100 },
            MinLimit = 0, // forces the axis to start at 0
            MaxLimit = 100, // forces the axis to end at 100
            SeparatorsPaint = new SolidColorPaint(SKColors.Black.WithAlpha(100))
        }
    };
}
