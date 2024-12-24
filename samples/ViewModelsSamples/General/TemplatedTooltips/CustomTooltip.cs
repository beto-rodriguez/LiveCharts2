using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.TemplatedTooltips;

public class CustomTooltip : IChartTooltip
{
    private Container<RoundedRectangleGeometry>? _container;
    private StackLayout? _layout;

    public void Show(IEnumerable<ChartPoint> foundPoints, Chart chart)
    {
        if (_container is null || _layout is null)
        {
            _container = new Container<RoundedRectangleGeometry>
            {
                Content = _layout = new StackLayout
                {
                    Padding = new(10),
                    Orientation = ContainerOrientation.Vertical,
                    HorizontalAlignment = Align.Middle,
                    VerticalAlignment = Align.Middle
                }
            };

            _container.Geometry.Fill = new SolidColorPaint(new SKColor(28, 49, 58));
            _container.Animate(new Animation(EasingFunctions.BounceOut, TimeSpan.FromMilliseconds(500)));

            var drawTask = chart.Canvas.AddGeometry(_container);
            drawTask.ZIndex = 10100;
        }

        _container.Opacity = 1;
        _container.ScaleTransform = new LvcPoint(1, 1);

        foreach (var child in _layout.Children.ToArray())
            _ = _layout.Children.Remove(child);

        foreach (var point in foundPoints)
        {
            var series = point.Context.Series;
            var miniature = (IDrawnElement<SkiaSharpDrawingContext>)series.GetMiniatureGeometry(point);

            var label = new LabelGeometry
            {
                Text = point.Coordinate.PrimaryValue.ToString("C2"),
                Paint = new SolidColorPaint(new SKColor(230, 230, 230)),
                TextSize = 15,
                Padding = new Padding(8, 0, 0, 0),
                VerticalAlign = Align.Start,
                HorizontalAlign = Align.Start
            };

            var sp = new StackLayout
            {
                Padding = new Padding(0, 4),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    miniature,
                    label
                }
            };

            _layout.Children.Add(sp);
        }

        var size = _container.Measure();
        var location = foundPoints.GetTooltipLocation(size, chart);

        _container.X = location.X;
        _container.Y = location.Y;

        chart.Canvas.Invalidate();
    }

    public void Hide(Chart chart)
    {
        if (chart is null || _container is null) return;

        _container.Opacity = 0f;
        _container.ScaleTransform = new LvcPoint(0f, 0f);

        chart.Canvas.Invalidate();
    }
}
