using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.ConditionalDraw;
using LiveChartsCore.Measure;
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
            Values = new[] { 2, 5, 4, 6, 8, 3, 2, 4, 6 },
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

        Series = new ISeries[] { series };
    }

    public ISeries[] Series { get; set; }
}
