using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.Basic;

[ObservableObject]
public partial class ViewModel
{
    public ISeries[] Series { get; set; } =
    {
        new LineSeries<ObservablePoint>
        {
            Values = new ObservablePoint[]
            {
                new ObservablePoint { X = 2, Y = 0 },
                new ObservablePoint { X = 3, Y = 3 },
                new ObservablePoint { X = 5, Y = null },
                new ObservablePoint { X = 6, Y = 4 },
                new ObservablePoint { X = 8, Y = 6 }
            },
            Fill = null
        }
    };
}
