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
using LiveChartsCore.Measure;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines the stack panel class.
/// </summary>
public class StackPanel<TDrawingContext> : VisualElement<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private LvcPoint _position;

    /// <summary>
    /// Gets the children collection in the <see cref="StackPanel{TDrawingContext}"/>.
    /// </summary>
    public HashSet<VisualElement<TDrawingContext>> Children { get; } = new();

    /// <summary>
    /// Gets or sets the panel orientation.
    /// </summary>
    public ContainerOrientation Orientation { get; set; }

    /// <summary>
    /// Gets or sets the vertical alignment.
    /// </summary>
    public Align VerticalAlignment { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align HorizontalAlignment { get; set; } = Align.Middle;

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualLocation"/>
    public override LvcPoint GetActualLocation()
    {
        return _position;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualSize"/>
    public override LvcSize GetActualSize()
    {
        return Orientation == ContainerOrientation.Horizontal
            ? Children.Aggregate(new LvcSize(), (current, next) =>
            {
                var size = next.GetActualSize();

                return new LvcSize(
                    current.Width + size.Width,
                    size.Height > current.Height ? size.Height : current.Height);
            })
            : Children.Aggregate(new LvcSize(), (current, next) =>
            {
                var size = next.GetActualSize();

                return new LvcSize(
                    size.Width > current.Width ? size.Width : current.Width,
                    current.Height + size.Height);
            });
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext}, Scaler, Scaler)"/>
    public override LvcSize Measure(Chart<TDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        foreach (var child in Children) _ = child.Measure(chart, primaryScaler, secondaryScaler);
        return GetActualSize();
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.OnInvalidated(Chart{TDrawingContext}, Scaler, Scaler)"/>
    protected internal override void OnInvalidated(Chart<TDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        var x = X;
        var y = Y;

        _position = new((float)x, (float)y);
        var controlSize = Measure(chart, primaryScaler, secondaryScaler);

        if (Orientation == ContainerOrientation.Horizontal)
        {
            foreach (var child in Children)
            {
                var childSize = child.GetActualSize();

                child._x = x;
                child._y = VerticalAlignment == Align.Middle
                    ? y + (controlSize.Height - childSize.Height) / 2f
                    : VerticalAlignment == Align.End
                        ? y + controlSize.Height - childSize.Height
                        : y;

                child.OnInvalidated(chart, primaryScaler, secondaryScaler);

                x += childSize.Width;
            }
        }
        else
        {
            foreach (var child in Children)
            {
                var childSize = child.GetActualSize();

                child._x = HorizontalAlignment == Align.Middle
                    ? x + (controlSize.Width - childSize.Width) / 2f
                    : HorizontalAlignment == Align.End
                        ? x + controlSize.Width - childSize.Width
                        : x;
                child._y = y;

                child.OnInvalidated(chart, primaryScaler, secondaryScaler);

                y += childSize.Height;
            }
        }
    }

    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return Array.Empty<IPaint<TDrawingContext>>();
    }
}
