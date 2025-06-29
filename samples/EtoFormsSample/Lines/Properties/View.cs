using System;
using Eto.Forms;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Eto;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace EtoFormsSample.Lines.Properties;

public class View : Panel
{
    private readonly CartesianChart cartesianChart;
    private LineSeries<double> _lineSeries;
    private Random _random = new();

    public View()
    {
        _lineSeries = new LineSeries<double>
        {
            Values = [-2, -1, 3, 5, 3, 4, 6],
            Stroke = new SolidColorPaint(SKColors.Black),
            Fill = new SolidColorPaint(SKColor.Parse("#30000000")),
            GeometryStroke = new SolidColorPaint(SKColors.Black),
            GeometryFill = new SolidColorPaint(SKColor.Parse("#30000000")),
            LineSmoothness = 0.5,
            GeometrySize = 20
        };

        cartesianChart = new CartesianChart
        {
            Series = [_lineSeries]
        };

        var b1 = new Button { Text = "new values" };
        b1.Click += (sender, e) => ChangeValuesInstance();

        var b2 = new Button { Text = "new fill" };
        b2.Click += (sender, e) => NewFill();

        var b3 = new Button { Text = "new stroke" };
        b3.Click += (sender, e) => NewStroke();

        var b4 = new Button { Text = "newGfill" };
        b4.Click += (sender, e) => NewGeometryFill();

        var b5 = new Button { Text = "newGstroke" };
        b5.Click += (sender, e) => NewGeometryStroke();

        var b6 = new Button { Text = "+ smooth" };
        b6.Click += (sender, e) => IncreaseLineSmoothness();

        var b7 = new Button { Text = "- smooth" };
        b7.Click += (sender, e) => DecreaseLineSmoothness();

        var b8 = new Button { Text = "+ size" };
        b8.Click += (sender, e) => IncreaseGeometrySize();

        var b9 = new Button { Text = "- size" };
        b9.Click += (sender, e) => DecreaseGeometrySize();

        var b10 = new Button { Text = "new series" };
        b10.Click += (sender, e) => ChangeSeriesInstance();

        var buttons = new StackLayout(b1, b2, b3, b4, b5, b6, b7, b8, b9, b10) { Orientation = Orientation.Horizontal, Padding = 2, Spacing = 4 };

        Content = new DynamicLayout(buttons, cartesianChart);
    }

    private void ChangeValuesInstance()
    {
        var t = 0;
        var values = new double[10];
        for (var i = 0; i < 10; i++)
        {
            t += _random.Next(-5, 10);
            values[i] = t;
        }
        _lineSeries.Values = values;
    }

    private void NewStroke() => _lineSeries.Stroke = new SolidColorPaint(GetRandomColor());
    private void NewFill() => _lineSeries.Fill = new SolidColorPaint(GetRandomColor());
    private void NewGeometryFill() => _lineSeries.GeometryFill = new SolidColorPaint(GetRandomColor());
    private void NewGeometryStroke() => _lineSeries.GeometryStroke = new SolidColorPaint(GetRandomColor());

    private void IncreaseLineSmoothness()
    {
        if (_lineSeries.LineSmoothness >= 1) return;
        _lineSeries.LineSmoothness += 0.1;
    }

    private void DecreaseLineSmoothness()
    {
        if (_lineSeries.LineSmoothness <= 0) return;
        _lineSeries.LineSmoothness -= 0.1;
    }

    private void IncreaseGeometrySize()
    {
        if (_lineSeries.GeometrySize >= 60) return;
        _lineSeries.GeometrySize += 10;
    }

    private void DecreaseGeometrySize()
    {
        if (_lineSeries.GeometrySize <= 0) return;
        _lineSeries.GeometrySize -= 10;
    }

    private void ChangeSeriesInstance()
    {
        _lineSeries = new LineSeries<double>
        {
            Values = [-2, -1, 3, 5, 3, 4, 6],
            Stroke = new SolidColorPaint(SKColors.Black),
            Fill = new SolidColorPaint(SKColor.Parse("#30000000")),
            GeometryStroke = new SolidColorPaint(SKColors.Black),
            GeometryFill = new SolidColorPaint(SKColor.Parse("#30000000")),
            LineSmoothness = 0.5,
            GeometrySize = 20
        };
        cartesianChart.Series = [_lineSeries];
    }

    private SKColor GetRandomColor()
    {
        var r = _random.Next(0, 255);
        var g = _random.Next(0, 255);
        var b = _random.Next(0, 255);
        return new SKColor((byte)r, (byte)g, (byte)b);
    }
}
