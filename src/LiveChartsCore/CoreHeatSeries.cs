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
using LiveChartsCore.Painting;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore;

/// <summary>
/// Defines a column series.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
public abstract class CoreHeatSeries<TModel, TVisual, TLabel>
    : CartesianSeries<TModel, TVisual, TLabel>, IHeatSeries
        where TVisual : BoundedDrawnGeometry, IColoredGeometry, new()
        where TLabel : BaseLabelGeometry, new()
{
    private Paint? _paintTaks;
    private Bounds _weightBounds = new();
    private int _heatKnownLength = 0;
    private List<Tuple<double, LvcColor>> _heatStops = [];
    private LvcColor[] _heatMap =
    [
        LvcColor.FromArgb(255, 87, 103, 222), // cold (min value)
        LvcColor.FromArgb(255, 95, 207, 249) // hot (max value)
    ];
    private double[]? _colorStops;
    private Padding _pointPadding = new(4);
    private double? _minValue;
    private double? _maxValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreHeatSeries{TModel, TVisual, TLabel}"/> class.
    /// </summary>
    /// <param name="values">The values.</param>
    protected CoreHeatSeries(IReadOnlyCollection<TModel>? values)
        : base(GetProperties(), values)
    {
        DataPadding = new LvcPoint(0, 0);
        YToolTipLabelFormatter = (point) =>
        {
            var cc = (CartesianChartEngine)point.Context.Chart.CoreChart;
            var cs = (ICartesianSeries)point.Context.Series;

            var ax = cc.YAxes[cs.ScalesYAt];

            var labeler = ax.Labeler;
            if (ax.Labels is not null) labeler = Labelers.BuildNamedLabeler(ax.Labels);

            var c = point.Coordinate;

            return $"{labeler(c.PrimaryValue)} {c.TertiaryValue}";
        };
        DataLabelsPosition = DataLabelsPosition.Middle;
    }

    /// <inheritdoc cref="IHeatSeries.HeatMap"/>
    public LvcColor[] HeatMap
    {
        get => _heatMap;
        set => SetProperty(ref _heatMap, value);
    }

    /// <inheritdoc cref="IHeatSeries.ColorStops"/>
    public double[]? ColorStops { get => _colorStops; set => SetProperty(ref _colorStops, value); }

    /// <inheritdoc cref="IHeatSeries.PointPadding"/>
    public Padding PointPadding { get => _pointPadding; set => SetProperty(ref _pointPadding, value); }

    /// <inheritdoc cref="IHeatSeries.MinValue"/>
    public double? MinValue { get => _minValue; set => SetProperty(ref _minValue, value); }

    /// <inheritdoc cref="IHeatSeries.MaxValue"/>
    public double? MaxValue { get => _maxValue; set => SetProperty(ref _maxValue, value); }

    /// <inheritdoc cref="ChartElement.Invalidate(Chart)"/>
    public override void Invalidate(Chart chart)
    {
        if (_paintTaks is null)
        {
            _paintTaks = LiveCharts.DefaultSettings.GetProvider().GetSolidColorPaint();
            _paintTaks.PaintStyle = PaintStyle.Fill;
        }

        var cartesianChart = (CartesianChartEngine)chart;
        var primaryAxis = cartesianChart.YAxes[ScalesYAt];
        var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;
        var secondaryScale = secondaryAxis.GetNextScaler(cartesianChart);
        var primaryScale = primaryAxis.GetNextScaler(cartesianChart);
        var previousPrimaryScale = primaryAxis.GetActualScaler(cartesianChart);
        var previousSecondaryScale = secondaryAxis.GetActualScaler(cartesianChart);

        var uws = secondaryScale.MeasureInPixels(secondaryAxis.UnitWidth);
        var uwp = primaryScale.MeasureInPixels(primaryAxis.UnitWidth);

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
        var clipping = GetClipRectangle(cartesianChart);

        if (_paintTaks is not null)
        {
            _paintTaks.ZIndex = actualZIndex + 0.2;
            _paintTaks.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(_paintTaks);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = actualZIndex + 0.3;
            DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var dls = (float)DataLabelsSize;
        var pointsCleanup = ChartPointCleanupContext.For(everFetched);

        var p = PointPadding;

        if (_heatKnownLength != HeatMap.Length)
        {
            _heatStops = HeatFunctions.BuildColorStops(HeatMap, ColorStops);
            _heatKnownLength = HeatMap.Length;
        }

        var hasSvg = this.HasVariableSvgGeometry();

        var isFirstDraw = !chart.IsDrawn(((ISeries)this).SeriesId);

        var provider = LiveCharts.DefaultSettings.GetProvider();

        foreach (var point in Fetch(cartesianChart))
        {
            var coordinate = point.Coordinate;
            var visual = point.Context.Visual as TVisual;
            var primary = primaryScale.ToPixels(coordinate.PrimaryValue);
            var secondary = secondaryScale.ToPixels(coordinate.SecondaryValue);
            var tertiary = (float)coordinate.TertiaryValue;

            var baseColor = HeatFunctions.InterpolateColor(tertiary, _weightBounds, HeatMap, _heatStops);

            if (point.IsEmpty || !IsVisible)
            {
                if (visual is not null)
                {
                    visual.X = secondary - uws * 0.5f;
                    visual.Y = primary - uwp * 0.5f;
                    visual.Width = uws;
                    visual.Height = uwp;
                    visual.RemoveOnCompleted = true;
                    visual.Color = LvcColor.FromArgb(0, visual.Color);
                    point.Context.Visual = null;
                }

                if (point.Context.Label is not null)
                {
                    var label = (TLabel)point.Context.Label;

                    label.X = secondary - uws * 0.5f;
                    label.Y = primary - uwp * 0.5f;
                    label.Opacity = 0;
                    label.RemoveOnCompleted = true;

                    point.Context.Label = null;
                }

                pointsCleanup.Clean(point);

                continue;
            }

            if (visual is null)
            {
                var xi = secondary - uws * 0.5f;
                var yi = primary - uwp * 0.5f;

                if (previousSecondaryScale is not null && previousPrimaryScale is not null)
                {
                    var previousP = previousPrimaryScale.ToPixels(pivot);
                    var previousPrimary = previousPrimaryScale.ToPixels(coordinate.PrimaryValue);
                    var bp = Math.Abs(previousPrimary - previousP);
                    var cyp = coordinate.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                    xi = previousSecondaryScale.ToPixels(coordinate.SecondaryValue) - uws * 0.5f;
                    yi = previousPrimaryScale.ToPixels(coordinate.PrimaryValue) - uwp * 0.5f;
                }

                var r = new TVisual
                {
                    X = xi + p.Left,
                    Y = yi + p.Top,
                    Width = uws - p.Left - p.Right,
                    Height = uwp - p.Top - p.Bottom,
                    Color = LvcColor.FromArgb(0, baseColor.R, baseColor.G, baseColor.B)
                };

                visual = r;
                point.Context.Visual = visual;
                OnPointCreated(point);

                _ = everFetched.Add(point);
            }

            if (hasSvg)
            {
                var svgVisual = (IVariableSvgPath)visual;
                if (_geometrySvgChanged || svgVisual.SVGPath is null)
                    svgVisual.SVGPath = GeometrySvg ?? throw new Exception("svg path is not defined");
            }

            _paintTaks?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

            visual.X = secondary - uws * 0.5f + p.Left;
            visual.Y = primary - uwp * 0.5f + p.Top;
            visual.Width = uws - p.Left - p.Right;
            visual.Height = uwp - p.Top - p.Bottom;
            visual.Color = LvcColor.FromArgb(baseColor.A, baseColor.R, baseColor.G, baseColor.B);
            visual.RemoveOnCompleted = false;

            if (point.Context.HoverArea is not RectangleHoverArea ha)
                point.Context.HoverArea = ha = new RectangleHoverArea();
            _ = ha
                .SetDimensions(secondary - uws * 0.5f, primary - uwp * 0.5f, uws, uwp)
                .CenterXToolTip()
                .CenterYToolTip();

            pointsCleanup.Clean(point);

            if (DataLabelsPaint is not null)
            {
                var label = (TLabel?)point.Context.Label;

                if (label is null)
                {
                    var l = new TLabel { X = secondary - uws * 0.5f, Y = primary - uws * 0.5f, RotateTransform = (float)DataLabelsRotation, MaxWidth = (float)DataLabelsMaxWidth };
                    l.Animate(EasingFunction ?? cartesianChart.ActualEasingFunction, AnimationsSpeed ?? cartesianChart.ActualAnimationsSpeed);
                    label = l;
                    point.Context.Label = l;
                }

                DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);

                label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisual, TLabel>(point));
                label.TextSize = dls;
                label.Padding = DataLabelsPadding;
                label.Paint = DataLabelsPaint;

                if (isFirstDraw)
                    label.CompleteTransition(
                        nameof(label.TextSize), nameof(label.X), nameof(label.Y), nameof(label.RotateTransform));

                var labelPosition = GetLabelPosition(
                     secondary - uws * 0.5f + p.Left, primary - uwp * 0.5f + p.Top, uws - p.Left - p.Right, uwp - p.Top - p.Bottom,
                     label.Measure(), DataLabelsPosition, SeriesProperties, coordinate.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                label.X = labelPosition.X;
                label.Y = labelPosition.Y;
            }

            OnPointMeasured(point);
        }

        pointsCleanup.CollectPoints(
            everFetched, cartesianChart.View, primaryScale, secondaryScale, SoftDeleteOrDisposePoint);
        _geometrySvgChanged = false;
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel}.GetBounds(Chart, ICartesianAxis, ICartesianAxis)"/>
    public override SeriesBounds GetBounds(Chart chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var seriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);
        var b = seriesBounds.Bounds.TertiaryBounds;
        _weightBounds = new(_minValue ?? b.Min, _maxValue ?? b.Max);
        return seriesBounds;
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel}.GetRequestedSecondaryOffset"/>
    protected override double GetRequestedSecondaryOffset() => 0.5f;

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel}.GetRequestedPrimaryOffset"/>
    protected override double GetRequestedPrimaryOffset() => 0.5f;

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.SetDefaultPointTransitions(ChartPoint)"/>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        var chart = chartPoint.Context.Chart;
        if (chartPoint.Context.Visual is not TVisual visual) throw new Exception("Unable to initialize the point instance.");
        visual.Animate(EasingFunction ?? chart.CoreChart.ActualEasingFunction, AnimationsSpeed ?? chart.CoreChart.ActualAnimationsSpeed);
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel}.SoftDeleteOrDisposePoint(ChartPoint, Scaler, Scaler)"/>
    protected internal override void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
    {
        var visual = (TVisual?)point.Context.Visual;
        if (visual is null) return;
        if (DataFactory is null) throw new Exception("Data provider not found");

        visual.Color = LvcColor.FromArgb(255, visual.Color);
        visual.RemoveOnCompleted = true;

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.GetMiniaturesSketch"/>
    [Obsolete($"Replaced by ${nameof(GetMiniatureGeometry)}")]
    public override Sketch GetMiniaturesSketch()
    {
        var schedules = new List<PaintSchedule>();

        var solidPaint = LiveCharts.DefaultSettings.GetProvider().GetSolidColorPaint();
        var st = solidPaint.StrokeThickness;
        solidPaint.PaintStyle = PaintStyle.Fill;

        if (st > MAX_MINIATURE_STROKE_WIDTH)
        {
            st = MAX_MINIATURE_STROKE_WIDTH;
            solidPaint.StrokeThickness = MAX_MINIATURE_STROKE_WIDTH;
        }

        var visual = new TVisual
        {
            X = st * 0.5f,
            Y = st * 0.5f,
            Height = (float)MiniatureShapeSize,
            Width = (float)MiniatureShapeSize,
            Color = HeatMap[0] // ToDo <- draw the gradient?
        };
        schedules.Add(new PaintSchedule(solidPaint, visual));

        return new Sketch(MiniatureShapeSize, MiniatureShapeSize, GeometrySvg)
        {
            PaintSchedules = schedules
        };
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.GetMiniature"/>
    [Obsolete($"Replaced by ${nameof(GetMiniatureGeometry)}")]
    public override IChartElement GetMiniature(ChartPoint? point, int zindex)
    {
        // ToDo <- draw the gradient?
        // what to show in the legend?
        return new GeometryVisual<TVisual, TLabel>
        {
            Width = 0,
            Height = 0,
        };
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel}.GetMiniatureGeometry"/>
    public override IDrawnElement GetMiniatureGeometry(ChartPoint? point)
    {
        // ToDo <- draw the gradient?
        // what to show in the legend?

        return new TVisual
        {
            Width = 0,
            Height = 0,
        };
    }

    /// <inheritdoc cref="ChartElement.GetPaintTasks"/>
    protected internal override Paint?[] GetPaintTasks() =>
        [_paintTaks];

    private static SeriesProperties GetProperties()
    {
        return SeriesProperties.Heat | SeriesProperties.PrimaryAxisVerticalOrientation |
            SeriesProperties.Solid | SeriesProperties.PrefersXYStrategyTooltips;
    }
}
