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
    public class StackedColumnSeries<TModel, TVisual, TDrawingContext> : CartesianSeries<TModel, TVisual, TDrawingContext>
        where TVisual : class, ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly static float pivot = 0;
        private int stackGroup;

        public StackedColumnSeries()
            : base(SeriesProperties.Bar | SeriesProperties.Stacked | SeriesProperties.VerticalOrientation)
        {

        }

        public int StackGroup { get => stackGroup; set => stackGroup = value; }
        public double MaxColumnWidth { get; set; } = 30;
        public TransitionsSetterDelegate<ISizedGeometry<TDrawingContext>>? TransitionsSetter { get; set; }

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
            float p = yScale.ScaleToUi(pivot);

            var pos = chart.SeriesContext.GetStackedColumnPostion(this);
            var count = chart.SeriesContext.GetStackedColumnSeriesCount();
            float cp = 0f;

            if (count > 1)
            {
                uw = uw / count;
                uwm = 0.5f * uw;
                cp = (pos - (count / 2f)) * uw + uwm;
            }

            if (uw > MaxColumnWidth)
            {
                uw = unchecked((float)MaxColumnWidth);
                uwm = uw / 2f;
            }

            if (Fill != null) chart.Canvas.AddPaintTask(Fill);
            if (Stroke != null) chart.Canvas.AddPaintTask(Stroke);

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);
            var ts = TransitionsSetter ?? SetDefaultTransitions;

            var stacker = chart.SeriesContext.GetStackPosition(this, GetStackGroup());
            if (stacker == null) throw new NullReferenceException("Unexpected null stacker");

            foreach (var point in Fetch(chart))
            {
                var x = xScale.ScaleToUi(point.SecondaryValue);

                if (point.PointContext.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = x - uwm + cp,
                        Y = p,
                        Width = uw,
                        Height = 0
                    };

                    ts(r, chartAnimation);

                    point.PointContext.Visual = r;
                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                }

                var sizedGeometry = point.PointContext.Visual;

                var sy = stacker.GetStack(point);
                var yi = yScale.ScaleToUi(sy.Start);
                var yj = yScale.ScaleToUi(sy.End);

                sizedGeometry.X = x - uwm + cp;
                sizedGeometry.Y = yj;
                sizedGeometry.Width = uw;
                sizedGeometry.Height = yi - yj;

                point.PointContext.HoverArea = new RectangleHoverArea().SetDimensions(x - uwm + cp, yj, uw, yi - yj);
                OnPointMeasured(point, sizedGeometry);
                chart.MeasuredDrawables.Add(sizedGeometry);
            }
        }

        public override BiDimensinalBounds GetBounds(
           CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(chart, x, y);

            var tick = y.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);

            return new BiDimensinalBounds
            {
                SecondaryBounds = new Bounds
                {
                    Max = baseBounds.SecondaryBounds.Max + 0.5,
                    Min = baseBounds.SecondaryBounds.Min - 0.5
                },
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tick.Value,
                    min = 0
                }
            };
        }

        protected virtual void SetDefaultTransitions(ISizedGeometry<TDrawingContext> visual, Animation defaultAnimation)
        {
            visual
                .DefinePropertyTransitions(nameof(visual.X), nameof(visual.Width))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();

            visual
                .DefinePropertyTransitions(nameof(visual.Y), nameof(visual.Height))
                .WithAnimation(animation => animation
                    .WithEasingFunction(EasingFunctions.BounceOut)
                    .WithDuration((long)(defaultAnimation.duration * 1.5))
                    .RepeatTimes(defaultAnimation.repeatTimes))
                .CompleteCurrentTransitions();
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

        public override int GetStackGroup() => stackGroup;
    }
}
