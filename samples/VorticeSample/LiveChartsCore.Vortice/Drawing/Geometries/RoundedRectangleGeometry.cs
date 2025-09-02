using LiveChartsCore.Drawing;
using Vortice.Direct2D1;

namespace LiveChartsCore.Vortice.Drawing.Geometries;

public class RoundedRectangleGeometry : BaseRoundedRectangleGeometry, IDrawnElement<VorticeDrawingContext>
{
    public void Draw(VorticeDrawingContext context)
    {
        var activeLvcPaint = context.ActiveLvcPaint;
        if (activeLvcPaint is null)
            return; // should not happen?

        var isStroke = activeLvcPaint.PaintStyle.HasFlag(LiveChartsCore.Painting.PaintStyle.Stroke);

        var br = BorderRadius;

        var roundedRect = new RoundedRectangle(new(X, Y, Width, Height), br.X, br.Y);

        if (isStroke)
        {
            context.RenderTarget.DrawRoundedRectangle(
                roundedRect, context.ActiveBrush, activeLvcPaint.StrokeThickness);
        }
        else
        {
            context.RenderTarget.FillRoundedRectangle(
                roundedRect, context.ActiveBrush);
        }
    }
}
