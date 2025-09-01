using LiveChartsCore.Drawing;
using Vortice;
using Vortice.DirectWrite;

namespace LiveChartsCore.Vortice.Drawing.Geometries;

public class LabelGeometry : BaseLabelGeometry, IDrawnElement<VorticeDrawingContext>
{
    private string _previousText = null!;
    private IDWriteTextFormat _textFormat = null!;
    private IDWriteTextLayout _textLayout = null!;

    public virtual void Draw(VorticeDrawingContext context)
    {
        Validate();

        var app = Application.Current!;

        app.TextRenderer.ActiveBrush = context.ActiveBrush;
        _textLayout.Draw(app.TextRenderer, X, Y);
    }

    /// <inheritdoc cref="DrawnGeometry.Measure()" />
    public override LvcSize Measure()
    {
        Validate();

        var textSize = _textLayout.Metrics;

        var x = Padding.Left + Padding.Right;
        var y = Padding.Top + Padding.Bottom;

        return new LvcSize(textSize.Width + x, textSize.Height + y)
            .GetRotatedSize(RotateTransform);
    }

    internal override void OnDisposed()
    {
        base.OnDisposed();

        _textFormat?.Dispose();
        _textLayout?.Dispose();
    }

    private void Validate()
    {
        if (Text == _previousText) return;

        _previousText = Text;

        _textLayout?.Dispose();
        _textFormat?.Dispose();

        var app = Application.Current!;

        _textFormat = app.WriteFactory.CreateTextFormat("Arial", FontWeight.Regular, FontStyle.Normal, TextSize);
        _textLayout = app.WriteFactory.CreateTextLayout(Text, _textFormat, MaxWidth, float.MaxValue);
    }
}
