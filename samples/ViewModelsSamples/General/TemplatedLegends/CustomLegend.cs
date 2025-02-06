using System.Linq;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.SKCharts;
using SkiaSharp;

namespace ViewModelsSamples.General.TemplatedLegends;

public class CustomLegend : SKDefaultLegend
{
    protected override Layout<SkiaSharpDrawingContext> GetLayout(Chart chart)
    {
        var _stackLayout = new StackLayout
        {
            Orientation = ContainerOrientation.Vertical,
            Padding = new Padding(15, 4),
            HorizontalAlignment = Align.Start,
            VerticalAlignment = Align.Middle,
        };

        foreach (var series in chart.Series.Where(x => x.IsVisibleAtLegend))
        {
            var sl = new StackLayout
            {
                Padding = new Padding(12, 6),
                VerticalAlignment = Align.Middle,
                HorizontalAlignment = Align.Middle,
                Children =
                {
                    new RectangleGeometry
                    {
                        Fill = (series as IStrokedAndFilled)?.Fill?.CloneTask(),
                        Stroke = new SolidColorPaint(new SKColor(30, 30, 30), 3),
                        Width = 20,
                        Height = 50
                    },
                    new LabelGeometry
                    {
                        Text = series.Name ?? string.Empty,
                        Paint = new SolidColorPaint(new SKColor(30, 30, 30)),
                        TextSize = 20,
                        Padding = new Padding(8, 2, 0, 2),
                        VerticalAlign = Align.Start,
                        HorizontalAlign = Align.Start
                    }
                }
            };

            _stackLayout.Children.Add(sl);
        }

        return _stackLayout;
    }
}
