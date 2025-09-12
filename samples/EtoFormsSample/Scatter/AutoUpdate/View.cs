using System;
using System.Collections.ObjectModel;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;

namespace EtoFormsSample.Scatter.AutoUpdate;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;
    private readonly ObservableCollection<ISeries> _seriesCollection;
    private readonly ObservableCollection<ObservablePoint> _juanaValues;
    private readonly Random _random = new();

    public View()
    {
        _juanaValues = new ObservableCollection<ObservablePoint>
        {
            new(0, 2),
            new(1, 5),
            new(2, 4)
        };

        _seriesCollection = new ObservableCollection<ISeries>
        {
            new ScatterSeries<ObservablePoint>
            {
                Name = "Juana",
                Values = _juanaValues
            },
            new ScatterSeries<ObservablePoint>
            {
                Name = "Mary",
                Values = new ObservableCollection<ObservablePoint>
                {
                    new(0, 5),
                    new(1, 4),
                    new(2, 1)
                }
            }
        };

        cartesianChart = new CartesianChart
        {
            Series = _seriesCollection
        };

        var b1 = new Button { Text = "Add item" };
        b1.Click += (sender, e) => AddItem();

        var b2 = new Button { Text = "Replace item" };
        b2.Click += (sender, e) => ReplaceItem();

        var b3 = new Button { Text = "Remove item" };
        b3.Click += (sender, e) => RemoveItem();

        var b4 = new Button { Text = "Add series" };
        b4.Click += (sender, e) => AddSeries();

        var b5 = new Button { Text = "Remove series" };
        b5.Click += (sender, e) => RemoveSeries();

        var buttons = new StackLayout(b1, b2, b3, b4, b5) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

        Content = new DynamicLayout(buttons, cartesianChart);
    }

    private void AddSeries()
    {
        _seriesCollection.Add(
            new ScatterSeries<ObservablePoint>
            {
                Name = $"User #{_seriesCollection.Count}",
                Values = new ObservableCollection<ObservablePoint>(FetchValues())
            });
    }

    private void RemoveSeries()
    {
        if (_seriesCollection.Count <= 1) return;
        _seriesCollection.RemoveAt(_seriesCollection.Count - 1);
    }

    private void AddItem()
    {
        var newPoint = new ObservablePoint(_juanaValues.Count, _random.Next(0, 10));
        _juanaValues.Add(newPoint);
    }

    private void RemoveItem()
    {
        if (_juanaValues.Count < 2) return;
        _juanaValues.RemoveAt(0);
    }

    private void ReplaceItem()
    {
        if (_juanaValues.Count < 2) return;
        var randomIndex = _random.Next(0, _juanaValues.Count - 1);
        _juanaValues[randomIndex] = new ObservablePoint(randomIndex, _random.Next(1, 10));
    }

    private ObservablePoint[] FetchValues()
    {
        return new ObservablePoint[]
        {
            new(0, _random.Next(0, 10)),
            new(1, _random.Next(0, 10)),
            new(2, _random.Next(0, 10))
        };
    }
}
