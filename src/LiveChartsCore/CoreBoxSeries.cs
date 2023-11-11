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

using System;
using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines a candle sticks series.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TMiniatureGeometry">The type of the miniature geometry, used in tool tips and legends.</typeparam> 
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}" />
/// <seealso cref="ICartesianSeries{TDrawingContext}" />
/// <seealso cref="IHeatSeries{TDrawingContext}" />
public abstract class CoreBoxSeries<TModel, TVisual, TLabel, TMiniatureGeometry, TDrawingContext>
    : StrokeAndFillCartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IBoxSeries<TDrawingContext>
        where TVisual : class, IBoxGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TMiniatureGeometry : ISizedGeometry<TDrawingContext>, new()
{
    private double _pading = 5;
    private double _maxBarWidth = 25;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreBoxSeries{TModel, TVisual, TLabel, TMiniatureGeometry, TDrawingContext}"/> class.
    /// </summary>
    protected CoreBoxSeries()
        : base(
             SeriesProperties.BoxSeries | SeriesProperties.PrimaryAxisVerticalOrientation |
             SeriesProperties.Solid | SeriesProperties.PrefersXStrategyTooltips)
    {
        YToolTipLabelFormatter = p =>
        {
            var c = p.Coordinate;
            return
                $"Max {c.PrimaryValue}, Min {c.QuinaryValue}{Environment.NewLine}" +
                $"1stQ {c.TertiaryValue}, 2dnQ {c.QuaternaryValue}, M {c.SenaryValue}";
        };

        DataLabelsFormatter = p =>
        {
            var c = p.Coordinate;
            return
                $"Max {c.PrimaryValue}, Min {c.QuinaryValue}{Environment.NewLine}" +
                $"1stQ {c.TertiaryValue}, 2dnQ {c.QuaternaryValue}, M {c.SenaryValue}"; ;
        };

        DataPadding = new LvcPoint(0, 1);
    }

    /// <inheritdoc cref="IBoxSeries{TDrawingContext}.MaxBarWidth"/>
    public double MaxBarWidth { get => _maxBarWidth; set => SetProperty(ref _maxBarWidth, value); }

    /// <inheritdoc cref="IBoxSeries{TDrawingContext}.Padding"/>
    public double Padding { get => _pading; set => SetProperty(ref _pading, value); }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        var cartesianChart = (CartesianChart<TDrawingContext>)chart;
        var primaryAxis = cartesianChart.YAxes[ScalesYAt];
        var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;
        var secondaryScale = secondaryAxis.GetNextScaler(cartesianChart);
        var primaryScale = primaryAxis.GetNextScaler(cartesianChart);
        var previousPrimaryScale = primaryAxis.GetActualScaler(cartesianChart);
        var previousSecondaryScale = secondaryAxis.GetActualScaler(cartesianChart);

        var uw = secondaryScale.MeasureInPixels(secondaryAxis.UnitWidth);
        var puw = previousSecondaryScale is null ? 0 : previousSecondaryScale.MeasureInPixels(secondaryAxis.UnitWidth);
        var uwm = 0.5f * uw;

        var helper = new MeasureHelper(secondaryScale, cartesianChart, this, secondaryAxis, primaryScale.ToPixels(pivot),
            cartesianChart.DrawMarginLocation.Y, cartesianChart.DrawMarginLocation.Y + cartesianChart.DrawMarginSize.Height);

        var tp = chart.TooltipPosition;

        if (uw > MaxBarWidth)
        {
            uw = (float)MaxBarWidth;
            uwm = uw * 0.5f;
            puw = uw;
        }

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
        var clipping = GetClipRectangle(cartesianChart);

        if (Stroke is not null)
        {
            Stroke.ZIndex = actualZIndex + 0.2;
            Stroke.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(Stroke);
        }
        if (Fill is not null)
        {
            Fill.ZIndex = actualZIndex + 0.1;
            Fill.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(Fill);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = actualZIndex + 0.3;
            DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var dls = (float)DataLabelsSize;
        var pointsCleanup = ChartPointCleanupContext.For(everFetched);

        foreach (var point in Fetch(cartesianChart))
        {
            var coordinate = point.Coordinate;
            var visual = point.Context.Visual as TVisual;
            var secondary = secondaryScale.ToPixels(coordinate.SecondaryValue);

            var high = primaryScale.ToPixels(coordinate.PrimaryValue);
            var open = primaryScale.ToPixels(coordinate.TertiaryValue);
            var close = primaryScale.ToPixels(coordinate.QuaternaryValue);
            var low = primaryScale.ToPixels(coordinate.QuinaryValue);
            var median = primaryScale.ToPixels(coordinate.SenaryValue);
            var middle = open;

            if (point.IsEmpty)
            {
                if (visual is not null)
                {
                    visual.X = secondary - helper.uwm + helper.cp;
                    visual.Width = uw;
                    visual.Y = median; // y coordinate is the highest
                    visual.Third = median;
                    visual.First = median;
                    visual.Min = median;
                    visual.Median = median;
                    visual.RemoveOnCompleted = true;
                    point.Context.Visual = null;
                }
                continue;
            }

            if (visual is null)
            {
                var xi = secondary - helper.uwm + helper.cp;
                var uwi = helper.uw;
                var hi = 0f;

                if (previousSecondaryScale is not null && previousPrimaryScale is not null)
                {
                    var previousP = previousPrimaryScale.ToPixels(pivot);
                    var previousPrimary = previousPrimaryScale.ToPixels(coordinate.PrimaryValue);
                    var bp = Math.Abs(previousPrimary - previousP);
                    var cyp = coordinate.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                    xi = previousSecondaryScale.ToPixels(coordinate.SecondaryValue) - uwm;
                    uwi = helper.uw;
                    hi = cartesianChart.IsZoomingOrPanning ? bp : 0;
                }

                var r = new TVisual
                {
                    X = xi,
                    Width = uwi,
                    Y = median, // y == high
                    Third = median,
                    First = median,
                    Min = median,
                    Median = median
                };

                visual = r;
                point.Context.Visual = visual;
                OnPointCreated(point);

                _ = everFetched.Add(point);
            }

            Stroke?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
            Fill?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

            var x = secondary - helper.uwm + helper.cp;

            visual.X = x;
            visual.Width = helper.uw;
            visual.Y = high;
            visual.Third = open;
            visual.First = close;
            visual.Min = low;
            visual.Median = median;
            visual.RemoveOnCompleted = false;

            if (point.Context.HoverArea is not RectangleHoverArea ha)
                point.Context.HoverArea = ha = new RectangleHoverArea();

            _ = ha.SetDimensions(secondary - helper.actualUw * 0.5f, high, helper.actualUw, Math.Abs(low - high));

            switch (tp)
            {
                case TooltipPosition.Hidden:
                    break;
                case TooltipPosition.Auto:
                case TooltipPosition.Top: _ = ha.CenterXToolTip().StartYToolTip(); break;
                case TooltipPosition.Bottom: _ = ha.CenterXToolTip().EndYToolTip(); break;
                case TooltipPosition.Left: _ = ha.StartXToolTip().CenterYToolTip(); break;
                case TooltipPosition.Right: _ = ha.EndXToolTip().CenterYToolTip(); break;
                case TooltipPosition.Center: _ = ha.CenterXToolTip().CenterYToolTip(); break;
                default:
                    break;
            }

            pointsCleanup.Clean(point);

            if (DataLabelsPaint is not null)
            {
                var label = (TLabel?)point.Context.Label;

                if (label is null)
                {
                    var l = new TLabel { X = secondary - helper.uwm + helper.cp, Y = high, RotateTransform = (float)DataLabelsRotation, MaxWidth = (float)DataLabelsMaxWidth };
                    l.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
                    label = l;
                    point.Context.Label = l;
                }

                DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);

                label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));
                label.TextSize = dls;
                label.Padding = DataLabelsPadding;
                var m = label.Measure(DataLabelsPaint);
                var labelPosition = GetLabelPosition(
                    x, high, helper.uw, Math.Abs(low - high), m, DataLabelsPosition,
                    SeriesProperties, coordinate.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                if (DataLabelsTranslate is not null) label.TranslateTransform =
                        new LvcPoint(m.Width * DataLabelsTranslate.Value.X, m.Height * DataLabelsTranslate.Value.Y);

                label.X = labelPosition.X;
                label.Y = labelPosition.Y;
            }

            OnPointMeasured(point);
        }

        pointsCleanup.CollectPoints(
            everFetched, cartesianChart.View, primaryScale, secondaryScale, SoftDeleteOrDisposePoint);
    }

    /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, ICartesianAxis, ICartesianAxis)"/>
    public override SeriesBounds GetBounds(
        CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var rawBounds = DataFactory.GetFinancialBounds(chart, this, secondaryAxis, primaryAxis);
        if (rawBounds.HasData) return rawBounds;

        var rawBaseBounds = rawBounds.Bounds;

        var tickPrimary = primaryAxis.GetTick(chart.ControlSize, rawBaseBounds.VisiblePrimaryBounds);
        var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, rawBaseBounds.VisibleSecondaryBounds);

        var ts = tickSecondary.Value * DataPadding.X;
        var tp = tickPrimary.Value * DataPadding.Y;

        // using different methods for both primary and secondary axis seems to be the best solution
        // if this the following 2 lines needs to be changed again, please ensure that the following test passes:
        // https://github.com/beto-rodriguez/LiveCharts2/issues/522
        // https://github.com/beto-rodriguez/LiveCharts2/issues/642

        if (rawBaseBounds.VisibleSecondaryBounds.Delta == 0) ts = secondaryAxis.UnitWidth * DataPadding.X;
        if (rawBaseBounds.VisiblePrimaryBounds.Delta == 0) tp = rawBaseBounds.VisiblePrimaryBounds.Max * 0.25f;

        var rgs = GetRequestedGeometrySize();
        var rso = GetRequestedSecondaryOffset();
        var rpo = GetRequestedPrimaryOffset();

        var dimensionalBounds = new DimensionalBounds
        {
            SecondaryBounds = new Bounds
            {
                Max = rawBaseBounds.SecondaryBounds.Max + rso * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.SecondaryBounds.Min - rso * secondaryAxis.UnitWidth,
                MinDelta = rawBaseBounds.SecondaryBounds.MinDelta,
                PaddingMax = ts,
                PaddingMin = ts,
                RequestedGeometrySize = rgs
            },
            PrimaryBounds = new Bounds
            {
                Max = rawBaseBounds.PrimaryBounds.Max + rpo * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.PrimaryBounds.Min - rpo * secondaryAxis.UnitWidth,
                MinDelta = rawBaseBounds.PrimaryBounds.MinDelta,
                PaddingMax = tp,
                PaddingMin = tp,
                RequestedGeometrySize = rgs
            },
            VisibleSecondaryBounds = new Bounds
            {
                Max = rawBaseBounds.VisibleSecondaryBounds.Max + rso * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.VisibleSecondaryBounds.Min - rso * secondaryAxis.UnitWidth,
            },
            VisiblePrimaryBounds = new Bounds
            {
                Max = rawBaseBounds.VisiblePrimaryBounds.Max + rpo * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.VisiblePrimaryBounds.Min - rpo * secondaryAxis.UnitWidth
            },
            TertiaryBounds = rawBaseBounds.TertiaryBounds,
            VisibleTertiaryBounds = rawBaseBounds.VisibleTertiaryBounds
        };

        return new SeriesBounds(dimensionalBounds, false);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.GetMiniaturesSketch"/>
    public override Sketch<TDrawingContext> GetMiniaturesSketch()
    {
        var schedules = new List<PaintSchedule<TDrawingContext>>();

        if (Fill is not null) schedules.Add(BuildMiniatureSchedule(Fill, new TMiniatureGeometry()));
        if (Stroke is not null) schedules.Add(BuildMiniatureSchedule(Stroke, new TMiniatureGeometry()));

        return new Sketch<TDrawingContext>(MiniatureShapeSize, MiniatureShapeSize, GeometrySvg)
        {
            PaintSchedules = schedules
        };
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.GetRequestedSecondaryOffset"/>
    protected override double GetRequestedSecondaryOffset()
    {
        return 0.5f;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        var chart = chartPoint.Context.Chart;
        if (chartPoint.Context.Visual is not TVisual visual) throw new Exception("Unable to initialize the point instance.");
        visual.Animate(EasingFunction ?? chart.EasingFunction, AnimationsSpeed ?? chart.AnimationsSpeed);
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.SoftDeleteOrDisposePoint(ChartPoint, Scaler, Scaler)"/>
    protected internal override void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        if (DataFactory is null) throw new Exception("Data provider not found");

        var chartView = (ICartesianChartView<TDrawingContext>)point.Context.Chart;
        if (chartView.Core.IsZoomingOrPanning)
        {
            visual.CompleteTransition(null);
            visual.RemoveOnCompleted = true;
            DataFactory.DisposePoint(point);
            return;
        }

        var p = primaryScale.ToPixels(pivot);
        var secondary = secondaryScale.ToPixels(point.Coordinate.SecondaryValue);

        visual.X = secondary;
        visual.Y = p;
        visual.Third = p;
        visual.First = p;
        visual.Min = p;
        visual.Median = p;
        visual.RemoveOnCompleted = true;

        DataFactory.DisposePoint(point);

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.OnPointerEnter(ChartPoint)"/>
    protected override void OnPointerEnter(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.Opacity = 0.8f;

        base.OnPointerEnter(point);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.OnPointerLeft(ChartPoint)"/>
    protected override void OnPointerLeft(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.Opacity = 1;

        base.OnPointerLeft(point);
    }

    /// <summary>
    /// Called when [paint changed].
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected override void OnPaintChanged(string? propertyName)
    {
        base.OnPaintChanged(propertyName);
        OnPropertyChanged();
    }

    /// <summary>
    /// A mesure helper class.
    /// </summary>
    protected class MeasureHelper
    {
        /// <summary>
        /// Initializes a new instance of the measue helper class.
        /// </summary>
        /// <param name="scaler">The scaler.</param>
        /// <param name="cartesianChart">The chart.</param>
        /// <param name="boxSeries">The series.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="p">The pivot.</param>
        /// <param name="minP">The min pivot allowed.</param>
        /// <param name="maxP">The max pivot allowed.</param>
        public MeasureHelper(
            Scaler scaler,
            CartesianChart<TDrawingContext> cartesianChart,
            IBoxSeries<TDrawingContext> boxSeries,
            ICartesianAxis axis,
            float p,
            float minP,
            float maxP)
        {
            this.p = p;
            if (p < minP) this.p = minP;
            if (p > maxP) this.p = maxP;

            uw = scaler.MeasureInPixels(axis.UnitWidth);
            actualUw = uw;

            var gp = (float)boxSeries.Padding;

            if (uw - gp < 1) gp -= uw - gp;

            uw -= gp;
            uwm = 0.5f * uw;

            int pos, count;

            pos = cartesianChart.SeriesContext.GetBoxPosition(boxSeries);
            count = cartesianChart.SeriesContext.GetBoxSeriesCount();

            cp = 0f;

            var padding = (float)boxSeries.Padding;

            uw /= count;
            var mw = (float)boxSeries.MaxBarWidth;
            if (uw > mw) uw = mw;
            uwm = 0.5f * uw;
            cp = (pos - count / 2f) * uw + uwm;

            // apply the pading
            uw -= padding;
            cp += padding * 0.5f;

            if (uw < 1)
            {
                uw = 1;
                uwm = 0.5f;
            }
        }

        /// <summary>
        /// helper units.
        /// </summary>
        public float uw, uwm, cp, p, actualUw;
    }
}
