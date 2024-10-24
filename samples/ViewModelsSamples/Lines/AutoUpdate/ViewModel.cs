using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.AutoUpdate;

public partial class ViewModel
{
    private readonly Random _random = new();

    // We use the ObservableCollection class to let the chart know // mark
    // when a new item is added or removed from the chart. // mark
    public ObservableCollection<ISeries> Series { get; set; }

    // The ObservablePoints property is an ObservableCollection of ObservableValue // mark
    // it means that the chart is listening for changes in this collection // mark
    // and also for changes in the properties of each element in the collection // mark
    public ObservableCollection<ObservableValue> ObservableValues { get; set; }

    public ViewModel()
    {
        ObservableValues = [
            new() { Value = 2 },
            new() { Value = 5 },
            new() { Value = 4 }
        ];

        Series = [
            new LineSeries<ObservableValue>(ObservableValues)
        ];
    }

    [RelayCommand]
    public void AddItem()
    {
        var randomValue = _random.Next(1, 10);

        // the new value is added to the collection // mark
        // the chart is listening, and will update and animate the change // mark

        ObservableValues.Add(new() { Value = randomValue });
    }

    [RelayCommand]
    public void RemoveItem()
    {
        if (ObservableValues.Count == 0) return;

        // the last value is removed from the collection // mark
        // the chart is listening, and will update and animate the change // mark

        ObservableValues.RemoveAt(0);
    }

    [RelayCommand]
    public void UpdateItem()
    {
        var randomValue = _random.Next(1, 10);
        var lastItem = ObservableValues[ObservableValues.Count - 1];

        // becase lastItem is an ObservableObject and implements INotifyPropertyChanged // mark
        // the chart is listening for changes in the Value property // mark
        // and will update and animate the change // mark

        lastItem.Value = randomValue;
    }

    [RelayCommand]
    public void ReplaceItem()
    {
        var randomValue = _random.Next(1, 10);
        var randomIndex = _random.Next(0, ObservableValues.Count - 1);

        // replacing and item also triggers the chart to update and animate the change // mark

        ObservableValues[randomIndex] = new(randomValue);
    }

    [RelayCommand]
    public void AddSeries()
    {
        var values = Enumerable.Range(0, 3)
            .Select(_ => _random.Next(0, 10))
            .ToArray();

        // a new line series is added to the chart // mark

        Series.Add(new LineSeries<int>(values));
    }

    [RelayCommand]
    public void RemoveSeries()
    {
        if (Series.Count == 1) return;

        // the last series is removed from the chart // mark

        Series.RemoveAt(Series.Count - 1);
    }
}

// All LiveCharts objects (Series, Axes, etc) implement INotifyPropertyChanged // mark
// this means that the chart is listening for changes in the properties // mark
// the chart will reflect the changes and animate them // mark
