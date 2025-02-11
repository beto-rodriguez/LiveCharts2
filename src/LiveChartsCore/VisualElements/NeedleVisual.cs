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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Painting;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines a visual element in a chart that draws a needle.
/// </summary>
/// <typeparam name="TGeometry">The type of the geometry.</typeparam>
/// <typeparam name="TLabelGeometry">The type of the label.</typeparam>
public class NeedleVisual<TGeometry, TLabelGeometry> : VisualElement
    where TGeometry : BaseNeedleGeometry, new()
    where TLabelGeometry : BaseLabelGeometry, new()
{
    private Paint? _fill;
    private double _value;
    private TGeometry? _geometry;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public double Value { get => _value; set => SetProperty(ref _value, value); }

    /// <summary>
    /// Gets or sets the fill paint.
    /// </summary>
    public Paint? Fill
    {
        get => _fill;
        set => SetPaintProperty(ref _fill, value);
    }

    /// <inheritdoc cref="VisualElement.OnInvalidated(Chart)"/>
    protected internal override void OnInvalidated(Chart chart)
    {
        ApplyTheme<NeedleVisual<TGeometry, TLabelGeometry>>();

        if (chart is not PieChartEngine pieChart)
            throw new Exception("The needle visual can only be added to a pie chart");

        var drawLocation = pieChart.DrawMarginLocation;
        var drawMarginSize = pieChart.DrawMarginSize;

        var minDimension = drawMarginSize.Width < drawMarginSize.Height
            ? drawMarginSize.Width
            : drawMarginSize.Height;

        var cx = drawLocation.X + drawMarginSize.Width * 0.5f;
        var cy = drawLocation.Y + drawMarginSize.Height * 0.5f;

        var h = minDimension * 0.45f;

        var view = (IPieChartView)pieChart.View;
        var initialRotation = (float)Math.Truncate(view.InitialRotation);
        var completeAngle = (float)view.MaxAngle;

        var startValue = view.MinValue;
        var endValue = view.MaxValue;

        if (_geometry is null)
        {
            _geometry = new()
            {
                X = cx,
                Y = cy,
                Radius = h,
                RotateTransform = initialRotation - 90
            };
            _geometry.Animate(chart);
        }

        _geometry.X = cx;
        _geometry.Y = cy;
        _geometry.Radius = h;

        var p = (Value - startValue) / (endValue - startValue);
        _geometry.RotateTransform = (float)(initialRotation + p * completeAngle - 90); // -90 to match the pie start angle
        _geometry.TranslateTransform = Translate;

        if (Fill is not null)
        {
            Fill.ZIndex = Fill.ZIndex == 0 ? 999 : Fill.ZIndex;
            Fill.AddGeometryToPaintTask(chart.Canvas, _geometry);
            pieChart.Canvas.AddDrawableTask(Fill);
        }
    }

    /// <inheritdoc cref="VisualElement.Measure(Chart)"/>
    public override LvcSize Measure(Chart chart) => new();

    /// <inheritdoc cref="VisualElement.SetParent(DrawnGeometry)"/>
    protected internal override void SetParent(DrawnGeometry parent)
    {
        if (_geometry is null) return;
        ((IDrawnElement)_geometry).Parent = parent;
    }

    /// <inheritdoc cref="VisualElement.GetDrawnGeometries"/>
    protected internal override Animatable?[] GetDrawnGeometries() =>
        [_geometry];

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Paint?[] GetPaintTasks() =>
        [_fill];
}
