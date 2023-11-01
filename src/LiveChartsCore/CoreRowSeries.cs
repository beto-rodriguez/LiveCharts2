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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines the row series 
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="BarSeries{TModel, TVisual, TLabel, TDrawingContext}" />
/// <typeparam name="TErrorGeometry">The type of the error geometry.</typeparam>
public class CoreRowSeries<TModel, TVisual, TLabel, TDrawingContext, TErrorGeometry> : BarSeries<TModel, TVisual, TLabel, TDrawingContext>
    where TVisual : class, ISizedGeometry<TDrawingContext>, new()
    where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    where TErrorGeometry : class, ILineGeometry<TDrawingContext>, new()
    where TDrawingContext : DrawingContext
{
    private readonly bool _isRounded = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreRowSeries{TModel, TVisual, TLabel, TDrawingContext, TErrorGeometry}"/> class.
    /// </summary>
    public CoreRowSeries(bool isStacked = false)
        : base(
              SeriesProperties.Bar | SeriesProperties.PrimaryAxisHorizontalOrientation |
              SeriesProperties.Solid | SeriesProperties.PrefersYStrategyTooltips | (isStacked ? SeriesProperties.Stacked : 0))
    {
        DataPadding = new LvcPoint(1, 0);
        _isRounded = typeof(IRoundedGeometry<TDrawingContext>).IsAssignableFrom(typeof(TVisual));
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        var cartesianChart = (CartesianChart<TDrawingContext>)chart;
        var primaryAxis = cartesianChart.YAxes[ScalesYAt];
        var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;
        var secondaryScale = primaryAxis.GetNextScaler(cartesianChart);
        var primaryScale = secondaryAxis.GetNextScaler(cartesianChart);
        var previousPrimaryScale = secondaryAxis.GetActualScaler(cartesianChart);
        var previousSecondaryScale = primaryAxis.GetActualScaler(cartesianChart);

        var isStacked = (SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked;
        var clipping = GetClipRectangle(cartesianChart);

        var helper = new MeasureHelper(
            secondaryScale, cartesianChart, this, secondaryAxis, primaryScale.ToPixels(pivot),
            cartesianChart.DrawMarginLocation.X,
            cartesianChart.DrawMarginLocation.X + cartesianChart.DrawMarginSize.Width, isStacked, true);

        var pHelper = previousSecondaryScale == null || previousPrimaryScale == null
            ? null
            : new MeasureHelper(
                previousSecondaryScale, cartesianChart, this, secondaryAxis, previousPrimaryScale.ToPixels(pivot),
                cartesianChart.DrawMarginLocation.X,
                cartesianChart.DrawMarginLocation.X + cartesianChart.DrawMarginSize.Width, isStacked, true);

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
        if (Fill is not null)
        {
            Fill.ZIndex = actualZIndex + 0.1;
            Fill.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(Fill);
        }
        if (Stroke is not null)
        {
            Stroke.ZIndex = actualZIndex + 0.2;
            Stroke.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(Stroke);
        }
        if (ErrorPaint is not null)
        {
            ErrorPaint.ZIndex = actualZIndex + 0.3;
            ErrorPaint.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(ErrorPaint);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = actualZIndex + 0.4;
            DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var dls = (float)DataLabelsSize;
        var pointsCleanup = ChartPointCleanupContext.For(everFetched);

        var rx = (float)Rx;
        var ry = (float)Ry;

        var stacker = isStacked ? cartesianChart.SeriesContext.GetStackPosition(this, GetStackGroup()) : null;
        var hasSvg = this.HasSvgGeometry();

        var isFirstDraw = !chart._drawnSeries.Contains(((ISeries)this).SeriesId);

        foreach (var point in Fetch(cartesianChart))
        {
            var coordinate = point.Coordinate;

            var visual = point.Context.Visual as TVisual;
            var e = point.Context.AdditionalVisuals as ErrorVisual<TErrorGeometry>;

            var primary = primaryScale.ToPixels(coordinate.PrimaryValue);
            var secondary = secondaryScale.ToPixels(coordinate.SecondaryValue);
            var b = Math.Abs(primary - helper.p);

            if (point.IsEmpty)
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
                var yi = secondary - helper.uwm + helper.cp;
                var pi = helper.p;
                var uwi = helper.uw;
                var hi = 0f;

                if (previousSecondaryScale is not null && previousPrimaryScale is not null && pHelper is not null)
                {
                    var previousPrimary = previousPrimaryScale.ToPixels(coordinate.PrimaryValue);
                    var bp = Math.Abs(previousPrimary - pHelper.p);
                    var cyp = coordinate.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                    yi = previousSecondaryScale.ToPixels(coordinate.SecondaryValue) - pHelper.uwm + pHelper.cp;
                    pi = cartesianChart.IsZoomingOrPanning ? cyp : pHelper.p;
                    uwi = pHelper.uw;
                    hi = cartesianChart.IsZoomingOrPanning ? bp : 0;
                }

                var r = new TVisual
                {
                    X = pi,
                    Y = yi,
                    Width = hi,
                    Height = uwi
                };

                if (_isRounded)
                {
                    var rounded = (IRoundedGeometry<TDrawingContext>)r;
                    rounded.BorderRadius = new LvcPoint(rx, ry);
                }

                if (ErrorPaint is not null)
                {
                    e = new ErrorVisual<TErrorGeometry>();

                    e.YError.X = pi;
                    e.YError.X1 = pi;
                    e.YError.Y = yi;
                    e.YError.Y1 = yi;

                    e.XError.X = pi;
                    e.XError.X1 = pi;
                    e.XError.Y = yi;
                    e.XError.Y1 = yi;

                    point.Context.AdditionalVisuals = e;
                }

                visual = r;
                point.Context.Visual = visual;
                OnPointCreated(point);

                _ = everFetched.Add(point);
            }

            if (hasSvg)
            {
                var svgVisual = (ISvgPath<TDrawingContext>)visual;
                if (_geometrySvgChanged || svgVisual.SVGPath is null)
                    svgVisual.SVGPath = GeometrySvg ?? throw new Exception("svg path is not defined");
            }

            Fill?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
            Stroke?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
            ErrorPaint?.AddGeometryToPaintTask(cartesianChart.Canvas, e!.YError);
            ErrorPaint?.AddGeometryToPaintTask(cartesianChart.Canvas, e!.XError);

            var cx = secondaryAxis.IsInverted
                ? (coordinate.PrimaryValue > pivot ? primary : primary - b)
                : (coordinate.PrimaryValue > pivot ? primary - b : primary);
            var y = secondary - helper.uwm + helper.cp;

            if (stacker is not null)
            {
                var sx = stacker.GetStack(point);

                float primaryI, primaryJ;
                if (coordinate.PrimaryValue >= 0)
                {
                    primaryI = primaryScale.ToPixels(sx.Start);
                    primaryJ = primaryScale.ToPixels(sx.End);
                }
                else
                {
                    primaryI = primaryScale.ToPixels(sx.NegativeStart);
                    primaryJ = primaryScale.ToPixels(sx.NegativeEnd);
                }

                cx = primaryJ;
                b = primaryI - primaryJ;
            }

            visual.X = cx;
            visual.Y = y;
            visual.Width = b;
            visual.Height = helper.uw;

            if (!coordinate.PointError.IsEmpty && ErrorPaint is not null)
            {
                var pe = coordinate.PointError;
                var ye = secondary - helper.uwm + helper.cp + helper.uw * 0.5f;

                // Note #20231608
                // Because coordinates are inverted in the row series
                // we use the XError as the YError and vice versa
                // strange.. this should be improved.

                e!.YError!.X = primary + primaryScale.MeasureInPixels(pe.Yi);
                e.YError.X1 = primary - primaryScale.MeasureInPixels(pe.Yi);
                e.YError.Y = ye;
                e.YError.Y1 = ye;
                e.YError.RemoveOnCompleted = false;

                e.XError!.X = primary;
                e.XError.X1 = primary;
                e.XError.Y = ye - secondaryScale.MeasureInPixels(pe.Xi);
                e.XError.Y1 = ye + secondaryScale.MeasureInPixels(pe.Xj);
                e.XError.RemoveOnCompleted = false;
            }

            if (_isRounded)
            {
                var rounded = (IRoundedGeometry<TDrawingContext>)visual;
                rounded.BorderRadius = new LvcPoint(rx, ry);
            }
            visual.RemoveOnCompleted = false;

            if (point.Context.HoverArea is not RectangleHoverArea ha)
                point.Context.HoverArea = ha = new RectangleHoverArea();

            _ = ha.SetDimensions(cx, secondary - helper.actualUw * 0.5f, b, helper.actualUw)
                .CenterXToolTip().StartYToolTip();

            pointsCleanup.Clean(point);

            if (DataLabelsPaint is not null)
            {
                var label = (TLabel?)point.Context.Label;

                if (label is null)
                {
                    var l = new TLabel { X = helper.p, Y = secondary - helper.uwm + helper.cp, RotateTransform = (float)DataLabelsRotation, MaxWidth = (float)DataLabelsMaxWidth };
                    l.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
                    label = l;
                    point.Context.Label = l;
                }

                DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);

                label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));
                label.TextSize = dls;
                label.Padding = DataLabelsPadding;

                if (isFirstDraw)
                    label.CompleteTransition(
                        nameof(label.TextSize), nameof(label.X), nameof(label.Y), nameof(label.RotateTransform));

                var m = label.Measure(DataLabelsPaint);
                var labelPosition = GetLabelPosition(
                    cx, y, b, helper.uw, label.Measure(DataLabelsPaint),
                    DataLabelsPosition, SeriesProperties, coordinate.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                if (DataLabelsTranslate is not null) label.TranslateTransform =
                        new LvcPoint(m.Width * DataLabelsTranslate.Value.X, m.Height * DataLabelsTranslate.Value.Y);

                label.X = labelPosition.X;
                label.Y = labelPosition.Y;
            }

            OnPointMeasured(point);
        }

        pointsCleanup.CollectPoints(
            everFetched, cartesianChart.View, primaryScale, secondaryScale, SoftDeleteOrDisposePoint);
        _geometrySvgChanged = false;
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

        var easing = EasingFunction ?? chart.EasingFunction;
        var speed = AnimationsSpeed ?? chart.AnimationsSpeed;

        visual.Animate(easing, speed);

        if (chartPoint.Context.AdditionalVisuals is not null)
        {
            var e = (ErrorVisual<TErrorGeometry>)chartPoint.Context.AdditionalVisuals;
            e.YError.Animate(easing, speed);
            e.XError.Animate(easing, speed);
        }
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

        visual.X = p;
        visual.Y = secondary - visual.Height * 0.5f;
        visual.Width = 0;
        visual.RemoveOnCompleted = true;

        if (point.Context.AdditionalVisuals is not null)
        {
            var e = (ErrorVisual<TErrorGeometry>)point.Context.AdditionalVisuals;

            e.YError.Y = secondary - visual.Height * 0.5f;
            e.YError.Y1 = secondary - visual.Height * 0.5f;
            e.YError.RemoveOnCompleted = true;

            e.XError.X = p;
            e.XError.X1 = p;
            e.XError.RemoveOnCompleted = true;
        }

        DataFactory.DisposePoint(point);

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, ICartesianAxis, ICartesianAxis)"/>
    public override SeriesBounds GetBounds(CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var rawBounds = DataFactory.GetCartesianBounds(chart, this, primaryAxis, secondaryAxis);
        if (rawBounds.HasData) return rawBounds;

        var rawBaseBounds = rawBounds.Bounds;

        var tickPrimary = primaryAxis.GetTick(chart.ControlSize, rawBaseBounds.VisibleSecondaryBounds);
        var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, rawBaseBounds.VisiblePrimaryBounds);

        var ts = tickSecondary.Value * DataPadding.X;
        var tp = tickPrimary.Value * DataPadding.Y;

        // using different methods for both primary and secondary axis seems to be the best solution
        // if this the following 2 lines needs to be changed again, please ensure that the following test passes:
        // https://github.com/beto-rodriguez/LiveCharts2/issues/522
        // https://github.com/beto-rodriguez/LiveCharts2/issues/642

        if (rawBaseBounds.VisibleSecondaryBounds.Delta == 0) tp = secondaryAxis.UnitWidth * DataPadding.X;
        if (rawBaseBounds.VisiblePrimaryBounds.Delta == 0) ts = rawBaseBounds.VisiblePrimaryBounds.Max * 0.25f;

        var rgs = GetRequestedGeometrySize();
        var rso = GetRequestedSecondaryOffset();
        var rpo = GetRequestedPrimaryOffset();

        var dimensionalBounds = new DimensionalBounds
        {
            SecondaryBounds = new Bounds
            {
                Max = rawBaseBounds.PrimaryBounds.Max + rpo * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.PrimaryBounds.Min - rpo * secondaryAxis.UnitWidth,
                MinDelta = rawBaseBounds.PrimaryBounds.MinDelta,
                PaddingMax = ts,
                PaddingMin = ts,
                RequestedGeometrySize = rgs
            },
            PrimaryBounds = new Bounds
            {
                Max = rawBaseBounds.SecondaryBounds.Max + rso * primaryAxis.UnitWidth,
                Min = rawBaseBounds.SecondaryBounds.Min - rso * primaryAxis.UnitWidth,
                MinDelta = rawBaseBounds.SecondaryBounds.MinDelta,
                PaddingMax = tp,
                PaddingMin = tp,
                RequestedGeometrySize = rgs
            },
            VisibleSecondaryBounds = new Bounds
            {
                Max = rawBaseBounds.VisiblePrimaryBounds.Max + rpo * secondaryAxis.UnitWidth,
                Min = rawBaseBounds.VisiblePrimaryBounds.Min - rpo * secondaryAxis.UnitWidth
            },
            VisiblePrimaryBounds = new Bounds
            {
                Max = rawBaseBounds.VisibleSecondaryBounds.Max + rso * primaryAxis.UnitWidth,
                Min = rawBaseBounds.VisibleSecondaryBounds.Min - rso * primaryAxis.UnitWidth,
            },
            TertiaryBounds = rawBaseBounds.TertiaryBounds,
            VisibleTertiaryBounds = rawBaseBounds.VisibleTertiaryBounds
        };

        return new SeriesBounds(dimensionalBounds, false);
    }
}
