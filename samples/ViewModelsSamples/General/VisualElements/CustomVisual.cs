using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.VisualElements;
using SkiaSharp;

namespace ViewModelsSamples.General.VisualElements;

public class CustomVisual : Visual
{
    protected override CustomSkiaShape DrawnElement { get; } =
        new CustomSkiaShape { Stroke = new SolidColorPaint(SKColors.Red) };

    protected override void Measure(Chart chart)
    {
        DrawnElement.X = 250;
        DrawnElement.Y = 100;
        DrawnElement.Width = 40;
        DrawnElement.Height = 40;
    }
}
