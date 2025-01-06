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
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

// LEGACY OBJECTS, WILL BE REMOVED IN FUTURE VERSIONS

/// <summary>
/// Defines a geometry with a size.
/// </summary>
[Obsolete($"Renamed to {nameof(BoundedDrawnGeometry)}")]
public abstract class SizedGeometry : BoundedDrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)"/>
    public virtual void Draw(SkiaSharpDrawingContext context) =>
        OnDraw(context, context.ActiveSkiaPaint);

    /// <summary>
    /// Legacy method, will be removed in future versions.
    /// </summary>
    [Obsolete($"Use the {nameof(Draw)} method instead.")]
    public abstract void OnDraw(SkiaSharpDrawingContext context, SKPaint paint);
}

/// <summary>
/// Obsolete.
/// </summary>
[Obsolete($"Renamed to {nameof(DrawnGeometry)}")]
public abstract class Geometry : DrawnGeometry, IDrawnElement<SkiaSharpDrawingContext>
{
    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)"/>
    public virtual void Draw(SkiaSharpDrawingContext context) =>
        OnDraw(context, context.ActiveSkiaPaint);

    /// <summary>
    /// Legacy method, will be removed in future versions.
    /// </summary>
    [Obsolete($"Use the {nameof(Draw)} method instead.")]
    public abstract void OnDraw(SkiaSharpDrawingContext context, SKPaint paint);

    /// <inheritdoc cref="DrawnGeometry.Measure()" />
    public override LvcSize Measure() =>
        new(0, 0);

    /// <summary>
    /// Legacy method, will be removed in future versions.
    /// </summary>
    /// <param name="paintTasks"></param>
    /// <returns></returns>
    protected virtual LvcSize OnMeasure(Paint paintTasks) => Measure();
}
