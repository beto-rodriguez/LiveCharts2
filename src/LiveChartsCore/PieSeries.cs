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
    public class PieSeries<TModel, TVisual, TLabel, TDrawingContext>
        : DrawableSeries<TModel, TVisual, TLabel, TDrawingContext>, IDisposable, IPieSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IDoughnutVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        public PieSeries() : base(SeriesProperties.PieSeries | SeriesProperties.Stacked)
        {
            HoverState = LiveCharts.PieSeriesHoverKey;
        }

        public double Pushout { get; set; } = 5; // pixels
        public double InnerRadius { get; set; } = 0; // pixels
        public double MaxOuterRadius { get; set; } = 1; // 0 - 1
        public double HoverPushout { get; set; } = 20; // pixels

        public void Measure(PieChart<TDrawingContext> chart)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var minDimension = drawMarginSize.Width < drawMarginSize.Height ? drawMarginSize.Width : drawMarginSize.Height;

            var maxPushout = (float)chart.PushoutBounds.Max;
            var pushout = (float)Pushout;
            var innerRadius = (float)InnerRadius;
            var maxOuterRadius = (float)MaxOuterRadius;

            minDimension = minDimension - (Stroke?.StrokeThickness ?? 0) * 2 - maxPushout * 2;
            minDimension *= maxOuterRadius;

            if (Fill != null) chart.Canvas.AddDrawableTask(Fill);
            if (Stroke != null) chart.Canvas.AddDrawableTask(Stroke);

            var cx = drawLocation.X + drawMarginSize.Width * 0.5f;
            var cy = drawLocation.Y + drawMarginSize.Height * 0.5f;

            var stacker = chart.SeriesContext.GetStackPosition(this, GetStackGroup());
            if (stacker == null) throw new NullReferenceException("Unexpected null stacker");

            foreach (var point in Fetch(chart))
            {
                if (point.IsNull)
                {
                    if (point.Context.Visual != null)
                    {
                        point.Context.Visual.CenterX = drawLocation.X + drawMarginSize.Width * 0.5f;
                        point.Context.Visual.CenterY = drawLocation.Y + drawMarginSize.Height * 0.5f;
                        point.Context.Visual.X = cx;
                        point.Context.Visual.Y = cy;
                        point.Context.Visual.Width = 0;
                        point.Context.Visual.Height = 0;
                        point.Context.Visual.SweepAngle = 0;
                        point.Context.Visual.StartAngle = 0;
                        point.Context.Visual.PushOut = 0;
                        point.Context.Visual.InnerRadius = 0;
                        point.Context.Visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                if (point.Context.Visual == null)
                {
                    var p = new TVisual
                    {
                        CenterX = drawLocation.X + drawMarginSize.Width * 0.5f,
                        CenterY = drawLocation.Y + drawMarginSize.Height * 0.5f,
                        X = cx,
                        Y = cy,
                        Width = 0,
                        Height = 0,
                        SweepAngle = 0,
                        StartAngle = 0,
                        PushOut = 0,
                        InnerRadius = 0
                    };

                    point.Context.Visual = p;
                    OnPointCreated(point);
                    p.CompleteAllTransitions();

                    if (Fill != null) Fill.AddGeometyToPaintTask(p);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(p);
                }

                var dougnutGeometry = point.Context.Visual;

                var stack = stacker.GetStack(point);
                var stackedValue = stack.Start;
                var total = stack.Total;

                dougnutGeometry.PushOut = pushout;
                dougnutGeometry.CenterX = drawLocation.X + drawMarginSize.Width * 0.5f;
                dougnutGeometry.CenterY = drawLocation.Y + drawMarginSize.Height * 0.5f;
                dougnutGeometry.X = (drawMarginSize.Width - minDimension) * 0.5f;
                dougnutGeometry.Y = (drawMarginSize.Height - minDimension) * 0.5f;
                dougnutGeometry.Width = minDimension;
                dougnutGeometry.Height = minDimension;
                dougnutGeometry.InnerRadius = innerRadius;
                dougnutGeometry.PushOut = pushout;
                dougnutGeometry.RemoveOnCompleted = false;
                var start = stackedValue / total * 360;
                var end = (stackedValue + point.PrimaryValue) / total * 360 - start;
                dougnutGeometry.StartAngle = start;
                dougnutGeometry.SweepAngle = end;
                if (start == 0 && end == 360) dougnutGeometry.SweepAngle = 359.9999f;

                point.Context.HoverArea = new SemicircleHoverArea().SetDimensions(cx, cy, start, start + end, minDimension * 0.5f);

                OnPointMeasured(point);
                chart.MeasuredDrawables.Add(dougnutGeometry);
            }
        }

        public DimensinalBounds GetBounds(PieChart<TDrawingContext> chart)
        {
            var stack = chart.SeriesContext.GetStackPosition(this, GetStackGroup());
            if (stack == null) throw new NullReferenceException("Unexpected null stacker");

            var bounds = new DimensinalBounds();
            foreach (var point in Fetch(chart))
            {
                stack.StackPoint(point);
                bounds.PrimaryBounds.AppendValue(point.PrimaryValue);
                bounds.SecondaryBounds.AppendValue(point.SecondaryValue);
                bounds.TertiaryBounds.AppendValue(Pushout > HoverPushout ? Pushout : HoverPushout);
            }

            return bounds;
        }

        protected override void DefaultOnPointAddedToSate(TVisual visual, IChartView<TDrawingContext> chart)
        {
            visual.PushOut = (float)HoverPushout;
        }

        protected override void DefaultOnRemovedFromState(TVisual visual, IChartView<TDrawingContext> chart)
        {
            visual.PushOut = (float)Pushout;
        }

        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual
                {
                    X = 0,
                    Y = 0,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize,
                    CenterX = (float)LegendShapeSize * 0.5f,
                    CenterY = (float)LegendShapeSize * 0.5f,
                    StartAngle = 0,
                    SweepAngle = 359.9999f
                };
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            var w = LegendShapeSize;
            if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = 0,
                    Y = 0,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize,
                    CenterX = (float)LegendShapeSize * 0.5f,
                    CenterY = (float)LegendShapeSize * 0.5f,
                    StartAngle = 0,
                    SweepAngle = 359.9999f
                };
                w += 2 * strokeClone.StrokeThickness;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
        }

        // for now multiple stack levels at a pie chart is not supported.
        public override int GetStackGroup() => 0;

        protected override void SetDefaultPointTransitions(IChartPoint<TVisual, TLabel, TDrawingContext> chartPoint)
        {
            var visual = chartPoint.Context.Visual;
            var chart = chartPoint.Context.Chart;

            if (visual == null) throw new Exception("Unable to initialize the point instance.");

            visual
                .TransitionateProperties(
                    nameof(visual.CenterX),
                    nameof(visual.CenterY),
                    nameof(visual.X),
                    nameof(visual.Y),
                    nameof(visual.Width),
                    nameof(visual.Height),
                    nameof(visual.StartAngle),
                    nameof(visual.SweepAngle),
                    nameof(visual.PushOut),
                    nameof(visual.InnerRadius))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));
        }
    }
}
