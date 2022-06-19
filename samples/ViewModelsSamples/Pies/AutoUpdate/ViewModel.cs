﻿using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Pies.AutoUpdate;

[ObservableObject]
public partial class ViewModel
{
    private readonly Random _random = new();

    public ViewModel()
    {
        // Use ObservableCollections to let the chart listen for changes (or any INotifyCollectionChanged). // mark
        Series = new ObservableCollection<ISeries>
        {
            // Use the ObservableValue or ObservablePoint types to let the chart listen for property changes // mark
            // or use any INotifyPropertyChanged implementation // mark
            new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(2) } },
            new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(5) } },
            new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(3) } },
            new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(7) } },
            new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(4) } },
            new PieSeries<ObservableValue> { Values = new[] { new ObservableValue(3) } }
        };
    }

    public ObservableCollection<ISeries> Series { get; set; }

    [ICommand]
    public void AddSeries()
    {
        //  for this sample only 15 series are supported.
        if (Series.Count == 15) return;

        Series.Add(
            new PieSeries<ObservableValue>
            {
                Values = new[] { new ObservableValue(_random.Next(1, 10)) }
            });
    }

    [ICommand]
    public void UpdateAll()
    {
        foreach (var series in Series)
        {
            if (series.Values is null) continue;

            foreach (var value in series.Values)
            {
                var observableValue = (ObservableValue)value;
                observableValue.Value = _random.Next(1, 10);
            }
        }
    }

    [ICommand]
    public void RemoveSeries()
    {
        if (Series.Count == 1) return;

        Series.RemoveAt(Series.Count - 1);
    }
}
