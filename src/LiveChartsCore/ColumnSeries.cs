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
    /// <summary>
    /// Defines the data to plot as columns.
    /// </summary>
    public class ColumnSeries<TModel, TVisual, TDrawingContext> : CartesianSeries<TModel, TVisual, TDrawingContext>, IColumnSeries<TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        public ColumnSeries()
            : base (SeriesProperties.Bar | SeriesProperties.VerticalOrientation)
        {
            HoverState = LiveCharts.ColumnSeriesHoverKey;
        }

        public double Pivot { get; set; }
        public double MaxColumnWidth { get; set; } = 30;
        public bool IgnoresColumnPosition { get; set; } = false;

        Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>? IColumnSeries<TDrawingContext>.OnPointCreated
        {
            get => OnPointCreated as Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointCreated = value;
        }

        Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>? IColumnSeries<TDrawingContext>.OnPointAddedToState
        {
            get => OnPointAddedToState as Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointAddedToState = value;
        }

        Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>? IColumnSeries<TDrawingContext>.OnPointRemovedFromState
        {
            get => OnPointRemovedFromState as Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointRemovedFromState = value;
        }

        public override void Measure(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            float uw = xScale.ScaleToUi(1f) - xScale.ScaleToUi(0f);
            float uwm = 0.5f * uw;
            float sw = Stroke?.StrokeWidth ?? 0;
            float p = yScale.ScaleToUi(unchecked((float)Pivot));
             
            var pos = chart.SeriesContext.GetColumnPostion(this);
            var count = chart.SeriesContext.GetColumnSeriesCount();
            float cp = 0f;

            if (!IgnoresColumnPosition && count > 1)
            {
                uw = uw / count;
                uwm = 0.5f * uw;
                cp = (pos - (count/2f)) * uw + uwm;
            }

            if (uw > MaxColumnWidth)
            {
                uw = unchecked((float)MaxColumnWidth);
                uwm = uw / 2f;
            }

            if (Fill != null) chart.Canvas.AddDrawableTask(Fill);
            if (Stroke != null) chart.Canvas.AddDrawableTask(Stroke);

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);
            var ts = OnPointCreated ?? DefaultOnPointCreated;

            foreach (var point in Fetch(chart))
            {
                var x = xScale.ScaleToUi(point.SecondaryValue);
                var y = yScale.ScaleToUi(point.PrimaryValue);
                float b = Math.Abs(y - p);

                if (point.PointContext.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = x - uwm + cp,
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

                var cy = point.PrimaryValue > Pivot ? y : y - b;

                sizedGeometry.X = x - uwm + cp;
                sizedGeometry.Y = cy;
                sizedGeometry.Width = uw;
                sizedGeometry.Height = b;

                point.PointContext.HoverArea = new RectangleHoverArea().SetDimensions(x - uwm + cp, cy, uw, b);
                OnPointMeasured(point, sizedGeometry);
                chart.MeasuredDrawables.Add(sizedGeometry);
            }
        }

        public override DimensinalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(chart, x, y);

            var tick = y.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);

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
                    .WithEasingFunction(EasingFunctions.BounceOut)
                    .WithDuration((long)(chart.AnimationsSpeed.TotalMilliseconds * 1.5)));
        }

        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = (float)LegendShapeSize, Width = (float)LegendShapeSize };
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            var w = LegendShapeSize;
            if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeWidth,
                    Y = strokeClone.StrokeWidth,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize
                };
                w += 2 * strokeClone.StrokeWidth;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
        }
    }
}
