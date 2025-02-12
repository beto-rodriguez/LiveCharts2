using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace ViewModelsSamples.Pies.AngularGauge;

public class CustomNeedle : NeedleGeometry
{
    public override void Draw(SkiaSharpDrawingContext context)
    {
        var paint = context.ActiveSkiaPaint;

        context.Canvas.DrawRect(X - Width * 0.5f, Y, Width, Radius, paint);
    }
}
