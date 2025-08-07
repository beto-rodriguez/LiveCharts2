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

namespace LiveChartsCore.Painting;

/// <summary>
/// Defines a task that will draw a set of geometries.
/// </summary>
public class DrawnTask : Paint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrawnTask"/> class.
    /// </summary>
    /// <param name="canvas">The canvas.</param>
    /// <param name="drawables">The drawables.</param>
    public DrawnTask(CoreMotionCanvas canvas, params IDrawnElement[] drawables)
    {
        foreach (var drawable in drawables)
            AddGeometryToPaintTask(canvas, drawable);
    }

    /// <inheritdoc cref="Paint.CloneTask" />
    public override Paint CloneTask() => throw new NotImplementedException();

    internal override void ApplyOpacityMask(DrawingContext context, float opacity) { }

    internal override void OnPaintFinished(DrawingContext context) { }

    internal override void OnPaintStarted(DrawingContext drawingContext) =>
        // lets clear the active paint task, this will force the drawables to use their own paint tasks.
        drawingContext.ActiveLvcPaint = null;

    internal override void RestoreOpacityMask(DrawingContext context, float opacity) { }

    internal override Paint Transitionate(float progress, Paint target) => this;

    internal override void DisposeTask() { }
}
