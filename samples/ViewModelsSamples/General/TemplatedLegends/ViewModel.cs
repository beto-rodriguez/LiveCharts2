using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.TemplatedLegends;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Values = new ObservableCollection<double> { 2, 1, 3, 5, 3, 4, 6 }
        }
    };
}
