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
/// Defines a column series.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TLabel">The type of the label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public abstract class HeatSeries<TModel, TVisual, TLabel, TDrawingContext>
    : CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IHeatSeries<TDrawingContext>
        where TVisual : class, ISolidColorChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    private IPaint<TDrawingContext>? _paintTaks;
    private Bounds _weightBounds = new();
    private int _heatKnownLength = 0;
    private List<Tuple<double, LvcColor>> _heatStops = new();
    private LvcColor[] _heatMap =
    {
        LvcColor.FromArgb(255, 87, 103, 222), // cold (min value)
        LvcColor.FromArgb(255, 95, 207, 249) // hot (max value)
    };
    private double[]? _colorStops;
    private Padding _pointPadding = new(4);

    /// <summary>
    /// Initializes a new instance of the <see cref="HeatSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    protected HeatSeries()
        : base(
             SeriesProperties.Heat | SeriesProperties.PrimaryAxisVerticalOrientation |
             SeriesProperties.Solid | SeriesProperties.PrefersXYStrategyTooltips)
    {
        DataPadding = new LvcPoint(0, 0);
        TooltipLabelFormatter = (point) => $"{Name}: {point.TertiaryValue:N}";
    }

    /// <inheritdoc cref="IHeatSeries{TDrawingContext}.HeatMap"/>
    public LvcColor[] HeatMap
    {
        get => _heatMap;
        set
        {
            OnMiniatureChanged();
            SetProperty(ref _heatMap, value);
        }
    }

    /// <inheritdoc cref="IHeatSeries{TDrawingContext}.ColorStops"/>
    public double[]? ColorStops { get => _colorStops; set => SetProperty(ref _colorStops, value); }

    /// <inheritdoc cref="IHeatSeries{TDrawingContext}.PointPadding"/>
    public Padding PointPadding { get => _pointPadding; set => SetProperty(ref _pointPadding, value); }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override void Invalidate(Chart<TDrawingContext> chart)
    {
        _paintTaks ??= LiveCharts.DefaultSettings.GetProvider<TDrawingContext>().GetSolidColorPaint();

        var cartesianChart = (CartesianChart<TDrawingContext>)chart;
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
        if (_paintTaks is not null)
        {
            _paintTaks.ZIndex = actualZIndex + 0.2;
            _paintTaks.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(_paintTaks);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = actualZIndex + 0.3;
            //DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
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

        foreach (var point in Fetch(cartesianChart))
        {
            var visual = point.Context.Visual as TVisual;
            var primary = primaryScale.ToPixels(point.PrimaryValue);
            var secondary = secondaryScale.ToPixels(point.SecondaryValue);
            var tertiary = (float)point.TertiaryValue;

            var baseColor = HeatFunctions.InterpolateColor(tertiary, _weightBounds, HeatMap, _heatStops);

            if (point.IsEmpty)
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
                continue;
            }

            if (visual is null)
            {
                var xi = secondary - uws * 0.5f;
                var yi = primary - uwp * 0.5f;

                if (previousSecondaryScale is not null && previousPrimaryScale is not null)
                {
                    var previousP = previousPrimaryScale.ToPixels(pivot);
                    var previousPrimary = previousPrimaryScale.ToPixels(point.PrimaryValue);
                    var bp = Math.Abs(previousPrimary - previousP);
                    var cyp = point.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                    xi = previousSecondaryScale.ToPixels(point.SecondaryValue) - uws * 0.5f;
                    yi = previousPrimaryScale.ToPixels(point.PrimaryValue) - uwp * 0.5f;
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

            _paintTaks?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);

            visual.X = secondary - uws * 0.5f + p.Left;
            visual.Y = primary - uwp * 0.5f + p.Top;
            visual.Width = uws - p.Left - p.Right;
            visual.Height = uwp - p.Top - p.Bottom;
            visual.Color = LvcColor.FromArgb(baseColor.A, baseColor.R, baseColor.G, baseColor.B);
            visual.RemoveOnCompleted = false;

            if (point.Context.HoverArea is not RectangleHoverArea ha)
                point.Context.HoverArea = ha = new RectangleHoverArea();
            _ = ha.SetDimensions(secondary - uws * 0.5f, primary - uwp * 0.5f, uws, uwp);

            pointsCleanup.Clean(point);

            if (DataLabelsPaint is not null)
            {
                var label = (TLabel?)point.Context.Label;

                if (label is null)
                {
                    var l = new TLabel { X = secondary - uws * 0.5f, Y = primary - uws * 0.5f, RotateTransform = (float)DataLabelsRotation };

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
                var labelPosition = GetLabelPosition(
                     secondary, primary, uws, uws, label.Measure(DataLabelsPaint), DataLabelsPosition,
                     SeriesProperties, point.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                label.X = labelPosition.X;
                label.Y = labelPosition.Y;
            }

            OnPointMeasured(point);
        }

        pointsCleanup.CollectPoints(
            everFetched, cartesianChart.View, primaryScale, secondaryScale, SoftDeleteOrDisposePoint);
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Invalidate(Chart{TDrawingContext})"/>
    public override SeriesBounds GetBounds(CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        var seriesBounds = base.GetBounds(chart, secondaryAxis, primaryAxis);
        _weightBounds = seriesBounds.Bounds.TertiaryBounds;
        return seriesBounds;
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.GetRequestedSecondaryOffset"/>
    protected override double GetRequestedSecondaryOffset()
    {
        return 0.5f;
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.GetRequestedPrimaryOffset"/>
    protected override double GetRequestedPrimaryOffset()
    {
        return 0.5f;
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
                nameof(visual.Height),
                nameof(visual.Color))
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

        visual.Color = LvcColor.FromArgb(255, visual.Color);
        visual.RemoveOnCompleted = true;

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.GetMiniatresSketch"/>
    public override Sketch<TDrawingContext> GetMiniatresSketch()
    {
        var schedules = new List<PaintSchedule<TDrawingContext>>();

        var strokeClone = LiveCharts.DefaultSettings.GetProvider<TDrawingContext>().GetSolidColorPaint();
        var st = strokeClone.StrokeThickness;

        if (st > MAX_MINIATURE_STROKE_WIDTH)
        {
            st = MAX_MINIATURE_STROKE_WIDTH;
            strokeClone.StrokeThickness = MAX_MINIATURE_STROKE_WIDTH;
        }

        var visual = new TVisual
        {
            X = st * 0.5f,
            Y = st * 0.5f,
            Height = (float)MiniatureShapeSize,
            Width = (float)MiniatureShapeSize,
            Color = HeatMap[0] // ToDo <- draw the gradient?
        };
        strokeClone.ZIndex = 1;
        schedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));

        return new Sketch<TDrawingContext>()
        {
            Height = MiniatureShapeSize,
            Width = MiniatureShapeSize,
            PaintSchedules = schedules
        };
    }

    /// <inheritdoc cref="ChartSeries{TModel, TVisual, TLabel, TDrawingContext}.MiniatureEquals(IChartSeries{TDrawingContext})"/>
    public override bool MiniatureEquals(IChartSeries<TDrawingContext> instance)
    {
        return instance is HeatSeries<TModel, TVisual, TLabel, TDrawingContext> hSeries
            && Name == instance.Name && HeatMap == hSeries.HeatMap;
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.GetPaintTasks"/>
    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _paintTaks, hoverPaint };
    }
}
