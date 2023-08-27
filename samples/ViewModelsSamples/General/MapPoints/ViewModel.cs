using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.MapPoints;

public partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        var paints = new SolidColorPaint[]
        {
            new(SKColors.Red),
            new(SKColors.Green),
            new(SKColors.Blue),
            new(SKColors.Yellow)
        };

        var series = new ColumnSeries<int>
        {
            Name = "Mary",
            Values = new[] { 2, 5, 4, 6, 8, 3, 2, 4, 6 }
        }
        .MapPoints(point =>
        {
            // skip the points with null visuals
            if (point.Visual is null) return;

            // get a paint from the array
            var paint = paints[point.Index % paints.Length];

            // set the paint to the visual
            point.Visual.Fill = paint;
        });

        Series = new ISeries[] { series };
    }

    public ISeries[] Series { get; set; }
}
