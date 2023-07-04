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

// Ignore Spelling: Quinary

using System;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines a point in a chart.
/// </summary>
public class ChartPoint
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPoint"/> class.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <param name="entity">The entity.</param>
    public ChartPoint(IChartView chart, ISeries series, IChartEntity entity)
    {
        Context = new ChartPointContext(chart, series, entity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartPoint"/> class.
    /// </summary>
    /// <param name="point">The point.</param>
    protected ChartPoint(ChartPoint point) : this(point.Context.Chart, point.Context.Series, point.Context.Entity)
    { }

    private ChartPoint()
    {
        Context = new ChartPointContext();
    }

    /// <summary>
    /// Gets a new instance of an empty chart point.
    /// </summary>
    public static ChartPoint Empty => new() { Coordinate = Coordinate.Empty };

    /// <summary>
    /// Gets the position of the point the collection that was used when the point was drawn.
    /// </summary>
    public int Index => Context.Entity.MetaData?.EntityIndex ?? 0;

    /// <summary>
    /// Gets or sets the coordinate.
    /// </summary>
    public Coordinate Coordinate
    {
        get => Context.Entity.Coordinate;
        set => Context.Entity.Coordinate = value;
    }

    /// <summary>
    /// Gets or a value indicating whether this instance is empty, LivveCharts will ignore the point in the chart.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty => Context.Entity.Coordinate.IsEmpty;

    /// <summary>
    /// Gets or sets the primary value.
    /// </summary>
    /// <value>
    /// The primary value.
    /// </value>
    [Obsolete($"Instead set the {nameof(Coordinate)} of the point.")]
    public double PrimaryValue
    {
        get => Context.Entity.Coordinate.PrimaryValue;
        set => SetCoordinate(primary: value);
    }

    /// <summary>
    /// Gets or sets the secondary value.
    /// </summary>
    /// <value>
    /// The secondary value.
    /// </value>
    [Obsolete($"Use {nameof(Coordinate)} instead.")]
    public double SecondaryValue
    {
        get => Context.Entity.Coordinate.SecondaryValue;
        set => SetCoordinate(secondary: value);
    }

    /// <summary>
    /// Gets or sets the tertiary value.
    /// </summary>
    /// <value>
    /// The tertiary value.
    /// </value>
    [Obsolete($"Use {nameof(Coordinate)} instead.")]
    public double TertiaryValue
    {
        get => Context.Entity.Coordinate.TertiaryValue;
        set => SetCoordinate(tertiary: value);
    }

    /// <summary>
    /// Gets or sets the quaternary value.
    /// </summary>
    /// <value>
    /// The quaternary value.
    /// </value>
    [Obsolete($"Use {nameof(Coordinate)} instead.")]
    public double QuaternaryValue
    {
        get => Context.Entity.Coordinate.QuaternaryValue;
        set => SetCoordinate(quaternary: value);
    }

    /// <summary>
    /// Gets or sets the quinary value.
    /// </summary>
    /// <value>
    /// The quinary value.
    /// </value>
    [Obsolete($"Use {nameof(Coordinate)} instead.")]
    public double QuinaryValue
    {
        get => Context.Entity.Coordinate.QuinaryValue;
        set => SetCoordinate(quinary: value);
    }

    /// <summary>
    /// Gets or sets the stacked value, if the point do not belongs to a stacked series then this property is null.
    /// </summary>
    public StackedValue? StackedValue { get; set; }

    /// <summary>
    /// Gets the point as data label.
    /// </summary>
    /// <value>
    /// As tooltip string.
    /// </value>
    public string AsDataLabel => Context.Series.GetDataLabelText(this) ?? string.Empty;

    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>
    /// The context.
    /// </value>
    public ChartPointContext Context { get; }

    internal bool RemoveOnCompleted { get; set; }

    /// <summary>
    /// Gets the distance to a given point.
    /// </summary>
    /// <param name="point">The point to calculate the distance to.</param>
    /// <returns>The distance in pixels.</returns>
    public double DistanceTo(LvcPoint point)
    {
        return Context.HoverArea?.DistanceTo(point) ?? double.NaN;
    }

    private void SetCoordinate(
        double primary = double.NaN, double secondary = double.NaN, double tertiary = double.NaN,
        double quaternary = double.NaN, double quinary = double.NaN)
    {
        // This is a method that allows previous versions of LiveCharts to map the entity to the chart coordinate
        // you should not use the setters of PrimaryValue, SecondaryValue, etc. instead set the Coordinate property.
        var current = Coordinate;

        if (double.IsNaN(primary)) primary = current.PrimaryValue;
        if (double.IsNaN(secondary)) secondary = current.SecondaryValue;
        if (double.IsNaN(tertiary)) tertiary = current.TertiaryValue;
        if (double.IsNaN(quaternary)) quaternary = current.QuaternaryValue;
        if (double.IsNaN(quinary)) quinary = current.QuinaryValue;

        Coordinate = new Coordinate(secondary, primary, tertiary, quaternary, quinary);
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
    public ChartPoint(ChartPoint point) : base(point)
    {
        StackedValue = point.StackedValue;
        Context.DataSource = point.Context.DataSource;
        Context.Visual = point.Context.Visual;
        Context.Label = point.Context.Label;
        Context.HoverArea = point.Context.HoverArea;
        Context.AdditionalVisuals = point.Context.AdditionalVisuals;
    }

    /// <summary>
    /// Gets the model, this is the actual object that represents the point in the chart.
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
