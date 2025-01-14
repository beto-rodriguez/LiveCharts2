using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.VisualElements;

public class ContainerVisual : Visual
{
    private readonly Container _container;

    public ContainerVisual()
    {
        _container = new Container
        {
            Content = new LabelGeometry
            {
                Text = "Hello",
                TextSize = 20,
                Padding = new(10),
                Paint = new SolidColorPaint(SKColors.Black),
                VerticalAlign = Align.Start,
                HorizontalAlign = Align.Start
            }
        };

        _container.Geometry.Stroke = new SolidColorPaint(SKColors.Black, 3);
        _container.Geometry.Fill = new SolidColorPaint(SKColors.LightGray);
    }

    protected override Container DrawnElement => _container;

    protected override void Measure(Chart chart)
    {
        DrawnElement.X = 500;
        DrawnElement.Y = 100;
    }
}
