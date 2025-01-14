using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace ViewModelsSamples.General.VisualElements;

public class CustomSkiaShape : BoundedDrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    public void Draw(SkiaSharpDrawingContext context)
    {
        var canvas = context.Canvas;
        var paint = context.ActiveSkiaPaint;

        var x = X + Width / 2f;
        var y = Y + Height / 2f;
        var r = (float)Width / 2f;

        canvas.DrawCircle(x, y, r, paint);
        canvas.DrawCircle(x, y, r * 0.75f, paint);
        canvas.DrawCircle(x, y, r * 0.5f, paint);
    }
}
