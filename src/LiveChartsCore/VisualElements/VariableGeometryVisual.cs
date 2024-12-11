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
using LiveChartsCore.Measure;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines a visual element that is useful to create series miniatures in the tool tips and legends.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VariableGeometryVisual"/> class.
/// </remarks>
/// <param name="geometry"></param>
public class VariableGeometryVisual(BoundedDrawnGeometry geometry) : BaseGeometryVisual
{
    private bool _isInitialized;

    /// <summary>
    /// Gets or sets the geometry.
    /// </summary>
    public BoundedDrawnGeometry Geometry
    {
        get => geometry;
        set
        {
            if (geometry == value) return;
            geometry = value;
            _isInitialized = false;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Occurs when the geometry is initialized.
    /// </summary>
    public event Action<BoundedDrawnGeometry>? GeometryInitialized;

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Animatable?[] GetDrawnGeometries() =>
        [geometry];

    /// <inheritdoc cref="CoreVisualElement.OnInvalidated(Chart)"/>
    protected internal override void OnInvalidated(Chart chart)
    {
        var x = (float)X;
        var y = (float)Y;
        var clipping = Clipping.GetClipRectangle(ClippingMode, chart);

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            if (PrimaryScaler is null || SecondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            x = SecondaryScaler.ToPixels(x);
            y = PrimaryScaler.ToPixels(y);
        }

        var size = Measure(chart);

        if (!_isInitialized)
        {
            Geometry.X = (float)X;
            Geometry.Y = (float)Y;
            Geometry.Width = size.Width;
            Geometry.Height = size.Height;

            GeometryInitialized?.Invoke(Geometry);

            Geometry.Animate(chart);
            _isInitialized = true;
        }

        Geometry.X = x;
        Geometry.Y = y;
        Geometry.Width = size.Width;
        Geometry.Height = size.Height;
        Geometry.RotateTransform = (float)Rotation;
        Geometry.TranslateTransform = Translate;

        if (Fill is not null)
        {
            chart.Canvas.AddDrawableTask(Fill);
            Fill.AddGeometryToPaintTask(chart.Canvas, Geometry);
            Fill.SetClipRectangle(chart.Canvas, clipping);
        }

        if (Stroke is not null)
        {
            chart.Canvas.AddDrawableTask(Stroke);
            Stroke.AddGeometryToPaintTask(chart.Canvas, Geometry);
            Stroke.SetClipRectangle(chart.Canvas, clipping);
        }
    }

    /// <inheritdoc cref="CoreVisualElement.SetParent(DrawnGeometry)"/>
    protected internal override void SetParent(DrawnGeometry parent)
    {
        if (geometry is null) return;
        ((IDrawnElement)geometry).Parent = parent;
    }

    /// <inheritdoc cref="CoreVisualElement.Measure(Chart)"/>
    public override LvcSize Measure(Chart chart)
    {
        var w = (float)Width;
        var h = (float)Height;

        if (SizeUnit == MeasureUnit.ChartValues)
        {
            if (PrimaryScaler is null || SecondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            w = SecondaryScaler.MeasureInPixels(w);
            h = PrimaryScaler.MeasureInPixels(h);
        }

        return new LvcSize(w, h);
    }
}
