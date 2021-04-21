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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using System;
using LiveChartsCore.Measure;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the row series 
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="BarSeries{TModel, TVisual, TLabel, TDrawingContext}" />
    public class RowSeries<TModel, TVisual, TLabel, TDrawingContext> : BarSeries<TModel, TVisual, TLabel, TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RowSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        public RowSeries()
            : base(SeriesProperties.Bar | SeriesProperties.HorizontalOrientation) { }

        /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.Measure"/>
        public override void Measure(
           CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var secondaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);
            var previousSecondaryScale =
                primaryAxis.PreviousDataBounds == null ? null : new Scaler(drawLocation, drawMarginSize, primaryAxis);
            var primaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);

            var uw = secondaryScale.ToPixels(1f) - secondaryScale.ToPixels(0f);
            var uwm = 0.5f * uw;
            var sw = Stroke?.StrokeThickness ?? 0;
            var p = primaryScale.ToPixels((float)Pivot);

            var pos = chart.SeriesContext.GetColumnPostion(this);
            var count = chart.SeriesContext.GetColumnSeriesCount();
            var cp = 0f;

            if (!IgnoresBarPosition && count > 1)
            {
                uw /= count;
                uwm = 0.5f * uw;
                cp = (pos - count / 2f) * uw + uwm;
            }

            if (uw < -1 * MaxBarWidth)
            {
                uw = (float)MaxBarWidth * -1;
                uwm = uw / 2f;
            }

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
            if (Fill != null)
            {
                Fill.ZIndex = actualZIndex + 0.1;
                Fill.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(Fill);
            }
            if (Stroke != null)
            {
                Stroke.ZIndex = actualZIndex + 0.1;
                Stroke.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                chart.Canvas.AddDrawableTask(Stroke);
            }
            if (DataLabelsDrawableTask != null)
            {
                DataLabelsDrawableTask.ZIndex = actualZIndex + 0.1;
                DataLabelsDrawableTask.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
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
                        visual.X = p;
                        visual.Y = secondary - uwm + cp;
                        visual.Width = 0;
                        visual.Height = uw;
                        visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                if (visual == null)
                {
                    var yi = secondary - uwm + cp;
                    if (previousSecondaryScale != null) yi = previousSecondaryScale.ToPixels(point.SecondaryValue) - uwm + cp;

                    var r = new TVisual
                    {
                        X = p,
                        Y = yi,
                        Width = 0,
                        Height = uw
                    };

                    visual = r;
                    point.Context.Visual = visual;
                    OnPointCreated(point);
                    r.CompleteAllTransitions();

                    _ = everFetched.Add(point);
                }

                if (Fill != null) Fill.AddGeometyToPaintTask(visual);
                if (Stroke != null) Stroke.AddGeometyToPaintTask(visual);

                var sizedGeometry = visual;

                var cx = point.PrimaryValue > Pivot ? primary - b : primary;
                var y = secondary - uwm + cp;

                sizedGeometry.X = cx;
                sizedGeometry.Y = y;
                sizedGeometry.Width = b;
                sizedGeometry.Height = uw;

                point.Context.HoverArea = new RectangleHoverArea().SetDimensions(primary, secondary - uwm + cp, b, uw);

                OnPointMeasured(point);
                _ = toDeletePoints.Remove(point);

                if (DataLabelsDrawableTask != null)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label == null)
                    {
                        var l = new TLabel { X = p, Y = secondary - uwm + cp };

                        _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(a =>
                                a.WithDuration(chart.AnimationsSpeed)
                                .WithEasingFunction(chart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = l;
                        DataLabelsDrawableTask.AddGeometyToPaintTask(l);
                    }

                    label.Text = DataLabelsFormatter(point);
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        cx, y, b, uw, label.Measure(DataLabelsDrawableTask), DataLabelsPosition, SeriesProperties, point.PrimaryValue > Pivot);
                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != chart.View) continue;
                SoftDeletePoint(point, primaryScale, secondaryScale);
                _ = everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override DimensionalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var baseBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);

            var tick = secondaryAxis.GetTick(chart.ControlSize, baseBounds.VisiblePrimaryBounds);

            return new DimensionalBounds
            {
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.SecondaryBounds.Max + 0.5,
                    Min = baseBounds.SecondaryBounds.Min - 0.5
                },
                SecondaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tick.Value,
                    min = baseBounds.PrimaryBounds.Min - tick.Value
                },
                VisiblePrimaryBounds = new Bounds
                {
                    Max = baseBounds.VisibleSecondaryBounds.Max + 0.5,
                    Min = baseBounds.VisibleSecondaryBounds.Min - 0.5
                },
                VisibleSecondaryBounds = new Bounds
                {
                    Max = baseBounds.VisiblePrimaryBounds.Max + tick.Value,
                    min = baseBounds.VisiblePrimaryBounds.Min - tick.Value
                },
            };
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var chart = chartPoint.Context.Chart;

            if (chartPoint.Context.Visual is not TVisual visual) throw new Exception("Unable to initialize the point instance.");

            _ = visual
                .TransitionateProperties(
                    nameof(visual.X),
                    nameof(visual.Width),
                    nameof(visual.Y),
                    nameof(visual.Height))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SoftDeletePoint(ChartPoint, Scaler, Scaler)"/>
        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual == null) return;

            var p = primaryScale.ToPixels(pivot);

            var secondary = secondaryScale.ToPixels(point.SecondaryValue);

            visual.X = p;
            visual.Y = secondary;
            visual.Width = 0;
            visual.RemoveOnCompleted = true;
        }
    }
}
