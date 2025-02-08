using System;
using System.Collections.Generic;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;

namespace ViewModelsSamples.General.TemplatedTooltips;

public class CustomTooltip : SKDefaultTooltip
{
    // the Initialize method is used to set up the tooltip animations, colors, etc.
    protected override void Initialize(Chart chart)
    {
        // the Wedge property, defines the size of the triangle that points to the target point.
        Wedge = 5;

        Geometry.Fill = new SolidColorPaint(new SKColor(200, 200, 200), 3);
        Geometry.Stroke = new SolidColorPaint(new SKColor(28, 49, 58), 3);
        Geometry.Wedge = Wedge;
        Geometry.WedgeThickness = 2;

        this.Animate(
            new Animation(
                EasingFunctions.BounceOut,
                TimeSpan.FromMilliseconds(500)));
    }

    // the GetLayout method is used to define the content of the tooltip,
    // it is called every time the tooltip changes.
    protected override Layout<SkiaSharpDrawingContext> GetLayout(IEnumerable<ChartPoint> foundPoints, Chart chart)
    {
        var layout = new StackLayout
        {
            Padding = new(10),
            Orientation = ContainerOrientation.Vertical,
            HorizontalAlignment = Align.Middle,
            VerticalAlignment = Align.Middle
        };

        var i = 0;
        foreach (var point in foundPoints)
        {
            i++;

            var series = point.Context.Series;
            var geometryPoint = (GeometryPoint)point.Context.DataSource!;

            var miniature =
                (IDrawnElement<SkiaSharpDrawingContext>)series.GetMiniatureGeometry(point);

            var label = new LabelGeometry
            {
                Text = point.Coordinate.PrimaryValue.ToString("C2"),
                Paint = new SolidColorPaint(new SKColor(30, 30, 30)),
                TextSize = 15,
                Padding = new Padding(8, 0),
                VerticalAlign = Align.Start,
                HorizontalAlign = Align.Start
            };

            var customContent = new VariableSVGPathGeometry
            {
                Fill = new SolidColorPaint(new SKColor(30, 30, 30)),
                Width = 30,
                Height = 30,
                SVGPath = geometryPoint.Geometry
            };

            var sp = new StackLayout
            {
                Orientation = ContainerOrientation.Horizontal,
                Padding = new Padding(0, 4),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    miniature,
                    label,
                    customContent
                }
            };

            layout.Children.Add(sp);
        }

        return layout;
    }

    public override void Show(IEnumerable<ChartPoint> foundPoints, Chart chart)
    {
        base.Show(foundPoints, chart);

        // write code here to add custom behavior when the tooltip is shown.
    }

    public override void Hide(Chart chart)
    {
        base.Hide(chart);

        // write code here to add custom behavior when the tooltip is hidden.
    }
}
