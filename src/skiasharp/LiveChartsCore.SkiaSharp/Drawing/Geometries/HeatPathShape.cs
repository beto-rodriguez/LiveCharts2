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
using LiveChartsCore.Geo;
using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines a path geometry with a specified color.
/// </summary>
/// <seealso cref="PathGeometry" />
public class HeatPathShape : PathGeometry, IHeatPathShape
{
    private readonly ColorMotionProperty _fillProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeatPathShape"/> class.
    /// </summary>
    public HeatPathShape() : base()
    {
        _fillProperty = RegisterMotionProperty(new ColorMotionProperty(nameof(FillColor), LvcColor.Empty));
    }

    /// <summary>
    /// Gets or sets the color of the fill.
    /// </summary>
    /// <value>
    /// The color of the fill.
    /// </value>
    public LvcColor FillColor
    {
        get => _fillProperty.GetMovement(this);
        set => _fillProperty.SetMovement(value, this);
    }

    /// <inheritdoc cref="PathGeometry.Draw(SkiaSharpDrawingContext)"/>
    public override void Draw(SkiaSharpDrawingContext context)
    {
        if (_commands.Count == 0) return;

        var toRemoveSegments = new List<IPathCommand<SKPath>>();

        using var path = new SKPath();
        var isValid = true;

        foreach (var segment in _commands)
        {
            segment.IsValid = true;
            segment.Execute(path, CurrentTime, this);
            isValid = isValid && segment.IsValid;

            if (segment.IsValid && segment.RemoveOnCompleted) toRemoveSegments.Add(segment);
        }

        foreach (var segment in toRemoveSegments)
        {
            _ = _commands.Remove(segment);
            isValid = false;
        }

        if (IsClosed) path.Close();

        var originalColor = context.Paint.Color;
        var originalStyle = context.Paint.Style;

        var fill = FillColor;

        if (fill != LvcColor.Empty)
        {
            context.Paint.Color = fill.AsSKColor();
            context.Paint.Style = SKPaintStyle.Fill;
        }

        context.Canvas.DrawPath(path, context.Paint);

        if (fill != LvcColor.Empty)
        {
            context.Paint.Color = originalColor;
            context.Paint.Style = originalStyle;
        }

        if (!isValid) IsValid = false;
    }

    /// <inheritdoc cref="IAnimatable.CompleteTransition(string[])" />
    public override void CompleteTransition(params string[]? propertyName)
    {
        foreach (var item in _commands)
        {
            item.CompleteTransition(propertyName);
        }

        base.CompleteTransition(propertyName);
    }
}

/// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}" />
[Obsolete]
public class PathGeometry : Drawable, IPathGeometry<SkiaSharpDrawingContext, SKPath>
{
    /// <summary>
    /// The commands
    /// </summary>
    protected readonly LinkedList<IPathCommand<SKPath>> _commands = new();

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.FirstCommand" />
    public LinkedListNode<IPathCommand<SKPath>>? FirstCommand => _commands.First;

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.LastCommand" />
    public LinkedListNode<IPathCommand<SKPath>>? LastCommand => _commands.Last;

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.CountCommands" />
    public int CountCommands => _commands.Count;

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.IsClosed" />
    public bool IsClosed { get; set; }

    /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public override void Draw(SkiaSharpDrawingContext context)
    {
        if (_commands.Count == 0) return;

        var toRemoveSegments = new List<IPathCommand<SKPath>>();

        using var path = new SKPath();
        var isValid = true;

        foreach (var segment in _commands)
        {
            segment.IsValid = true;
            segment.Execute(path, CurrentTime, this);
            isValid = isValid && segment.IsValid;

            if (segment.IsValid && segment.RemoveOnCompleted) toRemoveSegments.Add(segment);
        }

        foreach (var segment in toRemoveSegments)
        {
            _ = _commands.Remove(segment);
            isValid = false;
        }

        if (IsClosed)
            path.Close();

        context.Canvas.DrawPath(path, context.Paint);

        if (!isValid) IsValid = false;
    }

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.AddLast(IPathCommand{TPathArgs})" />
    public LinkedListNode<IPathCommand<SKPath>> AddLast(IPathCommand<SKPath> command)
    {
        IsValid = false;
        return _commands.AddLast(command);
    }

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.AddFirst(IPathCommand{TPathArgs})" />
    public LinkedListNode<IPathCommand<SKPath>> AddFirst(IPathCommand<SKPath> command)
    {
        IsValid = false;
        return _commands.AddFirst(command);
    }

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.AddAfter(LinkedListNode{IPathCommand{TPathArgs}}, IPathCommand{TPathArgs})" />
    public LinkedListNode<IPathCommand<SKPath>> AddAfter(LinkedListNode<IPathCommand<SKPath>> node, IPathCommand<SKPath> command)
    {
        IsValid = false;
        return _commands.AddAfter(node, command);
    }

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.AddBefore(LinkedListNode{IPathCommand{TPathArgs}}, IPathCommand{TPathArgs})" />
    public LinkedListNode<IPathCommand<SKPath>> AddBefore(LinkedListNode<IPathCommand<SKPath>> node, IPathCommand<SKPath> command)
    {
        IsValid = false;
        return _commands.AddBefore(node, command);
    }

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.ContainsCommand(IPathCommand{TPathArgs})" />
    public bool ContainsCommand(IPathCommand<SKPath> segment)
    {
        return _commands.Contains(segment);
    }

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.RemoveCommand(IPathCommand{TPathArgs})" />
    public bool RemoveCommand(IPathCommand<SKPath> command)
    {
        IsValid = false;
        return _commands.Remove(command);
    }

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.RemoveCommand(LinkedListNode{IPathCommand{TPathArgs}})" />
    public void RemoveCommand(LinkedListNode<IPathCommand<SKPath>> node)
    {
        IsValid = false;
        _commands.Remove(node);
    }

    /// <inheritdoc cref="IPathGeometry{TDrawingContext, TPathArgs}.ClearCommands" />
    public void ClearCommands()
    {
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
}
