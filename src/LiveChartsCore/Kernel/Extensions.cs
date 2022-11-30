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
using System.Linq;
using System.Runtime.CompilerServices;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.Kernel;

/// <summary>
/// LiveCharts kernel extensions.
/// </summary>
public static class Extensions
{
    private const double Cf = 3d;

    private static readonly Type s_nullableType = typeof(Nullable<>);

    /// <summary>
    /// Calculates the tooltip location.
    /// </summary>
    /// <typeparam name="TDrawingContext"></typeparam>
    /// <param name="foundPoints">The points.</param>
    /// <param name="tooltipSize">The tooltip size.</param>
    /// <param name="chart">The chart.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static LvcPoint GetTooltipLocation<TDrawingContext>(
        this IEnumerable<ChartPoint> foundPoints,
        LvcSize tooltipSize,
        Chart<TDrawingContext> chart)
            where TDrawingContext : DrawingContext
    {
        LvcPoint? location = null;

        if (chart is CartesianChart<TDrawingContext> or PolarChart<TDrawingContext>)
            location = _getCartesianTooltipLocation(foundPoints, chart.TooltipPosition, tooltipSize, chart.DrawMarginSize);
        if (chart is PieChart<TDrawingContext>)
            location = _getPieTooltipLocation(foundPoints, tooltipSize);

        if (location is null) throw new Exception("location not supported");

        var chartSize = chart.DrawMarginSize;

        var x = location.Value.X;
        var y = location.Value.Y;
        var w = chartSize.Width;
        var h = chartSize.Height;

        if (x + tooltipSize.Width > w) x = w - tooltipSize.Width;
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (y + tooltipSize.Height > h) y = h - tooltipSize.Height;

        return new LvcPoint(x, y);
    }

    private static LvcPoint? _getCartesianTooltipLocation(
        IEnumerable<ChartPoint> foundPoints, TooltipPosition position, LvcSize tooltipSize, LvcSize chartSize)
    {
        var count = 0f;

        var placementContext = new TooltipPlacementContext();

        foreach (var point in foundPoints)
        {
            if (point.Context.HoverArea is null) continue;
            point.Context.HoverArea.SuggestTooltipPlacement(placementContext);
            count++;
        }

        if (count == 0) return null;

        if (placementContext.MostBottom > chartSize.Height - tooltipSize.Height)
            placementContext.MostBottom = chartSize.Height - tooltipSize.Height;
        if (placementContext.MostTop < 0) placementContext.MostTop = 0;

        var avrgX = (placementContext.MostRight + placementContext.MostLeft) / 2f - tooltipSize.Width * 0.5f;
        var avrgY = (placementContext.MostTop + placementContext.MostBottom) / 2f - tooltipSize.Height * 0.5f;

        return position switch
        {
            TooltipPosition.Top => new LvcPoint(avrgX, placementContext.MostTop - tooltipSize.Height),
            TooltipPosition.Bottom => new LvcPoint(avrgX, placementContext.MostBottom),
            TooltipPosition.Left => new LvcPoint(placementContext.MostLeft - tooltipSize.Width, avrgY),
            TooltipPosition.Right => new LvcPoint(placementContext.MostRight, avrgY),
            TooltipPosition.Center => new LvcPoint(avrgX, avrgY),
            TooltipPosition.Hidden => new LvcPoint(),
            _ => new LvcPoint(),
        };
    }
    private static LvcPoint? _getPieTooltipLocation(
        IEnumerable<ChartPoint> foundPoints, LvcSize tooltipSize)
    {
        var placementContext = new TooltipPlacementContext();
        var found = false;

        foreach (var foundPoint in foundPoints)
        {
            if (foundPoint.Context.HoverArea is null) continue;
            foundPoint.Context.HoverArea.SuggestTooltipPlacement(placementContext);
            found = true;
            break; // we only care about the first one.
        }

        return found
            ? new LvcPoint(placementContext.PieX - tooltipSize.Width * 0.5f, placementContext.PieY - tooltipSize.Height * 0.5f)
            : null;
    }

    /// <summary>
    /// Gets the tick.
    /// </summary>
    /// <param name="axis">The axis.</param>
    /// <param name="controlSize">Size of the control.</param>
    /// <param name="bounds">The bounds.</param>
    /// <param name="maxLabelSize">The max label size.</param>
    /// <returns></returns>
    public static AxisTick GetTick(this ICartesianAxis axis, LvcSize controlSize, Bounds? bounds = null, LvcSize? maxLabelSize = null)
    {
        bounds ??= axis.VisibleDataBounds;

        var w = (maxLabelSize?.Width ?? 0d) * 0.60;
        if (w < 20 * Cf) w = 20 * Cf;

        var h = maxLabelSize?.Height ?? 0d;
        if (h < 12 * Cf) h = 12 * Cf;

        var max = axis.MaxLimit is null ? bounds.Max : axis.MaxLimit.Value;
        var min = axis.MinLimit is null ? bounds.Min : axis.MinLimit.Value;

        var range = max - min;
        if (range == 0) range = min;

        var separations = axis.Orientation == AxisOrientation.Y
            ? Math.Round(controlSize.Height / h, 0)
            : Math.Round(controlSize.Width / w, 0);

        var minimum = range / separations;

        var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

        var residual = minimum / magnitude;
        var tick = residual > 5 ? 10 * magnitude : residual > 2 ? 5 * magnitude : residual > 1 ? 2 * magnitude : magnitude;

        return new AxisTick { Value = tick, Magnitude = magnitude };
    }

    /// <summary>
    /// Gets the tick.
    /// </summary>
    /// <param name="axis">The axis.</param>
    /// <param name="chart">The chart.</param>
    /// <param name="bounds">The bounds.</param>
    /// <returns></returns> 
    public static AxisTick GetTick<TDrawingContext>(this IPolarAxis axis, PolarChart<TDrawingContext> chart, Bounds? bounds = null)
        where TDrawingContext : DrawingContext
    {
        bounds ??= axis.VisibleDataBounds;

        var max = axis.MaxLimit is null ? bounds.Max : axis.MaxLimit.Value;
        var min = axis.MinLimit is null ? bounds.Min : axis.MinLimit.Value;

        var controlSize = chart.ControlSize;
        var minD = controlSize.Width < controlSize.Height ? controlSize.Width : controlSize.Height;
        var radius = minD - chart.InnerRadius;
        var c = minD * chart.TotalAnge / 360;

        var range = max - min;
        var separations = axis.Orientation == PolarAxisOrientation.Angle
            ? Math.Round(c / (10 * Cf), 0)
            : Math.Round(radius / (30 * Cf), 0);
        var minimum = range / separations;

        var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

        var residual = minimum / magnitude;
        var tick = residual > 5 ? 10 * magnitude : residual > 2 ? 5 * magnitude : residual > 1 ? 2 * magnitude : magnitude;
        return new AxisTick { Value = tick, Magnitude = magnitude };
    }

    /// <summary>
    /// Creates a transition builder for the specified properties.
    /// </summary>
    /// <param name="animatable">The animatable.</param>
    /// <param name="properties">The properties, use null to apply the transition to all the properties.</param>
    /// <returns>The builder</returns>
    public static TransitionBuilder TransitionateProperties(this IAnimatable animatable, params string[]? properties)
    {
        return new TransitionBuilder(animatable, properties);
    }

    /// <summary>
    /// Determines whether is bar series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>
    ///   <c>true</c> if [is bar series] [the specified series]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBarSeries(this ISeries series)
    {
        return (series.SeriesProperties & SeriesProperties.Bar) != 0;
    }

    /// <summary>
    /// Determines whether is column series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>
    ///   <c>true</c> if [is column series] [the specified series]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsColumnSeries(this ISeries series)
    {
        return (series.SeriesProperties & (SeriesProperties.Bar | SeriesProperties.PrimaryAxisVerticalOrientation)) != 0;
    }

    /// <summary>
    /// Determines whether is row series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>
    ///   <c>true</c> if [is row series] [the specified series]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsRowSeries(this ISeries series)
    {
        return (series.SeriesProperties & (SeriesProperties.Bar | SeriesProperties.PrimaryAxisHorizontalOrientation)) != 0;
    }

    /// <summary>
    /// Determines whether is stacked series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>
    ///   <c>true</c> if [is stacked series] [the specified series]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsStackedSeries(this ISeries series)
    {
        return (series.SeriesProperties & (SeriesProperties.Stacked)) != 0;
    }

    /// <summary>
    /// Determines whether is vertical series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>
    ///   <c>true</c> if [is vertical series] [the specified series]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsVerticalSeries(this ISeries series)
    {
        return (series.SeriesProperties & (SeriesProperties.PrimaryAxisVerticalOrientation)) != 0;
    }

    /// <summary>
    /// Determines whether is horizontal series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>
    ///   <c>true</c> if [is horizontal series] [the specified series]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsHorizontalSeries(this ISeries series)
    {
        return (series.SeriesProperties & (SeriesProperties.PrimaryAxisHorizontalOrientation)) != 0;
    }

    /// <summary>
    /// Determines whether is a financial series.
    /// </summary>
    /// <param name="series"></param>
    /// <returns></returns>
    public static bool IsFinancialSeries(this ISeries series)
    {
        return (series.SeriesProperties & SeriesProperties.Financial) != 0;
    }

    /// <summary>
    /// Calculates the tooltips finding strategy based on the series properties.
    /// </summary>
    /// <param name="seriesCollection">The series collection</param>
    /// <returns></returns>
    public static TooltipFindingStrategy GetTooltipFindingStrategy(this IEnumerable<ISeries> seriesCollection)
    {
        var areAllX = true;
        var areAllY = true;

        foreach (var series in seriesCollection)
        {
            areAllX = areAllX && (series.SeriesProperties & SeriesProperties.PrefersXStrategyTooltips) != 0;
            areAllY = areAllY && (series.SeriesProperties & SeriesProperties.PrefersYStrategyTooltips) != 0;
        }

        return areAllX
            ? TooltipFindingStrategy.CompareOnlyXTakeClosest
            : (areAllY ? TooltipFindingStrategy.CompareOnlyYTakeClosest : TooltipFindingStrategy.CompareAllTakeClosest);
    }

    /// <summary>
    /// Finds the closest point to the specified location [in pixels].
    /// </summary>
    /// <param name="points">The points to look in to.</param>
    /// <param name="point">The location in pixels.</param>
    /// <returns></returns>
    public static ChartPoint<TModel, TVisual, TLabel>? FindClosestTo<TModel, TVisual, TLabel>(
        this IEnumerable<ChartPoint> points, LvcPoint point)
    {
        var closest = FindClosestTo(points, point);
        return closest is null
            ? null
            : new ChartPoint<TModel, TVisual, TLabel>(closest);
    }

    /// <summary>
    /// Finds the closest point to the specified location [in pixels].
    /// </summary>
    /// <param name="points">The points to look into.</param>
    /// <param name="point">The location in pixels.</param>
    /// <returns></returns>
    public static ChartPoint? FindClosestTo(this IEnumerable<ChartPoint> points, LvcPoint point)
    {
        var fp = new LvcPoint((float)point.X, (float)point.Y);

        return points
            .Select(p => new
            {
                distance = p.DistanceTo(fp),
                point = p
            })
            .OrderBy(p => p.distance)
            .FirstOrDefault()?.point;
    }

    /// <summary>
    /// Finds the closest visual to the specified location [in pixels].
    /// </summary>
    /// <typeparam name="T">The type of the drawing context.</typeparam>
    /// <param name="source">The visuals to look into.</param>
    /// <param name="point">The location in pixels.</param>
    /// <returns></returns>
    public static VisualElement<T>? FindClosestTo<T>(this IEnumerable<VisualElement<T>> source, LvcPoint point)
        where T : DrawingContext
    {
        return source.Select(visual =>
        {
            var location = visual.GetTargetLocation();
            var size = visual.GetTargetSize();

            return new
            {
                distance = Math.Sqrt(
                    Math.Pow(point.X - (location.X + size.Width * 0.5), 2) +
                    Math.Pow(point.Y - (location.Y + size.Height * 0.5), 2)),
                visual
            };
        })
        .OrderBy(p => p.distance)
        .FirstOrDefault()?.visual;
    }

    /// <summary>
    /// Gets a scaler for the given axis with the measured bounds (the target, the final dimension of the chart).
    /// </summary>
    /// <typeparam name="TDrawingContext"></typeparam>
    /// <param name="axis"></param>
    /// <param name="chart"></param>
    /// <returns></returns>
    public static Scaler GetNextScaler<TDrawingContext>(this ICartesianAxis axis, CartesianChart<TDrawingContext> chart)
        where TDrawingContext : DrawingContext
    {
        return new Scaler(chart.DrawMarginLocation, chart.DrawMarginSize, axis);
    }

    /// <summary>
    /// Gets a scaler that is built based on the dimensions of the chart at a given time, the scaler is built based on the
    /// animations that are happening in the chart at the moment this method is called.
    /// </summary>
    /// <typeparam name="TDrawingContext"></typeparam>
    /// <param name="axis"></param>
    /// <param name="chart"></param>
    /// <returns></returns>
    public static Scaler? GetActualScaler<TDrawingContext>(this ICartesianAxis axis, CartesianChart<TDrawingContext> chart)
        where TDrawingContext : DrawingContext
    {
        return !axis.ActualBounds.HasPreviousState
            ? null
            : new Scaler(
                chart.ActualBounds.Location,
                chart.ActualBounds.Size,
                axis,
                new Bounds
                {
                    Max = axis.ActualBounds.MaxVisibleBound,
                    Min = axis.ActualBounds.MinVisibleBound
                });
    }

    /// <summary>
    /// Returns an enumeration with only the first element.
    /// </summary>
    /// <typeparam name="T">The source type.</typeparam>
    /// <typeparam name="T1">The target type.</typeparam>
    /// <param name="source">The source enumeration.</param>
    /// <param name="predicate">The mapping predicate.</param>
    /// <returns></returns>
    public static IEnumerable<T1> SelectFirst<T, T1>(this IEnumerable<T> source, Func<T, T1> predicate)
    {
        foreach (var item in source)
        {
            yield return predicate(item);
            yield break;
        }
    }

    /// <summary>
    /// Gets the point for the given view.
    /// </summary>
    /// <param name="dictionary">The points dictionary.</param>
    /// <param name="view">The view.</param>
    /// <returns></returns>
    public static ChartPoint? GetPointForView(this Dictionary<IChartView, ChartPoint> dictionary, IChartView view)
    {
        return dictionary.TryGetValue(view, out var point) ? point : null;
    }

    /// <summary>
    /// Splits an enumerable of chartpoints by each null gap.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <param name="onDeleteNullPoint">Called when a point was deleted.</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<ChartPoint>> SplitByNullGaps(
        this IEnumerable<ChartPoint> points,
        Action<ChartPoint> onDeleteNullPoint)
    {
        using var builder = new GapsBuilder(points.GetEnumerator());
        while (!builder.Finished) yield return YieldReturnUntilNextNullChartPoint(builder, onDeleteNullPoint);
    }

    /// <summary>
    /// Builds a enumerator with the necessary data to build an Spline.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    internal static IEnumerable<SplineData> AsSplineData(this IEnumerable<ChartPoint> source)
    {
        using var e = source.Where(x => !x.IsEmpty).GetEnumerator();

        if (!e.MoveNext()) yield break;
        var data = new SplineData(e.Current);

        if (!e.MoveNext())
        {
            yield return data;
            yield break;
        }

        data.GoNext(e.Current);

        while (e.MoveNext())
        {
            yield return data;
            data.IsFirst = false;
            data.GoNext(e.Current);
        }

        data.IsFirst = false;
        yield return data;

        data.GoNext(data.Next);
        yield return data;
    }

    /// <summary>
    /// Returns <see langword="true" /> when the given type is either a reference type or of type <see cref="Nullable{T}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool CanBeNull(Type type)
    {
        return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == s_nullableType);
    }

    private static IEnumerable<ChartPoint> YieldReturnUntilNextNullChartPoint(
        GapsBuilder builder,
        Action<ChartPoint> onDeleteNullPoint)
    {
        while (builder.Enumerator.MoveNext())
        {
            if (builder.Enumerator.Current.IsEmpty)
            {
                var wasEmpty = builder.IsEmpty;
                builder.IsEmpty = true;
                onDeleteNullPoint(builder.Enumerator.Current);
                if (!wasEmpty) yield break; // if there are no points then do not return an empty enumerable...
            }
            else
            {
                yield return builder.Enumerator.Current;
                builder.IsEmpty = false;
            }
        }

        builder.Finished = true;
    }

    private class GapsBuilder : IDisposable
    {
        public GapsBuilder(IEnumerator<ChartPoint> enumerator)
        {
            Enumerator = enumerator;
        }

        public IEnumerator<ChartPoint> Enumerator { get; }

        public bool IsEmpty { get; set; } = true;

        public bool Finished { get; set; } = false;

        public void Dispose()
        {
            Enumerator.Dispose();
        }
    }

    internal class SplineData
    {
        public SplineData(ChartPoint start)
        {
            Previous = start;
            Current = start;
            Next = start;
            AfterNext = start;
        }

        public ChartPoint Previous { get; set; }

        public ChartPoint Current { get; set; }

        public ChartPoint Next { get; set; }

        public ChartPoint AfterNext { get; set; }

        public bool IsFirst { get; set; } = true;

        public void GoNext(ChartPoint point)
        {
            Previous = Current;
            Current = Next;
            Next = AfterNext;
            AfterNext = point;
        }
    }
}
