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
using LiveChartsCore.Drawing.Segments;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines an area geometry.
/// </summary>
/// <typeparam name="TSegment">The type of the segment.</typeparam>
public abstract class VectorGeometry<TSegment> : CoreVectorGeometry<TSegment>, ISkiaGeometry
    where TSegment : Segment
{
    /// <inheritdoc cref="IDrawable{TDrawingContext}.Children" />
    public IDrawable<SkiaSharpDrawingContext>[] Children { get; set; } = [];

    /// <summary>
    /// Called when the area begins the draw.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="path">The path.</param>
    /// <param name="segment">The segment.</param>
    protected virtual void OnOpen(SkiaSharpDrawingContext context, SKPath path, TSegment segment)
    { }

    /// <summary>
    /// Called to close the area.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="path">The path.</param>
    /// <param name="segment">The segment.</param>
    protected virtual void OnClose(SkiaSharpDrawingContext context, SKPath path, TSegment segment)
    { }

    /// <summary>
    /// Called to draw the segment.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="path">The path.</param>
    /// <param name="segment">The segment.</param>
    protected virtual void OnDrawSegment(SkiaSharpDrawingContext context, SKPath path, TSegment segment)
    { }

    /// <inheritdoc cref="IDrawable{TDrawingContext}.Draw(TDrawingContext)" />
    public void Draw(SkiaSharpDrawingContext context) => OnDraw(context, context.Paint);

    /// <inheritdoc cref="ISkiaGeometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        if (Commands.Count == 0) return;

        var toRemoveSegments = new List<TSegment>();

        using var path = new SKPath();
        var isValid = true;

        var currentTime = CurrentTime;
        var isFirst = true;
        TSegment? last = null;

        foreach (var segment in Commands)
        {
            segment.IsValid = true;
            segment.CurrentTime = currentTime;

            if (isFirst)
            {
                isFirst = false;
                OnOpen(context, path, segment);
            }

            OnDrawSegment(context, path, segment);
            isValid = isValid && segment.IsValid;

            if (segment.IsValid && segment.RemoveOnCompleted) toRemoveSegments.Add(segment);
            last = segment;
        }

        foreach (var segment in toRemoveSegments)
        {
            _ = Commands.Remove(segment);
            isValid = false;
        }

        if (last is not null) OnClose(context, path, last);

        var hasGeometryOpacity = Opacity < 1;

        if (Fill is null && Stroke is null)
        {
            if (hasGeometryOpacity) context.PaintTask.ApplyOpacityMask(context, this);
            context.Canvas.DrawPath(path, context.Paint);
            if (hasGeometryOpacity) context.PaintTask.RestoreOpacityMask(context, this);
        }
        else if (Fill is not null)
        {
            var originalPaint = context.Paint;
            var originalTask = context.PaintTask;

            Fill.IsStroke = false;
            Fill.InitializeTask(context);

            if (hasGeometryOpacity) Fill.ApplyOpacityMask(context, this);
            context.Canvas.DrawPath(path, context.Paint);
            if (hasGeometryOpacity) Fill.RestoreOpacityMask(context, this);

            Fill.Dispose();

            context.Paint = originalPaint;
            context.PaintTask = originalTask;
        }
        else
        {
            throw new NotImplementedException("Fill and Stroke per vector is experiental.");
        }

        if (!isValid) IsValid = false;
    }
}
