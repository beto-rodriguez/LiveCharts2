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
        var size = _textLayout.Metrics;

        var app = Application.Current!;

        app.TextRenderer.ActiveBrush = context.ActiveBrush;
        _textLayout.Draw(
            app.TextRenderer,
            X + GetRelativeX(HorizontalAlign, _textLayout.Metrics),
            Y + GetRelativeY(VerticalAlign, _textLayout.Metrics));
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

    private float GetRelativeX(Align align, TextMetrics size)
    {
        return align switch
        {
            Align.Start => Padding.Left,
            Align.Middle => -size.Width / 2f - Padding.Left,
            Align.End => -size.Width - Padding.Left,
            _ => throw new ArgumentOutOfRangeException(nameof(align), align, null)
        };
    }

    private static float GetRelativeY(Align align, TextMetrics size)
    {
        return align switch
        {
            Align.Start => 0f,
            Align.Middle => -size.Height / 2f,
            Align.End => -size.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(align), align, null)
        };
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
