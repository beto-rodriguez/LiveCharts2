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
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines a point in a chart.
/// </summary>
public class ChartPoint : ICoordinate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPoint"/> class.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    public ChartPoint(IChartView chart, ISeries series)
    {
        Context = new ChartPointContext(chart, series);
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is null.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is null; otherwise, <c>false</c>.
    /// </value>
    [Obsolete($"Renamed to {nameof(IsEmpty)}")]
    public bool IsNull { get => IsEmpty; set => IsEmpty = value; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is empty.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    [Obsolete($"Replaced by {nameof(Coordinate)}.{nameof(Coordinate.Empty)}")]
    public bool IsEmpty { get; set; }

    /// <summary>
    /// Gets or sets the primary value.
    /// </summary>
    /// <value>
    /// The primary value.
    /// </value>
    public double PrimaryValue
    {
        get => Coordinate.PrimaryValue;
        [Obsolete($"Coordinate setters are obsolete, please set the {nameof(Coordinate)} property instead.")]
        set => OnCoordinateChanged(
            new Coordinate(value, Coordinate.SecondaryValue, Coordinate.TertiaryValue, Coordinate.QuaternaryValue, Coordinate.QuinaryValue));
    }

    /// <summary>
    /// Gets or sets the secondary value.
    /// </summary>
    /// <value>
    /// The secondary value.
    /// </value>
    public double SecondaryValue
    {
        get => Coordinate.SecondaryValue;
        [Obsolete($"Coordinate setters are obsolete, please set the {nameof(Coordinate)} property instead.")]
        set => OnCoordinateChanged(
            new Coordinate(Coordinate.PrimaryValue, value, Coordinate.TertiaryValue, Coordinate.QuaternaryValue, Coordinate.QuinaryValue));
    }

    /// <summary>
    /// Gets or sets the tertiary value.
    /// </summary>
    /// <value>
    /// The tertiary value.
    /// </value>
    public double TertiaryValue
    {
        get => Coordinate.TertiaryValue;
        [Obsolete($"Coordinate setters are obsolete, please set the {nameof(Coordinate)} property instead.")]
        set => OnCoordinateChanged(
            new Coordinate(Coordinate.PrimaryValue, Coordinate.SecondaryValue, value, Coordinate.QuaternaryValue, Coordinate.QuinaryValue));
    }

    /// <summary>
    /// Gets or sets the quaternary value.
    /// </summary>
    /// <value>
    /// The quaternary value.
    /// </value>
    public double QuaternaryValue
    {
        get => Coordinate.QuaternaryValue;
        [Obsolete($"Coordinate setters are obsolete, please set the {nameof(Coordinate)} property instead.")]
        set => OnCoordinateChanged(
            new Coordinate(Coordinate.PrimaryValue, Coordinate.SecondaryValue, Coordinate.TertiaryValue, value, Coordinate.QuinaryValue));
    }

    /// <summary>
    /// Gets or sets the quinary value.
    /// </summary>
    /// <value>
    /// The quinary value.
    /// </value>
    public double QuinaryValue
    {
        get => Coordinate.QuinaryValue;
        [Obsolete($"Coordinate setters are obsolete, please set the {nameof(Coordinate)} property instead.")]
        set => OnCoordinateChanged(
            new Coordinate(Coordinate.PrimaryValue, Coordinate.SecondaryValue, Coordinate.TertiaryValue, Coordinate.QuaternaryValue, value));
    }

    /// <summary>
    /// Gets or sets the stacked value, if the point do not belongs to a stacked series then this property is null.
    /// </summary>
    public StackedValue? StackedValue { get; set; }

    /// <summary>
    /// Gets the point as tooltip string.
    /// </summary>
    /// <value>
    /// As tooltip string.
    /// </value>
    public string AsTooltipString => Context.Series.GetTooltipText(this);

    /// <summary>
    /// Gets the point as data label.
    /// </summary>
    /// <value>
    /// As tooltip string.
    /// </value>
    public string AsDataLabel => Context.Series.GetDataLabelText(this);

    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>
    /// The context.
    /// </value>
    public ChartPointContext Context { get; }

    /// <inheritdoc cref="ICoordinate.Coordinate" />
    public Coordinate Coordinate { get; set; }

    /// <summary>
    /// Gets the distance to a given point.
    /// </summary>
    /// <param name="point">The point to calculate the distance to.</param>
    /// <returns>The distance in pixels.</returns>
    public double DistanceTo(LvcPoint point)
    {
        return Context.HoverArea?.DistanceTo(point) ?? double.NaN;
    }

    private void OnCoordinateChanged(Coordinate coordinate)
    {
        Coordinate = coordinate;
    }
}

/// <summary>
/// Defines a point in a chart with known visual and label types.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
public class ChartPoint<TModel, TVisual, TLabel> : ChartPoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPoint{TModel, TVisual, TLabel}"/> class.
    /// </summary>
    /// <param name="point">The point.</param>
    public ChartPoint(ChartPoint point) : base(point.Context.Chart, point.Context.Series)
    {
        IsNull = point.IsNull;
        Coordinate = point.Coordinate;
        StackedValue = point.StackedValue;
        Context.Index = point.Context.Index;
        Context.DataSource = point.Context.DataSource;
        Context.Visual = point.Context.Visual;
        Context.Label = point.Context.Label;
        Context.HoverArea = point.Context.HoverArea;
    }

    /// <summary>
    /// Gets the model.
    /// </summary>
    /// <value>
    /// The model.
    /// </value>
    public TModel? Model => (TModel?)Context.DataSource;

    /// <summary>
    /// Gets the visual.
    /// </summary>
    /// <value>
    /// The visual.
    /// </value>
    public TVisual? Visual => (TVisual?)Context.Visual;

    /// <summary>
    /// Gets the label.
    /// </summary>
    /// <value>
    /// The label.
    /// </value>
    public TLabel? Label => (TLabel?)Context.Label;
}
