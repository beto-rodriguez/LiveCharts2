using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.Basic;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 2, 1, 3, 5, 3, 4, 6 },
            Fill = null
        },
        new LineSeries<double>
        {
            Values = new double[] { 8, 4, 6, 7, 9, 2, 5 },
            Fill = null,
            IsVisibleAtLegend = false
        }
    };
}
