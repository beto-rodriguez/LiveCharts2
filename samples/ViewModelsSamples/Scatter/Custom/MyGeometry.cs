using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace ViewModelsSamples.Scatter.Custom;

public class MyGeometry : BoundedDrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    public void Draw(SkiaSharpDrawingContext context)
    {
        var paint = context.ActiveSkiaPaint;
        var canvas = context.Canvas;
        var y = Y;

        while (y < Y + Height)
        {
            canvas.DrawLine(X, y, X + Width, y, paint);
            y += 10;
        }
    }
}
