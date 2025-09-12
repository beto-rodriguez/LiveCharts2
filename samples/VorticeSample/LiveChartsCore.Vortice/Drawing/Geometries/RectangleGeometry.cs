using LiveChartsCore.Drawing;
using Vortice.Mathematics;

namespace LiveChartsCore.Vortice.Drawing.Geometries;

public class RectangleGeometry : BoundedDrawnGeometry, IDrawnElement<VorticeDrawingContext>
{
    public void Draw(VorticeDrawingContext context)
    {
        var activeLvcPaint = context.ActiveLvcPaint;
        if (activeLvcPaint is null)
            return; // should not happen?

        var isStroke = activeLvcPaint.PaintStyle.HasFlag(LiveChartsCore.Painting.PaintStyle.Stroke);

        var rect = new Rect(X, Y, Width, Height);

        if (isStroke)
        {
            context.RenderTarget.DrawRectangle(
                rect, context.ActiveBrush, activeLvcPaint.StrokeThickness);
        }
        else
        {
            context.RenderTarget.FillRectangle(
                rect, context.ActiveBrush);
        }
    }
}
