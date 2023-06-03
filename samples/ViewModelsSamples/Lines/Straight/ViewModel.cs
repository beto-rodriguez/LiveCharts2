using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.Straight;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 5, 0, 5, 0, 5, 0 },
            Fill = null,
            GeometrySize = 0,
            // use the line smoothness property to control the curve
            // it goes from 0 to 1
            // where 0 is a straight line and 1 the most curved
            LineSmoothness = 0 // mark
        },
        new LineSeries<double>
        {
            Values = new double[] { 7, 2, 7, 2, 7, 2 },
            Fill = null,
            GeometrySize = 0,
            LineSmoothness = 1 // mark
        }
    };
}
