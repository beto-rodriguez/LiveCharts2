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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines a Cartesian series.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <seealso cref="Series{TModel, TVisual, TLabel}" />
/// <seealso cref="IDisposable" />
/// <seealso cref="ICartesianSeries" />
/// <remarks>
/// Initializes a new instance of the <see cref="CartesianSeries{TModel, TVisual, TLabel}"/> class.
/// </remarks>
/// <param name="properties">The series properties.</param>
/// <param name="values">The values.</param>
public abstract class CartesianSeries<TModel, TVisual, TLabel>(
    SeriesProperties properties,
    IReadOnlyCollection<TModel>? values)
        : Series<TModel, TVisual, TLabel>(properties, values), ICartesianSeries
            where TVisual : DrawnGeometry, new()
            where TLabel : BaseLabelGeometry, new()
{
    private LvcPoint? _labelsTranslate = null;
    private Func<ChartPoint, string>? _xTooltipLabelFormatter;
    private Func<ChartPoint, string>? _yTooltipLabelFormatter;

    /// <inheritdoc cref="ICartesianSeries.ScalesXAt"/>
    public int ScalesXAt { get; set => SetProperty(ref field, value); }

    /// <inheritdoc cref="ICartesianSeries.ScalesYAt"/>
    public int ScalesYAt { get; set => SetProperty(ref field, value); }

    /// <inheritdoc cref="ICartesianSeries.DataLabelsPosition"/>
    public DataLabelsPosition DataLabelsPosition { get; set => SetProperty(ref field, value); }

    /// <inheritdoc cref="ICartesianSeries.DataLabelsTranslate"/>
    public LvcPoint? DataLabelsTranslate { get => _labelsTranslate; set => SetProperty(ref _labelsTranslate, value); }

    /// <summary>
    /// Gets or sets the tool tip label formatter for the X axis, this function will build the label when a point in this series 
    /// is shown inside a tool tip.
    /// </summary>
    /// <value>
    /// The tool tip label formatter.
    /// </value>
    public Func<ChartPoint<TModel, TVisual, TLabel>, string>? XToolTipLabelFormatter
    {
        get => _xTooltipLabelFormatter;
        // hack #040425
        set => ((ICartesianSeries)this).XToolTipLabelFormatter = value is null ? null : p => value(ConvertToTypedChartPoint(p));
    }

    Func<ChartPoint, string>? ICartesianSeries.XToolTipLabelFormatter
    {
        get => _xTooltipLabelFormatter;
        set => SetProperty(ref _xTooltipLabelFormatter, value);
    }

    /// <summary>
    /// Gets or sets the tool tip label formatter for the Y axis, this function will build the label when a point in this series 
    /// is shown inside a tool tip.
    /// </summary>
    /// <value>
    /// The tool tip label formatter.
    /// </value>
    public Func<ChartPoint<TModel, TVisual, TLabel>, string>? YToolTipLabelFormatter
    {
        get => _yTooltipLabelFormatter;
        // hack #040425
        set => ((ICartesianSeries)this).YToolTipLabelFormatter = value is null ? null : p => value(ConvertToTypedChartPoint(p));
    }

    Func<ChartPoint, string>? ICartesianSeries.YToolTipLabelFormatter
    {
        get => _yTooltipLabelFormatter;
        set => SetProperty(ref _yTooltipLabelFormatter, value);
    }

    /// <inheritdoc cref="ICartesianSeries.ClippingMode"/>
    public ClipMode ClippingMode { get; set => SetProperty(ref field, value); } = ClipMode.XY;

    /// <inheritdoc cref="ICartesianSeries.GetBounds(Chart, ICartesianAxis, ICartesianAxis)"/>
    public virtual SeriesBounds GetBounds(
        Chart chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var rawBounds = DataFactory.GetCartesianBounds(chart, this, secondaryAxis, primaryAxis);
        if (rawBounds.HasData) return rawBounds;

        var rawBaseBounds = rawBounds.Bounds;

        var tickPrimary = primaryAxis.GetTick(chart.ControlSize, rawBaseBounds.VisiblePrimaryBounds);
        var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, rawBaseBounds.VisibleSecondaryBounds);

        var ts = tickSecondary.Value * DataPadding.X;
        var tp = tickPrimary.Value * DataPadding.Y;

        var rgs = GetRequestedGeometrySize();
        var rso = GetRequestedSecondaryOffset();
        var rpo = GetRequestedPrimaryOffset();

        var dimensionalBounds = new DimensionalBounds
        {
            SecondaryBounds = new Bounds
            {
                Max = rawBaseBounds.SecondaryBounds.Max + rso * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.SecondaryBounds.Min - rso * secondaryAxis.UnitWidth,
                MinDelta = rawBaseBounds.SecondaryBounds.MinDelta,
                PaddingMax = ts,
                PaddingMin = ts,
                RequestedGeometrySize = rgs
            },
            PrimaryBounds = new Bounds
            {
                Max = rawBaseBounds.PrimaryBounds.Max + rpo * primaryAxis.UnitWidth,
                Min = rawBaseBounds.PrimaryBounds.Min - rpo * primaryAxis.UnitWidth,
                MinDelta = rawBaseBounds.PrimaryBounds.MinDelta,
                PaddingMax = tp,
                PaddingMin = tp,
                RequestedGeometrySize = rgs
            },
            VisibleSecondaryBounds = new Bounds
            {
                Max = rawBaseBounds.VisibleSecondaryBounds.Max + rso * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.VisibleSecondaryBounds.Min - rso * secondaryAxis.UnitWidth,
            },
            VisiblePrimaryBounds = new Bounds
            {
                Max = rawBaseBounds.VisiblePrimaryBounds.Max + rpo * primaryAxis.UnitWidth,
                Min = rawBaseBounds.VisiblePrimaryBounds.Min - rpo * primaryAxis.UnitWidth
            },
            TertiaryBounds = rawBaseBounds.TertiaryBounds,
            VisibleTertiaryBounds = rawBaseBounds.VisibleTertiaryBounds
        };

        return new SeriesBounds(dimensionalBounds, false);
    }

    /// <inheritdoc cref="ISeries.GetPrimaryToolTipText(ChartPoint)"/>
    public override string? GetPrimaryToolTipText(ChartPoint point)
    {
        string? label = null;

        if (YToolTipLabelFormatter is not null)
            label = YToolTipLabelFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));

        if (label is null)
        {
            var cc = (CartesianChartEngine)point.Context.Chart.CoreChart;
            var cs = (ICartesianSeries)point.Context.Series;

            var ax = cc.YAxes[cs.ScalesYAt];

            var c = point.Coordinate;

            label = ax.Labels is not null
                ? Labelers.BuildNamedLabeler(ax.Labels)(c.PrimaryValue)
                : ax.Labeler(c.PrimaryValue);
        }

        return label;
    }

    /// <inheritdoc cref="ISeries.GetSecondaryToolTipText(ChartPoint)"/>
    public override string? GetSecondaryToolTipText(ChartPoint point)
    {
        string? label = null;

        if (XToolTipLabelFormatter is not null)
            label = XToolTipLabelFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));

        if (label is null)
        {
            var cc = (CartesianChartEngine)point.Context.Chart.CoreChart;
            var cs = (ICartesianSeries)point.Context.Series;

            var ax = cc.XAxes[cs.ScalesXAt];

            var c = point.Coordinate;

            label = ax.Labels is not null
                ? Labelers.BuildNamedLabeler(ax.Labels)(c.SecondaryValue)
                : (ax.Labeler != Labelers.Default
                    ? ax.Labeler(c.SecondaryValue)
                    : LiveCharts.IgnoreToolTipLabel);
        }

        return label;
    }

    /// <summary>
    /// Gets the clip rectangle for the series.
    /// </summary>
    /// <param name="cartesianChart">The cartesian chart.</param>
    /// <returns></returns>
    protected virtual LvcRectangle GetClipRectangle(CartesianChartEngine cartesianChart) =>
        Clipping.GetClipRectangle(ClippingMode, cartesianChart);

    /// <summary>
    /// Gets the geometry size to calculate the series bounds.
    /// </summary>
    /// <returns>The geometry requested size.</returns>
    protected virtual double GetRequestedGeometrySize() => 0;

    /// <summary>
    /// Gets the requested secondary offset [normalized as proportion of the axis' unit width].
    /// </summary>
    /// <returns>The offset.</returns>
    protected virtual double GetRequestedSecondaryOffset() => 0;

    /// <summary>
    /// Gets the requested secondary offset [normalized as proportion of the axis' unit width].
    /// </summary>
    /// <returns>The offset</returns>
    protected virtual double GetRequestedPrimaryOffset() => 0;

    /// <summary>
    /// Deletes the series from the user interface.
    /// </summary>
    /// <param name="chart"></param>
    /// <inheritdoc cref="ISeries.SoftDeleteOrDispose(IChartView)" />
    public override void SoftDeleteOrDispose(IChartView chart)
    {
        var core = ((ICartesianChartView)chart).Core;

        var secondaryAxis = core.XAxes.Length > ScalesXAt ? core.XAxes[ScalesXAt] : null;
        var primaryAxis = core.YAxes.Length > ScalesYAt ? core.YAxes[ScalesYAt] : null;

        var secondaryScale = secondaryAxis is null
            ? new Scaler()
            : new Scaler(core.DrawMarginLocation, core.DrawMarginSize, secondaryAxis);
        var primaryScale = primaryAxis is null
            ? new Scaler()
            : new Scaler(core.DrawMarginLocation, core.DrawMarginSize, primaryAxis);

        var deleted = new List<ChartPoint>();
        foreach (var point in everFetched)
        {
            if (point.Context.Chart != chart) continue;

            SoftDeleteOrDisposePoint(point, primaryScale, secondaryScale);
            deleted.Add(point);
        }

        foreach (var pt in GetPaintTasks())
        {
            if (pt is not null) core.Canvas.RemovePaintTask(pt);
        }

        foreach (var item in deleted) _ = everFetched.Remove(item);
    }

    /// <summary>
    /// Softs the delete point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="primaryScale">The primary scale.</param>
    /// <param name="secondaryScale">The secondary scale.</param>
    /// <returns></returns>
    protected internal abstract void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale);

    /// <summary>
    /// Gets the label position.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="labelSize">Size of the label.</param>
    /// <param name="position">The position.</param>
    /// <param name="seriesProperties">The series properties.</param>
    /// <param name="isGreaterThanPivot">if set to <c>true</c> [is greater than pivot].</param>
    /// <param name="drawMarginLocation">The draw margin location.</param>
    /// <param name="drawMarginSize">the draw margin size</param>
    /// <returns></returns>
    protected internal virtual LvcPoint GetLabelPosition(
        float x,
        float y,
        float width,
        float height,
        LvcSize labelSize,
        DataLabelsPosition position,
        SeriesProperties seriesProperties,
        bool isGreaterThanPivot,
        LvcPoint drawMarginLocation,
        LvcSize drawMarginSize)
    {
        var middleX = (x + x + width) * 0.5f;
        var middleY = (y + y + height) * 0.5f;

        return position switch
        {
            DataLabelsPosition.Middle
                => new LvcPoint(middleX, middleY),
            DataLabelsPosition.Top
                => new LvcPoint(middleX, y - labelSize.Height * 0.5f),
            DataLabelsPosition.Bottom
                => new LvcPoint(middleX, y + height + labelSize.Height * 0.5f),
            DataLabelsPosition.Left
                => new LvcPoint(x - labelSize.Width * 0.5f, middleY),
            DataLabelsPosition.Right
                => new LvcPoint(x + width + labelSize.Width * 0.5f, middleY),
            DataLabelsPosition.End =>
                (seriesProperties & SeriesProperties.PrimaryAxisHorizontalOrientation) == SeriesProperties.PrimaryAxisHorizontalOrientation
                    ? (isGreaterThanPivot
                        ? new LvcPoint(x + width + labelSize.Width * 0.5f, middleY)
                        : new LvcPoint(x - labelSize.Width * 0.5f, middleY))
                    : (isGreaterThanPivot
                        ? new LvcPoint(middleX, y - labelSize.Height * 0.5f)
                        : new LvcPoint(middleX, y + height + labelSize.Height * 0.5f)),
            DataLabelsPosition.Start =>
                 (seriesProperties & SeriesProperties.PrimaryAxisHorizontalOrientation) == SeriesProperties.PrimaryAxisHorizontalOrientation
                    ? (isGreaterThanPivot
                        ? new LvcPoint(x - labelSize.Width * 0.5f, middleY)
                        : new LvcPoint(x + width + labelSize.Width * 0.5f, middleY))
                    : (isGreaterThanPivot
                        ? new LvcPoint(middleX, y + height + labelSize.Height * 0.5f)
                        : new LvcPoint(middleX, y - labelSize.Height * 0.5f)),
            _ => throw new Exception("Position not supported"),
        };
    }
}
