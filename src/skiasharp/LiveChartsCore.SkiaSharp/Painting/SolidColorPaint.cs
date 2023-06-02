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
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting;

/// <summary>
/// Defines a set of geometries that will be painted using a solid color.
/// </summary>
/// <seealso cref="Paint" />
public class SolidColorPaint : Paint
{
    private SkiaSharpDrawingContext? _drawingContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SolidColorPaint"/> class.
    /// </summary>
    public SolidColorPaint()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolidColorPaint"/> class.
    /// </summary>
    /// <param name="color">The color.</param>
    public SolidColorPaint(SKColor color)
        : base(color)
    {
        Color = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SolidColorPaint"/> class.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="strokeWidth">Width of the stroke.</param>
    public SolidColorPaint(SKColor color, float strokeWidth)
        : base(color)
    {
        _strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), strokeWidth));
        Color = color;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.CloneTask" />
    public override IPaint<SkiaSharpDrawingContext> CloneTask()
    {
        var clone = new SolidColorPaint
        {
            Style = Style,
            IsStroke = IsStroke,
            IsFill = IsFill,
            Color = Color,
            IsAntialias = IsAntialias,
            StrokeThickness = StrokeThickness,
            StrokeCap = StrokeCap,
            StrokeJoin = StrokeJoin,
            StrokeMiter = StrokeMiter,
            FontFamily = FontFamily,
            SKFontStyle = SKFontStyle,
            SKTypeface = SKTypeface,
            PathEffect = PathEffect?.Clone(),
            ImageFilter = ImageFilter?.Clone()
        };

        return clone;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.InitializeTask(TDrawingContext)" />
    public override void InitializeTask(SkiaSharpDrawingContext drawingContext)
    {
        _skiaPaint ??= new SKPaint();

        _skiaPaint.Color = Color;
        _skiaPaint.IsAntialias = IsAntialias;
        _skiaPaint.IsStroke = IsStroke;
        _skiaPaint.StrokeCap = StrokeCap;
        _skiaPaint.StrokeJoin = StrokeJoin;
        _skiaPaint.StrokeMiter = StrokeMiter;
        _skiaPaint.StrokeWidth = StrokeThickness;
        _skiaPaint.Style = IsStroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill;

        if (HasCustomFont) _skiaPaint.Typeface = GetSKTypeface();

        if (PathEffect is not null)
        {
            PathEffect.CreateEffect(drawingContext);
            _skiaPaint.PathEffect = PathEffect.SKPathEffect;
        }

        if (ImageFilter is not null)
        {
            ImageFilter.CreateFilter(drawingContext);
            _skiaPaint.ImageFilter = ImageFilter.SKImageFilter;
        }

        var clip = GetClipRectangle(drawingContext.MotionCanvas);
        if (clip != LvcRectangle.Empty)
        {
            _ = drawingContext.Canvas.Save();
            drawingContext.Canvas.ClipRect(new SKRect(clip.X, clip.Y, clip.X + clip.Width, clip.Y + clip.Height));
            _drawingContext = drawingContext;
        }

        drawingContext.Paint = _skiaPaint;
        drawingContext.PaintTask = this;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.ApplyOpacityMask(TDrawingContext, IPaintable{TDrawingContext})" />
    public override void ApplyOpacityMask(SkiaSharpDrawingContext context, IPaintable<SkiaSharpDrawingContext> geometry)
    {
        if (context.PaintTask is null || context.Paint is null) return;

        var baseColor = context.PaintTask.Color;
        context.Paint.Color =
            new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, (byte)(255 * geometry.Opacity));
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.RestoreOpacityMask(TDrawingContext, IPaintable{TDrawingContext})" />
    public override void RestoreOpacityMask(SkiaSharpDrawingContext context, IPaintable<SkiaSharpDrawingContext> geometry)
    {
        if (context.PaintTask is null || context.Paint is null) return;

        var baseColor = context.PaintTask.Color;
        context.Paint.Color = baseColor;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public override void Dispose()
    {
        // Note #301222
        // Disposing typefaces could cause render issues.
        // Does this causes memory leaks?
        // Should the user dispose typefaces manually?
        //if (HasCustomFont && _skiaPaint != null) _skiaPaint.Typeface.Dispose();
        PathEffect?.Dispose();
        ImageFilter?.Dispose();

        if (_drawingContext is not null && GetClipRectangle(_drawingContext.MotionCanvas) != LvcRectangle.Empty)
        {
            _drawingContext.Canvas.Restore();
            _drawingContext = null;
        }

        base.Dispose();
    }
}
