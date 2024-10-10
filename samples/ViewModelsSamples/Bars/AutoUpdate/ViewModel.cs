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
    public ObservableCollection<ObservablePoint> ObservablePoints { get; set; }

    public ViewModel()
    {
        ObservablePoints = [
            new() { X = 0, Y = 2 },
            new() { X = 1, Y = 5 },
            new() { X = 2, Y = 4 }
        ];

        Series = [
            new ColumnSeries<ObservablePoint>(ObservablePoints)
        ];
    }

    [RelayCommand]
    public void AddSeries()
    {
        // Because the Series property is an ObservableCollection, // mark
        // the chart will listen for changes and update // mark
        // in this case a new series is added to the chart // mark

        var newColumnSeries = new ColumnSeries<int>(FetchVales());
        Series.Add(newColumnSeries);
    }

    [RelayCommand]
    public void RemoveSeries()
    {
        if (Series.Count == 1) return;

        // This will also remove the series from the UI. // mark
        Series.RemoveAt(Series.Count - 1);
    }

    [RelayCommand]
    public void AddItem()
    {
        var newPoint = new ObservablePoint
        {
            X = ObservablePoints.Count,
            Y = _random.Next(0, 10)
        };

        // The new point will be drawn at the end of the chart // mark
        ObservablePoints.Add(newPoint);
    }

    [RelayCommand]
    public void RemoveItem()
    {
        if (ObservablePoints.Count < 2) return;

        // Because the ObservablePoints property is an ObservableCollection, // mark
        // the chart will listen for changes and update // mark
        // in this case a point is removed from the chart // mark

        ObservablePoints.RemoveAt(0);
    }

    [RelayCommand]
    public void ReplaceItem()
    {
        var randomIndex = _random.Next(0, ObservablePoints.Count - 1);

        // The chart will update the point at the specified index // mark
        ObservablePoints[randomIndex] = new(ObservablePoints[randomIndex].X, _random.Next(1, 10));
    }

    private int[] FetchVales()
    {
        return [
            _random.Next(0, 10),
            _random.Next(0, 10),
            _random.Next(0, 10)
        ];
    }
}
