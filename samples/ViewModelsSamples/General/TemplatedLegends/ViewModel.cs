using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.TemplatedLegends;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Name = "Roger",
            Values = new ObservableCollection<double> { 2, 1, 3, 5, 3, 4, 6 }
        }
    };
}
