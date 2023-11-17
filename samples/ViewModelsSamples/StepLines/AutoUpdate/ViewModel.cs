using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.StepLines.AutoUpdate;

public partial class ViewModel : ObservableObject
{
    private int _index = 0;
    private readonly Random _random = new();
    private readonly ObservableCollection<ObservablePoint> _observableValues;

    public ViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). // mark
        _observableValues =
        [
            // Use the ObservableValue or ObservablePoint types to let the chart listen for property changes // mark
            // or use any INotifyPropertyChanged implementation // mark
            new ObservablePoint(_index++, 2),
            new(_index++, 5), // the ObservablePoint type is redundant and inferred by the compiler (C# 9 and above)
            new(_index++, 4),
            new(_index++, 5),
            new(_index++, 2),
            new(_index++, 6),
            new(_index++, 6),
            new(_index++, 6),
            new(_index++, 4),
            new(_index++, 2),
            new(_index++, 3),
            new(_index++, 4),
            new(_index++, 3)
        ];

        Series =
        [
            new StepLineSeries<ObservablePoint>
            {
                Values = _observableValues,
                // Fill = null
            }
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
        var randomValue = _random.Next(1, 10);
        _observableValues.Add(
            new ObservablePoint { X = _index++, Y = randomValue });
    }

    [RelayCommand]
    public void RemoveItem()
    {
        _observableValues.RemoveAt(0);
    }

    [RelayCommand]
    public void UpdateItem()
    {
        var randomValue = _random.Next(1, 10);
        _observableValues[_observableValues.Count - 1].Y = randomValue;
    }

    [RelayCommand]
    public void ReplaceItem()
    {
        var randomValue = _random.Next(1, 10);
        var randomIndex = _random.Next(0, _observableValues.Count - 1);
        _observableValues[randomIndex] =
            new ObservablePoint { X = _observableValues[randomIndex].X, Y = randomValue };
    }

    [RelayCommand]
    public void AddSeries()
    {
        //  for this sample only 5 series are supported.
        if (Series.Count == 5) return;

        Series.Add(
            new StepLineSeries<int>
            {
                Values = new List<int>
                {
                    _random.Next(0, 10),
                    _random.Next(0, 10),
                    _random.Next(0, 10)
                }
            });
    }

    [RelayCommand]
    public void RemoveSeries()
    {
        if (Series.Count == 1) return;

        Series.RemoveAt(Series.Count - 1);
    }
}
