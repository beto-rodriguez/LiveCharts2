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

using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Segments;
using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines an area geometry.
/// </summary>
/// <typeparam name="TSegment">The type of the segment.</typeparam>
public abstract class VectorGeometry<TSegment> : Drawable, IVectorGeometry<TSegment, SkiaSharpDrawingContext>
    where TSegment : class, IAnimatable, IConsecutivePathSegment
{
    private readonly LinkedList<TSegment> _commands = new();
    private readonly FloatMotionProperty _pivotProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="VectorGeometry{TSegment}"/> class.
    /// </summary>
    public VectorGeometry()
    {
        _pivotProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(Pivot), 0f));
    }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.FirstCommand" />
    public LinkedListNode<TSegment>? FirstCommand => _commands.First;

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.LastCommand" />
    public LinkedListNode<TSegment>? LastCommand => _commands.Last;

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.CountCommands" />
    public int CountCommands => _commands.Count;

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.ClosingMethod" />
    public VectorClosingMethod ClosingMethod { get; set; }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.Pivot" />
    public float Pivot { get => _pivotProperty.GetMovement(this); set => _pivotProperty.SetMovement(value, this); }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.AddLast(TSegment)" />
    public LinkedListNode<TSegment> AddLast(TSegment command)
    {
        IsValid = false;
        return _commands.AddLast(command);
    }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.AddFirst(TSegment)" />
    public LinkedListNode<TSegment> AddFirst(TSegment command)
    {
        IsValid = false;
        return _commands.AddFirst(command);
    }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.AddAfter(LinkedListNode{TSegment}, TSegment)" />
    public LinkedListNode<TSegment> AddAfter(LinkedListNode<TSegment> node, TSegment command)
    {
        IsValid = false;
        return _commands.AddAfter(node, command);
    }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.AddBefore(LinkedListNode{TSegment}, TSegment)" />
    public LinkedListNode<TSegment> AddBefore(LinkedListNode<TSegment> node, TSegment command)
    {
        IsValid = false;
        return _commands.AddBefore(node, command);
    }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.ContainsCommand(TSegment)" />
    public bool ContainsCommand(TSegment segment)
    {
        return _commands.Contains(segment);
    }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.RemoveCommand(TSegment)" />
    public bool RemoveCommand(TSegment command)
    {
        IsValid = false;
        return _commands.Remove(command);
    }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.RemoveCommand(LinkedListNode{TSegment})" />
    public void RemoveCommand(LinkedListNode<TSegment> node)
    {
        IsValid = false;
        _commands.Remove(node);
    }

    /// <inheritdoc cref="IVectorGeometry{TSegment, TDrawingContext}.ClearCommands" />
    public void ClearCommands()
    {
        IsValid = false;
        _commands.Clear();
    }

    /// <inheritdoc cref="IAnimatable.CompleteTransition(string[])" />
    public override void CompleteTransition(params string[]? propertyName)
    {
        foreach (var segment in _commands)
        {
            segment.CompleteTransition(propertyName);
        }

        base.CompleteTransition(propertyName);
    }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void Draw(SkiaSharpDrawingContext context)
    {
        if (_commands.Count == 0) return;

        var toRemoveSegments = new List<TSegment>();

        using var path = new SKPath();
        var isValid = true;

        var currentTime = CurrentTime;
        var isFirst = true;
        TSegment? last = null;

        foreach (var segment in _commands)
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
            _ = _commands.Remove(segment);
            isValid = false;
        }

        if (last is not null) OnClose(context, path, last);

        context.Canvas.DrawPath(path, context.Paint);

        if (!isValid) IsValid = false;
    }

    /// <summary>
    /// Called when the area begins the draw.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="path">The path.</param>
    /// <param name="segment">The segment.</param>
    protected abstract void OnOpen(SkiaSharpDrawingContext context, SKPath path, TSegment segment);

    /// <summary>
    /// Called to close the area.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="path">The path.</param>
    /// <param name="segment">The segment.</param>
    protected abstract void OnClose(SkiaSharpDrawingContext context, SKPath path, TSegment segment);

    /// <summary>
    /// Called to draw the segment.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="path">The path.</param>
    /// <param name="segment">The segment.</param>
    protected abstract void OnDrawSegment(SkiaSharpDrawingContext context, SKPath path, TSegment segment);
}
