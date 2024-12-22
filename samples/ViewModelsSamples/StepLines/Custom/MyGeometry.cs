using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace ViewModelsSamples.StepLines.Custom;

public class MyGeometry : BoundedDrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    public void Draw(SkiaSharpDrawingContext context)
    {
        var paint = context.ActiveSkiaPaint;
        var canvas = context.Canvas;

        canvas.DrawRect(X, Y, Width, Height, paint);
        canvas.DrawLine(X, Y, X + Width, Y + Height, paint);
        canvas.DrawLine(X + Width, Y, X, Y + Height, paint);
    }
}
