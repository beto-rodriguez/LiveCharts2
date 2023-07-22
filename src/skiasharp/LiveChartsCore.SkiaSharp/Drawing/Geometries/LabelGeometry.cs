// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <inheritdoc cref="ILabelGeometry{TDrawingContext}" />
public class LabelGeometry : Geometry, ILabelGeometry<SkiaSharpDrawingContext>
{
    private readonly FloatMotionProperty _textSizeProperty;
    private readonly ColorMotionProperty _backgroundProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelGeometry"/> class.
    /// </summary>
    public LabelGeometry()
        : base(true)
    {
        _textSizeProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(TextSize), 11));
        _backgroundProperty = RegisterMotionProperty(new ColorMotionProperty(nameof(Background), LvcColor.Empty));
        TransformOrigin = new LvcPoint(0f, 0f);
    }

    /// <summary>
    /// Gets or sets the vertical align.
    /// </summary>
    /// <value>
    /// The vertical align.
    /// </value>
    public Align VerticalAlign { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the horizontal align.
    /// </summary>
    /// <value>
    /// The horizontal align.
    /// </value>
    public Align HorizontalAlign { get; set; } = Align.Middle;

    /// <inheritdoc cref="ILabelGeometry{TDrawingContext}.Text" />
    public string Text { get; set; } = string.Empty;

    /// <inheritdoc cref="ILabelGeometry{TDrawingContext}.TextSize" />
    public float TextSize { get => _textSizeProperty.GetMovement(this); set => _textSizeProperty.SetMovement(value, this); }

    /// <inheritdoc cref="ILabelGeometry{TDrawingContext}.Background" />
    public LvcColor Background { get => _backgroundProperty.GetMovement(this); set => _backgroundProperty.SetMovement(value, this); }

    /// <inheritdoc cref="ILabelGeometry{TDrawingContext}.Padding" />
    public Padding Padding { get; set; } = new();

    /// <inheritdoc cref="ILabelGeometry{TDrawingContext}.LineHeight" />
    public float LineHeight { get; set; } = 1.75f;

#if DEBUG
    /// <summary>
    /// This property is only available on debug mode, it indicates if the debug lines should be shown.
    /// </summary>
    public static bool ShowDebugLines { get; set; } = true;
#endif

    private LvcPoint GetAlignmentOffset(SKRect bounds)
    {
        var p = Padding;

        var w = bounds.Width + p.Left + p.Right;
        var h = bounds.Height * LineHeight + p.Top + p.Bottom;

        float l = -bounds.Left, t = -bounds.Top;

        switch (VerticalAlign)
        {
            case Align.Start: t += 0; break;
            case Align.Middle: t -= h * 0.5f; break;
            case Align.End: t -= h + 0; break;
            default:
                break;
        }
        switch (HorizontalAlign)
        {
            case Align.Start: l += 0; break;
            case Align.Middle: l -= w * 0.5f; break;
            case Align.End: l -= w + 0; break;
            default:
                break;
        }

        return new(l, t);
    }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        // it seems that skia allocates a lot of memory when drawing text
        // for now we are not focused on performance, but this should be improved in the future

        // for a reason the text size is not set on the InitializeTask() method.
        context.Paint.TextSize = TextSize;

        var verticalPos = 0f;

        foreach (var line in Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
        {
            var textBounds = new SKRect();
            _ = context.Paint.MeasureText(line, ref textBounds);

            var lhd = (textBounds.Height * LineHeight - textBounds.Height) * 0.5f;
            var ao = GetAlignmentOffset(textBounds);
            var p = Padding;

            context.Canvas.DrawText(line, X + ao.X + p.Left, Y + ao.Y + p.Top + lhd + verticalPos, paint);

#if DEBUG
            if (ShowDebugLines)
            {
                using var r = new SKPaint { Color = new SKColor(255, 0, 0), IsStroke = true };
                using var b = new SKPaint { Color = new SKColor(0, 0, 255), IsStroke = true };

                context.Canvas.DrawRect(X - 2.5f, Y - 2.5f, 5, 5, b);

                context.Canvas.DrawRect(
                    X + ao.X,
                    Y + ao.Y - textBounds.Height + verticalPos,
                    textBounds.Width + Padding.Left + Padding.Right,
                    textBounds.Height * LineHeight + Padding.Top + Padding.Bottom,
                    r);

                context.Canvas.DrawRect(
                    X + ao.X + p.Left,
                    Y + ao.Y - textBounds.Height + p.Top + verticalPos,
                    textBounds.Width,
                    textBounds.Height * LineHeight,
                    b);
            }
#endif

            verticalPos += textBounds.Height * LineHeight;
        }
    }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})" />
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> paint)
    {
        var skiaPaint = (Paint)paint;
        var typeface = skiaPaint.GetSKTypeface();

        using var p = new SKPaint
        {
            Color = skiaPaint.Color,
            IsAntialias = skiaPaint.IsAntialias,
            IsStroke = skiaPaint.IsStroke,
            StrokeWidth = skiaPaint.StrokeThickness,
            TextSize = TextSize,
            Typeface = typeface
        };

        var w = 0f;
        var h = 0f;

        foreach (var line in Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
        {
            var bounds = new SKRect();
            _ = p.MeasureText(line, ref bounds);

            if (bounds.Width > w) w = bounds.Width;
            h += bounds.Height * LineHeight;
        }

        // Note #301222
        // Disposing typefaces could cause render issues (Blazor) at least on SkiaSharp (2.88.3)
        // Could this cause memory leaks?
        // Should the user dispose typefaces manually?
        // typeface.Dispose();

        return new LvcSize(
            w + Padding.Left + Padding.Right,
            h + Padding.Top + Padding.Bottom);
    }
}
