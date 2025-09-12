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
using LiveChartsCore.Kernel;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines the stack panel class.
/// </summary>
/// <typeparam name="TBackgroundGeometry">The type of the background geometry.</typeparam>
[Obsolete($"Replaced by StackLayout")]
public class StackPanel<TBackgroundGeometry> : VisualElement
    where TBackgroundGeometry : BoundedDrawnGeometry, new()
{
    /// <summary>
    /// Gets the children collection.
    /// </summary>
    public List<VisualElement> Children { get; } = [];

    /// <summary>
    /// Gets or sets the panel orientation.
    /// </summary>
    public ContainerOrientation Orientation { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the vertical alignment.
    /// </summary>
    public Align VerticalAlignment { get; set => SetProperty(ref field, value); } = Align.Middle;

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align HorizontalAlignment { get; set => SetProperty(ref field, value); } = Align.Middle;

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public Padding Padding { get; set => SetProperty(ref field, value); } = new();

    /// <summary>
    /// Gets or sets the background paint.
    /// </summary>
    public Paint? BackgroundPaint
    {
        get;
        set => SetPaintProperty(ref field, value);
    }

    /// <summary>
    /// Gets the background geometry.
    /// </summary>
    public TBackgroundGeometry BackgroundGeometry { get; } = new();

    /// <summary>
    /// Gets or sets the maximum width. When the maximum with is reached, a new row is created.
    /// </summary>
    public double MaxWidth { get; set => SetProperty(ref field, value); } = double.MaxValue;

    /// <summary>
    /// Gets or sets the maximum height. When the maximum height is reached, a new column is created.
    /// </summary>
    public double MaxHeight { get; set => SetProperty(ref field, value); } = double.MaxValue;

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Paint?[] GetPaintTasks() =>
        [BackgroundPaint];

    /// <inheritdoc cref="VisualElement.GetDrawnGeometries"/>
    protected internal override Animatable?[] GetDrawnGeometries() =>
        [BackgroundGeometry];

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

    /// <inheritdoc cref="VisualElement.OnInvalidated(Chart)"/>
    protected internal override void OnInvalidated(Chart chart)
    {
        var controlSize = Measure(chart);

        // NOTE #20231605
        // force the background to have at least an invisible geometry
        // we use this geometry in the motion canvas to track the position
        // of the stack panel as the time and animations elapse.
        BackgroundPaint ??= LiveCharts.DefaultSettings
                .GetProvider()
                .GetSolidColorPaint(new LvcColor(0, 0, 0, 0));

        chart.Canvas.AddDrawableTask(BackgroundPaint, zone: CanvasZone.NoClip);
        BackgroundGeometry.X = (float)X;
        BackgroundGeometry.Y = (float)Y;
        BackgroundGeometry.Width = controlSize.Width;
        BackgroundGeometry.Height = controlSize.Height;
        BackgroundGeometry.RotateTransform = (float)Rotation;
        BackgroundGeometry.TranslateTransform = Translate;
        BackgroundPaint.AddGeometryToPaintTask(chart.Canvas, BackgroundGeometry);
    }

    /// <inheritdoc cref="VisualElement.SetParent(DrawnGeometry)"/>
    protected internal override void SetParent(DrawnGeometry parent)
    {
        if (BackgroundGeometry is null) return;
        ((IDrawnElement)BackgroundGeometry).Parent = parent;
    }

    /// <inheritdoc cref="VisualElement.Measure(Chart)"/>
    public override LvcSize Measure(Chart chart)
    {
        var xl = Padding.Left;
        var yl = Padding.Top;
        var rowHeight = -1f;
        var columnWidth = -1f;
        var mx = 0f;
        var my = 0f;

        List<MeasureResult> line = [];

        LvcSize alignCurrentLine()
        {
            var mx = -1f;
            var my = -1f;

            foreach (var child in line)
            {
                if (Orientation == ContainerOrientation.Horizontal)
                {
                    child.Visual._y = VerticalAlignment switch
                    {
                        Align.Start => yl,
                        Align.Middle => yl + (rowHeight - child.Size.Height) / 2f,
                        Align.End => yl + rowHeight - child.Size.Height,
                        _ => throw new NotImplementedException()
                    };
                }
                else
                {
                    child.Visual._x = HorizontalAlignment switch
                    {
                        Align.Start => xl,
                        Align.Middle => xl + (columnWidth - child.Size.Width) / 2f,
                        Align.End => xl + columnWidth - child.Size.Width,
                        _ => throw new NotImplementedException()
                    };
                }

                child.Visual.OnInvalidated(chart);
                child.Visual.SetParent(BackgroundGeometry);
                if (child.Size.Width > mx) mx = child.Size.Width;
                if (child.Size.Height > my) my = child.Size.Height;
            }

            line = [];
            return new LvcSize(mx, my);
        }

        foreach (var child in Children)
        {
            var childSize = child.Measure(chart);

            if (Orientation == ContainerOrientation.Horizontal)
            {
                if (xl + childSize.Width > MaxWidth)
                {
                    var lineSize = alignCurrentLine();
                    xl = Padding.Left;
                    yl += lineSize.Height;
                    rowHeight = -1f;
                }

                if (rowHeight < childSize.Height) rowHeight = childSize.Height;
                child._x = xl;

                xl += childSize.Width;
            }
            else
            {
                if (yl + childSize.Height > MaxHeight)
                {
                    var lineSize = alignCurrentLine();
                    yl = Padding.Top;
                    xl += lineSize.Width;
                    columnWidth = -1f;
                }

                if (columnWidth < childSize.Width) columnWidth = childSize.Width;
                child._y = yl;

                yl += childSize.Height;
            }

            if (xl > mx) mx = xl;
            if (yl > my) my = yl;
            line.Add(new MeasureResult(child, childSize));
        }

        if (line.Count > 0)
        {
            var lineSize = alignCurrentLine();

            if (Orientation == ContainerOrientation.Horizontal)
            {
                yl += lineSize.Height;
            }
            else
            {
                xl += lineSize.Width;
            }

            if (xl > mx) mx = xl;
            if (yl > my) my = yl;
        }

        return new LvcSize(mx + Padding.Right, my + Padding.Bottom);
    }

    /// <inheritdoc cref="ChartElement.RemoveFromUI(Chart)"/>
    public override void RemoveFromUI(Chart chart)
    {
        foreach (var child in Children)
        {
            child.RemoveFromUI(chart);
        }

        base.RemoveFromUI(chart);
    }

    private class MeasureResult(VisualElement visual, LvcSize size)
    {
        public VisualElement Visual { get; set; } = visual;
        public LvcSize Size { get; set; } = size;
    }
}

/// <summary>
/// Obsolete.
/// </summary>
/// <typeparam name="TBackgroundGeometry"></typeparam>
/// <typeparam name="TDrawingContext"></typeparam>
[Obsolete($"Replaced by StackLayout")]
public class StackPanel<TBackgroundGeometry, TDrawingContext> : StackPanel<TBackgroundGeometry>
    where TBackgroundGeometry : BoundedDrawnGeometry, new()
    where TDrawingContext : DrawingContext
{ }
