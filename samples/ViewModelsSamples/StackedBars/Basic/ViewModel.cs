using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.StackedBars.Basic;

public class ViewModel
{
    public ISeries[] Series { get; set; } =
    [
        new StackedColumnSeries<int>
        {
            Values = [3, 5, -3, 2, 5, -4, -2],
            Stroke = null,
            DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
            DataLabelsSize = 14,
            DataLabelsPosition = DataLabelsPosition.Middle,
            YToolTipLabelFormatter =
                p => $"{p.Coordinate.PrimaryValue:N} ({p.StackedValue!.Share:P})"
        },
        new StackedColumnSeries<int>
        {
            Values = [4, 2, -3, 2, 3, 4, -2],
            Stroke = null,
            DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
            DataLabelsSize = 14,
            DataLabelsPosition = DataLabelsPosition.Middle,
            YToolTipLabelFormatter =
                p => $"{p.Coordinate.PrimaryValue:N} ({p.StackedValue!.Share:P})"
        },
        new StackedColumnSeries<int>
        {
            Values = [-2, 6, 6, 5, 4, 3, -2],
            Stroke = null,
            DataLabelsPaint = new SolidColorPaint(new SKColor(45, 45, 45)),
            DataLabelsSize = 14,
            DataLabelsPosition = DataLabelsPosition.Middle,
            YToolTipLabelFormatter =
                p => $"{p.Coordinate.PrimaryValue:N} ({p.StackedValue!.Share:P})"
        }
    ];
}
