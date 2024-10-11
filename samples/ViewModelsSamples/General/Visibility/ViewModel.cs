using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.General.Visibility;

public partial class ViewModel
{
    public ISeries[] Series { get; set; } = [
        new ColumnSeries<double>
        {
            Values = [2, 5, 4, 3],
            IsVisible = true
        },
        new ColumnSeries<double>
        {
            Values = [6, 3, 2, 8],
            IsVisible = true
        },
        new ColumnSeries<double>
        {
            Values = [4, 2, 8, 7],
            IsVisible = true
        }
    ];

    [RelayCommand]
    public void ToggleSeries0() =>
        Series[0].IsVisible = !Series[0].IsVisible;

    [RelayCommand]
    public void ToggleSeries1() =>
        Series[1].IsVisible = !Series[1].IsVisible;

    [RelayCommand]
    public void ToggleSeries2() =>
        Series[2].IsVisible = !Series[2].IsVisible;
}
