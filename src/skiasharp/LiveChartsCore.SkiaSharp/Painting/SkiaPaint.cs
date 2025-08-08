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
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.Painting.ImageFilters;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting;

/// <summary>
/// Initializes a new instance of the <see cref="SkiaPaint"/> class.
/// </summary>
/// <param name="strokeThickness">The stroke thickness.</param>
/// <param name="strokeMiter">The stroke miter.</param>
public abstract class SkiaPaint(float strokeThickness = 1f, float strokeMiter = 0f)
    : Paint(strokeThickness, strokeMiter)
{
    internal SKPaint? _skiaPaint;

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    [Obsolete($"Use the {nameof(SKTypeface)} property and assign it to {nameof(SKTypeface)}.{nameof(SKTypeface.FromFamilyName)}(fontFamily, fontStyle)")]
    public string? FontFamily
    {
        get => field;
        set
        {
            field = value;
            SKTypeface = value is null
                ? null
                : SKTypeface.FromFamilyName(value, SKFontStyle ?? SKTypeface.Default.FontStyle);
        }
    }

    /// <summary>
    /// Gets or sets the font style.
    /// </summary>
    [Obsolete($"Use the {nameof(SKTypeface)} property and assign it to {nameof(SKTypeface)}.{nameof(SKTypeface.FromFamilyName)}(fontFamily, fontStyle)")]
    public SKFontStyle? SKFontStyle
    {
        get => field;
        set
        {
            field = value;
            SKTypeface = value is null
                ? null
                : SKTypeface.FromFamilyName(SKTypeface.Default.FamilyName ?? "Arial", value);
        }
    }

    /// <summary>
    /// Gets or sets the SKTypeface.
    /// </summary>
    public SKTypeface? SKTypeface { get; set; }

    /// <summary>
    /// Gets or sets the stroke cap.
    /// </summary>
    /// <value>
    /// The stroke cap.
    /// </value>
    public SKStrokeCap StrokeCap { get; set; }

    /// <summary>
    /// Gets or sets the stroke join.
    /// </summary>
    /// <value>
    /// The stroke join.
    /// </value>
    public SKStrokeJoin StrokeJoin { get; set; }

    /// <summary>
    /// Gets or sets the path effect.
    /// </summary>
    /// <value>
    /// The path effect.
    /// </value>
    public PathEffect? PathEffect { get; set; }

    /// <summary>
    /// Gets or sets the image filer.
    /// </summary>
    /// <value>
    /// The image filer.
    /// </value>
    public ImageFilter? ImageFilter { get; set; }

    internal bool IsGlobalSKTypeface =>
        GetSKTypeface() == (LiveChartsSkiaSharp.DefaultSKTypeface ?? SKTypeface.Default);

    internal static void Map(SkiaPaint from, SkiaPaint to, float progress = 1)
    {
        to.PaintStyle = from.PaintStyle;
        to.IsAntialias = from.IsAntialias;
        to.StrokeCap = from.StrokeCap;
        to.StrokeJoin = from.StrokeJoin;
        to.SKTypeface = from.SKTypeface;

        to.StrokeThickness = from.StrokeThickness + progress * (to.StrokeThickness - from.StrokeThickness);
        to.StrokeMiter = from.StrokeMiter + progress * (to.StrokeMiter - from.StrokeMiter);
        to.PathEffect = PathEffect.Transitionate(from.PathEffect, to.PathEffect, progress);
        to.ImageFilter = ImageFilter.Transitionate(from.ImageFilter, to.ImageFilter, progress);
    }

    internal SKPaint UpdateSkiaPaint(SkiaSharpDrawingContext? context, IDrawnElement? drawnElement)
    {
        SKPaint paint;

        if (_skiaPaint is null)
        {
            paint = new SKPaint();
            _skiaPaint = paint;

            paint.Style = PaintStyle.HasFlag(PaintStyle.Stroke)
                ? SKPaintStyle.Stroke
                : SKPaintStyle.Fill;

            if (PaintStyle.HasFlag(PaintStyle.Text))
                paint.Typeface = GetSKTypeface();
        }
        else
        {
            paint = _skiaPaint;
        }

        paint.IsAntialias = IsAntialias;
        paint.StrokeCap = StrokeCap;
        paint.StrokeJoin = StrokeJoin;
        paint.StrokeMiter = StrokeMiter;
        paint.StrokeWidth = StrokeThickness;

        if (PathEffect is not null)
        {
            PathEffect.CreateEffect();
            paint.PathEffect = PathEffect._sKPathEffect;
        }

        if (ImageFilter is not null)
        {
            ImageFilter.CreateFilter();
            paint.ImageFilter = ImageFilter._sKImageFilter;
        }

        // special case for text paints.
        // when  the label is mesured, we do not have a context yet.
        if (context is null)
            return paint;

        var clip = GetClipRectangle(context.MotionCanvas);

        if (drawnElement is not null)
        {
            paint.StrokeWidth = drawnElement.StrokeThickness;

            if (drawnElement.ClippingBounds != LvcRectangle.Unset)
                clip = drawnElement.ClippingBounds;
        }

        if (clip != LvcRectangle.Empty)
        {
            _ = context.Canvas.Save();
            context.Canvas.ClipRect(new SKRect(clip.X, clip.Y, clip.X + clip.Width, clip.Y + clip.Height));
        }

        context.ActiveSkiaPaint = paint;

        return paint;
    }

    internal SKTypeface GetSKTypeface() =>
        SKTypeface ?? LiveChartsSkiaSharp.DefaultSKTypeface ?? SKTypeface.Default;

    internal override void OnPaintFinished(DrawingContext context, IDrawnElement? drawnElement)
    {
        var skiaContext = (SkiaSharpDrawingContext)context;

        var clip = GetClipRectangle(skiaContext.MotionCanvas);

        if (drawnElement is not null)
        {
            if (drawnElement.ClippingBounds != LvcRectangle.Unset)
                clip = drawnElement.ClippingBounds;
        }

        if (context is not null && clip != LvcRectangle.Empty)
            skiaContext.Canvas.Restore();
    }

    internal override void DisposeTask()
    {
        if (_skiaPaint is not null && !IsGlobalSKTypeface)
            _skiaPaint.Typeface?.Dispose();

        PathEffect?.Dispose();
        ImageFilter?.Dispose();

        _skiaPaint?.Dispose();
        _skiaPaint = null;
    }
}
