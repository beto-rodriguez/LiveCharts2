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

// Ignore Spelling: animatable

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
    private const float MinLabelSize = 10; // Assume the label size is at least 10px

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
        var location = chart is CartesianChart<TDrawingContext> or PolarChart<TDrawingContext>
            ? _getCartesianTooltipLocation(foundPoints, chart, tooltipSize)
            : _getPieTooltipLocation(foundPoints, chart, tooltipSize);

        var controlSize = chart.ControlSize;

        if (location.Y < 0) location.Y = 0;
        if (location.X < 0) location.X = 0;
        if (location.Y + tooltipSize.Height > controlSize.Height) location.Y = controlSize.Height - tooltipSize.Height;
        if (location.X + tooltipSize.Width > controlSize.Width) location.X = controlSize.Width - tooltipSize.Width;

        return location;
    }

    private static LvcPoint _getCartesianTooltipLocation<TDrawingContext>(
        IEnumerable<ChartPoint> foundPoints,
        Chart<TDrawingContext> chart,
        LvcSize tooltipSize)
            where TDrawingContext : DrawingContext
    {
        var count = 0f;
        var placementContext = new TooltipPlacementContext(chart.TooltipPosition);

        foreach (var point in foundPoints)
        {
            if (point.Context.HoverArea is null) continue;
            point.Context.HoverArea.SuggestTooltipPlacement(placementContext, tooltipSize);
            count++;
        }

        if (count == 0) return new();

        var avrgX = (placementContext.MostRight + placementContext.MostLeft) * 0.5f - tooltipSize.Width * 0.5f;
        var avrgY = (placementContext.MostTop + placementContext.MostBottom) * 0.5f - tooltipSize.Height * 0.5f;

        var position = chart.TooltipPosition;

        var x = avrgX;
        var y = placementContext.MostAutoTop - tooltipSize.Height;
        chart.AutoToolTipsInfo.ToolTipPlacement = placementContext.AutoPopPupPlacement;

        switch (position)
        {
            case TooltipPosition.Top:
                x = avrgX;
                y = placementContext.MostTop - tooltipSize.Height;
                chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Top;
                break;
            case TooltipPosition.Bottom:
                x = avrgX;
                y = placementContext.MostBottom;
                chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Bottom;
                break;
            case TooltipPosition.Left:
                x = placementContext.MostLeft - tooltipSize.Width;
                y = avrgY;
                chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Left;
                break;
            case TooltipPosition.Right:
                x = placementContext.MostRight;
                y = avrgY;
                chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Right;
                break;
            case TooltipPosition.Center:
                x = avrgX;
                y = avrgY;
                chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Top;
                break;
            case TooltipPosition.Hidden:
            case TooltipPosition.Auto:
            default:
                break;
        }

        if (x < 0)
        {
            // the tooltip is out of the left edge
            // we return TooltipPosition.Right
            chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Right;
            return new(placementContext.MostRight, avrgY);
        }

        if (x + tooltipSize.Width > chart.ControlSize.Width)
        {
            // the tooltip is out of the right edge
            // we return TooltipPosition.Left
            chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Left;
            return new(placementContext.MostLeft - tooltipSize.Width, avrgY);
        }

        if (y < 0)
        {
            y += tooltipSize.Height;
            chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Bottom;
        }
        if (y + tooltipSize.Height > chart.ControlSize.Height)
        {
            y -= tooltipSize.Height;
            chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Top;
        }

        return new(x, y);
    }
    private static LvcPoint _getPieTooltipLocation<TDrawingContext>(
        IEnumerable<ChartPoint> foundPoints, Chart<TDrawingContext> chart, LvcSize tooltipSize)
            where TDrawingContext : DrawingContext
    {
        var placementContext = new TooltipPlacementContext(TooltipPosition.Auto);

        foreach (var foundPoint in foundPoints)
        {
            if (foundPoint.Context.HoverArea is null) continue;
            foundPoint.Context.HoverArea.SuggestTooltipPlacement(placementContext, tooltipSize);
        }

        chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Top;

        var p = new LvcPoint(
            placementContext.PieX - tooltipSize.Width * 0.5f,
            placementContext.PieY - tooltipSize.Height);

        if (p.Y < 0)
        {
            chart.AutoToolTipsInfo.ToolTipPlacement = PopUpPlacement.Bottom;

            p = new LvcPoint(
                placementContext.PieX - tooltipSize.Width * 0.5f,
                placementContext.PieY);
        }

        return p;
    }

    /// <summary>
    /// Gets the tick.
    /// </summary>
    /// <param name="axis">The axis.</param>
    /// <param name="controlSize">Size of the control.</param>
    /// <param name="bounds">The bounds.</param>
    /// <returns></returns>
    public static AxisTick GetTick(this ICartesianAxis axis, LvcSize controlSize, Bounds? bounds = null)
    {
        bounds ??= axis.VisibleDataBounds;
        var maxLabelSize = axis.PossibleMaxLabelSize;

        var w = maxLabelSize.Width;
        var h = maxLabelSize.Height;

        var r = Math.Abs(axis.LabelsRotation % 90);

        if (r is >= 20 and <= 70)
        {
            // if the labels are rotated, we assume that they can overlap.
            var d = 0.35f * (float)Math.Sqrt(w * w + h * h);
            w = d;
            h = d;
        }
        else
        {
            // modify the size of the label to avoid overlapping
            // and improve readability.

            const float xGrowFactor = 1.10f;
            if (axis.Orientation == AxisOrientation.X) w *= xGrowFactor;

            const float yGrowFactor = 1.5f;
            if (axis.Orientation == AxisOrientation.Y) h *= yGrowFactor;
        }

        if (w < MinLabelSize) w = MinLabelSize;
        if (h < MinLabelSize) h = MinLabelSize;

        var max = axis.MaxLimit is null ? bounds.Max : axis.MaxLimit.Value;
        var min = axis.MinLimit is null ? bounds.Min : axis.MinLimit.Value;

        AxisLimit.ValidateLimits(ref min, ref max);

        var unit = axis.UnitWidth;

        max /= unit;
        min /= unit;

        var range = max - min;
        if (range == 0) range = min;

        var separations = axis.Orientation == AxisOrientation.Y
            ? Math.Round(controlSize.Height / h, 0)
            : Math.Round(controlSize.Width / w, 0);

        var minimum = range / separations;

        var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

        var residual = minimum / magnitude;
        var tick = residual > 5 ? 10 * magnitude : residual > 2 ? 5 * magnitude : residual > 1 ? 2 * magnitude : magnitude;

        return new AxisTick { Value = tick * unit, Magnitude = magnitude * unit };
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
            ? Math.Round(c / 30, 0)
            : Math.Round(radius / 90, 0);
        var minimum = range / separations;

        var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

        var residual = minimum / magnitude;
        var tick = residual > 5 ? 10 * magnitude : residual > 2 ? 5 * magnitude : residual > 1 ? 2 * magnitude : magnitude;
        return new AxisTick { Value = tick, Magnitude = magnitude };
    }

    /// <summary>
    /// Sets the transition of the given <paramref name="properties"/> to the <paramref name="animation"/>.
    /// </summary>
    /// <param name="animatable">The animatable object.</param>
    /// <param name="animation">The animation.</param>
    /// <param name="properties">
    /// The properties, if this argument is not set then all the animatable properties in the object will use the given animation.
    /// </param>
    public static void Animate(this IAnimatable animatable, Animation animation, params string[]? properties)
    {
        animatable.SetTransition(animation, properties);
        animatable.CompleteTransition(properties);
    }

    /// <summary>
    /// Sets the transition of the given <paramref name="properties"/> to the specified <paramref name="easingFunction"/> and <paramref name="speed"/>.
    /// </summary>
    /// <param name="animatable">The animatable object.</param>
    /// <param name="easingFunction">The animation's easing function.</param>
    /// <param name="speed">The animation's speed.</param>
    /// <param name="properties">
    /// The properties, if this argument is not set then all the animatable properties in the object will use the given animation.
    /// </param>
    public static void Animate(this IAnimatable animatable, Func<float, float>? easingFunction, TimeSpan speed, params string[]? properties)
    {
        Animate(animatable, new Animation(easingFunction, speed), properties);
    }

    /// <summary>
    /// Sets the transition of the given <paramref name="properties"/> to the animations config in the chart,
    /// if the properties are not set, then all the animatable properties in the object will use the given animation.
    /// </summary>
    /// <param name="animatable">The animatable object.</param>
    /// <param name="chart">
    /// The chart, an animation will be built based on the <see cref="Chart{TDrawingContext}.AnimationsSpeed"/>
    /// and <see cref="Chart{TDrawingContext}.EasingFunction"/>.
    /// </param>
    /// <param name="properties">
    /// The properties, if this argument is not set then all the animatable properties in the object will use the given animation.
    /// </param>
    public static void Animate<TDrawingContext>(this IAnimatable animatable, Chart<TDrawingContext> chart, params string[]? properties)
        where TDrawingContext : DrawingContext
    {
        Animate(animatable, new Animation(chart.EasingFunction, chart.AnimationsSpeed), properties);
    }

    /// <summary>
    /// Sets the transition of the given <paramref name="properties"/> to the animations config in the chart
    /// for all the geometries in a <see cref="VisualElement{TDrawingContext}"/>.
    /// </summary>
    /// <typeparam name="TDrawingContext"></typeparam>
    /// <param name="visual">The visual.</param>
    /// <param name="animation">The animation.</param>
    /// <param name="properties">The properties.</param>
    public static void Animate<TDrawingContext>(this VisualElement<TDrawingContext> visual, Animation animation, params string[]? properties)
        where TDrawingContext : DrawingContext
    {
        foreach (var animatable in visual.GetDrawnGeometries())
        {
            if (animatable is null) continue;
            Animate(animatable, animation, properties);
        }
    }

    /// <summary>
    /// Sets the transition of the given <paramref name="properties"/> to the specified <paramref name="easingFunction"/> and <paramref name="speed"/>
    /// for all the geometries in a <see cref="VisualElement{TDrawingContext}"/>.
    /// </summary>
    /// <param name="visual">The visual object.</param>
    /// <param name="easingFunction">The animation's easing function.</param>
    /// <param name="speed">The animation's speed.</param>
    /// <param name="properties">
    /// The properties, if this argument is not set then all the animatable properties in the object will use the given animation.
    /// </param>
    public static void Animate<TDrawingContext>(this VisualElement<TDrawingContext> visual, Func<float, float>? easingFunction, TimeSpan speed, params string[]? properties)
        where TDrawingContext : DrawingContext
    {
        Animate(visual, new Animation(easingFunction, speed), properties);
    }

    /// <summary>
    /// Sets the transition of the given <paramref name="properties"/> to the animations config in the chart,
    /// if the properties are not set, then all the animatable properties in the object will use the given animation.
    /// The transition will be set for all the geometries in a <see cref="VisualElement{TDrawingContext}"/>.
    /// </summary>
    /// <param name="visual">The visual`` object.</param>
    /// <param name="chart">
    /// The chart, an animation will be built based on the <see cref="Chart{TDrawingContext}.AnimationsSpeed"/>
    /// and <see cref="Chart{TDrawingContext}.EasingFunction"/>.
    /// </param>
    /// <param name="properties">
    /// The properties, if this argument is not set then all the animatable properties in the object will use the given animation.
    /// </param>
    public static void Animate<TDrawingContext>(this VisualElement<TDrawingContext> visual, Chart<TDrawingContext> chart, params string[]? properties)
        where TDrawingContext : DrawingContext
    {
        Animate(visual, new Animation(chart.EasingFunction, chart.AnimationsSpeed), properties);
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
        return (series.SeriesProperties & (SeriesProperties.Bar | SeriesProperties.PrimaryAxisVerticalOrientation)) ==
            (SeriesProperties.Bar | SeriesProperties.PrimaryAxisVerticalOrientation);
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
        return (series.SeriesProperties & (SeriesProperties.Bar | SeriesProperties.PrimaryAxisHorizontalOrientation)) ==
            (SeriesProperties.Bar | SeriesProperties.PrimaryAxisHorizontalOrientation);
    }

    /// <summary>
    /// Determines whether is box series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>
    ///   <c>true</c> if [is box series] [the specified series]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsBoxSeries(this ISeries series)
    {
        return (series.SeriesProperties & (SeriesProperties.BoxSeries)) == SeriesProperties.BoxSeries;
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
    /// Determines whether is a pie series.
    /// </summary>
    /// <param name="series">The series.</param>
    public static bool IsPieSeries(this ISeries series)
    {
        return (series.SeriesProperties & SeriesProperties.PieSeries) != 0;
    }

    /// <summary>
    /// Determines whether is bar series.
    /// </summary>
    /// <param name="series">The series.</param>
    /// <returns>
    ///   <c>true</c> if [is bar series] [the specified series]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasSvgGeometry(this ISeries series)
    {
        return (series.SeriesProperties & SeriesProperties.IsSVGPath) != 0;
    }

    /// <summary>
    /// Calculates the tooltips finding strategy based on the series properties.
    /// </summary>
    /// <param name="seriesCollection">The series collection.</param>
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
            : (areAllY
                ? TooltipFindingStrategy.CompareOnlyYTakeClosest
                : TooltipFindingStrategy.CompareAllTakeClosest);
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
            data.IsNextEmpty = true;
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
        public bool IsNextEmpty { get; set; }

        public void GoNext(ChartPoint point)
        {
            Previous = Current;
            Current = Next;
            Next = AfterNext;
            AfterNext = point;
        }
    }
}
