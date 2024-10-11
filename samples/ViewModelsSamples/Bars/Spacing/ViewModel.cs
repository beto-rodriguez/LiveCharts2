using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Bars.Spacing;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double>
        {
            Values = [ 20, 50, 40, 20, 40, 30, 50, 20, 50, 40 ],

            // Defines the distance between every bars in the series
            Padding = 0,

            // Defines the max width a bar can have
            MaxBarWidth = double.MaxValue
        }
    ];
}
