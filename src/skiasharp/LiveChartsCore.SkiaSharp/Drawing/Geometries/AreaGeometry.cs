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
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines an area geometry.
/// </summary>
public class AreaGeometry : Drawable, ICubicBezierAreaGeometry<SkiaSharpDrawingContext>
{
    private readonly LinkedList<CubicBezier> _commands = new();

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.FirstCommand" />
    public LinkedListNode<CubicBezier>? FirstCommand => _commands.First;

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.LastCommand" />
    public LinkedListNode<CubicBezier>? LastCommand => _commands.Last;

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.CountCommands" />
    public int CountCommands => _commands.Count;

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.IsClosed" />
    public bool IsClosed { get; set; }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.Pivot" />
    public float Pivot { get; set; }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void Draw(SkiaSharpDrawingContext context)
    {
        if (_commands.Count == 0) return;

        var toRemoveSegments = new List<CubicBezier>();

        using var path = new SKPath();
        var isValid = true;

        var currentTime = CurrentTime;
        var isFirst = true;

        foreach (var segment in _commands)
        {
            segment.IsValid = true;
            segment.CurrentTime = currentTime;

            if (isFirst)
            {
                isFirst = false;
            }

            path.CubicTo(segment.X0, segment.Y0, segment.X1, segment.Y1, segment.X2, segment.Y2);
            isValid = isValid && segment.IsValid;

            if (segment.IsValid && segment.RemoveOnCompleted) toRemoveSegments.Add(segment);
        }

        foreach (var segment in toRemoveSegments)
        {
            _ = _commands.Remove(segment);
            isValid = false;
        }

        if (IsClosed) path.Close();
        context.Canvas.DrawPath(path, context.Paint);

        if (!isValid) SetInvalidState();
    }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.AddLast(CubicBezier)" />
    public LinkedListNode<CubicBezier> AddLast(CubicBezier command)
    {
        SetInvalidState();
        return _commands.AddLast(command);
    }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.AddFirst(CubicBezier)" />
    public LinkedListNode<CubicBezier> AddFirst(CubicBezier command)
    {
        SetInvalidState();
        return _commands.AddFirst(command);
    }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.AddAfter(LinkedListNode{CubicBezier}, CubicBezier)" />
    public LinkedListNode<CubicBezier> AddAfter(LinkedListNode<CubicBezier> node, CubicBezier command)
    {
        SetInvalidState();
        return _commands.AddAfter(node, command);
    }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.AddBefore(LinkedListNode{CubicBezier}, CubicBezier)" />
    public LinkedListNode<CubicBezier> AddBefore(LinkedListNode<CubicBezier> node, CubicBezier command)
    {
        SetInvalidState();
        return _commands.AddBefore(node, command);
    }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.ContainsCommand(CubicBezier)" />
    public bool ContainsCommand(CubicBezier segment)
    {
        return _commands.Contains(segment);
    }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.RemoveCommand(CubicBezier)" />
    public bool RemoveCommand(CubicBezier command)
    {
        SetInvalidState();
        return _commands.Remove(command);
    }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.RemoveCommand(LinkedListNode{CubicBezier})" />
    public void RemoveCommand(LinkedListNode<CubicBezier> node)
    {
        SetInvalidState();
        _commands.Remove(node);
    }

    /// <inheritdoc cref="ICubicBezierAreaGeometry{TDrawingContext}.ClearCommands" />
    public void ClearCommands()
    {
        _commands.Clear();
    }

    /// <inheritdoc cref="IAnimatable.CompleteAllTransitions" />
    public override void CompleteAllTransitions()
    {
        foreach (var segment in _commands)
        {
            segment.CompleteAllTransitions();
        }

        base.CompleteAllTransitions();
    }
}
