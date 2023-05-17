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
public class StackPanel<TBackgroundGeometry, TDrawingContext> : VisualElement<TDrawingContext>
    where TDrawingContext : DrawingContext
    where TBackgroundGeometry : ISizedGeometry<TDrawingContext>, new()
{
    private LvcPoint _targetPosition;
    private IPaint<TDrawingContext>? _backgroundPaint;
    private readonly TBackgroundGeometry _boundsGeometry = new();

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

    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _backgroundPaint };
    }

    internal override IAnimatable?[] GetDrawnGeometries()
    {
        return new IAnimatable?[] { _boundsGeometry };
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.OnInvalidated(Chart{TDrawingContext}, Scaler, Scaler)"/>
    protected internal override void OnInvalidated(Chart<TDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        var xl = Padding.Left;
        var yl = Padding.Top;

        _targetPosition = new((float)X, (float)Y);
        var controlSize = Measure(chart, primaryScaler, secondaryScaler);

        // NOTE #20231605
        // force the background to have at least an invisible geometry
        // we use this geometry in the motion canvas to track the position
        // of the stack panel as the time and animations elapse.
        BackgroundPaint ??= LiveCharts.DefaultSettings
                .GetProvider<TDrawingContext>()
                .GetSolidColorPaint(new LvcColor(0, 0, 0, 0));

        chart.Canvas.AddDrawableTask(BackgroundPaint);
        _boundsGeometry.X = _targetPosition.X;
        _boundsGeometry.Y = _targetPosition.Y;
        _boundsGeometry.Width = controlSize.Width;
        _boundsGeometry.Height = controlSize.Height;
        BackgroundPaint.AddGeometryToPaintTask(chart.Canvas, _boundsGeometry);

        foreach (var child in Children)
        {
            _ = child.Measure(chart, primaryScaler, secondaryScaler);
            var childSize = child.GetTargetSize();

            if (Orientation == ContainerOrientation.Horizontal)
            {
                child._x = xl;
                child._y = VerticalAlignment == Align.Middle
                    ? yl + (controlSize.Height - Padding.Top - Padding.Bottom - childSize.Height) / 2f
                    : VerticalAlignment == Align.End
                        ? yl + controlSize.Height - Padding.Top - Padding.Bottom - childSize.Height
                        : yl;

                xl += childSize.Width;
            }
            else
            {
                child._x = HorizontalAlignment == Align.Middle
                    ? xl + (controlSize.Width - Padding.Left - Padding.Right - childSize.Width) / 2f
                    : HorizontalAlignment == Align.End
                        ? xl + controlSize.Width - Padding.Left - Padding.Right - childSize.Width
                        : xl;
                child._y = yl;

                yl += childSize.Height;
            }

            child.OnInvalidated(chart, primaryScaler, secondaryScaler);
            child.SetParent(_boundsGeometry);
        }
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.SetParent(IGeometry{TDrawingContext})"/>
    protected internal override void SetParent(IGeometry<TDrawingContext> parent)
    {
        if (_boundsGeometry is null) return;
        _boundsGeometry.Parent = parent;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext}, Scaler, Scaler)"/>
    public override LvcSize Measure(Chart<TDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        foreach (var child in Children) _ = child.Measure(chart, primaryScaler, secondaryScaler);
        return GetTargetSize();
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

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetTargetLocation"/>
    public override LvcPoint GetTargetLocation()
    {
        return _targetPosition;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetTargetSize"/>
    public override LvcSize GetTargetSize()
    {
        var size = Orientation == ContainerOrientation.Horizontal
            ? Children.Aggregate(new LvcSize(), (current, next) =>
            {
                var size = next.GetTargetSize();

                return new LvcSize(
                    current.Width + size.Width,
                    size.Height > current.Height ? size.Height : current.Height);
            })
            : Children.Aggregate(new LvcSize(), (current, next) =>
            {
                var size = next.GetTargetSize();

                return new LvcSize(
                    size.Width > current.Width ? size.Width : current.Width,
                    current.Height + size.Height);
            });

        return new LvcSize(Padding.Left + Padding.Right + size.Width, Padding.Top + Padding.Bottom + size.Height);
    }
}
