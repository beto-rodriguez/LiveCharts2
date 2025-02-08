using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ViewModelsSamples.General.TemplatedLegends;

public class LegendItem : StackLayout
{
    public LegendItem(ISeries series)
    {
        Orientation = ContainerOrientation.Horizontal;
        Padding = new Padding(12, 6);
        VerticalAlignment = Align.Middle;
        HorizontalAlignment = Align.Middle;
        Opacity = series.IsVisible ? 1 : 0.5f;

        var miniature = (IDrawnElement<SkiaSharpDrawingContext>)series.GetMiniatureGeometry(null);
        if (miniature is BoundedDrawnGeometry bounded)
            bounded.Height = 40;

        Children = [
            miniature,
            new LabelGeometry
            {
                Text = series.Name ?? "?",
                TextSize = 20,
                Paint = new SolidColorPaint(new SKColor(30, 30, 30)),
                Padding = new Padding(8, 2, 0, 2),
                VerticalAlign = Align.Start,
                HorizontalAlign = Align.Start
            }
        ];
    }
}
