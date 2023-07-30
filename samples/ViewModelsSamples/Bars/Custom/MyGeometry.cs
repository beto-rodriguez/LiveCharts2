using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;

namespace ViewModelsSamples.Bars.Custom;

public class MyGeometry : SizedGeometry
{
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        var canvas = context.Canvas;
        var y = Y;

        while (y < Y + Height)
        {
            canvas.DrawLine(X, y, X + Width, y, paint);
            y += 5;
        }
    }
}
