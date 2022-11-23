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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.VisualElements;

/// <summary>
/// Defines a visual element in a chart that draws a rectangle geometry in the user interface.
/// </summary>
public class VariableGeometryVisual : BaseGeometryVisual
{
    private ISizedGeometry<SkiaSharpDrawingContext> _geometry;
    private bool _isInitialized;
    private LvcSize _actualSize = new();
    private LvcPoint _targetPosition = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="VariableGeometryVisual"/> class.
    /// </summary>
    /// <param name="geometry"></param>
    public VariableGeometryVisual(ISizedGeometry<SkiaSharpDrawingContext> geometry)
    {
        _geometry = geometry;
    }

    /// <summary>
    /// Gets or sets the geometry.
    /// </summary>
    public ISizedGeometry<SkiaSharpDrawingContext> Geometry
    {
        get => _geometry;
        set
        {
            if (_geometry == value) return;
            _geometry = value;
            _isInitialized = false;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Occurs when the geometry is initialized.
    /// </summary>
    public event Action<ISizedGeometry<SkiaSharpDrawingContext>>? GeometryIntialized;

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext}, Scaler, Scaler)"/>
    public override LvcSize Measure(Chart<SkiaSharpDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        var w = (float)Width;
        var h = (float)Height;

        if (SizeUnit == MeasureUnit.ChartValues)
        {
            if (primaryScaler is null || secondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            w = secondaryScaler.MeasureInPixels(w);
            h = primaryScaler.MeasureInPixels(h);
        }

        return _actualSize = new LvcSize(w, h);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetTargetSize"/>
    public override LvcSize GetTargetSize()
    {
        return _actualSize;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.OnInvalidated(Chart{TDrawingContext}, Scaler, Scaler)"/>
    protected internal override void OnInvalidated(Chart<SkiaSharpDrawingContext> chart, Scaler? primaryScaler, Scaler? secondaryScaler)
    {
        var x = (float)X;
        var y = (float)Y;

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            if (primaryScaler is null || secondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            x = secondaryScaler.ToPixels(x);
            y = primaryScaler.ToPixels(y);
        }

        _targetPosition = new((float)X + _xc, (float)Y + _yc);
        _ = Measure(chart, primaryScaler, secondaryScaler);

        if (!_isInitialized)
        {
            var cp = GetPositionRelativeToParent();

            Geometry.X = cp.X;
            Geometry.Y = cp.Y;
            Geometry.Width = _actualSize.Width;
            Geometry.Height = _actualSize.Height;

            GeometryIntialized?.Invoke(Geometry);

            _ = Geometry
                .TransitionateProperties()
                .WithAnimation(chart)
                .CompleteCurrentTransitions();

            _isInitialized = true;
        }

        Geometry.X = x + _xc;
        Geometry.Y = y + _yc;
        Geometry.Width = _actualSize.Width;
        Geometry.Height = _actualSize.Height;

        var drawing = chart.Canvas.Draw();
        if (Fill is not null) _ = drawing.SelectPaint(Fill).Draw(Geometry);
        if (Stroke is not null) _ = drawing.SelectPaint(Stroke).Draw(Geometry);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetTargetLocation"/>
    public override LvcPoint GetTargetLocation()
    {
        return _targetPosition;
    }
}
