using System;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.Scrollable;

public partial class ViewModel
{
    private readonly ObservableCollection<ObservablePoint> _values = new();

    public ViewModel()
    {
        var trend = 1000;
        var r = new Random();

        for (var i = 0; i < 500; i++)
        {
            _values.Add(new ObservablePoint(i, trend += r.Next(-20, 20)));
        }

        Series = new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = _values,
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1)
            }
        };

        ScrollbarSeries = new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = _values,
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1)
            }
        };

        ScrollableAxes = new[] { new Axis() };

        Thumbs = new[]
        {
            new RectangularSection
            {
                Fill = new SolidColorPaint(new SKColor(255, 205, 210, 100))
            },
        };

        InvisibleX = new[] { new Axis { IsVisible = false } };
        InvisibleY = new[] { new Axis { IsVisible = false } };

        // force the left margin to be 100 in both charts, this will
        // align the start point of the series, no matter the size
        // of the labels in the Y axis of both chart.
        var auto = LiveChartsCore.Measure.Margin.Auto;
        Margin = new(100, auto, auto, auto);
    }

    public ISeries[] Series { get; set; } =
    {
        new ScatterSeries<ObservablePoint>
        {
            Values = new ObservableCollection<ObservablePoint>
            {
                new(2.2, 5.4),
                new(4.5, 2.5),
                new(4.2, 7.4),
                new(6.4, 9.9),
                new(4.2, 9.2),
                new(5.8, 3.5),
                new(7.3, 5.8),
                new(8.9, 3.9),
                new(6.1, 4.6),
                new(9.4, 7.7),
                new(8.4, 8.5),
                new(3.6, 9.6),
                new(4.4, 6.3),
                new(5.8, 4.8),
                new(6.9, 3.4),
                new(7.6, 1.8),
                new(8.3, 8.3),
                new(9.9, 5.2),
                new(8.1, 4.7),
                new(7.4, 3.9),
                new(6.8, 2.3),
                new(5.3, 7.1),
            }
        }
    };

    public Axis[] ScrollableAxes { get; set; }

    public ISeries[] ScrollbarSeries { get; set; }
    public Axis[] InvisibleX { get; set; }
    public Axis[] InvisibleY { get; set; }
    public LiveChartsCore.Measure.Margin Margin { get; set; }
    public RectangularSection[] Thumbs { get; set; }
}
