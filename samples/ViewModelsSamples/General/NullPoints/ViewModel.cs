using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.NullPoints;

public partial class ViewModel : ObservableObject
{
    public ISeries[] Series { get; set; } = new ISeries[]
    {
        new ColumnSeries<double?>
        {
            Values = new double?[] { 5, 4, null, 3, 2, 6, 5, 6, 2 }
        },

        new LineSeries<double?>
        {
            Values = new double?[] { 2, 6, 5, 3, null, 5, 2, 4, null }
        },

        new LineSeries<ObservablePoint?>
        {
            Values = new ObservablePoint?[]
            {
                new ObservablePoint { X = 0, Y = 1 },
                new ObservablePoint { X = 1, Y = 4 },
                null,
                new ObservablePoint { X = 4, Y = 5 },
                new ObservablePoint { X = 6, Y = 1 },
                new ObservablePoint { X = 8, Y = 6 },
            }
        }
    };
}
