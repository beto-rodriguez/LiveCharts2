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
using System.Xml.Serialization;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines the relative panel class.
/// </summary>
/// <typeparam name="TDrawingContext"></typeparam>
public class RelativePanel<TDrawingContext> : VisualElement<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private LvcPoint _position;

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    public LvcSize Size { get; set; }

    /// <summary>
    /// Gets the children collection.
    /// </summary>
    public HashSet<VisualElement<TDrawingContext>> Children { get; } = new();

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualLocation"/>
    public override LvcPoint GetActualLocation()
    {
        return _position;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualSize"/>
    public override LvcSize GetActualSize()
    {
        return Size;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext}, Scaler?, Scaler?)"/>
    public override LvcSize Measure(Chart<TDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        foreach (var child in Children) _ = child.Measure(chart, primaryScaler, secondaryScaler);
        return GetActualSize();
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})"/>
    public override void RemoveFromUI(Chart<TDrawingContext> chart)
    {
        foreach (var child in Children)
        {
            child.RemoveFromUI(chart);
        }

        base.RemoveFromUI(chart);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.OnInvalidated(Chart{TDrawingContext}, Scaler, Scaler)"/>
    protected internal override void OnInvalidated(Chart<TDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        var x = X + _parentX;
        var y = Y + _parentY;

        _position = new((float)x, (float)y);

        foreach (var child in Children)
        {
            //var childSize = child.GetActualSize();
            child._parent = _parent;
            child._parentPaddingX = _parentPaddingX;
            child._parentPaddingY = _parentPaddingY;
            child._x = x;
            child._y = y;
            child.OnInvalidated(chart, primaryScaler, secondaryScaler);
        }
    }

    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return Array.Empty<IPaint<TDrawingContext>>();
    }
}
