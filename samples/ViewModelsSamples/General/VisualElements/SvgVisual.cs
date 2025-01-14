using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.VisualElements;

public class SvgVisual : Visual
{
    protected override VariableSVGPathGeometry DrawnElement { get; } =
        new VariableSVGPathGeometry
        {
            Stroke = new SolidColorPaint(SKColors.Blue),
            Path = SKPath.ParseSvgPathData(SVGPoints.Star)
        };

    protected override void Measure(Chart chart)
    {
        DrawnElement.X = 200;
        DrawnElement.Y = 100;
        DrawnElement.Width = 40;
        DrawnElement.Height = 40;
    }
}

