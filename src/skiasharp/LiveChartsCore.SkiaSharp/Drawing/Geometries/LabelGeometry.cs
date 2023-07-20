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
using SkiaSharp.HarfBuzz;

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

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        var size = OnMeasure(context.PaintTask);

        var bg = Background;
        if (bg != LvcColor.Empty)
        {
            using (var bgPaint = new SKPaint { Color = new SKColor(bg.R, bg.G, bg.B, (byte)(bg.A * Opacity)) })
            {
                var p = Padding;
                context.Canvas.DrawRect(X - p.Left, Y - size.Height + p.Bottom, size.Width, size.Height, bgPaint);
            }
        }

        var lines = GetLines(Text);
        double linesCount = lines.Length;
        var lineNumber = 0;
        var lhd = (GetActualLineHeight(paint) - GetRawLineHeight(paint)) * 0.5f;

        foreach (var line in lines)
        {
            var ph = (float)(++lineNumber / linesCount) * size.Height;
            var yLine = ph - size.Height;
            DrawLine(line, yLine - lhd, context, paint);
        }
    }

    /// <inheritdoc cref="Geometry.OnMeasure(IPaint{SkiaSharpDrawingContext})" />
    protected override LvcSize OnMeasure(IPaint<SkiaSharpDrawingContext> drawable)
    {
        var skiaSpaint = (Paint)drawable;
        var typeface = skiaSpaint.GetSKTypeface();

        using var p = new SKPaint
        {
            Color = skiaSpaint.Color,
            IsAntialias = skiaSpaint.IsAntialias,
            IsStroke = skiaSpaint.IsStroke,
            StrokeWidth = skiaSpaint.StrokeThickness,
            TextSize = TextSize,
            Typeface = typeface
        };

        var bounds = MeasureLines(p);

        // Note #301222
        // Disposing typefaces could cause render issues.
        // Does this causes memory leaks?
        // Should the user dispose typefaces manually?
        //typeface.Dispose();

        return new LvcSize(bounds.Width + Padding.Left + Padding.Right, bounds.Height + Padding.Top + Padding.Bottom);
    }

    /// <inheritdoc cref="Geometry.ApplyCustomGeometryTransform(SkiaSharpDrawingContext)" />
    protected override void ApplyCustomGeometryTransform(SkiaSharpDrawingContext context)
    {
        context.Paint.TextSize = TextSize;
        var size = MeasureLines(context.Paint);

        const double toRadians = Math.PI / 180d;
        var p = Padding;
        float w = 0.5f, h = 0.5f;

        switch (VerticalAlign)
        {
            case Align.Start: h = 1f * size.Height + p.Top; break;
            case Align.Middle: h = 0.5f * (size.Height + p.Top - p.Bottom); break;
            case Align.End: h = 0f * size.Height - p.Bottom; break;
            default:
                break;
        }
        switch (HorizontalAlign)
        {
            case Align.Start: w = 0f * size.Width - p.Left; break;
            case Align.Middle: w = 0.5f * (size.Width - p.Left + p.Right); break;
            case Align.End: w = 1 * size.Width + p.Right; break;
            default:
                break;
        }

        var rotation = RotateTransform;
        rotation = (float)(rotation * toRadians);

        var xp = -Math.Cos(rotation) * w + -Math.Sin(rotation) * h;
        var yp = -Math.Sin(rotation) * w + Math.Cos(rotation) * h;

        // translate the label to the upper-left corner
        // just for consistency with the rest of the shapes in the library (and Skia??),
        // and also translate according to the vertical an horizontal alignment properties
        context.Canvas.Translate((float)xp, (float)yp);
    }

    private void DrawLine(string content, float yLine, SkiaSharpDrawingContext context, SKPaint paint)
    {
        if (paint.Typeface is not null)
        {
            using var eventTextShaper = new SKShaper(paint.Typeface);
            context.Canvas.DrawShapedText(content, new SKPoint(X, Y + yLine), paint);
            return;
        }

        context.Canvas.DrawText(content, new SKPoint(X, Y + yLine), paint);
    }

    private LvcSize MeasureLines(SKPaint paint)
    {
        float w = 0f, h = 0f;
        var lineHeight = GetActualLineHeight(paint);

        foreach (var line in GetLines(Text))
        {
            var bounds = new SKRect();

            _ = paint.MeasureText(line, ref bounds);

            if (bounds.Width > w) w = bounds.Width;
            h += lineHeight;
        }

        return new LvcSize(w, h);
    }

    private float GetActualLineHeight(SKPaint paint)
    {
        var boundsH = new SKRect();
        _ = paint.MeasureText("█", ref boundsH);
        return LineHeight * boundsH.Height;
    }

    private float GetRawLineHeight(SKPaint paint)
    {
        var boundsH = new SKRect();
        _ = paint.MeasureText("█", ref boundsH);
        return boundsH.Height;
    }

    private string[] GetLines(string multiLineText)
    {
        return string.IsNullOrEmpty(multiLineText)
            ? Array.Empty<string>()
            : multiLineText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
    }
}
