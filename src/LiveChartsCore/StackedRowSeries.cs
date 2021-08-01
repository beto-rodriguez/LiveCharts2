﻿// The MIT License(MIT)
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
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Data;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a stacked row series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="StackedBarSeries{TModel, TVisual, TLabel, TDrawingContext}" />
    public class StackedRowSeries<TModel, TVisual, TLabel, TDrawingContext> : StackedBarSeries<TModel, TVisual, TLabel, TDrawingContext>
        where TVisual : class, IRoundedRectangleChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StackedRowSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        public StackedRowSeries()
            : base(
                  SeriesProperties.Bar | SeriesProperties.PrimaryAxisHorizontalOrientation | SeriesProperties.Stacked |
                  SeriesProperties.Solid | SeriesProperties.PrefersXStrategyTooltips)
        {

        }

        /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
        public override void Measure(Chart<TDrawingContext> chart)
        {
            var cartesianChart = (CartesianChart<TDrawingContext>)chart;
            var primaryAxis = cartesianChart.YAxes[ScalesYAt];
            var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

            var drawLocation = cartesianChart.DrawMarginLocation;
            var drawMarginSize = cartesianChart.DrawMarginSize;
            var secondaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);
            var previousSecondaryScale =
                primaryAxis.PreviousDataBounds is null ? null : new Scaler(drawLocation, drawMarginSize, primaryAxis);
            var primaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);

            var uw = secondaryScale.MeasureInPixels(secondaryAxis.UnitWidth); //secondaryScale.ToPixels(1f) - secondaryScale.ToPixels(0f);
            var uwm = 0.5f * uw;
            var sw = Stroke?.StrokeThickness ?? 0;
            var p = primaryScale.ToPixels(pivot);

            var pos = cartesianChart.SeriesContext.GetStackedColumnPostion(this);
            var count = cartesianChart.SeriesContext.GetStackedColumnSeriesCount();
            var cp = 0f;

            if (count > 1)
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
            if (Fill is not null)
            {
                Fill.ZIndex = actualZIndex + 0.1;
                Fill.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                cartesianChart.Canvas.AddDrawableTask(Fill);
            }
            if (Stroke is not null)
            {
                Stroke.ZIndex = actualZIndex + 0.2;
                Stroke.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                cartesianChart.Canvas.AddDrawableTask(Stroke);
            }
            if (DataLabelsPaint is not null)
            {
                DataLabelsPaint.ZIndex = actualZIndex + 0.3;
                DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
            }
            var dls = (float)DataLabelsSize;
            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

            var stacker = cartesianChart.SeriesContext.GetStackPosition(this, GetStackGroup());
            if (stacker is null) throw new NullReferenceException("Unexpected null stacker");

            var rx = (float)Rx;
            var ry = (float)Ry;

            foreach (var point in Fetch(cartesianChart))
            {
                var visual = point.Context.Visual as TVisual;
                var secondary = secondaryScale.ToPixels(point.SecondaryValue);

                if (point.IsNull)
                {
                    if (visual is not null)
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

                if (visual is null)
                {
                    var yi = secondary - uwm + cp;
                    if (previousSecondaryScale is not null) yi = previousSecondaryScale.ToPixels(point.SecondaryValue) - uwm + cp;

                    var r = new TVisual
                    {
                        X = p,
                        Y = yi,
                        Width = 0,
                        Height = uw,
                        Rx = rx,
                        Ry = ry
                    };

                    visual = r;
                    point.Context.Visual = visual;
                    OnPointCreated(point);
                    r.CompleteAllTransitions();

                    _ = everFetched.Add(point);
                }

                if (Fill is not null) Fill.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                if (Stroke is not null) Stroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

                var sizedGeometry = visual;

                var sy = stacker.GetStack(point);
                var primaryI = primaryScale.ToPixels(sy.Start);
                var primaryJ = primaryScale.ToPixels(sy.End);
                var y = secondary - uwm + cp;

                sizedGeometry.X = primaryJ;
                sizedGeometry.Y = y;
                sizedGeometry.Width = primaryI - primaryJ;
                sizedGeometry.Height = uw;
                sizedGeometry.Rx = rx;
                sizedGeometry.Ry = ry;
                sizedGeometry.RemoveOnCompleted = false;

                point.Context.HoverArea = new RectangleHoverArea().SetDimensions(secondary - uwm + cp, primaryJ, uw, primaryI - primaryJ);

                OnPointMeasured(point);
                _ = toDeletePoints.Remove(point);

                if (DataLabelsPaint is not null)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label is null)
                    {
                        var l = new TLabel { X = secondary - uwm + cp, Y = p };

                        _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = label;
                    }

                    DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);
                    label.Text = DataLabelsFormatter(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        primaryJ, y, primaryI - primaryJ, uw, label.Measure(DataLabelsPaint), DataLabelsPosition, SeriesProperties, point.PrimaryValue > Pivot);
                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != cartesianChart.View) continue;
                SoftDeletePoint(point, primaryScale, secondaryScale);
                _ = everFetched.Remove(point);
            }
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="secondaryAxis">The secondary axis.</param>
        /// <param name="primaryAxis">The primary axis.</param>
        /// <returns></returns>
        public override SeriesBounds GetBounds(
         CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var baseSeriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);
            if (baseSeriesBounds.IsPrevious) return baseSeriesBounds;
            var baseBounds = baseSeriesBounds.Bounds;

            var tickPrimary = primaryAxis.GetTick(chart.ControlSize, baseBounds.VisiblePrimaryBounds);
            var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, baseBounds.VisibleSecondaryBounds);

            var ts = tickSecondary.Value * DataPadding.X;
            var tp = tickPrimary.Value * DataPadding.Y;

            if (baseBounds.VisibleSecondaryBounds.Delta == 0)
            {
                var ms = baseBounds.VisibleSecondaryBounds.Min == 0 ? 1 : baseBounds.VisibleSecondaryBounds.Min;
                ts = 0.1 * ms * DataPadding.X;
            }

            if (baseBounds.VisiblePrimaryBounds.Delta == 0)
            {
                var mp = baseBounds.VisiblePrimaryBounds.Min == 0 ? 1 : baseBounds.VisiblePrimaryBounds.Min;
                tp = 0.1 * mp * DataPadding.Y;
            }

            return
                new SeriesBounds(
                    new DimensionalBounds
                    {
                        PrimaryBounds = new Bounds
                        {
                            Max = baseBounds.SecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth + ts,
                            Min = baseBounds.SecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth - ts
                        },
                        SecondaryBounds = new Bounds
                        {
                            Max = baseBounds.PrimaryBounds.Max + tp,
                            Min = baseBounds.PrimaryBounds.Min < 0 ? baseBounds.PrimaryBounds.Min - tp : 0
                        },
                        VisiblePrimaryBounds = new Bounds
                        {
                            Max = baseBounds.VisibleSecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth + ts,
                            Min = baseBounds.VisibleSecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth - ts
                        },
                        VisibleSecondaryBounds = new Bounds
                        {
                            Max = baseBounds.VisiblePrimaryBounds.Max + tp,
                            Min = baseBounds.VisiblePrimaryBounds.Min < 0 ? baseBounds.PrimaryBounds.Min - tp : 0
                        },
                        MinDeltaPrimary = baseBounds.MinDeltaPrimary,
                        MinDeltaSecondary = baseBounds.MinDeltaSecondary
                    }, false);
        }

        /// <summary>
        /// Sets the default point transitions.
        /// </summary>
        /// <param name="chartPoint">The chart point.</param>
        /// <exception cref="Exception">Unable to initialize the point instance.</exception>
        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var chart = chartPoint.Context.Chart;

            if (chartPoint.Context.Visual is not TVisual visual) throw new Exception("Unable to initialize the point instance.");

            _ = visual
                .TransitionateProperties(
                    nameof(visual.X),
                    nameof(visual.Width))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                        .WithEasingFunction(EasingFunction ?? chart.EasingFunction));

            _ = visual
                .TransitionateProperties(
                    nameof(visual.Y),
                    nameof(visual.Height))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                        .WithEasingFunction(EasingFunction ?? chart.EasingFunction));
        }

        /// <summary>
        /// Softs the delete point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="primaryScale">The primary scale.</param>
        /// <param name="secondaryScale">The secondary scale.</param>
        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual is null) return;

            var p = primaryScale.ToPixels(pivot);

            var secondary = secondaryScale.ToPixels(point.SecondaryValue);

            visual.X = p;
            visual.Y = secondary;
            visual.Width = 0;
            visual.RemoveOnCompleted = true;

            if (dataProvider is null) throw new Exception("Data provider not found");
            dataProvider.DisposePoint(point);

            var label = (TLabel?)point.Context.Label;
            if (label is null) return;

            label.TextSize = 1;
            label.RemoveOnCompleted = true;
        }
    }
}
