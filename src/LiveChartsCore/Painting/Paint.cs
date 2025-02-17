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
using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;

namespace LiveChartsCore.Painting;

/// <summary>
/// Defines a set of geometries that will be drawn according to this instance specifications.
/// </summary>
public abstract class Paint : Animatable, IDisposable
{
    private readonly Dictionary<object, HashSet<IDrawnElement>> _geometriesByCanvas = [];
    private readonly Dictionary<object, LvcRectangle> _clipRectangles = [];
    private readonly FloatMotionProperty _strokeMiterTransition;
    private readonly FloatMotionProperty _strokeWidthTransition;

    /// <summary>
    /// Initializes a new instance of the <see cref="Paint"/> class.
    /// </summary>
    protected Paint(float strokeThickness = 1f, float strokeMiter = 0f)
    {
        _strokeWidthTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeThickness), strokeThickness));
        _strokeMiterTransition = RegisterMotionProperty(new FloatMotionProperty(nameof(StrokeMiter), strokeMiter));
    }

    /// <summary>
    /// Gets or sets the index of the z.
    /// </summary>
    /// <value>
    /// The index of the z.
    /// </value>
    public double ZIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is stroke.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is stroke; otherwise, <c>false</c>.
    /// </value>
    internal PaintStyle PaintStyle { get; set; }

    /// <summary>
    /// Obsolete.
    /// </summary>
    [Obsolete("This is not accesible now at this point, instead set the Fill or Stroke properties of the desired geomemtry.")]
    public bool IsStroke
    {
        get => PaintStyle.HasFlag(PaintStyle.Stroke);
        set => PaintStyle = value ? PaintStyle.Stroke : PaintStyle.Fill;
    }

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    public string? FontFamily { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is antialias.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is antialias; otherwise, <c>false</c>.
    /// </value>
    public bool IsAntialias { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this instance is paused.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is paused; otherwise, <c>false</c>.
    /// </value>
    public bool IsPaused { get; set; }

    /// <summary>
    /// Gets or sets the stroke thickness.
    /// </summary>
    /// <value>
    /// The stroke thickness.
    /// </value>
    public float StrokeThickness
    {
        get => _strokeWidthTransition.GetMovement(this);
        set => _strokeWidthTransition.SetMovement(value, this);
    }

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
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    public bool IsEmpty => _geometriesByCanvas.Count == 0;

    /// <summary>
    /// Gets the geometries.
    /// </summary>
    /// <returns></returns>
    /// <param name="canvas">The canvas.</param>
    public IEnumerable<IDrawnElement> GetGeometries(CoreMotionCanvas canvas)
    {
        var geometries = GetGeometriesByCanvas(canvas) ?? [];

        foreach (var geometry in geometries)
            yield return geometry;
    }

    /// <summary>
    /// Sets the geometries for the given canvas.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="geometries">The geometries.</param>
    public void SetGeometries(CoreMotionCanvas canvas, HashSet<IDrawnElement> geometries)
    {
        _geometriesByCanvas[canvas] = geometries;
        IsValid = false;
    }

    /// <summary>
    /// Adds a geometry to the paint task for the given canvas.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="geometry">The geometry.</param>
    public void AddGeometryToPaintTask(CoreMotionCanvas canvas, IDrawnElement geometry)
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

    /// <summary>
    /// Removes the given geometry from paint task.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="geometry">The geometry.</param>
    public void RemoveGeometryFromPaintTask(CoreMotionCanvas canvas, IDrawnElement geometry)
    {
        _ = GetGeometriesByCanvas(canvas)?.Remove(geometry);

        IsValid = false;
    }

    /// <summary>
    /// Removes all geometry from paint task.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public void ClearGeometriesFromPaintTask(CoreMotionCanvas canvas)
    {
        GetGeometriesByCanvas(canvas)?.Clear();

        IsValid = false;
    }

    /// <summary>
    /// Releases the canvas resources.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    public void ReleaseCanvas(CoreMotionCanvas canvas) =>
        _ = _geometriesByCanvas.Remove(canvas);

    /// <summary>
    /// Gets the clip rectangle for the given canvas.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <returns>The clip rectangle.</returns>
    public LvcRectangle GetClipRectangle(CoreMotionCanvas canvas) =>
        _clipRectangles.TryGetValue(canvas, out var clip) ? clip : LvcRectangle.Empty;

    /// <summary>
    /// Sets the clip rectangle for the given canvas.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="value">The rectangle.</param>
    public void SetClipRectangle(CoreMotionCanvas canvas, LvcRectangle value) =>
        _clipRectangles[canvas] = value;

    /// <summary>
    /// Initializes the task.
    /// </summary>
    /// <param name="drawingContext">The context.</param>
    public abstract void InitializeTask(DrawingContext drawingContext);

    /// <summary>
    /// Sets the opacity according to the given geometry.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="opacity">The opacity.</param>
    public abstract void ApplyOpacityMask(DrawingContext context, float opacity);

    /// <summary>
    /// Resets the opacity.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="opacity">The opacity.</param>
    public abstract void RestoreOpacityMask(DrawingContext context, float opacity);

    /// <summary>
    /// Clones the task.
    /// </summary>
    /// <returns>A new instance with the same properties.</returns>
    public abstract Paint CloneTask();

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public abstract void Dispose();

    private HashSet<IDrawnElement>? GetGeometriesByCanvas(object canvas)
    {
        return _geometriesByCanvas.TryGetValue(canvas, out var geometries)
            ? geometries
            : null;
    }
}
