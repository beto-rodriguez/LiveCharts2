using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.Scrollable;

public partial class ViewModel
{
    private bool _isDown = false;
    private readonly ObservableCollection<ObservablePoint> _values = [];

    public ISeries[] Series { get; set; }
    public Axis[] ScrollableAxes { get; set; }
    public ISeries[] ScrollbarSeries { get; set; }
    public Axis[] InvisibleX { get; set; }
    public Axis[] InvisibleY { get; set; }
    public LiveChartsCore.Measure.Margin Margin { get; set; }
    public RectangularSection[] Thumbs { get; set; }

    public ViewModel()
    {
        var trend = 1000;
        var r = new Random();

        for (var i = 0; i < 500; i++)
            _values.Add(new ObservablePoint(i, trend += r.Next(-20, 20)));

        Series = [
            new LineSeries<ObservablePoint>
            {
                Values = _values,
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1)
            }
        ];

        ScrollbarSeries = [
            new LineSeries<ObservablePoint>
            {
                Values = _values,
                GeometryStroke = null,
                GeometryFill = null,
                DataPadding = new(0, 1)
            }
        ];

        ScrollableAxes = [new Axis()];

        Thumbs = [
            new RectangularSection
            {
                Fill = new SolidColorPaint(new SKColor(255, 205, 210, 100))
            }
        ];

        InvisibleX = [new Axis { IsVisible = false }];
        InvisibleY = [new Axis { IsVisible = false }];

        // force the left margin to be 100 and the right margin 50 in both charts, this will
        // align the start and end point of the "draw margin",
        // no matter the size of the labels in the Y axis of both chart.
        var auto = LiveChartsCore.Measure.Margin.Auto;
        Margin = new(100, auto, 50, auto);
    }

    [RelayCommand]
    public void ChartUpdated(ChartCommandArgs args)
    {
        var cartesianChart = (ICartesianChartView)args.Chart;

        var x = cartesianChart.XAxes.First();

        // update the scroll bar thumb when the chart is updated (zoom/pan)
        // this will let the user know the current visible range
        var thumb = Thumbs[0];

        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;
    }

    [RelayCommand]
    public void PointerDown(PointerCommandArgs args) =>
        _isDown = true;

    [RelayCommand]
    public void PointerMove(PointerCommandArgs args)
    {
        if (!_isDown) return;

        var chart = (ICartesianChartView)args.Chart;
        var positionInData = chart.ScalePixelsToData(args.PointerPosition);

        var thumb = Thumbs[0];
        var currentRange = thumb.Xj - thumb.Xi;

        // update the scroll bar thumb when the user is dragging the chart
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;

        // update the chart visible range
        ScrollableAxes[0].MinLimit = thumb.Xi;
        ScrollableAxes[0].MaxLimit = thumb.Xj;
    }

    [RelayCommand]
    public void PointerUp(PointerCommandArgs args) =>
        _isDown = false;
}
