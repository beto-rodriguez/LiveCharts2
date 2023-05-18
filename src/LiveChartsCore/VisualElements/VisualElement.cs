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
using System.ComponentModel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
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
    private int _scalesXAt;
    private int _scalesYAt;

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
    public double X { get => _x; set { _x = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the Y coordinate [in Pixels or ChartValues, see <see cref="LocationUnit"/>].
    /// </summary>
    public double Y { get => _y; set { _y = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the unit of the <see cref="X"/> and <see cref="Y"/> properties.
    /// </summary>
    public MeasureUnit LocationUnit { get; set; } = MeasureUnit.Pixels;

    /// <summary>
    /// Gets or sets the axis index where the series is scaled in the X plane, the index must exist 
    /// in the <see cref="ICartesianChartView{TDrawingContext}.YAxes"/> collection.
    /// </summary>
    /// <value>
    /// The index of the axis.
    /// </value>
    public int ScalesXAt { get => _scalesXAt; set { _scalesXAt = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the axis index where the series is scaled in the Y plane, the index must exist 
    /// in the <see cref="ICartesianChartView{TDrawingContext}.YAxes"/> collection.
    /// </summary>
    /// <value>
    /// The index of the axis.
    /// </value>
    public int ScalesYAt { get => _scalesYAt; set { _scalesYAt = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        CartesianChart<TDrawingContext>? cartesianChart = null;

        if (chart is CartesianChart<TDrawingContext> cc)
        {
            //var primaryAxis = cc.YAxes[ScalesYAt];
            //var secondaryAxis = cc.XAxes[ScalesXAt];

            //SecondaryScaler = secondaryAxis.GetNextScaler(cc);
            //PrimaryScaler = primaryAxis.GetNextScaler(cc);
        }

        // Todo: polar and pie
        // if (chart is PolarChart<TDrawingContext> pc)
        // if (chart is PieChart<TDrawingContext> pc)
        // if (chart is PolarChart<TDrawingContext> pc)
        // {
        //     var primaryAxis = pc.AngleAxes[ScalesYAt];
        //     var secondaryAxis = pc.RadiusAxes[ScalesXAt];

        //     var primary = new PolarScaler(
        //         chart.DrawMarginLocation, chart.DrawMarginSize, primaryAxis, secondaryAxis, pc.InnerRadius, pc.InitialRotation, pc.TotalAnge);
        // }

        foreach (var paintTask in GetPaintTasks())
        {
            if (paintTask is null) continue;

            if (cartesianChart is not null)
            {
                //paintTask.SetClipRectangle(
                //    cartesianChart.Canvas,
                //    new LvcRectangle(cartesianChart.DrawMarginLocation, cartesianChart.DrawMarginSize));
            }

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

    internal virtual IEnumerable<VisualElement<TDrawingContext>> IsHitBy(Chart<TDrawingContext> chart, LvcPoint point)
    {
        var motionCanvas = chart.Canvas;
        if (motionCanvas.StartPoint is not null)
        {
            point.X -= motionCanvas.StartPoint.Value.X;
            point.Y -= motionCanvas.StartPoint.Value.Y;
        }

        var size = Measure(chart);

        // it returns an enumerable because there are more complex types where a visual can contain more than one element
        if (point.X >= X && point.X <= X + size.Width &&
            point.Y >= Y && point.Y <= Y + size.Height)
        {
            yield return this;
        }
    }

    internal abstract IAnimatable?[] GetDrawnGeometries();

    internal virtual void AlignToTopLeftCorner()
    {
        // just a workaround to align labels as the rest of the geometries.
    }
}
