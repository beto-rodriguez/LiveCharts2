using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.StepLines.Basic;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new StepLineSeries<double?>
        {
            Values = new ObservableCollection<double?> { 2, 1, 3, 4, 3, 4, 6 },
            Fill = null
        }
    };
}
