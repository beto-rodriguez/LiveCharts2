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
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines the stack panel class.
/// </summary>
public class StackPanel<TBackgroundGemetry, TDrawingContext> : VisualElement<TDrawingContext>
    where TDrawingContext : DrawingContext
    where TBackgroundGemetry : ISizedGeometry<TDrawingContext>, new()
{
    private LvcPoint _position;
    private IPaint<TDrawingContext>? _backgroundPaint;
    private TBackgroundGemetry? _backgroundGeometry;

    /// <summary>
    /// Gets the children collection.
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

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public Padding Padding { get; set; } = new();

    /// <summary>
    /// Gets or sets the background paint.
    /// </summary>
    public IPaint<TDrawingContext>? BackgroundPaint
    {
        get => _backgroundPaint;
        set => SetPaintProperty(ref _backgroundPaint, value);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualLocation"/>
    public override LvcPoint GetActualLocation()
    {
        return _position;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualSize"/>
    public override LvcSize GetActualSize()
    {
        var size = Orientation == ContainerOrientation.Horizontal
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

        return new LvcSize(Padding.Left + Padding.Right + size.Width, Padding.Top + Padding.Bottom + size.Height);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext}, Scaler, Scaler)"/>
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
        _position = new((float)X, (float)Y);

        var x = Padding.Left + _parentX;// + X;
        var y = Padding.Top + _parentY;// + Y;

        var controlSize = Measure(chart, primaryScaler, secondaryScaler);

        if (_backgroundPaint is not null)
        {
            chart.Canvas.AddDrawableTask(_backgroundPaint);
            if (_backgroundGeometry is null)
            {
                _backgroundGeometry = new TBackgroundGemetry
                {
                    X = _position.X,
                    Y = _position.Y,
                    Width = controlSize.Width,
                    Height = controlSize.Height
                };
                _ = _backgroundGeometry
                    .TransitionateProperties()
                    .WithAnimation(chart)
                    .CompleteCurrentTransitions();
            }
            _backgroundGeometry.X = _position.X;
            _backgroundGeometry.Y = _position.Y;
            _backgroundGeometry.Width = controlSize.Width;
            _backgroundGeometry.Height = controlSize.Height;
            _backgroundPaint.AddGeometryToPaintTask(chart.Canvas, _backgroundGeometry);
        }

        if (Orientation == ContainerOrientation.Horizontal)
        {
            foreach (var child in Children)
            {
                _ = child.Measure(chart, primaryScaler, secondaryScaler);
                var childSize = child.GetActualSize();

                child._parent = _backgroundGeometry ?? _parent;
                child._parentPaddingX = Padding.Left + _parentPaddingX;
                child._parentPaddingY = Padding.Top + _parentPaddingY;
                child._parentX = _position.X;
                child._parentY = _position.Y;
                child._x = x;
                child._y = VerticalAlignment == Align.Middle
                    ? y + (controlSize.Height - Padding.Top - Padding.Bottom - childSize.Height) / 2f
                    : VerticalAlignment == Align.End
                        ? y + controlSize.Height - Padding.Top - Padding.Bottom - childSize.Height
                        : y;

                child.OnInvalidated(chart, primaryScaler, secondaryScaler);

                x += childSize.Width;
            }
        }
        else
        {
            foreach (var child in Children)
            {
                _ = child.Measure(chart, primaryScaler, secondaryScaler);
                var childSize = child.GetActualSize();

                child._parent = _backgroundGeometry ?? _parent;
                child._parentPaddingX = Padding.Left + _parentPaddingX;
                child._parentPaddingY = Padding.Top + _parentPaddingY;
                child._parentX = _position.X;
                child._parentY = _position.Y;
                child._x = HorizontalAlignment == Align.Middle
                    ? x + (controlSize.Width - Padding.Left - Padding.Right - childSize.Width) / 2f
                    : HorizontalAlignment == Align.End
                        ? x + controlSize.Width - Padding.Left - Padding.Right - childSize.Width
                        : x;
                child._y = y;

                child.OnInvalidated(chart, primaryScaler, secondaryScaler);

                y += childSize.Height;
            }
        }
    }

    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _backgroundPaint };
    }
}
