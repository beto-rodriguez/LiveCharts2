using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Bars.AutoUpdate;

public partial class ViewModel
{
    private readonly Random _random = new();

    // We use the ObservableCollection class, to let the chart know // mark
    // when a new item is added or removed from the chart. // mark
    public ObservableCollection<ISeries> Series { get; set; }
    public ObservableCollection<ObservableValue> ObservablePoints { get; set; } = [];

    public ViewModel()
    {
        UpdateSeries();

        Series = [new ColumnSeries<ObservableValue>(ObservablePoints)];
    }

    [RelayCommand]
    public void UpdateSeries()
    {
        ObservablePoints.Clear();

        for (var i = 0; i < 5; i++)
        {
            var value = _random.Next(100);
            ObservablePoints.Add(new(value));
        }
    }

    [RelayCommand]
    public void ClearSeries() =>
        ObservablePoints.Clear();

    [RelayCommand]
    public void ToggleVisibility() =>
        Series[0].IsVisible = !Series[0].IsVisible;

    [RelayCommand]
    public void AddPoint() =>
        ObservablePoints.Add(new(100));

}
