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

using LiveChartsCore.Drawing;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting;

/// <summary>
/// Defines a set of geometries that will be painted using a solid color.
/// </summary>
/// <seealso cref="Paint" />
public class SolidColorPaint : SkiaPaint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SolidColorPaint"/> class.
    /// </summary>
    public SolidColorPaint()
        : base()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolidColorPaint"/> class.
    /// </summary>
    /// <param name="color">The color.</param>
    public SolidColorPaint(SKColor color)
        : base()
    {
        Color = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolidColorPaint"/> class.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="strokeWidth">Width of the stroke.</param>
    public SolidColorPaint(SKColor color, float strokeWidth)
        : base(strokeWidth)
    {
        Color = color;
    }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public SKColor Color { get; set; }

    /// <inheritdoc cref="Paint.CloneTask" />
    public override Paint CloneTask()
    {
        var clone = new SolidColorPaint
        {
            PaintStyle = PaintStyle,
            Color = Color,
            IsAntialias = IsAntialias,
            StrokeThickness = StrokeThickness,
            StrokeCap = StrokeCap,
            StrokeJoin = StrokeJoin,
            StrokeMiter = StrokeMiter,
            SKTypeface = SKTypeface,
            PathEffect = PathEffect?.Clone(),
            ImageFilter = ImageFilter?.Clone()
        };

        return clone;
    }

    internal override void OnPaintStarted(DrawingContext drawingContext)
    {
        var skiaContext = (SkiaSharpDrawingContext)drawingContext;
        _skiaPaint ??= new SKPaint();

        _skiaPaint.Color = Color;
        _skiaPaint.IsAntialias = IsAntialias;
        _skiaPaint.StrokeCap = StrokeCap;
        _skiaPaint.StrokeJoin = StrokeJoin;
        _skiaPaint.StrokeMiter = StrokeMiter;
        _skiaPaint.StrokeWidth = StrokeThickness;
        _skiaPaint.Style = PaintStyle.HasFlag(PaintStyle.Stroke) ? SKPaintStyle.Stroke : SKPaintStyle.Fill;
        if (PaintStyle.HasFlag(PaintStyle.Text))
            _skiaPaint.Typeface = GetSKTypeface();

        if (PathEffect is not null)
        {
            PathEffect.CreateEffect(skiaContext);
            _skiaPaint.PathEffect = PathEffect.SKPathEffect;
        }

        if (ImageFilter is not null)
        {
            ImageFilter.CreateFilter(skiaContext);
            _skiaPaint.ImageFilter = ImageFilter.SKImageFilter;
        }

        var clip = GetClipRectangle(skiaContext.MotionCanvas);
        if (clip != LvcRectangle.Empty)
        {
            _ = skiaContext.Canvas.Save();
            skiaContext.Canvas.ClipRect(new SKRect(clip.X, clip.Y, clip.X + clip.Width, clip.Y + clip.Height));
        }

        skiaContext.ActiveSkiaPaint = _skiaPaint;
    }

    internal override void OnPaintFinished(DrawingContext context)
    {
        var skiaContext = (SkiaSharpDrawingContext)context;

        if (_skiaPaint is not null && !IsGlobalSKTypeface)
            _skiaPaint.Typeface?.Dispose();
        PathEffect?.Dispose();
        ImageFilter?.Dispose();

        if (context is not null && GetClipRectangle(skiaContext.MotionCanvas) != LvcRectangle.Empty)
            skiaContext.Canvas.Restore();

        _skiaPaint?.Dispose();
        _skiaPaint = null;
    }

    internal override Paint Transitionate(float progress, Paint target)
    {
        if (target._source is not SolidColorPaint paint) return target;

        var clone = (SolidColorPaint)CloneTask();

        clone.StrokeThickness = StrokeThickness + progress * (paint.StrokeThickness - StrokeThickness);
        clone.StrokeMiter = StrokeMiter + progress * (paint.StrokeMiter - StrokeMiter);
        clone.PathEffect = PathEffect?.Transitionate(progress, paint.PathEffect);
        clone.ImageFilter = ImageFilters.ImageFilter.Transitionate(ImageFilter, paint.ImageFilter, progress);

        clone.Color = new SKColor(
            (byte)(Color.Red + progress * (paint.Color.Red - Color.Red)),
            (byte)(Color.Green + progress * (paint.Color.Green - Color.Green)),
            (byte)(Color.Blue + progress * (paint.Color.Blue - Color.Blue)),
            (byte)(Color.Alpha + progress * (paint.Color.Alpha - Color.Alpha)));

        return clone;
    }

    internal override void ApplyOpacityMask(DrawingContext context, float opacity)
    {
        var skiaContext = (SkiaSharpDrawingContext)context;
        var baseColor = Color;
        skiaContext.ActiveSkiaPaint.Color =
            new SKColor(
                baseColor.Red,
                baseColor.Green,
                baseColor.Blue,
                (byte)(baseColor.Alpha * opacity));
    }

    internal override void RestoreOpacityMask(DrawingContext context, float opacity)
    {
        var skiaContext = (SkiaSharpDrawingContext)context;
        skiaContext.ActiveSkiaPaint.Color = Color;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>a string.</returns>
    public override string ToString() =>
        $"({Color.Red}, {Color.Green}, {Color.Blue})";

}
