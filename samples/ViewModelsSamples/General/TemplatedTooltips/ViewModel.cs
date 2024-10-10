using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.TemplatedTooltips;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double>
        {
            Values = [3, 7, 3, 1, 4, 5, 6 ],
        },
        new LineSeries<double>
        {
            Values = [2, 1, 3, 5, 3, 4, 6 ],
            Fill = null
        }
    ];
}
