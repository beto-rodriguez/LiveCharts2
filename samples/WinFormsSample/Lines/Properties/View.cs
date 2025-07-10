using System;
using System.Windows.Forms;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace WinFormsSample.Lines.Properties;

public partial class View : UserControl
{
    private readonly CartesianChart cartesianChart;
    private LineSeries<double> _lineSeries;
    private Random _random = new();

    public View()
    {
        InitializeComponent();
        Size = new System.Drawing.Size(100, 100);

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
            Series = [_lineSeries],
            Location = new System.Drawing.Point(0, 50),
            Size = new System.Drawing.Size(100, 50),
            Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
        };

        Controls.Add(cartesianChart);

        var b1 = new Button { Text = "new values", Location = new System.Drawing.Point(0, 0) };
        b1.Click += (sender, e) => ChangeValuesInstance();
        Controls.Add(b1);

        var b2 = new Button { Text = "new fill", Location = new System.Drawing.Point(80, 0) };
        b2.Click += (sender, e) => NewFill();
        Controls.Add(b2);

        var b3 = new Button { Text = "new stroke", Location = new System.Drawing.Point(160, 0) };
        b3.Click += (sender, e) => NewStroke();
        Controls.Add(b3);

        var b4 = new Button { Text = "newGfill", Location = new System.Drawing.Point(240, 0) };
        b4.Click += (sender, e) => NewGeometryFill();
        Controls.Add(b4);

        var b5 = new Button { Text = "newGstroke", Location = new System.Drawing.Point(320, 0) };
        b5.Click += (sender, e) => NewGeometryStroke();
        Controls.Add(b5);

        var b6 = new Button { Text = "+ smooth", Location = new System.Drawing.Point(400, 0) };
        b6.Click += (sender, e) => IncreaseLineSmoothness();
        Controls.Add(b6);

        var b7 = new Button { Text = "- smooth", Location = new System.Drawing.Point(480, 0) };
        b7.Click += (sender, e) => DecreaseLineSmoothness();
        Controls.Add(b7);

        var b8 = new Button { Text = "+ size", Location = new System.Drawing.Point(560, 0) };
        b8.Click += (sender, e) => IncreaseGeometrySize();
        Controls.Add(b8);

        var b9 = new Button { Text = "- size", Location = new System.Drawing.Point(640, 0) };
        b9.Click += (sender, e) => DecreaseGeometrySize();
        Controls.Add(b9);

        var b10 = new Button { Text = "new series", Location = new System.Drawing.Point(720, 0) };
        b10.Click += (sender, e) => ChangeSeriesInstance();
        Controls.Add(b10);
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
