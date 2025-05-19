using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.Measure;

namespace ViewModelsSamples.General.Tooltips;

public partial class ViewModel : ObservableObject
{
    [ObservableProperty]
    public partial TooltipPosition SelectedPosition { get; set; } =
        TooltipPosition.Top;

    public TooltipPosition[] Positions { get; } = [
        TooltipPosition.Hidden,
        TooltipPosition.Top,
        TooltipPosition.Bottom,
        TooltipPosition.Right,
        TooltipPosition.Left,
        TooltipPosition.Center
    ];
}
