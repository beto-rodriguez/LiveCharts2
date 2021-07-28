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

using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// LiveCharts kerner extensions.
    /// </summary>
    public static class Extensions
    {
        private const double Cf = 3d;

        /// <summary>
        /// Returns the left, top coordinate of the tooltip based on the found points, the position and the tooltip size.
        /// </summary>
        /// <param name="foundPoints"></param>
        /// <param name="position"></param>
        /// <param name="tooltipSize"></param>
        /// <param name="chartSize"></param>
        /// <returns></returns>
        public static PointF? GetCartesianTooltipLocation(
            this IEnumerable<TooltipPoint> foundPoints, TooltipPosition position, SizeF tooltipSize, SizeF chartSize)
        {
            var count = 0f;

            var placementContext = new TooltipPlacementContext();

            foreach (var point in foundPoints)
            {
                if (point.Point.Context.HoverArea is null) continue;
                point.Point.Context.HoverArea.SuggestTooltipPlacement(placementContext);
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
                TooltipPosition.Top => new PointF(avrgX, placementContext.MostTop - tooltipSize.Height),
                TooltipPosition.Bottom => new PointF(avrgX, placementContext.MostBottom),
                TooltipPosition.Left => new PointF(placementContext.MostLeft - tooltipSize.Width, avrgY),
                TooltipPosition.Right => new PointF(placementContext.MostRight, avrgY),
                TooltipPosition.Center => new PointF(avrgX, avrgY),
                TooltipPosition.Hidden => new PointF(),
                _ => new PointF(),
            };
        }

        /// <summary>
        ///  Returns the left, top coordinate of the tooltip based on the found points, the position and the tooltip size.
        /// </summary>
        /// <param name="foundPoints">The found points.</param>
        /// <param name="position">The position.</param>
        /// <param name="tooltipSize">Size of the tooltip.</param>
        /// <returns></returns>
        public static PointF? GetPieTooltipLocation(
            this IEnumerable<TooltipPoint> foundPoints, TooltipPosition position, SizeF tooltipSize)
        {
            var placementContext = new TooltipPlacementContext();
            var found = false;

            foreach (var foundPoint in foundPoints)
            {
                if (foundPoint.Point.Context.HoverArea is null) continue;
                foundPoint.Point.Context.HoverArea.SuggestTooltipPlacement(placementContext);
                found = true;
                break; // we only care about the first one.
            }

            return found ? new PointF(placementContext.PieX, placementContext.PieY) : null;
        }

        /// <summary>
        /// Gets the tick.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="axis">The axis.</param>
        /// <param name="controlSize">Size of the control.</param>
        /// <returns></returns>
        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize)
            where TDrawingContext : DrawingContext
        {
            return GetTick(axis, controlSize, axis.VisibleDataBounds);
        }

        /// <summary>
        /// Gets the tick.
        /// </summary>
        /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
        /// <param name="axis">The axis.</param>
        /// <param name="controlSize">Size of the control.</param>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize, Bounds bounds)
           where TDrawingContext : DrawingContext
        {
            var max = axis.MaxLimit is null ? bounds.Max : axis.MaxLimit.Value;
            var min = axis.MinLimit is null ? bounds.Min : axis.MinLimit.Value;

            var range = max - min;
            var separations = axis.Orientation == AxisOrientation.Y
                ? Math.Round(controlSize.Height / (12 * Cf), 0)
                : Math.Round(controlSize.Width / (20 * Cf), 0);
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
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        /// <exception cref="Exception">At least one property is required when calling {nameof(TransitionateProperties)}</exception>
        public static TransitionBuilder TransitionateProperties(this IAnimatable animatable, params string[] properties)
        {
            return properties is null || properties.Length == 0
                ? throw new Exception($"At least one property is required when calling {nameof(TransitionateProperties)}")
                : new TransitionBuilder(animatable, properties);
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
        /// Adds a point to the specified state.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        /// <param name="state">The state.</param>
        public static void AddToState(this ChartPoint chartPoint, string state)
        {
            chartPoint.Context.Series.AddPointToState(chartPoint, state);
        }

        /// <summary>
        /// Removes a point from the specified state.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        /// <param name="state">The state.</param>
        public static void RemoveFromState(this ChartPoint chartPoint, string state)
        {
            chartPoint.Context.Series.RemovePointFromState(chartPoint, state);
        }

        /// <summary>
        /// Adds a point to the hover state.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        public static void AddToHoverState(this ChartPoint chartPoint)
        {
            chartPoint.Context.Series.AddPointToState(chartPoint, chartPoint.Context.Series.HoverState);
        }

        /// <summary>
        /// Removes a point from the hover state.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        public static void RemoveFromHoverState(this ChartPoint chartPoint)
        {
            chartPoint.Context.Series.RemovePointFromState(chartPoint, chartPoint.Context.Series.HoverState);
        }
    }
}
