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
/// Defines a scatter series.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}" />
/// <seealso cref="IScatterSeries{TDrawingContext}" />
public class ScatterSeries<TModel, TVisual, TLabel, TDrawingContext>
    : StrokeAndFillCartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IScatterSeries<TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
{
    private Bounds _weightBounds = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ScatterSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    public ScatterSeries()
        : base(SeriesProperties.Scatter | SeriesProperties.Solid | SeriesProperties.PrefersXYStrategyTooltips)
    {
        DataPadding = new LvcPoint(1, 1);

        DataLabelsFormatter = (point) => $"{point.SecondaryValue}, {point.PrimaryValue}";
        TooltipLabelFormatter = (point) => $"{point.Context.Series.Name} {point.SecondaryValue}, {point.PrimaryValue}";
    }

    /// <summary>
    /// Gets or sets the minimum size of the geometry.
    /// </summary>
    /// <value>
    /// The minimum size of the geometry.
    /// </value>
    public double MinGeometrySize { get; set; } = 6d;

    /// <summary>
    /// Gets or sets the size of the geometry.
    /// </summary>
    /// <value>
    /// The size of the geometry.
    /// </value>
    public double GeometrySize { get; set; } = 24d;

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        var cartesianChart = (CartesianChart<TDrawingContext>)chart;
        var primaryAxis = cartesianChart.YAxes[ScalesYAt];
        var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;
        var xScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
        var yScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);

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

        var gs = (float)GeometrySize;
        var hgs = gs / 2f;
        var sw = Stroke?.StrokeThickness ?? 0;
        var requiresWScale = _weightBounds.Max - _weightBounds.Min > 0;
        var wm = -(GeometrySize - MinGeometrySize) / (_weightBounds.Max - _weightBounds.Min);

        var uwx = xScale.MeasureInPixels(secondaryAxis.UnitWidth);
        var uwy = yScale.MeasureInPixels(secondaryAxis.UnitWidth);

        uwx = uwx < gs ? gs : uwx;
        uwy = uwy < gs ? gs : uwy;

        foreach (var point in Fetch(cartesianChart))
        {
            var visual = (TVisual?)point.Context.Visual;

            var x = xScale.ToPixels(point.SecondaryValue);
            var y = yScale.ToPixels(point.PrimaryValue);

            if (point.IsEmpty)
            {
                if (visual is not null)
                {
                    visual.X = x - hgs;
                    visual.Y = y - hgs;
                    visual.Width = 0;
                    visual.Height = 0;
                    visual.RemoveOnCompleted = true;
                    point.Context.Visual = null;
                }
                continue;
            }

            if (requiresWScale)
            {
                gs = (float)(wm * (_weightBounds.Max - point.TertiaryValue) + GeometrySize);
                hgs = gs / 2f;
            }

            if (visual is null)
            {
                var r = new TVisual
                {
                    X = x,
                    Y = y,
                    Width = 0,
                    Height = 0
                };

                visual = r;
                point.Context.Visual = visual;
                OnPointCreated(point);

                _ = everFetched.Add(point);
            }

            if (Fill is not null) Fill.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
            if (Stroke is not null) Stroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

            var sizedGeometry = visual;

            sizedGeometry.X = x - hgs;
            sizedGeometry.Y = y - hgs;
            sizedGeometry.Width = gs;
            sizedGeometry.Height = gs;

            sizedGeometry.RemoveOnCompleted = false;

            point.Context.HoverArea = new RectangleHoverArea(x - uwx * 0.5f, y - uwy * 0.5f, uwx, uwy);

            _ = toDeletePoints.Remove(point);

            if (DataLabelsPaint is not null)
            {
                if (point.Context.Label is not TLabel label)
                {
                    var l = new TLabel { X = x - hgs, Y = y - hgs, RotateTransform = (float)DataLabelsRotation };

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
                    x - hgs, y - hgs, gs, gs, m, DataLabelsPosition,
                    SeriesProperties, point.PrimaryValue > 0, drawLocation, drawMarginSize);
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
            SoftDeleteOrDisposePoint(point, yScale, xScale);
            _ = everFetched.Remove(point);
        }
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override SeriesBounds GetBounds(CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var seriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);
        _weightBounds = seriesBounds.Bounds.TertiaryBounds;
        return seriesBounds;
    }

    /// <inheritdoc cref="OnSeriesMiniatureChanged"/>
    protected override void OnSeriesMiniatureChanged()
    {
        var context = new CanvasSchedule<TDrawingContext>();
        var w = LegendShapeSize;
        var sh = 0f;

        if (Stroke is not null)
        {
            var strokeClone = Stroke.CloneTask();
            var st = Stroke.StrokeThickness;
            if (st > MaxSeriesStroke)
            {
                st = MaxSeriesStroke;
                strokeClone.StrokeThickness = MaxSeriesStroke;
            }

            var visual = new TVisual
            {
                X = st + MaxSeriesStroke - st,
                Y = st + MaxSeriesStroke - st,
                Height = (float)LegendShapeSize,
                Width = (float)LegendShapeSize
            };
            sh = st;
            strokeClone.ZIndex = 1;
            w += 2 * st;
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
        }

        if (Fill is not null)
        {
            var fillClone = Fill.CloneTask();
            var visual = new TVisual
            {
                X = sh + MaxSeriesStroke - sh,
                Y = sh + MaxSeriesStroke - sh,
                Height = (float)LegendShapeSize,
                Width = (float)LegendShapeSize
            };
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
        }

        context.Width = w;
        context.Height = w;

        CanvasSchedule = context;
    }

    /// <inheritdoc cref="SetDefaultPointTransitions(ChartPoint)"/>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        var visual = (TVisual?)chartPoint.Context.Visual;
        var chart = chartPoint.Context.Chart;

        if (visual is null) throw new Exception("Unable to initialize the point instance.");

        _ = visual
           .TransitionateProperties(
               nameof(visual.X),
               nameof(visual.Y),
               nameof(visual.Width),
               nameof(visual.Height))
           .WithAnimation(animation =>
               animation
                   .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                   .WithEasingFunction(EasingFunction ?? chart.EasingFunction))
           .CompleteCurrentTransitions();
    }

    /// <inheritdoc cref="SoftDeleteOrDisposePoint(ChartPoint, Scaler, Scaler)"/>
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

        var x = secondaryScale.ToPixels(point.SecondaryValue);
        var y = primaryScale.ToPixels(point.PrimaryValue);

        visual.X = x;
        visual.Y = y;
        visual.Height = 0;
        visual.Width = 0;
        visual.RemoveOnCompleted = true;

        DataFactory.DisposePoint(point);

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }
}
