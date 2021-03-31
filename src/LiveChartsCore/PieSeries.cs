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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using LiveChartsCore.Measure;
using System.Drawing;

namespace LiveChartsCore
{
    /// <inheritdoc cref="IPieSeries{TDrawingContext}" />
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

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.Pushout"/>
        public double Pushout { get; set; } = 5;

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.InnerRadius"/>
        public double InnerRadius { get; set; } = 0;

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.MaxOuterRadius"/>
        public double MaxOuterRadius { get; set; } = 1;

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.HoverPushout"/>
        public double HoverPushout { get; set; } = 20;

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.Measure(PieChart{TDrawingContext})"/>
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

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
            if (Fill != null)
            {
                Fill.ZIndex = actualZIndex + 0.1;
                Fill.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(Fill);
            }
            if (Stroke != null)
            {
                Stroke.ZIndex = actualZIndex + 0.2;
                Stroke.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(Stroke);
            }
            if (DataLabelsDrawableTask != null)
            {
                DataLabelsDrawableTask.ZIndex = actualZIndex + 0.3;
                DataLabelsDrawableTask.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(DataLabelsDrawableTask);
            }

            var cx = drawLocation.X + drawMarginSize.Width * 0.5f;
            var cy = drawLocation.Y + drawMarginSize.Height * 0.5f;

            var stacker = chart.SeriesContext.GetStackPosition(this, GetStackGroup());
            if (stacker == null) throw new NullReferenceException("Unexpected null stacker");

            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            foreach (var point in Fetch(chart))
            {
                var visual = point.Context.Visual as TVisual;

                if (point.IsNull)
                {
                    if (visual != null)
                    {
                        visual.CenterX = drawLocation.X + drawMarginSize.Width * 0.5f;
                        visual.CenterY = drawLocation.Y + drawMarginSize.Height * 0.5f;
                        visual.X = cx;
                        visual.Y = cy;
                        visual.Width = 0;
                        visual.Height = 0;
                        visual.SweepAngle = 0;
                        visual.StartAngle = 0;
                        visual.PushOut = 0;
                        visual.InnerRadius = 0;
                        visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                var stack = stacker.GetStack(point);
                var stackedValue = stack.Start;
                var total = stack.Total;
                var start = stackedValue / total * 360;
                var end = (stackedValue + point.PrimaryValue) / total * 360 - start;

                if (visual == null)
                {
                    var p = new TVisual
                    {
                        CenterX = drawLocation.X + drawMarginSize.Width * 0.5f,
                        CenterY = drawLocation.Y + drawMarginSize.Height * 0.5f,
                        X = cx,
                        Y = cy,
                        Width = 0,
                        Height = 0,
                        StartAngle = 0,
                        SweepAngle = 0,
                        PushOut = 0,
                        InnerRadius = 0
                    };

                    visual = p;
                    point.Context.Visual = visual;
                    OnPointCreated(point);
                    p.CompleteAllTransitions();

                    everFetched.Add(point);
                }

                if (Fill != null) Fill.AddGeometyToPaintTask(visual);
                if (Stroke != null) Stroke.AddGeometyToPaintTask(visual);

                var dougnutGeometry = visual;

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
                dougnutGeometry.StartAngle = start;
                dougnutGeometry.SweepAngle = end;
                if (start == 0 && end == 360) dougnutGeometry.SweepAngle = 359.9999f;

                point.Context.HoverArea = new SemicircleHoverArea().SetDimensions(cx, cy, start, start + end, minDimension * 0.5f);

                OnPointMeasured(point);
                toDeletePoints.Remove(point);
            }

            var u = Scaler.GetDefaultScaler(AxisOrientation.X);
            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != chart.View) continue;
                SoftDeletePoint(point, u, u);
                everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="IPieSeries{TDrawingContext}.GetBounds(PieChart{TDrawingContext})"/>
        public DimensionalBounds GetBounds(PieChart<TDrawingContext> chart)
        {
            if (dataProvider == null) throw new Exception("Data provider not found");
            return dataProvider.GetPieBounds(chart, this);
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

        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var visual = chartPoint.Context.Visual as TVisual;
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

        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual == null) return;

            visual.StartAngle = visual.StartAngle + visual.SweepAngle;
            visual.SweepAngle = 0;
            visual.RemoveOnCompleted = true;
        }

        public override void Delete(IChartView chart)
        {
            var u = Scaler.GetDefaultScaler(AxisOrientation.X);

            var toDelete = new List<ChartPoint>();
            foreach (var point in everFetched)
            {
                if (point.Context.Chart != chart) continue;
                SoftDeletePoint(point, u, u);
                toDelete.Add(point);
            }

            foreach (var item in toDelete) everFetched.Remove(item);
        }
    }
}
