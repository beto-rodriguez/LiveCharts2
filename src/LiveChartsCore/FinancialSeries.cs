
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
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}" />
/// <seealso cref="ICartesianSeries{TDrawingContext}" />
/// <seealso cref="IHeatSeries{TDrawingContext}" />
public abstract class FinancialSeries<TModel, TVisual, TLabel, TDrawingContext>
    : CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IFinancialSeries<TDrawingContext>
        where TVisual : class, IFinancialVisualChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
{
    private IPaint<TDrawingContext>? _upStroke = null;
    private IPaint<TDrawingContext>? _upFill = null;
    private IPaint<TDrawingContext>? _downStroke = null;
    private IPaint<TDrawingContext>? _downFill = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="FinancialSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
    /// </summary>
    protected FinancialSeries()
        : base(
             SeriesProperties.Financial | SeriesProperties.PrimaryAxisVerticalOrientation |
             SeriesProperties.Solid | SeriesProperties.PrefersXStrategyTooltips)
    {
        TooltipLabelFormatter = p => $"{Name}, H: {p.PrimaryValue:N2}, O: {p.TertiaryValue:N2}, C: {p.QuaternaryValue:N2}, L: {p.QuinaryValue:N2}";
    }

    /// <inheritdoc cref="IFinancialSeries{TDrawingContext}.MaxBarWidth"/>
    public double MaxBarWidth { get; set; } = 25;

    /// <inheritdoc cref="IFinancialSeries{TDrawingContext}.UpStroke"/>
    public IPaint<TDrawingContext>? UpStroke
    {
        get => _upStroke;
        set => SetPaintProperty(ref _upStroke, value, true);
    }

    /// <inheritdoc cref="IFinancialSeries{TDrawingContext}.UpFill"/>
    public IPaint<TDrawingContext>? UpFill
    {
        get => _upFill;
        set => SetPaintProperty(ref _upFill, value);
    }

    /// <inheritdoc cref="IFinancialSeries{TDrawingContext}.DownStroke"/>
    public IPaint<TDrawingContext>? DownStroke
    {
        get => _downStroke;
        set => SetPaintProperty(ref _downStroke, value, true);
    }

    /// <inheritdoc cref="IFinancialSeries{TDrawingContext}.DownFill"/>
    public IPaint<TDrawingContext>? DownFill
    {
        get => _downFill;
        set => SetPaintProperty(ref _downFill, value);
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
    public override void Measure(Chart<TDrawingContext> chart)
    {
        var cartesianChart = (CartesianChart<TDrawingContext>)chart;
        var primaryAxis = cartesianChart.YAxes[ScalesYAt];
        var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;
        var secondaryScale = secondaryAxis.GetNextScaler(cartesianChart);
        var primaryScale = primaryAxis.GetNextScaler(cartesianChart);
        var previousPrimaryScale = primaryAxis.GetActualScalerScaler(cartesianChart);
        var previousSecondaryScale = secondaryAxis.GetActualScalerScaler(cartesianChart);

        var uw = secondaryScale.MeasureInPixels(secondaryAxis.UnitWidth);
        var puw = previousSecondaryScale is null ? 0 : previousSecondaryScale.MeasureInPixels(secondaryAxis.UnitWidth);
        var uwm = 0.5f * uw;

        if (uw > MaxBarWidth)
        {
            uw = (float)MaxBarWidth;
            uwm = uw * 0.5f;
            puw = uw;
        }

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;

        if (UpFill is not null)
        {
            UpFill.ZIndex = actualZIndex + 0.1;
            UpFill.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(UpFill);
        }
        if (DownFill is not null)
        {
            DownFill.ZIndex = actualZIndex + 0.1;
            DownFill.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(DownFill);
        }
        if (UpStroke is not null)
        {
            UpStroke.ZIndex = actualZIndex + 0.2;
            UpStroke.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(UpStroke);
        }
        if (DownStroke is not null)
        {
            DownStroke.ZIndex = actualZIndex + 0.2;
            DownStroke.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(DownStroke);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = actualZIndex + 0.3;
            //DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var dls = (float)DataLabelsSize;
        var toDeletePoints = new HashSet<ChartPoint>(everFetched);

        foreach (var point in Fetch(cartesianChart))
        {
            var visual = point.Context.Visual as TVisual;
            var secondary = secondaryScale.ToPixels(point.SecondaryValue);

            var high = primaryScale.ToPixels(point.PrimaryValue);
            var open = primaryScale.ToPixels(point.TertiaryValue);
            var close = primaryScale.ToPixels(point.QuaternaryValue);
            var low = primaryScale.ToPixels(point.QuinaryValue);
            var middle = open;

            if (point.IsEmpty)
            {
                if (visual is not null)
                {
                    visual.X = secondary - uwm;
                    visual.Width = uw;
                    visual.Y = middle; // y coordinate is the highest
                    visual.Open = middle;
                    visual.Close = middle;
                    visual.Low = middle;
                    visual.RemoveOnCompleted = true;
                    point.Context.Visual = null;
                }
                continue;
            }

            if (visual is null)
            {
                var xi = secondary - uwm;
                var uwi = uw;
                var hi = 0f;

                if (previousSecondaryScale is not null && previousPrimaryScale is not null)
                {
                    var previousP = previousPrimaryScale.ToPixels(pivot);
                    var previousPrimary = previousPrimaryScale.ToPixels(point.PrimaryValue);
                    var bp = Math.Abs(previousPrimary - previousP);
                    var cyp = point.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                    xi = previousSecondaryScale.ToPixels(point.SecondaryValue) - uwm;
                    uwi = puw;
                    hi = cartesianChart.IsZoomingOrPanning ? bp : 0;
                }

                var r = new TVisual
                {
                    X = xi,
                    Width = uwi,
                    Y = middle, // y == high
                    Open = middle,
                    Close = middle,
                    Low = middle
                };

                visual = r;
                point.Context.Visual = visual;
                OnPointCreated(point);

                _ = everFetched.Add(point);
            }

            if (open > close)
            {
                if (UpFill is not null) UpFill.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                if (UpStroke is not null) UpStroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                if (DownFill is not null) DownFill.RemoveGeometryFromPainTask(cartesianChart.Canvas, visual);
                if (DownStroke is not null) DownStroke.RemoveGeometryFromPainTask(cartesianChart.Canvas, visual);
            }
            else
            {
                if (DownFill is not null) DownFill.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                if (DownStroke is not null) DownStroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                if (UpFill is not null) UpFill.RemoveGeometryFromPainTask(cartesianChart.Canvas, visual);
                if (UpStroke is not null) UpStroke.RemoveGeometryFromPainTask(cartesianChart.Canvas, visual);
            }

            var x = secondary - uwm;

            visual.X = x;
            visual.Width = uw;
            visual.Y = high;
            visual.Open = open;
            visual.Close = close;
            visual.Low = low;
            visual.RemoveOnCompleted = false;

            var ha = new RectangleHoverArea().SetDimensions(secondary - uwm, high, uw, Math.Abs(low - high));
            point.Context.HoverArea = ha;

            _ = toDeletePoints.Remove(point);

            if (DataLabelsPaint is not null)
            {
                var label = (TLabel?)point.Context.Label;

                if (label is null)
                {
                    var l = new TLabel { X = secondary - uwm, Y = high, RotateTransform = (float)DataLabelsRotation };

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
                    x, high, uw, Math.Abs(low - high), m, DataLabelsPosition,
                    SeriesProperties, point.PrimaryValue > Pivot, drawLocation, drawMarginSize);
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

    /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, ICartesianAxis, ICartesianAxis)"/>
    public override SeriesBounds GetBounds(
        CartesianChart<TDrawingContext> chart, ICartesianAxis secondaryAxis, ICartesianAxis primaryAxis)
    {
        if (DataFactory is null) throw new Exception("A data provider is required");

        var baseSeriesBounds = DataFactory.GetFinancialBounds(chart, this, secondaryAxis, primaryAxis);
        if (baseSeriesBounds.HasData) return baseSeriesBounds;
        var baseBounds = baseSeriesBounds.Bounds;

        var tickPrimary = primaryAxis.GetTick(chart.ControlSize, baseBounds.VisiblePrimaryBounds);
        var tickSecondary = secondaryAxis.GetTick(chart.ControlSize, baseBounds.VisibleSecondaryBounds);

        var ts = tickSecondary.Value * DataPadding.X;
        var tp = tickPrimary.Value * DataPadding.Y;

        if (baseBounds.VisibleSecondaryBounds.Delta == 0) ts = secondaryAxis.UnitWidth * DataPadding.X;
        if (baseBounds.VisiblePrimaryBounds.Delta == 0) tp = primaryAxis.UnitWidth * DataPadding.Y;

        return
            new SeriesBounds(
                new DimensionalBounds
                {
                    SecondaryBounds = new Bounds
                    {
                        Max = baseBounds.SecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth,
                        Min = baseBounds.SecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth,
                        MinDelta = baseBounds.SecondaryBounds.MinDelta,
                        PaddingMax = ts,
                        PaddingMin = ts
                    },
                    PrimaryBounds = new Bounds
                    {
                        Max = baseBounds.PrimaryBounds.Max,
                        Min = baseBounds.PrimaryBounds.Min,
                        MinDelta = baseBounds.PrimaryBounds.MinDelta,
                        PaddingMax = tp,
                        PaddingMin = tp
                    },
                    VisibleSecondaryBounds = new Bounds
                    {
                        Max = baseBounds.VisibleSecondaryBounds.Max + 0.5 * secondaryAxis.UnitWidth,
                        Min = baseBounds.VisibleSecondaryBounds.Min - 0.5 * secondaryAxis.UnitWidth
                    },
                    VisiblePrimaryBounds = new Bounds
                    {
                        Max = baseBounds.VisiblePrimaryBounds.Max,
                        Min = baseBounds.VisiblePrimaryBounds.Min
                    }
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
                nameof(visual.Open),
                nameof(visual.Close),
                nameof(visual.Low))
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

        visual.X = secondary;
        visual.Y = p;
        visual.Open = p;
        visual.Close = p;
        visual.Low = p;
        visual.RemoveOnCompleted = true;

        DataFactory.DisposePoint(point);

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }

    /// <summary>
    /// Gets the paint tasks.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _upFill, _upStroke, _downFill, _downStroke, DataLabelsPaint, hoverPaint };
    }

    /// <summary>
    /// Called when [paint changed].
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns></returns>
    protected override void OnPaintChanged(string? propertyName)
    {
        OnSeriesMiniatureChanged();
        OnPropertyChanged();
    }

    /// <inheritdoc cref="IChartSeries{TDrawingContext}.MiniatureEquals(IChartSeries{TDrawingContext})"/>
    public override bool MiniatureEquals(IChartSeries<TDrawingContext> series)
    {
        return series is FinancialSeries<TModel, TVisual, TLabel, TDrawingContext> financial &&
            Name == series.Name &&
            UpFill == financial.UpFill && UpStroke == financial.UpStroke &&
            DownFill == financial.DownFill && DownStroke == financial.DownStroke;
    }

    /// <summary>
    /// Called when the paint context changes.
    /// </summary>
    protected override void OnSeriesMiniatureChanged()
    {
        var context = new CanvasSchedule<TDrawingContext>();
        var w = LegendShapeSize;

        if (_upStroke is not null)
        {
            var strokeClone = _upStroke.CloneTask();
            var st = _upStroke.StrokeThickness;
            if (st > MaxSeriesStroke)
            {
                st = MaxSeriesStroke;
                strokeClone.StrokeThickness = MaxSeriesStroke;
            }

            var visual = new TVisual
            {
                X = st + MaxSeriesStroke - st,
                Y = st + MaxSeriesStroke - st,
                Open = 5,
                Close = (float)LegendShapeSize - 5,
                Low = (float)LegendShapeSize,
                Width = (float)LegendShapeSize
            };
            strokeClone.ZIndex = 1;
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
        }

        //if (Fill is not null)
        //{
        //    var fillClone = Fill.CloneTask();
        //    var visual = new TVisual { X = sh + o, Y = sh + o, Height = (float)LegendShapeSize, Width = (float)LegendShapeSize };
        //    context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
        //}

        context.Width = w + MaxSeriesStroke * 2;
        context.Height = w + MaxSeriesStroke * 2;

        CanvasSchedule = context;
    }
}
