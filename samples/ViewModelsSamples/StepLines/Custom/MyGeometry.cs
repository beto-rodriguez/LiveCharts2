using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using SkiaSharp;

namespace ViewModelsSamples.StepLines.Custom;

public class MyGeometry : SizedGeometry
{
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        var canvas = context.Canvas;

        canvas.DrawRect(X, Y, Width, Height, paint);
        canvas.DrawLine(X, Y, X + Width, Y + Height, paint);
        canvas.DrawLine(X + Width, Y, X, Y + Height, paint);
    }
}
