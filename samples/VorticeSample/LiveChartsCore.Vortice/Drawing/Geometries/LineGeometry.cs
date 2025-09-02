using LiveChartsCore.Drawing;

namespace LiveChartsCore.Vortice.Drawing.Geometries;

public class LineGeometry : BaseLineGeometry, IDrawnElement<VorticeDrawingContext>
{
    public virtual void Draw(VorticeDrawingContext context) =>
        context.RenderTarget.DrawLine(
            new(X, Y), new(X1, Y1), context.ActiveBrush, context.ActiveLvcPaint?.StrokeThickness ?? 1f);
}
