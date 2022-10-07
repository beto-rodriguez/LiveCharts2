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
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="ChartSeries{TModel, TVisual, TLabel, TDrawingContext}" />
/// <seealso cref="IDisposable" />
/// <seealso cref="ICartesianSeries{TDrawingContext}" />
public abstract class CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>
    : ChartSeries<TModel, TVisual, TLabel, TDrawingContext>, ICartesianSeries<TDrawingContext>
    where TDrawingContext : DrawingContext
    where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
    where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    private int _scalesXAt;
    private int _scalesYAt;
    private DataLabelsPosition _labelsPosition;
    private LvcPoint? _labelsTranslate = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    /// <param name="properties">The series properties.</param>
    protected CartesianSeries(SeriesProperties properties) : base(properties) { }

    /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.ScalesXAt"/>
    public int ScalesXAt { get => _scalesXAt; set { _scalesXAt = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.ScalesYAt"/>
    public int ScalesYAt { get => _scalesYAt; set { _scalesYAt = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.DataLabelsPosition"/>
    public DataLabelsPosition DataLabelsPosition { get => _labelsPosition; set { _labelsPosition = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.DataLabelsTranslate"/>
    public LvcPoint? DataLabelsTranslate { get => _labelsTranslate; set { _labelsTranslate = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, ICartesianAxis, ICartesianAxis)"/>
    public virtual SeriesBounds GetBounds(
        CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var rawBounds = DataFactory.GetCartesianBounds(chart, this, secondaryAxis, primaryAxis);
        if (rawBounds.HasData) return rawBounds;

        var rawBaseBounds = rawBounds.Bounds;

        var tickPrimary = primaryAxis.GetTick(chart.ControlSize, rawBaseBounds.VisiblePrimaryBounds);
        var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, rawBaseBounds.VisibleSecondaryBounds);

        var ts = tickSecondary.Value * DataPadding.X;
        var tp = tickPrimary.Value * DataPadding.Y;

        // using different methods for both primary and secondary axis seems to be the best solution
        // if this the following 2 lines needs to be changed again, please ensure that the following test passes:
        // https://github.com/beto-rodriguez/LiveCharts2/issues/522
        // https://github.com/beto-rodriguez/LiveCharts2/issues/642

        if (rawBaseBounds.VisibleSecondaryBounds.Delta == 0) ts = secondaryAxis.UnitWidth * DataPadding.X;
        if (rawBaseBounds.VisiblePrimaryBounds.Delta == 0) tp = rawBaseBounds.VisiblePrimaryBounds.Max * 0.25f;

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
                Max = rawBaseBounds.PrimaryBounds.Max + rpo * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.PrimaryBounds.Min - rpo * secondaryAxis.UnitWidth,
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
                Max = rawBaseBounds.VisiblePrimaryBounds.Max + rpo * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.VisiblePrimaryBounds.Min - rpo * secondaryAxis.UnitWidth
            },
            TertiaryBounds = rawBaseBounds.TertiaryBounds,
            VisibleTertiaryBounds = rawBaseBounds.VisibleTertiaryBounds
        };

        if (GetIsInvertedBounds())
        {
            var tempSb = dimensionalBounds.SecondaryBounds;
            var tempPb = dimensionalBounds.PrimaryBounds;
            var tempVsb = dimensionalBounds.VisibleSecondaryBounds;
            var tempVpb = dimensionalBounds.VisiblePrimaryBounds;

            dimensionalBounds.SecondaryBounds = tempPb;
            dimensionalBounds.PrimaryBounds = tempSb;
            dimensionalBounds.VisibleSecondaryBounds = tempVpb;
            dimensionalBounds.VisiblePrimaryBounds = tempVsb;
        }

        return new SeriesBounds(dimensionalBounds, false);
    }

    /// <summary>
    /// Gets the geometry size to calculate the series bounds.
    /// </summary>
    /// <returns>The geometry requested size.</returns>
    protected virtual double GetRequestedGeometrySize()
    {
        return 0;
    }

    /// <summary>
    /// Gets the requested secondary offset [normalized as proportion of the axis' unit width].
    /// </summary>
    /// <returns>The offset.</returns>
    protected virtual double GetRequestedSecondaryOffset()
    {
        return 0;
    }

    /// <summary>
    /// Gets the requested secondary offset [normalized as proportion of the axis' unit width].
    /// </summary>
    /// <returns>The offset</returns>
    protected virtual double GetRequestedPrimaryOffset()
    {
        return 0;
    }

    /// <summary>
    /// Gets whether the requested bounds are inverted.
    /// </summary>
    /// <returns>The offset</returns>
    protected virtual bool GetIsInvertedBounds()
    {
        return false;
    }

    /// <summary>
    /// Deletes the series from the user interface.
    /// </summary>
    /// <param name="chart"></param>
    /// <inheritdoc cref="ISeries.SoftDeleteOrDispose(IChartView)" />
    public override void SoftDeleteOrDispose(IChartView chart)
    {
        var core = ((ICartesianChartView<TDrawingContext>)chart).Core;

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

        OnVisibilityChanged();
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
