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
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.Painting.ImageFilters;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Painting;

/// <inheritdoc cref="IPaint{TDrawingContext}" />
public abstract class Paint : Animatable, IDisposable, IPaint
{
    private readonly FloatMotionProperty _strokeMiterTransition;
    private readonly Dictionary<CoreMotionCanvas, HashSet<IDrawable<SkiaSharpDrawingContext>>> _geometriesByCanvas = [];
    private readonly Dictionary<CoreMotionCanvas, LvcRectangle> _clipRectangles = [];
    internal SKPaint? _skiaPaint;
    internal FloatMotionProperty _strokeWidthTransition;

    /// <summary>
    /// Initializes a new instance of the <see cref="Paint"/> class.
    /// </summary>
    protected Paint()
    {
        _strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), 1f));
        _strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), 0f));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Paint"/> class.
    /// </summary>
    /// <param name="color">The color.</param>
    protected Paint(SKColor color) : this()
    {
        Color = color;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.ZIndex"/>
    public double ZIndex { get; set; }

    /// <inheritdoc cref="IPaint{TDrawingContext}.StrokeThickness" />
    public float StrokeThickness { get => _strokeWidthTransition.GetMovement(this); set => _strokeWidthTransition.SetMovement(value, this); }

    /// <summary>
    /// Gets or sets the style.
    /// </summary>
    /// <value>
    /// The style.
    /// </value>
    public SKPaintStyle Style { get; set; }

    /// <inheritdoc cref="IPaint{TDrawingContext}.IsStroke" />
    public bool IsStroke { get; set; }

    /// <inheritdoc cref="IPaint{TDrawingContext}.IsFill" />
    public bool IsFill { get; set; }

    /// <inheritdoc cref="IPaint{TDrawingContext}.FontFamily" />
    public string? FontFamily { get; set; }

    /// <summary>
    /// Gets or sets the font style.
    /// </summary>
    public SKFontStyle? SKFontStyle { get; set; }

    /// <summary>
    /// Gets or sets the SKTypeface, if null, LiveCharts will build one based on the
    /// <see cref="FontFamily"/> and <see cref="SKFontStyle"/> properties. Default is null.
    /// </summary>
    public SKTypeface? SKTypeface { get; set; }

    /// <summary>
    /// Gets a value indication whether the paint has a custom font.
    /// </summary>
    public bool HasCustomFont =>
        LiveChartsSkiaSharp.DefaultSKTypeface is not null ||
        FontFamily is not null || SKTypeface is not null || SKFontStyle is not null;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is antialias.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is antialias; otherwise, <c>false</c>.
    /// </value>
    public bool IsAntialias { get; set; } = true;

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
    /// Gets or sets the stroke miter.
    /// </summary>
    /// <value>
    /// The stroke miter.
    /// </value>
    public float StrokeMiter
    {
        get => _strokeMiterTransition.GetMovement(this);
        set => _strokeMiterTransition.SetMovement(value, this);
    }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    public SKColor Color { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is paused.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is paused; otherwise, <c>false</c>.
    /// </value>
    public bool IsPaused { get; set; }

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

    /// <inheritdoc cref="IPaint{TDrawingContext}.InitializeTask(TDrawingContext)" />
    public abstract void InitializeTask(SkiaSharpDrawingContext drawingContext);

    /// <inheritdoc cref="IPaint{TDrawingContext}.GetGeometries(CoreMotionCanvas{TDrawingContext})" />
    public IEnumerable<IDrawable<SkiaSharpDrawingContext>> GetGeometries(CoreMotionCanvas canvas)
    {
        var enumerable = GetGeometriesByCanvas(canvas) ?? Enumerable.Empty<IDrawable<SkiaSharpDrawingContext>>();
        foreach (var item in enumerable)
        {
            yield return item;
        }
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.SetGeometries(CoreMotionCanvas{TDrawingContext}, HashSet{IDrawable{TDrawingContext}})" />
    public void SetGeometries(CoreMotionCanvas canvas, HashSet<IDrawable<SkiaSharpDrawingContext>> geometries)
    {
        _geometriesByCanvas[canvas] = geometries;
        IsValid = false;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.AddGeometryToPaintTask(CoreMotionCanvas{TDrawingContext}, IDrawable{TDrawingContext})" />
    public void AddGeometryToPaintTask(CoreMotionCanvas canvas, IDrawable<SkiaSharpDrawingContext> geometry)
    {
        var g = GetGeometriesByCanvas(canvas);
        if (g is null)
        {
            g = [];
            _geometriesByCanvas[canvas] = g;
        }
        _ = g.Add(geometry);
        IsValid = false;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.RemoveGeometryFromPainTask(CoreMotionCanvas{TDrawingContext}, IDrawable{TDrawingContext})" />
    public void RemoveGeometryFromPainTask(CoreMotionCanvas canvas, IDrawable<SkiaSharpDrawingContext> geometry)
    {
        _ = GetGeometriesByCanvas(canvas)?.Remove(geometry);
        IsValid = false;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.ClearGeometriesFromPaintTask(CoreMotionCanvas{TDrawingContext})"/>
    public void ClearGeometriesFromPaintTask(CoreMotionCanvas canvas)
    {
        GetGeometriesByCanvas(canvas)?.Clear();
        IsValid = false;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.ReleaseCanvas(CoreMotionCanvas{TDrawingContext})"/>
    public void ReleaseCanvas(CoreMotionCanvas canvas)
    {
        _ = _geometriesByCanvas.Remove(canvas);
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.GetClipRectangle(CoreMotionCanvas{TDrawingContext})" />
    public LvcRectangle GetClipRectangle(CoreMotionCanvas canvas)
    {
        return _clipRectangles.TryGetValue(canvas, out var clip) ? clip : LvcRectangle.Empty;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.SetClipRectangle(CoreMotionCanvas{TDrawingContext}, LvcRectangle)" />
    public void SetClipRectangle(CoreMotionCanvas canvas, LvcRectangle value)
    {
        _clipRectangles[canvas] = value;
    }

    /// <inheritdoc cref="IPaint{TDrawingContext}.CloneTask" />
    public abstract IPaint CloneTask();

    /// <inheritdoc cref="IPaint{TDrawingContext}.ApplyOpacityMask(TDrawingContext, IPaintable{TDrawingContext})" />
    public abstract void ApplyOpacityMask(SkiaSharpDrawingContext context, IPaintable<SkiaSharpDrawingContext> geometry);

    /// <inheritdoc cref="IPaint{TDrawingContext}.ApplyOpacityMask(TDrawingContext, IPaintable{TDrawingContext})" />
    public abstract void RestoreOpacityMask(SkiaSharpDrawingContext context, IPaintable<SkiaSharpDrawingContext> geometry);

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public virtual void Dispose()
    {
        _skiaPaint?.Dispose();
        _skiaPaint = null;
    }

    /// <summary>
    /// Gets a <see cref="SKTypeface"/> from the <see cref="FontFamily"/> property.
    /// </summary>
    /// <returns></returns>
    protected internal SKTypeface GetSKTypeface()
    {
        // return the defined typeface.
        if (SKTypeface is not null) return SKTypeface;

        // create one from the font family.
        if (FontFamily is not null) return SKTypeface.FromFamilyName(FontFamily, SKFontStyle ?? new SKFontStyle());

        // other wise ose the globally defined typeface.
        return LiveChartsSkiaSharp.DefaultSKTypeface ?? SKTypeface.Default;
    }

    private HashSet<IDrawable<SkiaSharpDrawingContext>>? GetGeometriesByCanvas(CoreMotionCanvas canvas)
    {
        return _geometriesByCanvas.TryGetValue(canvas, out var geometries)
            ? geometries
            : null;
    }
}
