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
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;

namespace LiveChartsCore.Kernel.Providers;

/// <summary>
/// Defines the default data factory.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TDrawingContext"></typeparam>
public class DataFactory<TModel, TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private readonly bool _isTModelChartEntity = false;
    private readonly bool _isValueType = false;
    private readonly Dictionary<object, Dictionary<int, IChartEntity>> _chartIndexEntityMap = new();
    private readonly Dictionary<object, Dictionary<TModel, IChartEntity>> _chartRefEntityMap = new();
    private DimensionalBounds _previousKnownBounds;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataFactory{TModel, TDrawingContext}"/> class.
    /// </summary>
    public DataFactory()
    {
        var bounds = new DimensionalBounds(true);
        _previousKnownBounds = bounds;

        var t = typeof(TModel);
        _isValueType = t.IsValueType;

        _isTModelChartEntity = typeof(IChartEntity).IsAssignableFrom(typeof(TModel));
    }

    /// <summary>
    /// Fetches the points for the specified series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <param name="chart">The chart.</param>
    /// <returns></returns>
    public virtual IEnumerable<ChartPoint> Fetch(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;

        var dataSource = _isTModelChartEntity
            ? EnumerateChartEntities(series, chart)
            : (_isValueType
                ? EnumerateByValEntities(series, chart)
                : EnumerateByRefEntities(series, chart));

        foreach (var value in dataSource)
        {
            if (value is null)
            {
                yield return ChartPoint.Empty;
                continue;
            }

            yield return value.ChartMetadata.ChartPoint ?? ChartPoint.Empty;
        }
    }

    /// <summary>
    /// Disposes a given point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns></returns>
    public virtual void DisposePoint(ChartPoint point)
    {
        if (_isTModelChartEntity) return;

        var canvas = (MotionCanvas<TDrawingContext>)point.Context.Chart.CoreChart.Canvas;

        if (_isValueType)
        {
            _ = _chartIndexEntityMap.TryGetValue(canvas.Sync, out var d);
            var map = d;
            if (map is null) return;
            _ = map.Remove(point.Context.Index);
        }
        else
        {
            _ = _chartRefEntityMap.TryGetValue(canvas.Sync, out var d);
            var map = d;
            if (map is null) return;
            var src = (TModel?)point.Context.DataSource;
            if (src is null) return;
            _ = map.Remove(src);
        }
    }

    /// <summary>
    /// Disposes the data provider from the given chart.
    /// </summary>
    /// <param name="chart"></param>
    public virtual void Dispose(IChart chart)
    {
        if (_isTModelChartEntity) return;

        if (_isValueType)
        {
            var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;
            _ = _chartIndexEntityMap.Remove(canvas.Sync);
        }
        else
        {
            var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;
            _ = _chartRefEntityMap.Remove(canvas.Sync);
        }
    }

    /// <summary>
    /// Gets the Cartesian bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <param name="plane1">The x.</param>
    /// <param name="plane2">The y.</param>
    /// <returns></returns>
    public virtual SeriesBounds GetCartesianBounds(
        Chart<TDrawingContext> chart,
        IChartSeries<TDrawingContext> series,
        IPlane plane1,
        IPlane plane2)
    {
        var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());

        var xMin = plane1.MinLimit ?? double.MinValue;
        var xMax = plane1.MaxLimit ?? double.MaxValue;
        var yMin = plane2.MinLimit ?? double.MinValue;
        var yMax = plane2.MaxLimit ?? double.MaxValue;

        var hasData = false;

        var bounds = new DimensionalBounds();

        ChartPoint? previous = null;

        foreach (var point in series.Fetch(chart))
        {
            var primary = point.PrimaryValue;
            var secondary = point.SecondaryValue;
            var tertiary = point.TertiaryValue;

            if (stack is not null) primary = stack.StackPoint(point);

            bounds.PrimaryBounds.AppendValue(primary);
            bounds.SecondaryBounds.AppendValue(secondary);
            bounds.TertiaryBounds.AppendValue(tertiary);

            if (primary >= yMin && primary <= yMax && secondary >= xMin && secondary <= xMax)
            {
                bounds.VisiblePrimaryBounds.AppendValue(primary);
                bounds.VisibleSecondaryBounds.AppendValue(secondary);
                bounds.VisibleTertiaryBounds.AppendValue(tertiary);
            }

            if (previous is not null)
            {
                var dx = Math.Abs(previous.SecondaryValue - point.SecondaryValue);
                var dy = Math.Abs(previous.PrimaryValue - point.PrimaryValue);
                if (dx < bounds.SecondaryBounds.MinDelta) bounds.SecondaryBounds.MinDelta = dx;
                if (dy < bounds.PrimaryBounds.MinDelta) bounds.PrimaryBounds.MinDelta = dy;
            }

            previous = point;
            hasData = true;
        }

        return !hasData
            ? new SeriesBounds(_previousKnownBounds, true)
            : new SeriesBounds(_previousKnownBounds = bounds, false);
    }

    /// <summary>
    /// Gets the financial bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns></returns>
    public virtual SeriesBounds GetFinancialBounds(
        CartesianChart<TDrawingContext> chart,
        IChartSeries<TDrawingContext> series,
        ICartesianAxis x,
        ICartesianAxis y)
    {
        var xMin = x.MinLimit ?? double.MinValue;
        var xMax = x.MaxLimit ?? double.MaxValue;
        var yMin = y.MinLimit ?? double.MinValue;
        var yMax = y.MaxLimit ?? double.MaxValue;

        var hasData = false;

        var bounds = new DimensionalBounds();
        ChartPoint? previous = null;
        foreach (var point in series.Fetch(chart))
        {
            var primaryMax = point.PrimaryValue;
            var primaryMin = point.QuinaryValue;
            var secondary = point.SecondaryValue;
            var tertiary = point.TertiaryValue;

            bounds.PrimaryBounds.AppendValue(primaryMax);
            bounds.PrimaryBounds.AppendValue(primaryMin);
            bounds.SecondaryBounds.AppendValue(secondary);
            bounds.TertiaryBounds.AppendValue(tertiary);

            if (primaryMax >= yMin && primaryMin <= yMax && secondary >= xMin && secondary <= xMax)
            {
                bounds.VisiblePrimaryBounds.AppendValue(primaryMax);
                bounds.VisiblePrimaryBounds.AppendValue(primaryMin);
                bounds.VisibleSecondaryBounds.AppendValue(secondary);
                bounds.VisibleTertiaryBounds.AppendValue(tertiary);
            }

            if (previous is not null)
            {
                var dx = Math.Abs(previous.SecondaryValue - point.SecondaryValue);
                var dy = Math.Abs(previous.PrimaryValue - point.PrimaryValue);
                if (dx < bounds.SecondaryBounds.MinDelta) bounds.SecondaryBounds.MinDelta = dx;
                if (dy < bounds.PrimaryBounds.MinDelta) bounds.PrimaryBounds.MinDelta = dy;
            }

            previous = point;
            hasData = true;
        }

        return !hasData
            ? new SeriesBounds(_previousKnownBounds, true)
            : new SeriesBounds(_previousKnownBounds = bounds, false);
    }

    /// <summary>
    /// Gets the pie bounds.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="series">The series.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException">Unexpected null stacker</exception>
    public virtual SeriesBounds GetPieBounds(
        PieChart<TDrawingContext> chart, IPieSeries<TDrawingContext> series)
    {
        var stack = chart.SeriesContext.GetStackPosition(series, series.GetStackGroup());
        if (stack is null) throw new NullReferenceException("Unexpected null stacker");

        var bounds = new DimensionalBounds();
        var hasData = false;

        foreach (var point in series.Fetch(chart))
        {
            _ = stack.StackPoint(point);
            bounds.PrimaryBounds.AppendValue(point.PrimaryValue);
            bounds.SecondaryBounds.AppendValue(point.SecondaryValue);
            bounds.TertiaryBounds.AppendValue(series.Pushout > series.HoverPushout ? series.Pushout : series.HoverPushout);
            hasData = true;
        }

        if (!hasData)
        {
            bounds.PrimaryBounds.AppendValue(0);
            bounds.SecondaryBounds.AppendValue(0);
            bounds.TertiaryBounds.AppendValue(0);
        }

        return new SeriesBounds(bounds, false);
    }

    private IEnumerable<IChartEntity> EnumerateChartEntities(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;
        var entities = (IEnumerable<IChartEntity>)series.Values;
        var index = 0;

        foreach (var entity in entities)
        {
            if (entity is null) continue;

            entity.ChartMetadata.ChartPoint ??= new ChartPoint(chart.View, series);
            entity.ChartMetadata.ChartPoint.Context.DataSource = entity;
            entity.ChartMetadata.ChartPoint.Context.Index = index;
            entity.ChartMetadata.EntityIndex = index++;
            entity.ChartMetadata.ChartPoint.Coordinate = entity.ChartMetadata.Coordinate;

            yield return entity;
        }
    }

    private IEnumerable<IChartEntity?> EnumerateByValEntities(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;

        var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;
        var mapper = series.Mapping ?? LiveCharts.CurrentSettings.GetMap<TModel>();
        var index = 0;

        _ = _chartIndexEntityMap.TryGetValue(canvas.Sync, out var d);
        if (d is null)
        {
            d = new Dictionary<int, IChartEntity>();
            _chartIndexEntityMap[canvas.Sync] = d;
        }
        var IndexEntityMap = d;

        foreach (var item in series.Values)
        {
            if (!IndexEntityMap.TryGetValue(index, out var entity))
            {
                IndexEntityMap[index] = entity = new ChartEntity();
            }

            entity.ChartMetadata.ChartPoint ??= new ChartPoint(chart.View, series);
            entity.ChartMetadata.ChartPoint.Context.DataSource = item;
            entity.ChartMetadata.EntityIndex = index;
            entity.ChartMetadata.ChartPoint.Context.Index = index++;

            mapper(item, entity.ChartMetadata.ChartPoint);

            yield return entity;
        }
    }

    private IEnumerable<IChartEntity?> EnumerateByRefEntities(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;

        var canvas = (MotionCanvas<TDrawingContext>)chart.Canvas;
        var mapper = series.Mapping ?? LiveCharts.CurrentSettings.GetMap<TModel>();
        var index = 0;

        _ = _chartRefEntityMap.TryGetValue(canvas.Sync, out var d);
        if (d is null)
        {
            d = new Dictionary<TModel, IChartEntity>();
            _chartRefEntityMap[canvas.Sync] = d;
        }
        var IndexEntityMap = d;

        foreach (var item in series.Values)
        {
            if (item is null)
            {
                yield return new ChartEntity();
                continue;
            }

            if (!IndexEntityMap.TryGetValue(item, out var entity))
            {
                IndexEntityMap[item] = entity = new ChartEntity();
            }

            entity.ChartMetadata.ChartPoint ??= new ChartPoint(chart.View, series);
            entity.ChartMetadata.ChartPoint.Context.DataSource = item;
            entity.ChartMetadata.EntityIndex = index;
            entity.ChartMetadata.ChartPoint.Context.Index = index++;

            mapper(item, entity.ChartMetadata.ChartPoint);

            yield return entity;
        }
    }
}
