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

using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;

namespace LiveChartsCore
{
    public class ColumnSeries<TModel, TVisual, TDrawingContext>: BarSeries<TModel, TVisual, TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        public ColumnSeries()
            : base(SeriesProperties.Bar | SeriesProperties.VerticalOrientation)
        {
        }

        public override void Measure(
           CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var secondaryScale = new ScaleContext(drawLocation, drawMarginSize, secondaryAxis.Orientation, secondaryAxis.DataBounds);
            var primaryScale = new ScaleContext(drawLocation, drawMarginSize, primaryAxis.Orientation, primaryAxis.DataBounds);

            float uw = secondaryScale.ScaleToUi(1f) - secondaryScale.ScaleToUi(0f);
            float uwm = 0.5f * uw;
            float sw = Stroke?.StrokeWidth ?? 0;
            float p = primaryScale.ScaleToUi(unchecked((float)Pivot));

            var pos = chart.SeriesContext.GetColumnPostion(this);
            var count = chart.SeriesContext.GetColumnSeriesCount();
            float cp = 0f;

            if (!IgnoresBarPosition && count > 1)
            {
                uw = uw / count;
                uwm = 0.5f * uw;
                cp = (pos - (count / 2f)) * uw + uwm;
            }

            if (uw > MaxBarWidth)
            {
                uw = unchecked((float)MaxBarWidth);
                uwm = uw / 2f;
            }

            if (Fill != null) chart.Canvas.AddDrawableTask(Fill);
            if (Stroke != null) chart.Canvas.AddDrawableTask(Stroke);

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);
            var ts = OnPointCreated ?? DefaultOnPointCreated;

            foreach (var point in Fetch(chart))
            {
                var primary = primaryScale.ScaleToUi(point.PrimaryValue);
                var secondary = secondaryScale.ScaleToUi(point.SecondaryValue);                
                float b = Math.Abs(primary - p);

                if (point.PointContext.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = secondary - uwm + cp,
                        Y = p,
                        Width = uw,
                        Height = 0
                    };

                    ts(r, chart.View);
                    r.CompleteAllTransitions();

                    point.PointContext.Visual = r;
                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                }

                var sizedGeometry = point.PointContext.Visual;

                var cy = point.PrimaryValue > Pivot ? primary : primary - b;

                sizedGeometry.X = secondary - uwm + cp;
                sizedGeometry.Y = cy;
                sizedGeometry.Width = uw;
                sizedGeometry.Height = b;

                point.PointContext.HoverArea = new RectangleHoverArea().SetDimensions(secondary - uwm + cp, cy, uw, b);
                OnPointMeasured(point, sizedGeometry);
                chart.MeasuredDrawables.Add(sizedGeometry);
            }
        }

        public override DimensinalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var baseBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);

            var tick = secondaryAxis.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);

            return new DimensinalBounds
            {
                SecondaryBounds = new Bounds
                {
                    Max = baseBounds.SecondaryBounds.Max + 0.5,
                    Min = baseBounds.SecondaryBounds.Min - 0.5
                },
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tick.Value,
                    min = baseBounds.PrimaryBounds.min - tick.Value
                }
            };
        }

        protected virtual void DefaultOnPointCreated(TVisual visual, IChartView<TDrawingContext> chart)
        {
            visual
                .TransitionateProperties(nameof(visual.X), nameof(visual.Width))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));

            visual
                .TransitionateProperties(nameof(visual.Y), nameof(visual.Height))
                .WithAnimation(animation => animation
                    .WithDuration((long)(chart.AnimationsSpeed.TotalMilliseconds * 1.5))
                    .WithEasingFunction(EasingFunctions.BounceOut));
        }
    }
}
