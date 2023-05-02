using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.TemplatedTooltips;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Values = new ObservableCollection<double> { 3, 7, 3, 1, 4, 5, 6 },
        },
        new LineSeries<double>
        {
            Values = new ObservableCollection<double> { 2, 1, 3, 5, 3, 4, 6 },
            Fill = null
        }
    };
}
