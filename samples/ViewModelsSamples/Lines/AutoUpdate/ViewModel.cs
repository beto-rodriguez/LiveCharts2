using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;

namespace ViewModelsSamples.Lines.AutoUpdate;

public class ViewModel
{
    private readonly Random _random = new();
    private readonly ObservableCollection<ObservableValue> _observableValues;

    public ViewModel()
    {
        // LiveCharts already provides the LiveChartsCore.Defaults.ObservableValue class.
        _observableValues = new ObservableCollection<ObservableValue>
            {
                new ObservableValue(2),
                new(5), // the ObservableValue type is redundant and inferred by the compiler (C# 9 and above)
                new(4),
                new(5),
                new(2),
                new(6),
                new(6),
                new(6),
                new(4),
                new(2),
                new(3),
                new(4),
                new(3)
            };

        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<ObservableValue>
            {
                Values = _observableValues,
                Fill = null
            }
        };
    }

    public ObservableCollection<ISeries> Series { get; set; }

    public void AddItem()
    {
        var randomValue = _random.Next(1, 10);
        _observableValues.Add(new(randomValue)); // or the old syntax = new ObservableValue(randomValue)
    }

    public void RemoveFirstItem()
    {
        if (_observableValues.Count == 0) return;
        _observableValues.RemoveAt(0);
    }

    public void UpdateLastItem()
    {
        var randomValue = _random.Next(1, 10);

        // we grab the last instance in our collection
        var lastInstance = _observableValues[_observableValues.Count - 1];

        // finally modify the value property and the chart is updated!
        lastInstance.Value = randomValue;
    }

    public void ReplaceRandomItem()
    {
        var randomValue = _random.Next(1, 10);
        var randomIndex = _random.Next(0, _observableValues.Count - 1);
        _observableValues[randomIndex] = new(randomValue);
    }

    public void AddSeries()
    {
        //  for this sample only 5 series are supported.
        if (Series.Count == 5) return;

        Series.Add(
            new LineSeries<int>
            {
                Values = new List<int>
                {
                    _random.Next(0, 10),
                    _random.Next(0, 10),
                    _random.Next(0, 10)
                }
            });
    }

    public void RemoveLastSeries()
    {
        if (Series.Count == 1) return;

        Series.RemoveAt(Series.Count - 1);
    }

    // The next commands are only to enable XAML bindings // mark
    // they are not used in the WinForms or blazor sample // mark
    public ICommand AddItemCommand => new Command(o => AddItem());
    public ICommand RemoveItemCommand => new Command(o => RemoveFirstItem());
    public ICommand UpdateItemCommand => new Command(o => UpdateLastItem());
    public ICommand ReplaceItemCommand => new Command(o => ReplaceRandomItem());
    public ICommand AddSeriesCommand => new Command(o => AddSeries());
    public ICommand RemoveSeriesCommand => new Command(o => RemoveLastSeries());
}
