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
using System.ComponentModel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines the base visual element class, inheriting from this class makes it easy to implement a visual element.
/// </summary>
public abstract class VisualElement<TDrawingContext> : ChartElement<TDrawingContext>, INotifyPropertyChanged
    where TDrawingContext : DrawingContext
{
    internal double _x;
    internal double _y;
    internal LvcPoint _translate = new();
    internal double _rotation;
    private int _scalesXAt;
    private int _scalesYAt;
    private MeasureUnit _locationUnit = MeasureUnit.Pixels;
    private ClipMode _clippingMode = ClipMode.XY;

    /// <summary>
    /// Gets the primary scaler.
    /// </summary>
    protected Scaler? PrimaryScaler { get; private set; }

    /// <summary>
    /// Gets the secondary scaler.
    /// </summary>
    protected Scaler? SecondaryScaler { get; private set; }

    /// <summary>
    /// Gets or sets the X coordinate [in Pixels or ChartValues, see <see cref="LocationUnit"/>].
    /// </summary>
    public double X { get => _x; set => SetProperty(ref _x, value); }

    /// <summary>
    /// Gets or sets the Y coordinate [in Pixels or ChartValues, see <see cref="LocationUnit"/>].
    /// </summary>
    public double Y { get => _y; set => SetProperty(ref _y, value); }

    /// <summary>
    /// Gets or sets the rotation.
    /// </summary>
    public double Rotation { get => _rotation; set { _rotation = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the translate transform.
    /// </summary>
    public LvcPoint Translate { get => _translate; set => SetProperty(ref _translate, value); }

    /// <summary>
    /// Gets or sets the unit of the <see cref="X"/> and <see cref="Y"/> properties.
    /// </summary>
    public MeasureUnit LocationUnit { get => _locationUnit; set => SetProperty(ref _locationUnit, value); }

    /// <summary>
    /// Gets or sets the axis index where the series is scaled in the X plane, the index must exist 
    /// in the <see cref="ICartesianChartView{TDrawingContext}.YAxes"/> collection.
    /// </summary>
    /// <value>
    /// The index of the axis.
    /// </value>
    public int ScalesXAt { get => _scalesXAt; set => SetProperty(ref _scalesXAt, value); }

    /// <summary>
    /// Gets or sets the axis index where the series is scaled in the Y plane, the index must exist 
    /// in the <see cref="ICartesianChartView{TDrawingContext}.YAxes"/> collection.
    /// </summary>
    /// <value>
    /// The index of the axis.
    /// </value>
    public int ScalesYAt { get => _scalesYAt; set => SetProperty(ref _scalesYAt, value); }

    /// <summary>
    /// Gets or sets the clipping mode, clipping restricts the visual element for being drawn outside of the chart area (DrawMargin),
    /// default is <see cref="ClipMode.XY"/>, and means that anything outside the chart bounds will be ignored.
    /// </summary>
    public ClipMode ClippingMode { get => _clippingMode; set => SetProperty(ref _clippingMode, value); }

    /// <summary>
    /// Called when the pointer goes down on the visual.
    /// </summary>
    public event VisualElementHandler<TDrawingContext>? PointerDown;

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        if (chart is CartesianChart<TDrawingContext> cc)
        {
            var primaryAxis = cc.YAxes[ScalesYAt];
            var secondaryAxis = cc.XAxes[ScalesXAt];

            SecondaryScaler = secondaryAxis.GetNextScaler(cc);
            PrimaryScaler = primaryAxis.GetNextScaler(cc);
        }

        foreach (var paintTask in GetPaintTasks())
        {
            if (paintTask is null) continue;
            chart.Canvas.AddDrawableTask(paintTask);
        }

        OnInvalidated(chart);
    }

    /// <summary>
    /// Measures the element and returns the size.
    /// </summary>
    /// <param name = "chart" > The chart.</param>
    public abstract LvcSize Measure(Chart<TDrawingContext> chart);

    /// <summary>
    /// Called when [paint changed].
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected override void OnPaintChanged(string? propertyName)
    {
        base.OnPaintChanged(propertyName);
        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Called when the visual is drawn.
    /// </summary>
    /// <param name="chart">The chart.</param>
    protected internal abstract void OnInvalidated(Chart<TDrawingContext> chart);

    /// <summary>
    /// Sets the parent to all the geometries in the visual.
    /// </summary>
    protected internal abstract void SetParent(IGeometry<TDrawingContext> parent);

    /// <summary>
    /// Gets the acdtual coordinate of the visual.
    /// </summary>
    /// <returns></returns>
    protected LvcPoint GetActualCoordinate()
    {
        var x = (float)X;
        var y = (float)Y;

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            if (PrimaryScaler is null || SecondaryScaler is null)
                throw new Exception($"You can not use {MeasureUnit.ChartValues} scale at this element.");

            x = SecondaryScaler.ToPixels(X);
            y = PrimaryScaler.ToPixels(Y);
        }

        return new(x, y);
    }

    internal virtual IEnumerable<VisualElement<TDrawingContext>> IsHitBy(Chart<TDrawingContext> chart, LvcPoint point)
    {
        var location = GetActualCoordinate();
        var size = Measure(chart);

        // it returns an enumerable because there are more complex types where a visual can contain more than one element
        if (point.X >= location.X && point.X <= location.X + size.Width &&
            point.Y >= location.Y && point.Y <= location.Y + size.Height)
        {
            yield return this;
        }
    }

    internal void InvokePointerDown(VisualElementEventArgs<TDrawingContext> args)
    {
        PointerDown?.Invoke(this, args);
    }

    internal abstract IAnimatable?[] GetDrawnGeometries();

    internal virtual void AlignToTopLeftCorner()
    {
        // just a workaround to align labels as the rest of the geometries.
    }
}
