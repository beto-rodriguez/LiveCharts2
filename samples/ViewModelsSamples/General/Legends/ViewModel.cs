using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Measure;

namespace ViewModelsSamples.General.Legends;

public partial class ViewModel : ObservableObject
{
    public LegendPosition[] Positions { get; } = [
        LegendPosition.Hidden,
        LegendPosition.Top,
        LegendPosition.Bottom,
        LegendPosition.Right,
        LegendPosition.Left
    ];

    [ObservableProperty]
    public partial LegendPosition SelectedPosition { get; set; } =
        LegendPosition.Left;
}
