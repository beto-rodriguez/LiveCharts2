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
using LiveChartsCore.Measure;
using System.Collections.Generic;

namespace LiveChartsCore
{
    public abstract class ColumnSeries<TModel, TVisual, TLabel, TDrawingContext> : BarSeries<TModel, TVisual, TLabel, TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
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
            var secondaryScale = new Scaler(
                drawLocation, drawMarginSize, secondaryAxis.Orientation, secondaryAxis.DataBounds, secondaryAxis.IsInverted);
            var primaryScale = new Scaler(
                drawLocation, drawMarginSize, primaryAxis.Orientation, primaryAxis.DataBounds, primaryAxis.IsInverted);
            var previousSecondaryScale = secondaryAxis.PreviousDataBounds == null ? null : new Scaler(
                drawLocation, drawMarginSize, secondaryAxis.Orientation, secondaryAxis.PreviousDataBounds, secondaryAxis.IsInverted);

            float uw = secondaryScale.ToPixels(1f) - secondaryScale.ToPixels(0f);
            float uwm = 0.5f * uw;
            float sw = Stroke?.StrokeThickness ?? 0;
            float p = primaryScale.ToPixels(pivot);

            var pos = chart.SeriesContext.GetColumnPostion(this);
            var count = chart.SeriesContext.GetColumnSeriesCount();
            float cp = 0f;

            if (!IgnoresBarPosition && count > 1)
            {
                uw /= count;
                uwm = 0.5f * uw;
                cp = (pos - count / 2f) * uw + uwm;
            }

            if (uw > MaxBarWidth)
            {
                uw = (float)MaxBarWidth;
                uwm = uw / 2f;
            }

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
            if (Fill != null)
            {
                Fill.ZIndex = actualZIndex + 0.1;
                chart.Canvas.AddDrawableTask(Fill);
            }
            if (Stroke != null) 
            {
                Stroke.ZIndex = actualZIndex + 0.2;
                chart.Canvas.AddDrawableTask(Stroke);
            }
            if (DataLabelsDrawableTask != null)
            {
                DataLabelsDrawableTask.ZIndex = actualZIndex + 0.3;
                chart.Canvas.AddDrawableTask(DataLabelsDrawableTask);
            }

            var dls = (float)DataLabelsSize;
            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            foreach (var point in Fetch(chart))
            {
                var visual = point.Context.Visual as TVisual;
                var primary = primaryScale.ToPixels(point.PrimaryValue);
                var secondary = secondaryScale.ToPixels(point.SecondaryValue);
                var b = Math.Abs(primary - p);

                if (point.IsNull)
                {
                    if (visual != null)
                    {
                        visual.X = secondary - uwm + cp;
                        visual.Y = p;
                        visual.Width = uw;
                        visual.Height = 0;
                        visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                if (visual == null)
                {
                    var xi = secondary - uwm + cp;
                    if (previousSecondaryScale != null) xi = previousSecondaryScale.ToPixels(point.SecondaryValue) - uwm + cp;

                    var r = new TVisual
                    {
                        X = xi,
                        Y = p,
                        Width = uw,
                        Height = 0
                    };

                    visual = r;
                    point.Context.Visual = visual;
                    OnPointCreated(point);
                    r.CompleteAllTransitions();

                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                    everFetched.Add(point);
                }

                var cy = point.PrimaryValue > pivot ? primary : primary - b;
                var x = secondary - uwm + cp;

                visual.X = x;
                visual.Y = cy;
                visual.Width = uw;
                visual.Height = b;
                visual.RemoveOnCompleted = false;

                var ha = new RectangleHoverArea().SetDimensions(secondary - uwm + cp, cy, uw, b);
                point.Context.HoverArea = ha;

                OnPointMeasured(point);
                toDeletePoints.Remove(point);
                chart.MeasuredDrawables.Add(visual);

                if (DataLabelsDrawableTask != null)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label == null)
                    {
                        var l = new TLabel { X = secondary - uwm + cp, Y = p };

                        l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(a =>
                                a.WithDuration(chart.AnimationsSpeed)
                                .WithEasingFunction(chart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = l;
                        DataLabelsDrawableTask.AddGeometyToPaintTask(l);
                    }

                    label.Text = DataLabelFormatter(point);
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        x, cy, uw, b, label.Measure(DataLabelsDrawableTask), DataLabelsPosition, SeriesProperties, point.PrimaryValue > Pivot);
                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;

                    chart.MeasuredDrawables.Add(label);
                }
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != chart.View) continue;
                SoftDeletePoint(point, primaryScale, secondaryScale);
                everFetched.Remove(point);
            }
        }

        public override DimensionalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var baseBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);

            var tick = secondaryAxis.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);

            return new DimensionalBounds
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

        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var visual = chartPoint.Context.Visual as TVisual;
            var chart = chartPoint.Context.Chart;

            if (visual == null) throw new Exception("Unable to initialize the point instance.");

            visual
                .TransitionateProperties(nameof(visual.X), nameof(visual.Width))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));

            visual
                .TransitionateProperties(nameof(visual.Y), nameof(visual.Height))
                .WithAnimation(animation => animation
                    .WithDuration(chart.AnimationsSpeed)
                    .WithEasingFunction(elasticFunction));
        }

        protected override  void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual == null) return;

            float p = primaryScale.ToPixels(pivot);

            var secondary = secondaryScale.ToPixels(point.SecondaryValue);

            visual.X = secondary;
            visual.Y = p;
            visual.Height = 0;
            visual.RemoveOnCompleted = true;
        }
    }
}
