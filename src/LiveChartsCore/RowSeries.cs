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

using System;
using System.Collections.Generic;
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
public class RowSeries<TModel, TVisual, TLabel, TDrawingContext> : BarSeries<TModel, TVisual, TLabel, TDrawingContext>
    where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
    where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    where TDrawingContext : DrawingContext
{
    private readonly bool _isRounded = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="RowSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    public RowSeries(bool isStacked = false)
        : base(
              SeriesProperties.Bar | SeriesProperties.PrimaryAxisHorizontalOrientation |
              SeriesProperties.Solid | SeriesProperties.PrefersYStrategyTooltips | (isStacked ? SeriesProperties.Stacked : 0))
    {
        _isRounded = typeof(IRoundedRectangleChartPoint<TDrawingContext>).IsAssignableFrom(typeof(TVisual));
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

        var helper = new MeasureHelper(secondaryScale, cartesianChart, this, secondaryAxis, primaryScale.ToPixels(pivot),
            cartesianChart.DrawMarginLocation.X, cartesianChart.DrawMarginLocation.X + cartesianChart.DrawMarginSize.Width, isStacked);

        var pHelper = previousSecondaryScale == null || previousPrimaryScale == null
            ? null
            : new MeasureHelper(
                previousSecondaryScale, cartesianChart, this, secondaryAxis, previousPrimaryScale.ToPixels(pivot),
                cartesianChart.DrawMarginLocation.X, cartesianChart.DrawMarginLocation.X + cartesianChart.DrawMarginSize.Width, isStacked);

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
        if (Fill is not null)
        {
            Fill.ZIndex = actualZIndex + 0.1;
            Fill.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(Fill);
        }
        if (Stroke is not null)
        {
            Stroke.ZIndex = actualZIndex + 0.2;
            Stroke.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(Stroke);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = actualZIndex + 0.3;
            //DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var dls = (float)DataLabelsSize;
        var toDeletePoints = new HashSet<ChartPoint>(everFetched);

        var rx = (float)Rx;
        var ry = (float)Ry;

        var stacker = isStacked ? cartesianChart.SeriesContext.GetStackPosition(this, GetStackGroup()) : null;

        foreach (var point in Fetch(cartesianChart))
        {
            var visual = point.Context.Visual as TVisual;
            var primary = primaryScale.ToPixels(point.PrimaryValue);
            var secondary = secondaryScale.ToPixels(point.SecondaryValue);
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
                    var previousPrimary = previousPrimaryScale.ToPixels(point.PrimaryValue);
                    var bp = Math.Abs(previousPrimary - pHelper.p);
                    var cyp = point.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                    yi = previousSecondaryScale.ToPixels(point.SecondaryValue) - pHelper.uwm + pHelper.cp;
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
                    var rounded = (IRoundedRectangleChartPoint<TDrawingContext>)r;
                    rounded.Rx = rx;
                    rounded.Ry = ry;
                }

                visual = r;
                point.Context.Visual = visual;
                OnPointCreated(point);

                _ = everFetched.Add(point);
            }

            if (Fill is not null) Fill.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
            if (Stroke is not null) Stroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

            var cx = point.PrimaryValue > pivot ? primary - b : primary;
            var y = secondary - helper.uwm + helper.cp;

            if (stacker is not null)
            {
                var sx = stacker.GetStack(point);

                float primaryI, primaryJ;
                if (point.PrimaryValue >= 0)
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

            if (_isRounded)
            {
                var rounded = (IRoundedRectangleChartPoint<TDrawingContext>)visual;
                rounded.Rx = rx;
                rounded.Ry = ry;
            }
            visual.RemoveOnCompleted = false;

            point.Context.HoverArea = new RectangleHoverArea(cx, secondary - helper.actualUw * 0.5f, b, helper.actualUw);

            _ = toDeletePoints.Remove(point);

            if (DataLabelsPaint is not null)
            {
                var label = (TLabel?)point.Context.Label;

                if (label is null)
                {
                    var l = new TLabel { X = helper.p, Y = secondary - helper.uwm + helper.cp, RotateTransform = (float)DataLabelsRotation };

                    _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                        .WithAnimation(animation =>
                            animation
                                .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));

                    l.CompleteTransition(null);
                    label = l;
                    point.Context.Label = l;
                }

                DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);

                label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));
                label.TextSize = dls;
                label.Padding = DataLabelsPadding;
                var m = label.Measure(DataLabelsPaint);
                var labelPosition = GetLabelPosition(
                    cx, y, b, helper.uw, label.Measure(DataLabelsPaint),
                    DataLabelsPosition, SeriesProperties, point.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                if (DataLabelsTranslate is not null) label.TranslateTransform =
                        new LvcPoint(m.Width * DataLabelsTranslate.Value.X, m.Height * DataLabelsTranslate.Value.Y);

                label.X = labelPosition.X;
                label.Y = labelPosition.Y;
            }

            OnPointMeasured(point);
        }

        foreach (var point in toDeletePoints)
        {
            if (point.Context.Chart != cartesianChart.View) continue;
            SoftDeleteOrDisposePoint(point, primaryScale, secondaryScale);
            _ = everFetched.Remove(point);
        }
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.GetRequestedSecondaryOffset"/>
    protected override double GetRequestedSecondaryOffset()
    {
        return 0.5f;
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.GetIsInvertedBounds"/>
    protected override bool GetIsInvertedBounds()
    {
        return true;
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
                    .WithEasingFunction(EasingFunction ?? chart.EasingFunction))
            .CompleteCurrentTransitions();
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
        var secondary = secondaryScale.ToPixels(point.SecondaryValue);

        visual.X = p;
        visual.Y = secondary;
        visual.Width = 0;
        visual.RemoveOnCompleted = true;

        DataFactory.DisposePoint(point);

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }
}
