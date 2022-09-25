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
using System.Linq;
using System.Runtime.CompilerServices;
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

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        Scaler? primary = null;
        Scaler? secondary = null;

        CartesianChart<TDrawingContext>? cartesianChart = null;

        if (chart is CartesianChart<TDrawingContext> cc)
        {
            cartesianChart = cc;
            var primaryAxis = cartesianChart.YAxes[ScalesYAt];
            var secondaryAxis = cartesianChart.XAxes[ScalesXAt];
            secondary = secondaryAxis.GetNextScaler(cartesianChart);
            primary = primaryAxis.GetNextScaler(cartesianChart);
        }

        foreach (var paintTask in GetPaintTasks())
        {
            if (paintTask is null) continue;

            if (cartesianChart is not null)
            {
                paintTask.SetClipRectangle(
                    cartesianChart.Canvas,
                    new LvcRectangle(cartesianChart.DrawMarginLocation, cartesianChart.DrawMarginSize));
            }

            chart.Canvas.AddDrawableTask(paintTask);
        }

        if (primary is null || secondary is null) throw new Exception($"This chart does not support VisualElements");

        Draw(chart, primary, secondary);
    }

    /// <summary>
    /// Measures the element and returns the size.
    /// </summary>
    /// <param name = "chart" > The chart.</param>
    /// <param name="primaryScaler">
    /// The primary axis scaler, normally the Y axis. If the chart is Polar then it is the Angle scaler. If the chart is a pie chart
    /// then it is the Values Scaler.
    /// </param>
    /// <param name="secondaryScaler">
    /// The secondary axis scaler, normally the X axis. If the chart is Polar then it is the Radius scaler. If the chart is a pie chart
    /// then it is the index Scaler.</param>
    /// <returns>The size of the element.</returns>
    public abstract LvcSize Measure(Chart<TDrawingContext> chart, Scaler primaryScaler, Scaler secondaryScaler);

    /// <summary>
    /// Gets the actual size of the element.
    /// </summary>
    /// <returns>The actual size.</returns>
    public abstract LvcSize GetActualSize();

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
    /// Called when a property changes.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Called when the visual is drawn.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="primaryScaler">
    /// The primary axis scaler, normally the Y axis. If the chart is Polar then it is the Angle scaler. If the chart is a pie chart
    /// then it is the Values Scaler.
    /// </param>
    /// <param name="secondaryScaler">
    /// The secondary axis scaler, normally the X axis. If the chart is Polar then it is the Radius scaler. If the chart is a pie chart
    /// then it is the index Scaler.</param>
    protected internal abstract void Draw(Chart<TDrawingContext> chart, Scaler primaryScaler, Scaler secondaryScaler);
}
