using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Sketches;

namespace ViewModelsSamples.Axes.NamedLabels;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<int>
        {
            Name = "Sales",
            Values = [200, 558, 458, 249],
        },
        new LineSeries<int>
        {
            Name = "Projected",
            Values = [300, 450, 400, 280],
            Fill = null
        }
    ];

    public ICartesianAxis[] XAxes { get; set; } = [
        new Axis
        {
            // Use the labels property to define named labels.
            Labels = ["Anne", "Johnny", "Zac", "Rosa"]
        }
    ];

    public ICartesianAxis[] YAxes { get; set; } = [
        new Axis
        {
            // Now the Y axis we will display labels as currency
            // LiveCharts provides some common formatters
            // in this case we are using the currency formatter.
            Labeler = Labelers.Currency

            // you could also build your own currency formatter
            // for example:
            // Labeler = (value) => value.ToString("C")

            // But the one that LiveCharts provides creates shorter labels when
            // the amount is in millions or trillions
        }
    ];

    public SolidColorPaint TooltipTextPaint { get; set; } =
        new SolidColorPaint
        {
            Color = new SKColor(242, 244, 195),
            SKTypeface = SKTypeface.FromFamilyName("Courier New")
        };

    public SolidColorPaint TooltipBackgroundPaint { get; set; } =
        new SolidColorPaint(new SKColor(72, 0, 50));
}
