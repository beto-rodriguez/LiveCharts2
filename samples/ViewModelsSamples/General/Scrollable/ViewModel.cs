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
            }
        };

        InvisibleX = new[] { new Axis { IsVisible = false } };
        InvisibleY = new[] { new Axis { IsVisible = false } };

        // force the left margin to be 100 and the right margin 20 in both charts, this will
        // align the start and end point of the "draw margin",
        // no matter the size of the labels in the Y axis of both chart.
        var auto = LiveChartsCore.Measure.Margin.Auto;
        Margin = new(100, auto, auto, 20);
    }

    public ISeries[] Series { get; set; }
    public Axis[] ScrollableAxes { get; set; }
    public ISeries[] ScrollbarSeries { get; set; }
    public Axis[] InvisibleX { get; set; }
    public Axis[] InvisibleY { get; set; }
    public LiveChartsCore.Measure.Margin Margin { get; set; }
    public RectangularSection[] Thumbs { get; set; }
}
