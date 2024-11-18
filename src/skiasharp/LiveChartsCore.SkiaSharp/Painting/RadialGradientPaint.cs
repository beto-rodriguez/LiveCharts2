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
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting;

/// <summary>
/// Defines a set of geometries that will be painted using a radial gradient shader.
/// </summary>
/// <seealso cref="SkiaPaint" />
public class RadialGradientPaint : SkiaPaint
{
    private SkiaSharpDrawingContext? _drawingContext;
    private SKPaint? _skiaPaint;
    private readonly SKColor[] _gradientStops;
    private readonly SKPoint _center;
    private readonly float _radius;
    private readonly float[]? _colorPos;
    private readonly SKShaderTileMode _tileMode;

    /// <summary>
    /// Initializes a new instance of the <see cref="RadialGradientPaint"/> class.
    /// </summary>
    /// <param name="gradientStops">The gradient stops.</param>
    /// <param name="center">
    /// The center point of the gradient, both X and Y in the range of 0 to 1, where 0 is the start of the axis and 1 the end,
    /// default is (0.5, 0.5).
    /// </param>
    /// <param name="radius">
    /// The radius, in the range of 0 to 1, where 1 is the minimum of both Width and Height of the chart, default is 0.5.
    /// </param>
    /// <param name="colorPos">
    /// An array of integers in the range of 0 to 1.
    /// These integers indicate the relative positions of the colors, You can set that argument to null to equally
    /// space the colors, default is null.
    /// </param>
    /// <param name="tileMode">
    /// The shader tile mode, default is <see cref="SKShaderTileMode.Repeat"/>.
    /// </param>
    public RadialGradientPaint(
        SKColor[] gradientStops,
        SKPoint? center = null,
        float radius = 0.5f,
        float[]? colorPos = null,
        SKShaderTileMode tileMode = SKShaderTileMode.Repeat)
    {
        _gradientStops = gradientStops;
        if (center is null) _center = new SKPoint(0.5f, 0.5f);
        _radius = radius;
        _colorPos = colorPos;
        _tileMode = tileMode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RadialGradientPaint"/> class.
    /// </summary>
    /// <param name="centerColor">Color of the center.</param>
    /// <param name="outerColor">Color of the outer.</param>
    public RadialGradientPaint(SKColor centerColor, SKColor outerColor)
        : this([centerColor, outerColor]) { }

    /// <inheritdoc cref="Paint.CloneTask" />
    public override Paint CloneTask()
    {
        return new RadialGradientPaint(_gradientStops, _center, _radius, _colorPos, _tileMode)
        {
            Style = Style,
            IsStroke = IsStroke,
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
    }

    /// <inheritdoc cref="Paint.InitializeTask(DrawingContext)" />
    public override void InitializeTask(DrawingContext drawingContext)
    {
        var skiaContext = (SkiaSharpDrawingContext)drawingContext;
        _skiaPaint ??= new SKPaint();

        var size = GetDrawRectangleSize(skiaContext);
        var center = new SKPoint(size.Location.X + _center.X * size.Width, size.Location.Y + _center.Y * size.Height);
        var r = size.Location.X + size.Width > size.Location.Y + size.Height
            ? size.Location.Y + size.Height
            : size.Location.X + size.Width;
        r *= _radius;

        _skiaPaint.Shader = SKShader.CreateRadialGradient(
                center,
                r,
                _gradientStops,
                _colorPos,
                _tileMode);

        _skiaPaint.IsAntialias = IsAntialias;
        _skiaPaint.IsStroke = true;
        _skiaPaint.StrokeWidth = StrokeThickness;
        _skiaPaint.StrokeCap = StrokeCap;
        _skiaPaint.StrokeJoin = StrokeJoin;
        _skiaPaint.StrokeMiter = StrokeMiter;
        _skiaPaint.Style = IsStroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill;

        if (HasCustomFont) _skiaPaint.Typeface = GetSKTypeface();

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
            _drawingContext = skiaContext;
        }

        skiaContext.Paint = _skiaPaint;
        skiaContext.PaintTask = this;
    }

    /// <inheritdoc cref="Paint.ApplyOpacityMask(DrawingContext, IDrawable)" />
    public override void ApplyOpacityMask(DrawingContext context, IDrawable geometry)
    {
        var skiaContext = (SkiaSharpDrawingContext)context;
        if (_skiaPaint is null) return;

        var size = GetDrawRectangleSize(skiaContext);
        var center = new SKPoint(size.Location.X + _center.X * size.Width, size.Location.Y + _center.Y * size.Height);
        var r = size.Location.X + size.Width > size.Location.Y + size.Height
            ? size.Location.Y + size.Height
            : size.Location.X + size.Width;
        r *= _radius;

        _skiaPaint.Shader = SKShader.CreateRadialGradient(
                center,
                r,
                _gradientStops.Select(x => new SKColor(x.Red, x.Green, x.Blue, (byte)(255 * geometry.Opacity))).ToArray(),
                _colorPos,
                _tileMode);
    }

    /// <inheritdoc cref="Paint.RestoreOpacityMask(DrawingContext, IDrawable)" />
    public override void RestoreOpacityMask(DrawingContext context, IDrawable geometry)
    {
        var skiaContext = (SkiaSharpDrawingContext)context;
        if (_skiaPaint is null) return;

        var size = GetDrawRectangleSize(skiaContext);
        var center = new SKPoint(size.Location.X + _center.X * size.Width, size.Location.Y + _center.Y * size.Height);
        var r = size.Location.X + size.Width > size.Location.Y + size.Height
            ? size.Location.Y + size.Height
            : size.Location.X + size.Width;
        r *= _radius;

        _skiaPaint.Shader = SKShader.CreateRadialGradient(
                center,
                r,
                _gradientStops,
                _colorPos,
                _tileMode);
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

        _skiaPaint?.Dispose();
        _skiaPaint = null;

        GC.SuppressFinalize(this);
    }

    private static SKRect GetDrawRectangleSize(SkiaSharpDrawingContext drawingContext) =>
        new(0, 0, drawingContext.Info.Width, drawingContext.Info.Width);
}
