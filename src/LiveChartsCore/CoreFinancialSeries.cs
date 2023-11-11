
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
public abstract class CoreFinancialSeries<TModel, TVisual, TLabel, TMiniatureGeometry, TDrawingContext>
    : CartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IFinancialSeries<TDrawingContext>
        where TVisual : class, IFinancialGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TMiniatureGeometry : ISizedGeometry<TDrawingContext>, new()
{
    private IPaint<TDrawingContext>? _upStroke = null;
    private IPaint<TDrawingContext>? _upFill = null;
    private IPaint<TDrawingContext>? _downStroke = null;
    private IPaint<TDrawingContext>? _downFill = null;
    private double _maxBarWidth = 25;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreFinancialSeries{TModel, TVisual, TLabel, TMiniatureGeometry, TDrawingContext}"/> class.
    /// </summary>
    protected CoreFinancialSeries()
        : base(
             SeriesProperties.Financial | SeriesProperties.PrimaryAxisVerticalOrientation |
             SeriesProperties.Solid | SeriesProperties.PrefersXStrategyTooltips)
    {
        YToolTipLabelFormatter = p =>
        {
            var c = p.Coordinate;
            return
                $"H {c.PrimaryValue:C2}{Environment.NewLine}" +
                $"O {c.TertiaryValue:C2}{Environment.NewLine}" +
                $"C {c.QuaternaryValue:C2}{Environment.NewLine}" +
                $"L {c.QuinaryValue:C2}";
        };

        DataLabelsFormatter = p =>
        {
            var c = p.Coordinate;
            return $"{c.PrimaryValue:C2} - {c.QuinaryValue:C2}";
        };
    }

    /// <inheritdoc cref="IFinancialSeries{TDrawingContext}.MaxBarWidth"/>
    public double MaxBarWidth { get => _maxBarWidth; set => SetProperty(ref _maxBarWidth, value); }

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

        var tp = chart.TooltipPosition;

        if (uw > MaxBarWidth)
        {
            uw = (float)MaxBarWidth;
            uwm = uw * 0.5f;
            puw = uw;
        }

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;
        var clipping = GetClipRectangle(cartesianChart);

        if (UpFill is not null)
        {
            UpFill.ZIndex = actualZIndex + 0.1;
            UpFill.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(UpFill);
        }
        if (DownFill is not null)
        {
            DownFill.ZIndex = actualZIndex + 0.1;
            DownFill.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(DownFill);
        }
        if (UpStroke is not null)
        {
            UpStroke.ZIndex = actualZIndex + 0.2;
            UpStroke.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(UpStroke);
        }
        if (DownStroke is not null)
        {
            DownStroke.ZIndex = actualZIndex + 0.2;
            DownStroke.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(DownStroke);
        }
        if (DataLabelsPaint is not null)
        {
            DataLabelsPaint.ZIndex = actualZIndex + 0.3;
            DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, clipping);
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
        }

        var dls = (float)DataLabelsSize;
        var pointsCleanup = ChartPointCleanupContext.For(everFetched);

        var isFirstDraw = !chart._drawnSeries.Contains(((ISeries)this).SeriesId);

        foreach (var point in Fetch(cartesianChart))
        {
            var coordinate = point.Coordinate;
            var visual = point.Context.Visual as TVisual;
            var secondary = secondaryScale.ToPixels(coordinate.SecondaryValue);

            var high = primaryScale.ToPixels(coordinate.PrimaryValue);
            var open = primaryScale.ToPixels(coordinate.TertiaryValue);
            var close = primaryScale.ToPixels(coordinate.QuaternaryValue);
            var low = primaryScale.ToPixels(coordinate.QuinaryValue);
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
                    var previousPrimary = previousPrimaryScale.ToPixels(coordinate.PrimaryValue);
                    var bp = Math.Abs(previousPrimary - previousP);
                    var cyp = coordinate.PrimaryValue > pivot ? previousPrimary : previousPrimary - bp;

                    xi = previousSecondaryScale.ToPixels(coordinate.SecondaryValue) - uwm;
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
                UpFill?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                UpStroke?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                DownFill?.RemoveGeometryFromPainTask(cartesianChart.Canvas, visual);
                DownStroke?.RemoveGeometryFromPainTask(cartesianChart.Canvas, visual);
            }
            else
            {
                DownFill?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                DownStroke?.AddGeometryToPaintTask(cartesianChart.Canvas, visual);
                UpFill?.RemoveGeometryFromPainTask(cartesianChart.Canvas, visual);
                UpStroke?.RemoveGeometryFromPainTask(cartesianChart.Canvas, visual);
            }

            var x = secondary - uwm;

            visual.X = x;
            visual.Width = uw;
            visual.Y = high;
            visual.Open = open;
            visual.Close = close;
            visual.Low = low;
            visual.RemoveOnCompleted = false;

            if (point.Context.HoverArea is not RectangleHoverArea ha)
                point.Context.HoverArea = ha = new RectangleHoverArea();

            _ = ha.SetDimensions(secondary - uwm, high, uw, Math.Abs(low - high));

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
                    var l = new TLabel { X = secondary - uwm, Y = high, RotateTransform = (float)DataLabelsRotation, MaxWidth = (float)DataLabelsMaxWidth };
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
                    x, high, uw, Math.Abs(low - high), m, DataLabelsPosition,
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
    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _upFill, _upStroke, _downFill, _downStroke, DataLabelsPaint };
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

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.GetMiniaturesSketch"/>
    public override Sketch<TDrawingContext> GetMiniaturesSketch()
    {
        var schedules = new List<PaintSchedule<TDrawingContext>>();

        if (UpStroke is not null) schedules.Add(BuildMiniatureSchedule(UpStroke, new TMiniatureGeometry()));

        return new Sketch<TDrawingContext>(MiniatureShapeSize, MiniatureShapeSize, GeometrySvg)
        {
            PaintSchedules = schedules
        };
    }
}
