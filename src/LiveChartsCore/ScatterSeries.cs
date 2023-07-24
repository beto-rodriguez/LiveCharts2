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
        where TVisual : class, ISizedGeometry<TDrawingContext>, new()
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

        DataLabelsFormatter = (point) => $"{point.Coordinate.PrimaryValue}";
        YToolTipLabelFormatter = point =>
        {
            var series = (ScatterSeries<TModel, TVisual, TLabel, TDrawingContext>)point.Context.Series;
            var c = point.Coordinate;
            return series.IsWeighted
                ? $"X = {c.SecondaryValue}{Environment.NewLine}" +
                  $"Y = {c.PrimaryValue}{Environment.NewLine}" +
                  $"W = {c.TertiaryValue}"
                : $"X = {c.SecondaryValue}{Environment.NewLine}" +
                  $"Y = {c.PrimaryValue}";
        };
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

    /// <summary>
    /// Gets a value indicating whether the points in this series use weight.
    /// </summary>
    public bool IsWeighted { get; private set; }

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
            DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var dls = (float)DataLabelsSize;
        var pointsCleanup = ChartPointCleanupContext.For(everFetched);

        var gs = (float)GeometrySize;
        var hgs = gs / 2f;
        var sw = Stroke?.StrokeThickness ?? 0;
        IsWeighted = _weightBounds.Max - _weightBounds.Min > 0;
        var wm = -(GeometrySize - MinGeometrySize) / (_weightBounds.Max - _weightBounds.Min);

        var uwx = xScale.MeasureInPixels(secondaryAxis.UnitWidth);
        var uwy = yScale.MeasureInPixels(secondaryAxis.UnitWidth);

        uwx = uwx < gs ? gs : uwx;
        uwy = uwy < gs ? gs : uwy;

        var hy = chart.ControlSize.Height * .5f;

        foreach (var point in Fetch(cartesianChart))
        {
            var coordinate = point.Coordinate;

            var visual = (TVisual?)point.Context.Visual;

            var x = xScale.ToPixels(coordinate.SecondaryValue);
            var y = yScale.ToPixels(coordinate.PrimaryValue);

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

            if (IsWeighted)
            {
                gs = (float)(wm * (_weightBounds.Max - coordinate.TertiaryValue) + GeometrySize);
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

            Fill?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
            Stroke?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

            var sizedGeometry = visual;

            sizedGeometry.X = x - hgs;
            sizedGeometry.Y = y - hgs;
            sizedGeometry.Width = gs;
            sizedGeometry.Height = gs;

            sizedGeometry.RemoveOnCompleted = false;

            if (point.Context.HoverArea is not RectangleHoverArea ha)
                point.Context.HoverArea = ha = new RectangleHoverArea();
            _ = ha.SetDimensions(x - uwx * 0.5f, y - uwy * 0.5f, uwx, uwy).CenterXToolTip().CenterYToolTip();

            pointsCleanup.Clean(point);

            if (DataLabelsPaint is not null)
            {
                if (point.Context.Label is not TLabel label)
                {
                    var l = new TLabel { X = x - hgs, Y = y - hgs, RotateTransform = (float)DataLabelsRotation };
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
                    x - hgs, y - hgs, gs, gs, m, DataLabelsPosition,
                    SeriesProperties, coordinate.PrimaryValue > 0, drawLocation, drawMarginSize);
                if (DataLabelsTranslate is not null) label.TranslateTransform =
                        new LvcPoint(m.Width * DataLabelsTranslate.Value.X, m.Height * DataLabelsTranslate.Value.Y);
                label.X = labelPosition.X;
                label.Y = labelPosition.Y;
            }

            OnPointMeasured(point);
        }

        pointsCleanup.CollectPoints(everFetched, cartesianChart.View, yScale, xScale, SoftDeleteOrDisposePoint);
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override SeriesBounds GetBounds(CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var seriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);
        _weightBounds = seriesBounds.Bounds.TertiaryBounds;
        return seriesBounds;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.GetMiniaturesSketch"/>
    public override Sketch<TDrawingContext> GetMiniaturesSketch()
    {
        var schedules = new List<PaintSchedule<TDrawingContext>>();

        if (Fill is not null) schedules.Add(BuildMiniatureSchedule(Fill, new TVisual()));
        if (Stroke is not null) schedules.Add(BuildMiniatureSchedule(Stroke, new TVisual()));

        return new Sketch<TDrawingContext>(MiniatureShapeSize, MiniatureShapeSize, GeometrySvg)
        {
            PaintSchedules = schedules
        };
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.OnPointerEnter(ChartPoint)"/>
    protected override void OnPointerEnter(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.Opacity = 0.8f;
        if (!IsWeighted) visual.ScaleTransform = new LvcPoint(1.1f, 1.1f);

        base.OnPointerEnter(point);
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.OnPointerLeft(ChartPoint)"/>
    protected override void OnPointerLeft(ChartPoint point)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        visual.Opacity = 1;
        if (!IsWeighted) visual.ScaleTransform = new LvcPoint(1f, 1f);

        base.OnPointerLeft(point);
    }

    /// <inheritdoc cref="SetDefaultPointTransitions(ChartPoint)"/>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        var visual = (TVisual?)chartPoint.Context.Visual;
        var chart = chartPoint.Context.Chart;

        if (visual is null) throw new Exception("Unable to initialize the point instance.");

        visual.Animate(EasingFunction ?? chart.EasingFunction, AnimationsSpeed ?? chart.AnimationsSpeed);
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

        var c = point.Coordinate;

        var x = secondaryScale.ToPixels(c.SecondaryValue);
        var y = primaryScale.ToPixels(c.PrimaryValue);

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
