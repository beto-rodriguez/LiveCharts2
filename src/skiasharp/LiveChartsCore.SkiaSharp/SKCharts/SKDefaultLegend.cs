// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <summary>
/// Defines the default legend for a chart.
/// </summary>
public class SKDefaultLegend : IChartLegend
{
    private Paint? _backgroundPaint = null;
    private bool _isInCanvas = false;
    private DrawablesTask? _drawableTask;

    // marked as internal only for testing purposes
    internal readonly Container _container;
    internal readonly StackLayout _stackLayout;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKDefaultLegend"/> class.
    /// </summary>
    public SKDefaultLegend()
    {
        _container = new Container
        {
            Content = _stackLayout = new()
            {
                Padding = new Padding(15, 4),
                HorizontalAlignment = Align.Start,
                VerticalAlignment = Align.Middle
            }
        };

        FontPaint = new SolidColorPaint(new SKColor(30, 30, 30, 255));
    }

    /// <summary>
    /// Gets or sets the legend font paint.
    /// </summary>
    public Paint? FontPaint { get; set; }

    /// <summary>
    /// Gets or sets the background paint.
    /// </summary>
    public Paint? BackgroundPaint
    {
        get => _backgroundPaint;
        set
        {
            _backgroundPaint = value;
            if (value is not null)
            {
                value.PaintStyle = PaintStyle.Fill;
            }
        }
    }

    /// <summary>
    /// Gets or sets the fonts size.
    /// </summary>
    public double TextSize { get; set; } = 15;

    /// <inheritdoc cref="IChartLegend.Draw(Chart)"/>
    public void Draw(Chart chart)
    {
        var legendPosition = chart.GetLegendPosition();

        _container.X = legendPosition.X;
        _container.Y = legendPosition.Y;

        if (!_isInCanvas)
        {
            _drawableTask = chart.Canvas.AddGeometry(_container);
            _isInCanvas = true;
        }

        if (chart.LegendPosition == LegendPosition.Hidden && _drawableTask is not null)
        {
            chart.Canvas.RemovePaintTask(_drawableTask);
            _isInCanvas = false;
            _drawableTask = null;
        }
    }

    /// <inheritdoc cref="IChartLegend.Measure(Chart)"/>
    public LvcSize Measure(Chart chart)
    {
        BuildLayout(chart);

        return _container.Measure();
    }

    private void BuildLayout(Chart chart)
    {
        if (chart.View.LegendTextPaint is not null) FontPaint = chart.View.LegendTextPaint;
        if (chart.View.LegendBackgroundPaint is not null) BackgroundPaint = chart.View.LegendBackgroundPaint;
        if (chart.View.LegendTextSize is not null) TextSize = chart.View.LegendTextSize.Value;

        _stackLayout.Orientation = chart.LegendPosition is LegendPosition.Left or LegendPosition.Right
            ? ContainerOrientation.Vertical
            : ContainerOrientation.Horizontal;

        if (_stackLayout.Orientation == ContainerOrientation.Horizontal)
        {
            _stackLayout.MaxWidth = chart.ControlSize.Width;
            _stackLayout.MaxHeight = double.MaxValue;
        }
        else
        {
            _stackLayout.MaxWidth = double.MaxValue;
            _stackLayout.MaxHeight = chart.ControlSize.Height;
        }

        _container.Geometry.Fill = BackgroundPaint;

        foreach (var visual in _stackLayout.Children.ToArray())
            _ = _stackLayout.Children.Remove(visual);

        foreach (var series in chart.Series.Where(x => x.IsVisibleAtLegend))
        {
            _stackLayout.Children.Add(new StackLayout
            {
                Padding = new Padding(12, 6),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    (IDrawnElement<SkiaSharpDrawingContext>)series.GetMiniatureGeometry(null),
                    new LabelGeometry
                    {
                        Text = series.Name ?? string.Empty,
                        Paint = FontPaint,
                        TextSize = (float)TextSize,
                        Padding = new Padding(8, 2, 0, 2),
                        MaxWidth = (float)LiveCharts.DefaultSettings.MaxTooltipsAndLegendsLabelsWidth,
                        VerticalAlign = Align.Start,
                        HorizontalAlign = Align.Start
                    }
                }
            });
        }
    }
}
