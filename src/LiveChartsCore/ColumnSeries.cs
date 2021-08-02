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
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Data;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a column series.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TVisual"></typeparam>
    /// <typeparam name="TLabel"></typeparam>
    /// <typeparam name="TDrawingContext"></typeparam>
    public abstract class ColumnSeries<TModel, TVisual, TLabel, TDrawingContext> : BarSeries<TModel, TVisual, TLabel, TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        private readonly bool _isRounded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        protected ColumnSeries()
            : base(
                  SeriesProperties.Bar | SeriesProperties.PrimaryAxisVerticalOrientation |
                  SeriesProperties.Solid | SeriesProperties.PrefersXStrategyTooltips)
        {
            DataPadding = new PointF(0, 1);
            _isRounded = typeof(IRoundedRectangleChartPoint<TDrawingContext>).IsAssignableFrom(typeof(TVisual));
        }

        /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
        public override void Measure(Chart<TDrawingContext> chart)
        {
            var cartesianChart = (CartesianChart<TDrawingContext>)chart;
            var primaryAxis = cartesianChart.YAxes[ScalesYAt];
            var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

            var drawLocation = cartesianChart.DrawMarginLocation;
            var drawMarginSize = cartesianChart.DrawMarginSize;
            var secondaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
            var primaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);
            var previousPrimaryScale = primaryAxis.PreviousDataBounds is null
                ? null
                : new Scaler(drawLocation, drawMarginSize, primaryAxis, true);
            var previousSecondaryScale = secondaryAxis.PreviousDataBounds is null
                ? null
                : new Scaler(drawLocation, drawMarginSize, secondaryAxis, true);

            var helper = new MeasureHelper(secondaryScale, cartesianChart, this, secondaryAxis, primaryScale.ToPixels(pivot));
            var pHelper = previousSecondaryScale == null || previousPrimaryScale == null
                ? null
                : new MeasureHelper(
                    previousSecondaryScale, cartesianChart, this, secondaryAxis, previousPrimaryScale.ToPixels(pivot));

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

            var rx = (float)Rx;
            var ry = (float)Ry;

            foreach (var point in Fetch(cartesianChart))
            {
                var visual = point.Context.Visual as TVisual;
                var primary = primaryScale.ToPixels(point.PrimaryValue);
                var secondary = secondaryScale.ToPixels(point.SecondaryValue);
                var b = Math.Abs(primary - helper.p);

                if (point.IsNull)
                {
                    if (visual is not null)
                    {
                        visual.X = secondary - helper.uwm + helper.cp;
                        visual.Y = helper.p;
                        visual.Width = helper.uw;
                        visual.Height = 0;
                        visual.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }
                    continue;
                }

                if (visual is null)
                {
                    var xi = secondary - helper.uwm + helper.cp;
                    var pi = helper.p;
                    var uwi = helper.uw;
                    var hi = 0f;

                    if (previousSecondaryScale is not null && previousPrimaryScale is not null && pHelper is not null)
                    {
                        var previousPrimary = previousPrimaryScale.ToPixels(point.PrimaryValue);
                        var bp = Math.Abs(previousPrimary - pHelper.p);
                        var cyp = point.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                        xi = previousSecondaryScale.ToPixels(point.SecondaryValue) - pHelper.uwm + pHelper.cp;
                        pi = cartesianChart.IsZoomingOrPanning ? cyp : pHelper.p;
                        uwi = pHelper.uw;
                        hi = cartesianChart.IsZoomingOrPanning ? bp : 0;
                    }

                    var r = new TVisual
                    {
                        X = xi,
                        Y = pi,
                        Width = uwi,
                        Height = hi
                    };

                    if (_isRounded)
                    {
                        var rounded = (IRoundedRectangleChartPoint<TDrawingContext>)r;
                        rounded.Rx = rx;
                        rounded.Ry = ry;
                    }

                    visual = r;
                    point.Context.Visual = visual;
                    OnPointCreated(point);
                    r.CompleteAllTransitions();

                    _ = everFetched.Add(point);
                }

                if (Fill is not null) Fill.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                if (Stroke is not null) Stroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

                var cy = point.PrimaryValue > pivot ? primary : primary - b;
                var x = secondary - helper.uwm + helper.cp;

                visual.X = x;
                visual.Y = cy;
                visual.Width = helper.uw;
                visual.Height = b;
                if (_isRounded)
                {
                    var rounded = (IRoundedRectangleChartPoint<TDrawingContext>)visual;
                    rounded.Rx = rx;
                    rounded.Ry = ry;
                }
                visual.RemoveOnCompleted = false;

                var ha = new RectangleHoverArea().SetDimensions(secondary - helper.uwm + helper.cp, cy, helper.uw, b);
                point.Context.HoverArea = ha;

                OnPointMeasured(point);
                _ = toDeletePoints.Remove(point);

                if (DataLabelsPaint is not null)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label is null)
                    {
                        var l = new TLabel { X = secondary - helper.uwm + helper.cp, Y = helper.p };

                        _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                            .WithAnimation(animation =>
                                animation
                                    .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                    .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));

                        l.CompleteAllTransitions();
                        label = l;
                        point.Context.Label = l;
                    }

                    DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);

                    label.Text = DataLabelsFormatter(new TypedChartPoint<TModel, TVisual, TLabel, TDrawingContext>(point));
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        x, cy, helper.uw, b, label.Measure(DataLabelsPaint),
                        DataLabelsPosition, SeriesProperties, point.PrimaryValue > Pivot);
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

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override SeriesBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
        {
            var baseSeriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);

            if (baseSeriesBounds.HasData) return baseSeriesBounds;
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
                        SecondaryBounds = new Bounds
                        {
                            Max = baseBounds.SecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth + ts,
                            Min = baseBounds.SecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth - ts
                        },
                        PrimaryBounds = new Bounds
                        {
                            Max = baseBounds.PrimaryBounds.Max + tp,
                            Min = baseBounds.PrimaryBounds.Min - tp
                        },
                        VisibleSecondaryBounds = new Bounds
                        {
                            Max = baseBounds.VisibleSecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth + ts,
                            Min = baseBounds.VisibleSecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth - ts
                        },
                        VisiblePrimaryBounds = new Bounds
                        {
                            Max = baseBounds.VisiblePrimaryBounds.Max + tp,
                            Min = baseBounds.VisiblePrimaryBounds.Min - tp
                        },
                        MinDeltaPrimary = baseBounds.MinDeltaPrimary,
                        MinDeltaSecondary = baseBounds.MinDeltaSecondary
                    },
                    false);
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
                        .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                        .WithEasingFunction(EasingFunction ?? chart.EasingFunction));
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SoftDeletePoint(ChartPoint, Scaler, Scaler)"/>
        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (TVisual?)point.Context.Visual;
            if (visual is null) return;

            var chartView = (ICartesianChartView<TDrawingContext>)point.Context.Chart;
            if (chartView.Core.IsZoomingOrPanning)
            {
                visual.CompleteAllTransitions();
                visual.RemoveOnCompleted = true;
                return;
            }

            var p = primaryScale.ToPixels(pivot);
            var secondary = secondaryScale.ToPixels(point.SecondaryValue);

            visual.X = secondary;
            visual.Y = p;
            visual.Height = 0;
            visual.RemoveOnCompleted = true;

            if (dataProvider is null) throw new Exception("Data provider not found");
            dataProvider.DisposePoint(point);

            var label = (TLabel?)point.Context.Label;
            if (label is null) return;

            label.TextSize = 1;
            label.RemoveOnCompleted = true;
        }

        private class MeasureHelper
        {
            public MeasureHelper(
                Scaler scaler,
                CartesianChart<TDrawingContext> cartesianChart,
                IBarSeries<TDrawingContext> barSeries,
                IAxis axis,
                float p)
            {
                this.p = p;

                uw = scaler.MeasureInPixels(axis.UnitWidth);

                var gp = (float)barSeries.GroupPadding;

                if (uw - gp < 1) gp = 0;

                uw -= gp;
                uwm = 0.5f * uw;

                var pos = cartesianChart.SeriesContext.GetColumnPostion(barSeries);
                var count = cartesianChart.SeriesContext.GetColumnSeriesCount();
                cp = 0f;

                if (!barSeries.IgnoresBarPosition && count > 1)
                {
                    uw /= count;
                    uwm = 0.5f * uw;
                    cp = (pos - count / 2f) * uw + uwm;
                }

                if (uw > barSeries.MaxBarWidth)
                {
                    uw = (float)barSeries.MaxBarWidth;
                    uwm = uw * 0.5f;
                }

                if (uw < 1)
                {
                    uw = 1;
                    uwm = 0.5f;
                }
            }

            public float uw, uwm, cp, p;
        }
    }
}
