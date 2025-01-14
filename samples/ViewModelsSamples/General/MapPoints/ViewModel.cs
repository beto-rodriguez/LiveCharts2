using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace ViewModelsSamples.General.MapPoints;

public class ViewModel
{
    public ISeries[] Series { get; set; }

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
            Values = [2, 5, 4, 6, 8, 3, 2, 4, 6],
            DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30)),
            DataLabelsPosition = DataLabelsPosition.Top
        }
        .OnPointMeasured(point =>
        {
            // this method is called for each point in the series
            // we can customize the visual here

            if (point.Visual is null) return;

            // get a paint from the array
            var paint = paints[point.Index % paints.Length];
            // set the paint to the visual
            point.Visual.Fill = paint;
        });

        Series = [series];
    }
}
