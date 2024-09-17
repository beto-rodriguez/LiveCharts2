using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Scatter.AutoUpdate;

public partial class ViewModel : ObservableObject
{
    private int _index = 0;
    private readonly Random _random = new();
    private readonly ObservableCollection<WeightedPoint> _observableValues;

    public ViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). // mark
        _observableValues =
        [
            // Use the WeightedPoint, ObservableValue or ObservablePoint types to let the chart listen for property changes // mark
            // or use any INotifyPropertyChanged implementation // mark
            new WeightedPoint(_index++, 2, 6),
            new(_index++, 5, 5), // the WeightedPoint type is redundant and inferred by the compiler (C# 9 and above)
            new(_index++, 4, 3),
            new(_index++, 5, 9),
            new(_index++, 2, 3),
            new(_index++, 6, 2),
            new(_index++, 6, 7),
            new(_index++, 6, 8),
            new(_index++, 4, 5),
            new(_index++, 2, 3),
            new(_index++, 3, 8),
            new(_index++, 8, 9),
            new(_index++, 3, 4)
        ];

        Series =
        [
            new ScatterSeries<WeightedPoint> { Values = _observableValues, GeometrySize = 50 }
        ];

        // in the following series notice that the type int does not implement INotifyPropertyChanged
        // and our Series.Values collection is of type List<T>
        // List<T> does not implement INotifyCollectionChanged
        // this means the following series is not listening for changes.
        //Series.Add(new LineSeries<int> { Values = new List<int> { 2, 4, 6, 1, 7, -2 } });
    }

    public ObservableCollection<ISeries> Series { get; set; }

    [RelayCommand]
    public void AddItem()
    {
        // for this sample only 50 items are supported.
        if (_observableValues.Count > 50) return;

        var randomValue = _random.Next(1, 10);
        var randomWeight = _random.Next(1, 10);
        _observableValues.Add(new WeightedPoint(_index++, randomValue, randomWeight));
    }

    [RelayCommand]
    public void RemoveItem()
    {
        if (_observableValues.Count < 2) return;

        _observableValues.RemoveAt(0);
    }

    [RelayCommand]
    public void ReplaceItem()
    {
        var randomValue = _random.Next(1, 10);
        var randomWeight = _random.Next(1, 10);
        var randomIndex = _random.Next(0, _observableValues.Count - 1);
        _observableValues[randomIndex] = new WeightedPoint(_observableValues[randomIndex].X, randomValue, randomWeight);
    }

    [RelayCommand]
    public void AddSeries()
    {
        //  for this sample only 5 series are supported.
        if (Series.Count == 5) return;

        Series.Add(
            new ScatterSeries<int>
            {
                Values = new List<int> { _random.Next(0, 10), _random.Next(0, 10), _random.Next(0, 10) }
            });
    }

    [RelayCommand]
    public void RemoveSeries()
    {
        if (Series.Count == 1) return;

        Series.RemoveAt(Series.Count - 1);
    }
}
