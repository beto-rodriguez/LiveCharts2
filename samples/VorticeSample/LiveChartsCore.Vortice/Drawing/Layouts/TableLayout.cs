using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;

namespace LiveChartsCore.Vortice.Drawing.Layouts;

/// <inheritdoc cref="CoreTableLayout{TDrawingContext}"/>
public class TableLayout : CoreTableLayout<VorticeDrawingContext>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableLayout"/> class.
    /// </summary>
    public TableLayout()
    { }

    /// <inheritdoc cref="CoreTableLayout{TDrawingContext}.AddChild(IDrawnElement{TDrawingContext}, int, int, Align?, Align?)"/>
    public new TableLayout AddChild(
        IDrawnElement<VorticeDrawingContext> drawable,
        int row,
        int column,
        Align? horizontalAlign = null,
        Align? verticalAlign = null)
    {
        _ = base.AddChild(drawable, row, column, horizontalAlign, verticalAlign);
        return this;
    }
}
