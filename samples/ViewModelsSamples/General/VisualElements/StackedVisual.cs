using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.VisualElements;

public class StackedVisual : Visual
{
    protected override StackLayout DrawnElement { get; } =
        new StackLayout
        {
            // X and Y coordinates are relative to the parent
            Orientation = ContainerOrientation.Vertical,
            Children = [
                new CircleGeometry
                {
                    Width = 40,
                    Height = 40,
                    Fill = new SolidColorPaint(SKColors.CadetBlue)
                },
                new RectangleGeometry
                {
                    Width = 40,
                    Height = 40,
                    Fill = new SolidColorPaint(SKColors.DeepSkyBlue)
                },
                new DiamondGeometry
                {
                    Width = 40,
                    Height = 40,
                    Fill = new SolidColorPaint(SKColors.LightSteelBlue)
                }
            ]
        };

    protected override void Measure(Chart chart)
    {
        DrawnElement.X = 350;
        DrawnElement.Y = 100;
    }
}

