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
using LiveChartsCore.Measure;

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines an Axis in a Cartesian chart.
/// </summary>
public interface ICartesianAxis : IPlane, INotifyPropertyChanged
{
    /// <summary>
    /// Gets the orientation.
    /// </summary>
    /// <value>
    /// The orientation.
    /// </value>
    AxisOrientation Orientation { get; }

    /// <summary>
    /// Gets or sets the padding around the tick labels along the axis.
    /// </summary>
    /// <value>
    /// The padding in pixels.
    /// </value>
    Padding Padding { get; set; }

    /// <summary>
    /// Gets or sets the xo, a reference used internally to calculate the axis position.
    /// </summary>
    /// <value>
    /// The xo.
    /// </value>
    float Xo { get; set; }

    /// <summary>
    /// Gets or sets the yo, a reference used internally to calculate the axis position.
    /// </summary>
    /// <value>
    /// The yo.
    /// </value>
    float Yo { get; set; }

    /// <summary>
    /// Gets or sets the size of the axis, this value is used internally to calculate the axis position.
    /// </summary>
    /// <value>
    /// The length.
    /// </value>
    LvcSize Size { get; set; }

    /// <summary>
    /// Gets or sets the min zoom delta, the minimum difference between the max and min visible limits of the axis.
    /// default is null and null means that the library will calculate this value based on the current data.
    /// </summary>
    double? MinZoomDelta { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the ticks are centered to the <see cref="IPlane.UnitWidth"/>, default is true.
    /// </summary>
    bool TicksAtCenter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the separators are centered to the <see cref="IPlane.UnitWidth"/>, default is true.
    /// </summary>
    bool SeparatorsAtCenter { get; set; }

    /// <summary>
    /// Gets or sets the reserved area for the labels.
    /// </summary>
    LvcRectangle LabelsDesiredSize { get; set; }

    /// <summary>
    /// Gets or sets the max label size, this value is used internally to measure the axis.
    /// </summary>
    LvcSize PossibleMaxLabelSize { get; }

    /// <summary>
    /// Gets or sets the reserved area for the name.
    /// </summary>
    LvcRectangle NameDesiredSize { get; set; }

    /// <summary>
    /// Places the title in the same direction as the axis, default is false.
    /// </summary>
    bool InLineNamePlacement { get; set; }

    /// <summary>
    /// Gets or sets the labels alignment, default is null and means that the library will set it based on the
    /// <see cref="Orientation"/> and <see cref="Position"/> properties.
    /// </summary>
    Align? LabelsAlignment { get; set; }

    /// <summary>
    /// Gets or sets the axis position.
    /// </summary>
    /// <value>
    /// The position.
    /// </value>
    AxisPosition Position { get; set; }

    /// <summary>
    /// Gets or sets the shared axes collection, useful to share the zooming an panning between several charts.
    /// </summary>
    public IEnumerable<ICartesianAxis>? SharedWith { get; set; }

    /// <summary>
    /// Initializes the axis for the specified orientation.
    /// </summary>
    /// <param name="orientation">The orientation.</param>
    void Initialize(AxisOrientation orientation);

    /// <summary>
    /// Occurs when the axis is initialized.
    /// </summary>
    event Action<ICartesianAxis>? Initialized;

    /// <summary>
    /// Gets the axis limits considering its own and the <see cref="SharedWith"/> axes.
    /// </summary>
    /// <returns>The limits.</returns>
    AxisLimit GetLimits();

    /// <summary>
    /// Sets the axis limits (own and shared).
    /// </summary>
    /// <param name="min">The min limit.</param>
    /// <param name="max">The max limit.</param>
    void SetLimits(double min, double max);
}

/// <summary>
/// Defines an Axis in a Cartesian chart.
/// </summary>
/// <typeparam name="TDrawingContext"></typeparam>
public interface ICartesianAxis<TDrawingContext> : ICartesianAxis
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the sub-separators paint.
    /// </summary>
    /// <value>
    /// The separators paint.
    /// </value>
    IPaint<TDrawingContext>? SubseparatorsPaint { get; set; }

    /// <summary>
    /// Gets or sets the number of subseparators to draw.
    /// </summary>
    int SubseparatorsCount { get; set; }

    /// <summary>
    /// Gets or sets whether the ticks path should be drawn.
    /// </summary>
    bool DrawTicksPath { get; set; }

    /// <summary>
    /// Gets or sets the separators paint.
    /// </summary>
    /// <value>
    /// The separators paint.
    /// </value>
    IPaint<TDrawingContext>? TicksPaint { get; set; }

    /// <summary>
    /// Gets or sets the separators paint.
    /// </summary>
    /// <value>
    /// The separators paint.
    /// </value>
    IPaint<TDrawingContext>? SubticksPaint { get; set; }

    /// <summary>
    /// Gets or sets the zero paint.
    /// </summary>
    /// <value>
    /// The separators paint.
    /// </value>
    IPaint<TDrawingContext>? ZeroPaint { get; set; }

    /// <summary>
    /// Gets or sets the crosshair paint.
    /// </summary>
    /// <value>
    /// The separators paint.
    /// </value>
    IPaint<TDrawingContext>? CrosshairPaint { get; set; }

    /// <summary>
    /// Gets or sets the crosshair labels paint.
    /// </summary>
    /// <value>
    /// The separators paint.
    /// </value>
    IPaint<TDrawingContext>? CrosshairLabelsPaint { get; set; }

    /// <summary>
    /// Gets or sets the crosshair background.
    /// </summary>
    /// <value>
    /// The separators paint.
    /// </value>
    LvcColor? CrosshairLabelsBackground { get; set; }

    /// <summary>
    /// Gets or sets the crosshair labels padding.
    /// </summary>
    Padding? CrosshairPadding { get; set; }

    /// <summary>
    /// Gets or sets whether the crosshair snaps to nearest series.
    /// </summary>
    bool CrosshairSnapEnabled { get; set; }

    /// <summary>
    /// Invalidates the crosshair visual.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="pointerPosition">The pointer position</param>
    void InvalidateCrosshair(Chart<TDrawingContext> chart, LvcPoint pointerPosition);

    /// <summary>
    /// Clears the crosshair visual.
    /// </summary>
    /// <param name="chart">The chart.</param>
    void ClearCrosshair(Chart<TDrawingContext> chart);
}
