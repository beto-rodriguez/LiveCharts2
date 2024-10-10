using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.Straight;

public class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new LineSeries<double>
        {
            Values = [5, 0, 5, 0, 5, 0],
            Fill = null,
            GeometrySize = 0,
            // use the line smoothness property to control the curve
            // it goes from 0 to 1
            // where 0 is a straight line and 1 the most curved
            LineSmoothness = 0 // mark
        },
        new LineSeries<double>
        {
            Values = [7, 2, 7, 2, 7, 2],
            Fill = null,
            GeometrySize = 0,
            LineSmoothness = 1 // mark
        }
    ];
}
