using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.VisualElements;

public class RectangleVisual : Visual
{
    protected override RectangleGeometry DrawnElement { get; } =
        new RectangleGeometry { Fill = new SolidColorPaint(SKColors.Red) };

    protected override void Measure(Chart chart)
    {
        DrawnElement.X = 100;
        DrawnElement.Y = 100;
        DrawnElement.Width = 40;
        DrawnElement.Height = 40;
    }
}
