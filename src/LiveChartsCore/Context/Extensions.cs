// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore.Context
{
    public static class Extensions
    {
        private const double cf = 3d;

        /// <summary>
        /// Returns the left, top coordinate of the tooltip based on the found points, the position and the tooltip size.
        /// </summary>
        /// <param name="foundPoints"></param>
        /// <param name="position"></param>
        /// <param name="tooltipSize"></param>
        /// <returns></returns>
        public static PointF? GetCartesianTooltipLocation(
            this IEnumerable<TooltipPoint> foundPoints, TooltipPosition position, SizeF tooltipSize)
        {
            float count = 0f;

            var placementContext = new TooltipPlacementContext();

            foreach (var point in foundPoints)
            {
                if (point.Point.Context.HoverArea == null) continue;
                point.Point.Context.HoverArea.SuggestTooltipPlacement(placementContext);
                count++;
            }

            if (count == 0) return null;

            var avrgX = ((placementContext.MostRight + placementContext.MostLeft) / 2f) - tooltipSize.Width * 0.5f;
            var avrgY = ((placementContext.MostTop + placementContext.MostBottom) / 2f) - tooltipSize.Height * 0.5f;

            switch (position)
            {
                case TooltipPosition.Top: return new PointF(avrgX, placementContext.MostTop - tooltipSize.Height);
                case TooltipPosition.Bottom: return new PointF(avrgX, placementContext.MostBottom);
                case TooltipPosition.Left: return new PointF(placementContext.MostLeft - tooltipSize.Width, avrgY);
                case TooltipPosition.Right: return new PointF(placementContext.MostRight, avrgY);
                case TooltipPosition.Center: return new PointF(avrgX, avrgY);
                default: throw new NotImplementedException();
            }
        }

        public static PointF? GetPieTooltipLocation(
            this IEnumerable<TooltipPoint> foundPoints, TooltipPosition position, SizeF tooltipSize) 
        {
            var placementContext = new TooltipPlacementContext();
            var found = false;

            foreach (var foundPoint in foundPoints)
            {
                if (foundPoint.Point.Context.HoverArea == null) continue;
                foundPoint.Point.Context.HoverArea.SuggestTooltipPlacement(placementContext);
                found = true;
                break; // we only care about the first one.
            }

            return found ? new PointF(placementContext.PieX, placementContext.PieY) : null;
        }

        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize)
            where TDrawingContext : DrawingContext
        {
            return GetTick(axis, controlSize, axis.DataBounds);
        }

        public static AxisTick GetTick<TDrawingContext>(this IAxis<TDrawingContext> axis, SizeF controlSize, Bounds bounds)
           where TDrawingContext : DrawingContext
        {
            var range = bounds.max - bounds.min;
            var separations = axis.Orientation == AxisOrientation.Y
                ? Math.Round(controlSize.Height / (12 * cf), 0)
                : Math.Round(controlSize.Width / (20 * cf), 0);
            var minimum = range / separations;

            var magnitude = Math.Pow(10, Math.Floor(Math.Log(minimum) / Math.Log(10)));

            var residual = minimum / magnitude;
            double tick;

            if (residual > 5) tick = 10 * magnitude;
            else if (residual > 2) tick = 5 * magnitude;
            else if (residual > 1) tick = 2 * magnitude;
            else tick = magnitude;

            return new AxisTick { Value = tick, Magnitude = magnitude };
        }

        public static TransitionBuilder TransitionateProperties(this IAnimatable animatable, params string[] properties)
        {
            if (properties == null || properties.Length == 0)
                throw new Exception($"At least one property is required when calling {nameof(TransitionateProperties)}"); 

            return new TransitionBuilder(animatable, properties);
        }

        public static bool IsBarSeries(this ISeries series)
            => (series.SeriesProperties & SeriesProperties.Bar) != 0;

        public static bool IsColumnSeries(this ISeries series)
            => (series.SeriesProperties & (SeriesProperties.Bar | SeriesProperties.VerticalOrientation)) != 0;

        public static bool IsRowSeries(this ISeries series)
            => (series.SeriesProperties & (SeriesProperties.Bar | SeriesProperties.HorizontalOrientation)) != 0;

        public static bool IsStackedSeries(this ISeries series)
            => (series.SeriesProperties & (SeriesProperties.Stacked)) != 0;

        public static bool IsVerticalSeries(this ISeries series)
            => (series.SeriesProperties & (SeriesProperties.VerticalOrientation)) != 0;

        public static bool IsHorizontalSeries(this ISeries series)
            => (series.SeriesProperties & (SeriesProperties.HorizontalOrientation)) != 0;

        public static void AddToState(this IChartPoint chartPoint, string state) 
        {
            chartPoint.Context.Series.AddPointToState(chartPoint, state);
        }

        public static void RemoveFromState(this IChartPoint chartPoint, string state) 
        {
            chartPoint.Context.Series.RemovePointFromState(chartPoint, state);
        }

        public static void AddToHoverState(this IChartPoint chartPoint)
        {
            chartPoint.Context.Series.AddPointToState(chartPoint, chartPoint.Context.Series.HoverState);
        }

        public static void RemoveFromHoverState(this IChartPoint chartPoint)
        {
            chartPoint.Context.Series.RemovePointFromState(chartPoint, chartPoint.Context.Series.HoverState);
        }
    }
}
