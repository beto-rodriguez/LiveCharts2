using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.VisualElements;

public class ScaledRectangleVisual : Visual
{
    protected override RectangleGeometry DrawnElement { get; } =
        new RectangleGeometry { Fill = new SolidColorPaint(SKColors.Red) };

    protected override void Measure(Chart chart)
    {
        var cartesianChart = (ICartesianChartView)chart.View;

        // use the ScaleDataToPixels function to scale data.
        var locationInDataScale = new LvcPointD(5, 5);
        var locationInPixels = cartesianChart.ScaleDataToPixels(locationInDataScale);

        DrawnElement.X = (float)locationInPixels.X;
        DrawnElement.Y = (float)locationInPixels.Y;
        DrawnElement.Width = 40;
        DrawnElement.Height = 40;
    }
}
