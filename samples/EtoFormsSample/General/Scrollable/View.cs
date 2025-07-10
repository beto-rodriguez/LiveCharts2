using System;
using System.Collections.ObjectModel;
using System.Linq;
using Eto.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.General.Scrollable;

public class View : Panel
{
    private readonly CartesianChart _mainChart;
    private readonly CartesianChart _scrollBarChart;
    private bool _isDown = false;
    private readonly ObservableCollection<ObservablePoint> _values = new();

    public View()
    {
        var trend = 1000;
        var r = new Random();
        for (var i = 0; i < 500; i++)
            _values.Add(new ObservablePoint(i, trend += r.Next(-20, 20)));

        var auto = LiveChartsCore.Measure.Margin.Auto;

        _mainChart = new CartesianChart
        {
            Series = [
                new LineSeries<ObservablePoint>
                {
                    Values = _values,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                }
            ],
            XAxes = [new Axis()],
            ZoomMode = LiveChartsCore.Measure.ZoomAndPanMode.X,
            DrawMargin = new(100, auto, 50, auto)
        };
        _mainChart.UpdateStarted += OnChart_Updated;

        _scrollBarChart = new CartesianChart
        {
            Series = [
                new LineSeries<ObservablePoint>
                {
                    Values = _values,
                    GeometryStroke = null,
                    GeometryFill = null,
                    DataPadding = new(0, 1)
                }
            ],
            DrawMargin = new(100, auto, 50, auto),
            Sections = [
                new RectangularSection
                {
                    Fill = new SolidColorPaint(new SKColor(255, 205, 210, 100))
                }
            ],
            XAxes = [new Axis { IsVisible = false }],
            YAxes = [new Axis { IsVisible = false }],
            TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden
        };
        _scrollBarChart.MouseDown += CartesianChart2_MouseDown;
        _scrollBarChart.MouseMove += CartesianChart2_MouseMove;
        _scrollBarChart.MouseUp += CartesianChart2_MouseUp;

        Content = new DynamicLayout(
            new DynamicRow(new DynamicControl { Control = _mainChart, YScale = true }),
            new DynamicRow(new DynamicControl { Control = _scrollBarChart, YScale = true })
        );
    }

    private void OnChart_Updated(IChartView chart)
    {
        var cartesianChart = (CartesianChart)chart;
        var x = cartesianChart.XAxes.First();
        var thumb = (RectangularSection)_scrollBarChart.Sections.First();
        thumb.Xi = x.MinLimit;
        thumb.Xj = x.MaxLimit;
    }

    private void CartesianChart2_MouseDown(object sender, MouseEventArgs e)
    {
        _isDown = true;
    }

    private void CartesianChart2_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDown) return;
        var positionInData = _scrollBarChart.ScalePixelsToData(new(e.Location.X, e.Location.Y));
        var thumb = (RectangularSection)_scrollBarChart.Sections.First();
        var currentRange = thumb.Xj - thumb.Xi;
        thumb.Xi = positionInData.X - currentRange / 2;
        thumb.Xj = positionInData.X + currentRange / 2;
        _mainChart.XAxes.First().MinLimit = thumb.Xi;
        _mainChart.XAxes.First().MaxLimit = thumb.Xj;
    }

    private void CartesianChart2_MouseUp(object sender, MouseEventArgs e)
    {
        _isDown = false;
    }
}
