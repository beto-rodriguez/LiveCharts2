﻿// The MIT License(MIT)
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
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines the relative panel class.
/// </summary>
[Obsolete($"Replaced by AbsoluteLayout")]
public class RelativePanel<TBackgroundGeometry> : VisualElement
    where TBackgroundGeometry : BoundedDrawnGeometry, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RelativePanel{TBackgroundGeometry}"/> class.
    /// </summary>
    public RelativePanel()
    {
        ClippingMode = ClipMode.None;
    }

    private Paint? _backgroundPaint;

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    public LvcSize Size { get; set; }

    /// <summary>
    /// Gets the children collection.
    /// </summary>
    public HashSet<VisualElement> Children { get; } = [];

    /// <summary>
    /// Gets or sets the background paint.
    /// </summary>
    public Paint? BackgroundPaint
    {
        get => _backgroundPaint;
        set => SetPaintProperty(ref _backgroundPaint, value);
    }

    /// <summary>
    /// Gets the background geometry.
    /// </summary>
    public TBackgroundGeometry BackgroundGeometry { get; } = new();

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Paint?[] GetPaintTasks() => [_backgroundPaint];

    /// <inheritdoc cref="VisualElement.GetDrawnGeometries"/>
    protected internal override Animatable?[] GetDrawnGeometries() =>
        [BackgroundGeometry];

    /// <inheritdoc cref="VisualElement.OnInvalidated(Chart)"/>
    protected internal override void OnInvalidated(Chart chart)
    {
        // NOTE #20231605
        // force the background to have at least an invisible geometry
        // we use this geometry in the motion canvas to track the position
        // of the stack panel as the time and animations elapse.
        BackgroundPaint ??= LiveCharts.DefaultSettings
                .GetProvider()
                .GetSolidColorPaint(new LvcColor(0, 0, 0, 0));

        var clipping = Clipping.GetClipRectangle(ClippingMode, chart);

        chart.Canvas.AddDrawableTask(BackgroundPaint);
        BackgroundGeometry.X = (float)X;
        BackgroundGeometry.Y = (float)Y;
        BackgroundGeometry.Width = Size.Width;
        BackgroundGeometry.Height = Size.Height;
        BackgroundGeometry.RotateTransform = (float)Rotation;
        BackgroundGeometry.TranslateTransform = Translate;
        BackgroundPaint.AddGeometryToPaintTask(chart.Canvas, BackgroundGeometry);
        BackgroundPaint.SetClipRectangle(chart.Canvas, clipping);

        foreach (var child in Children)
        {
            child.OnInvalidated(chart);
            child.SetParent(BackgroundGeometry);
        }
    }

    /// <inheritdoc cref="VisualElement.SetParent(DrawnGeometry)"/>
    protected internal override void SetParent(DrawnGeometry parent)
    {
        if (BackgroundGeometry is null) return;
        ((IDrawnElement)BackgroundGeometry).Parent = parent;
    }

    /// <inheritdoc cref="VisualElement.IsHitBy(Chart, LvcPoint)"/>
    protected internal override IEnumerable<VisualElement> IsHitBy(Chart chart, LvcPoint point)
    {
        var location = GetActualCoordinate();

        // see note #241104
        location.X += _translate.X;
        location.Y += _translate.Y;

        var size = Measure(chart);

        // it returns an enumerable because there are more complex types where a visual can contain more than one element
        if (point.X >= location.X && point.X <= location.X + size.Width &&
            point.Y >= location.Y && point.Y <= location.Y + size.Height)
        {
            yield return this;
        }

        var relativePoint = new LvcPoint(point.X - location.X, point.Y - location.Y);
        foreach (var child in Children)
        {
            foreach (var element in child.IsHitBy(chart, relativePoint))
            {
                yield return element;
            }
        }
    }

    /// <inheritdoc cref="VisualElement.Measure(Chart)"/>
    public override LvcSize Measure(Chart chart) =>
        Size;

    /// <inheritdoc cref="ChartElement.RemoveFromUI(Chart)"/>
    public override void RemoveFromUI(Chart chart)
    {
        foreach (var child in Children)
        {
            child.RemoveFromUI(chart);
        }

        base.RemoveFromUI(chart);
    }
}
