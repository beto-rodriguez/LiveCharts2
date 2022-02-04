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
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Segments;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines the data to plot as a line.
/// </summary>
/// <typeparam name="TModel">The type of the model to plot.</typeparam>
/// <typeparam name="TVisual">The type of the visual point.</typeparam>
/// <typeparam name="TLabel">The type of the data label.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <typeparam name="TPathGeometry">The type of the path geometry.</typeparam>
/// <typeparam name="TVisualPoint">The type of the visual point.</typeparam>
public class StepLineSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TVisualPoint>
    : StrokeAndFillCartesianSeries<TModel, TVisualPoint, TLabel, TDrawingContext>, IStepLineSeries<TDrawingContext>
        where TVisualPoint : StepLineVisualPoint<TDrawingContext, TVisual>, new()
        where TPathGeometry : IAreaGeometry<StepLineSegment, TDrawingContext>, new()
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
{
    private readonly Dictionary<object, List<TPathGeometry>> _fillPathHelperDictionary = new();
    private readonly Dictionary<object, List<TPathGeometry>> _strokePathHelperDictionary = new();
    private float _geometrySize = 14f;
    private IPaint<TDrawingContext>? _geometryFill;
    private IPaint<TDrawingContext>? _geometryStroke;
    private bool _enableNullSplitting = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="StepLineSeries{TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TVisualPoint}"/> class.
    /// </summary>
    /// <param name="isStacked">if set to <c>true</c> [is stacked].</param>
    public StepLineSeries(bool isStacked = false)
        : base(
              SeriesProperties.StepLine | SeriesProperties.PrimaryAxisVerticalOrientation |
              (isStacked ? SeriesProperties.Stacked : 0) | SeriesProperties.Sketch | SeriesProperties.PrefersXStrategyTooltips)
    {
        DataPadding = new LvcPoint(0.5f, 1f);
    }

    /// <inheritdoc cref="IStepLineSeries{TDrawingContext}.EnableNullSplitting"/>
    public bool EnableNullSplitting { get => _enableNullSplitting; set { _enableNullSplitting = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IStepLineSeries{TDrawingContext}.GeometrySize"/>
    public double GeometrySize { get => _geometrySize; set { _geometrySize = (float)value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IStepLineSeries{TDrawingContext}.GeometryFill"/>
    public IPaint<TDrawingContext>? GeometryFill
    {
        get => _geometryFill;
        set => SetPaintProperty(ref _geometryFill, value);
    }

    /// <inheritdoc cref="IStepLineSeries{TDrawingContext}.GeometrySize"/>
    public IPaint<TDrawingContext>? GeometryStroke
    {
        get => _geometryStroke;
        set => SetPaintProperty(ref _geometryStroke, value, true);
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
    public override void Measure(Chart<TDrawingContext> chart)
    {
        if (GetCustomMeasureHandler() is not null)
        {
            GetCustomMeasureHandler()!(chart);
            return;
        }

        var cartesianChart = (CartesianChart<TDrawingContext>)chart;
        var primaryAxis = cartesianChart.YAxes[ScalesYAt];
        var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

        var drawLocation = cartesianChart.DrawMarginLocation;
        var drawMarginSize = cartesianChart.DrawMarginSize;
        var secondaryScale = secondaryAxis.GetScaler(cartesianChart);
        var primaryScale = primaryAxis.GetScaler(cartesianChart);
        var actualSecondaryScale = secondaryAxis.GetActualScalerScaler(cartesianChart);
        var actualPrimaryScale = primaryAxis.GetActualScalerScaler(cartesianChart);

        var gs = _geometrySize;
        var hgs = gs / 2f;
        var sw = Stroke?.StrokeThickness ?? 0;
        var p = primaryScale.ToPixels(pivot);

        var chartAnimation = new Animation(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);

        var fetched = Fetch(cartesianChart);
        if (fetched is not ChartPoint[] points) points = fetched.ToArray();

        var segments = _enableNullSplitting
            ? SplitEachNull(points, secondaryScale, primaryScale)
            : new ChartPoint[][] { points };

        var stacker = (SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked
            ? cartesianChart.SeriesContext.GetStackPosition(this, GetStackGroup())
            : null;

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;

        if (stacker is not null)
        {
            // easy workaround to set an automatic and valid z-index for stacked area series
            // the problem of this solution is that the user needs to set z-indexes above 1000
            // if the user needs to add more series to the chart.
            actualZIndex = 1000 - stacker.Position;
            if (Fill is not null) Fill.ZIndex = actualZIndex;
            if (Stroke is not null) Stroke.ZIndex = actualZIndex;
        }

        var dls = unchecked((float)DataLabelsSize);

        var segmentI = 0;
        var toDeletePoints = new HashSet<ChartPoint>(everFetched);

        if (!_strokePathHelperDictionary.TryGetValue(chart.Canvas.Sync, out var strokePathHelperContainer))
        {
            strokePathHelperContainer = new List<TPathGeometry>();
            _strokePathHelperDictionary[chart.Canvas.Sync] = strokePathHelperContainer;
        }

        if (!_fillPathHelperDictionary.TryGetValue(chart.Canvas.Sync, out var fillPathHelperContainer))
        {
            fillPathHelperContainer = new List<TPathGeometry>();
            _fillPathHelperDictionary[chart.Canvas.Sync] = fillPathHelperContainer;
        }

        foreach (var item in strokePathHelperContainer) item.ClearCommands();
        foreach (var item in fillPathHelperContainer) item.ClearCommands();

        var uwx = secondaryScale.MeasureInPixels(secondaryAxis.UnitWidth);

        foreach (var segment in segments)
        {
            TPathGeometry fillPath;
            TPathGeometry strokePath;
            var isNew = false;

            if (segmentI >= fillPathHelperContainer.Count)
            {
                isNew = true;
                fillPath = new TPathGeometry { IsClosed = true };
                strokePath = new TPathGeometry { IsClosed = false };
                fillPathHelperContainer.Add(fillPath);
                strokePathHelperContainer.Add(strokePath);
            }
            else
            {
                fillPath = fillPathHelperContainer[segmentI];
                strokePath = strokePathHelperContainer[segmentI];
            }

            if (Fill is not null)
            {
                Fill.AddGeometryToPaintTask(cartesianChart.Canvas, fillPath);
                cartesianChart.Canvas.AddDrawableTask(Fill);
                Fill.ZIndex = actualZIndex + 0.1;
                Fill.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
                fillPath.Pivot = p;
                if (isNew)
                {
                    _ = fillPath.TransitionateProperties(nameof(fillPath.Pivot))
                        .WithAnimation(animation =>
                                    animation
                                        .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                        .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction))
                        .CompleteCurrentTransitions();
                }
            }
            if (Stroke is not null)
            {
                Stroke.AddGeometryToPaintTask(cartesianChart.Canvas, strokePath);
                cartesianChart.Canvas.AddDrawableTask(Stroke);
                Stroke.ZIndex = actualZIndex + 0.2;
                Stroke.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
                strokePath.Pivot = p;
                if (isNew)
                {
                    _ = strokePath.TransitionateProperties(nameof(strokePath.Pivot))
                        .WithAnimation(animation =>
                                    animation
                                        .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                        .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction))
                        .CompleteCurrentTransitions();
                }
            }

            double previousPrimary = 0, previousSecondary = 0;

            foreach (var point in segment)
            {
                var s = 0d;
                if (stacker is not null) s = stacker.GetStack(point).Start;

                var visual = (TVisualPoint?)point.Context.Visual;
                var dp = point.PrimaryValue + s - previousPrimary;
                var ds = point.SecondaryValue - previousSecondary;

                if (visual is null)
                {
                    var v = new TVisualPoint();
                    visual = v;

                    if (actualSecondaryScale is null || actualPrimaryScale is null)
                    {
                        v.Geometry.X = secondaryScale.ToPixels(point.SecondaryValue);
                        v.Geometry.Y = p;
                        v.Geometry.Width = 0;
                        v.Geometry.Height = 0;

                        v.StepSegment.X0 = secondaryScale.ToPixels(point.SecondaryValue - ds);
                        v.StepSegment.X1 = secondaryScale.ToPixels(point.SecondaryValue);
                        v.StepSegment.Y0 = p;
                        v.StepSegment.Y1 = p;
                    }
                    else
                    {
                        var xng = actualSecondaryScale.ToPixels(point.SecondaryValue);
                        var yng = actualPrimaryScale.ToPixels(point.PrimaryValue + s);

                        v.Geometry.X = xng - hgs;
                        v.Geometry.Y = yng - hgs;
                        v.Geometry.Width = gs;
                        v.Geometry.Height = gs;

                        v.StepSegment.X0 = actualSecondaryScale.ToPixels(point.SecondaryValue - ds);
                        v.StepSegment.X1 = actualSecondaryScale.ToPixels(point.SecondaryValue);
                        v.StepSegment.Y0 = actualPrimaryScale.ToPixels(point.PrimaryValue + s - dp);
                        v.StepSegment.Y1 = actualPrimaryScale.ToPixels(point.PrimaryValue + s);
                    }

                    point.Context.Visual = v;
                    OnPointCreated(point);
                }

                _ = everFetched.Add(point);

                if (GeometryFill is not null) GeometryFill.AddGeometryToPaintTask(cartesianChart.Canvas, visual.Geometry);
                if (GeometryStroke is not null) GeometryStroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual.Geometry);

                visual.StepSegment.X0 = secondaryScale.ToPixels(point.SecondaryValue - ds);
                visual.StepSegment.X1 = secondaryScale.ToPixels(point.SecondaryValue);
                visual.StepSegment.Y0 = primaryScale.ToPixels(point.PrimaryValue + s - dp);
                visual.StepSegment.Y1 = primaryScale.ToPixels(point.PrimaryValue + s);

                if (Fill is not null) _ = fillPath.AddLast(visual.StepSegment);
                if (Stroke is not null) _ = strokePath.AddLast(visual.StepSegment);

                var x = secondaryScale.ToPixels(point.SecondaryValue);
                var y = primaryScale.ToPixels(point.PrimaryValue + s);

                visual.Geometry.X = x - hgs;
                visual.Geometry.Y = y - hgs;
                visual.Geometry.Width = gs;
                visual.Geometry.Height = gs;
                visual.Geometry.RemoveOnCompleted = false;

                visual.FillPath = fillPath;
                visual.StrokePath = strokePath;

                var hags = gs < 8 ? 8 : gs;

                point.Context.HoverArea = new RectangleHoverArea(x - uwx * 0.5f, y - hgs, uwx, gs);

                _ = toDeletePoints.Remove(point);

                if (DataLabelsPaint is not null)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label is null)
                    {
                        var l = new TLabel { X = x - hgs, Y = p - hgs, RotateTransform = (float)DataLabelsRotation };

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
                    label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisualPoint, TLabel>(point));
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var labelPosition = GetLabelPosition(
                        x - hgs, y - hgs, gs, gs, label.Measure(DataLabelsPaint), DataLabelsPosition,
                        SeriesProperties, point.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }

                OnPointMeasured(point);
                previousPrimary = point.PrimaryValue + s;
                previousSecondary = point.SecondaryValue;
            }

            if (GeometryFill is not null)
            {
                cartesianChart.Canvas.AddDrawableTask(GeometryFill);
                GeometryFill.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
                GeometryFill.ZIndex = actualZIndex + 0.3;
            }
            if (GeometryStroke is not null)
            {
                cartesianChart.Canvas.AddDrawableTask(GeometryStroke);
                GeometryStroke.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
                GeometryStroke.ZIndex = actualZIndex + 0.4;
            }
            segmentI++;
        }

        while (segmentI > fillPathHelperContainer.Count)
        {
            var iFill = fillPathHelperContainer.Count - 1;
            var fillHelper = fillPathHelperContainer[iFill];
            if (Fill is not null) Fill.RemoveGeometryFromPainTask(cartesianChart.Canvas, fillHelper);
            fillPathHelperContainer.RemoveAt(iFill);

            var iStroke = strokePathHelperContainer.Count - 1;
            var strokeHelper = strokePathHelperContainer[iStroke];
            if (Stroke is not null) Stroke.RemoveGeometryFromPainTask(cartesianChart.Canvas, strokeHelper);
            strokePathHelperContainer.RemoveAt(iStroke);
        }

        if (DataLabelsPaint is not null)
        {
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
            //DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            DataLabelsPaint.ZIndex = actualZIndex + 0.5;
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

        var rgs = (GeometrySize + (GeometryStroke?.StrokeThickness ?? 0)) * 0.5f;

        return
            new SeriesBounds(
                new DimensionalBounds
                {
                    SecondaryBounds = new Bounds
                    {
                        Max = baseBounds.SecondaryBounds.Max + ts,
                        Min = baseBounds.SecondaryBounds.Min - ts,
                        MinDelta = baseBounds.SecondaryBounds.MinDelta,
                        PaddingMax = ts,
                        PaddingMin = ts,
                        RequestedGeometrySize = rgs
                    },
                    PrimaryBounds = new Bounds
                    {
                        Max = baseBounds.PrimaryBounds.Max + tp,
                        Min = baseBounds.PrimaryBounds.Min - tp,
                        MinDelta = baseBounds.PrimaryBounds.MinDelta,
                        PaddingMax = tp,
                        PaddingMin = tp,
                        RequestedGeometrySize = rgs
                    },
                    VisibleSecondaryBounds = new Bounds
                    {
                        Max = baseBounds.VisibleSecondaryBounds.Max + ts,
                        Min = baseBounds.VisibleSecondaryBounds.Min - ts
                    },
                    VisiblePrimaryBounds = new Bounds
                    {
                        Max = baseBounds.VisiblePrimaryBounds.Max + tp,
                        Min = baseBounds.VisiblePrimaryBounds.Min - tp
                    }
                }, false);
    }

    /// <inheritdoc cref="ChartSeries{TModel, TVisual, TLabel, TDrawingContext}.OnSeriesMiniatureChanged"/>
    protected override void OnSeriesMiniatureChanged()
    {
        var context = new CanvasSchedule<TDrawingContext>();
        var lss = (float)LegendShapeSize;
        var w = LegendShapeSize;
        var sh = 0f;

        if (_geometryStroke is not null)
        {
            var strokeClone = _geometryStroke.CloneTask();
            var st = _geometryStroke.StrokeThickness;
            if (st > MaxSeriesStroke)
            {
                st = MaxSeriesStroke;
                strokeClone.StrokeThickness = MaxSeriesStroke;
            }

            var visual = new TVisual
            {
                X = st + MaxSeriesStroke - st,
                Y = st + MaxSeriesStroke - st,
                Height = lss,
                Width = lss
            };
            sh = st;
            strokeClone.ZIndex = 1;
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
        }
        else if (Stroke is not null)
        {
            var strokeClone = Stroke.CloneTask();
            var st = strokeClone.StrokeThickness;
            if (st > MaxSeriesStroke)
            {
                st = MaxSeriesStroke;
                strokeClone.StrokeThickness = MaxSeriesStroke;
            }

            var visual = new TVisual
            {
                X = st + MaxSeriesStroke - st,
                Y = st + MaxSeriesStroke - st,
                Height = lss,
                Width = lss
            };
            sh = st;
            strokeClone.ZIndex = 1;
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
        }

        if (_geometryFill is not null)
        {
            var fillClone = _geometryFill.CloneTask();
            var visual = new TVisual { X = sh + MaxSeriesStroke - sh, Y = sh + MaxSeriesStroke - sh, Height = lss, Width = lss };
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
        }
        else if (Fill is not null)
        {
            var fillClone = Fill.CloneTask();
            var visual = new TVisual { X = sh + MaxSeriesStroke - sh, Y = sh + MaxSeriesStroke - sh, Height = lss, Width = lss };
            context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
        }

        context.Width = w + MaxSeriesStroke * 2;
        context.Height = w + MaxSeriesStroke * 2;

        CanvasSchedule = context;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        var chart = chartPoint.Context.Chart;

        if (chartPoint.Context.Visual is not TVisualPoint visual)
            throw new Exception("Unable to initialize the point instance.");

        _ = visual.Geometry
            .TransitionateProperties(
                nameof(visual.Geometry.X),
                nameof(visual.Geometry.Y),
                nameof(visual.Geometry.Width),
                nameof(visual.Geometry.Height))
            .WithAnimation(animation =>
                animation
                    .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                    .WithEasingFunction(EasingFunction ?? chart.EasingFunction))
            .CompleteCurrentTransitions();

        _ = visual.StepSegment
            .TransitionateProperties(
                nameof(visual.StepSegment.X0),
                nameof(visual.StepSegment.Y0),
                nameof(visual.StepSegment.X1),
                nameof(visual.StepSegment.Y1))
            .WithAnimation(animation =>
                animation
                    .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                    .WithEasingFunction(EasingFunction ?? chart.EasingFunction))
            .CompleteCurrentTransitions();
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.SoftDeleteOrDisposePoint(ChartPoint, Scaler, Scaler)"/>
    protected internal override void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
    {
        var visual = (TVisualPoint?)point.Context.Visual;
        if (visual is null) return;
        if (DataFactory is null) throw new Exception("Data provider not found");

        var chartView = (ICartesianChartView<TDrawingContext>)point.Context.Chart;
        if (chartView.Core.IsZoomingOrPanning)
        {
            visual.Geometry.CompleteAllTransitions();
            visual.Geometry.RemoveOnCompleted = true;
            DataFactory.DisposePoint(point);
            return;
        }

        var x = secondaryScale.ToPixels(point.SecondaryValue);
        var y = primaryScale.ToPixels(point.PrimaryValue);

        visual.Geometry.X = x;
        visual.Geometry.Y = y;
        visual.Geometry.Height = 0;
        visual.Geometry.Width = 0;
        visual.Geometry.RemoveOnCompleted = true;

        DataFactory.DisposePoint(point);

        var label = (TLabel?)point.Context.Label;
        if (label is null) return;

        label.TextSize = 1;
        label.RemoveOnCompleted = true;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SoftDeleteOrDispose(IChartView)"/>
    public override void SoftDeleteOrDispose(IChartView chart)
    {
        base.SoftDeleteOrDispose(chart);
        var canvas = ((ICartesianChartView<TDrawingContext>)chart).CoreCanvas;

        if (Fill is not null)
        {
            foreach (var activeChartContainer in _fillPathHelperDictionary.ToArray())
                foreach (var pathHelper in activeChartContainer.Value.ToArray())
                    Fill.RemoveGeometryFromPainTask(canvas, pathHelper);
        }

        if (Stroke is not null)
        {
            foreach (var activeChartContainer in _strokePathHelperDictionary.ToArray())
                foreach (var pathHelper in activeChartContainer.Value.ToArray())
                    Stroke.RemoveGeometryFromPainTask(canvas, pathHelper);
        }

        if (GeometryFill is not null) canvas.RemovePaintTask(GeometryFill);
        if (GeometryStroke is not null) canvas.RemovePaintTask(GeometryStroke);
    }

    /// <summary>
    /// Gets the paint tasks.
    /// </summary>
    /// <returns></returns>
    protected override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { Stroke, Fill, _geometryFill, _geometryStroke, DataLabelsPaint, hoverPaint };
    }

    private IEnumerable<ChartPoint[]> SplitEachNull(
       ChartPoint[] points,
       Scaler xScale,
       Scaler yScale)
    {
        var l = new List<ChartPoint>(points.Length);

        foreach (var point in points)
        {
            if (point.IsNull)
            {
                if (point.Context.Visual is TVisualPoint visual)
                {
                    var x = xScale.ToPixels(point.SecondaryValue);
                    var y = yScale.ToPixels(point.PrimaryValue);
                    var gs = _geometrySize;
                    var hgs = gs / 2f;
                    var sw = Stroke?.StrokeThickness ?? 0;
                    var p = yScale.ToPixels(pivot);
                    visual.Geometry.X = x - hgs;
                    visual.Geometry.Y = p - hgs;
                    visual.Geometry.Width = gs;
                    visual.Geometry.Height = gs;
                    visual.Geometry.RemoveOnCompleted = true;
                    point.Context.Visual = null;
                }

                if (l.Count > 0) yield return l.ToArray();
                l = new List<ChartPoint>(points.Length);
                continue;
            }

            l.Add(point);
        }

        if (l.Count > 0) yield return l.ToArray();
    }

    /// <inheritdoc/>
    public override void RemoveFromUI(Chart<TDrawingContext> chart)
    {
        base.RemoveFromUI(chart);

        _ = _fillPathHelperDictionary.Remove(chart.Canvas.Sync);
        _ = _strokePathHelperDictionary.Remove(chart.Canvas.Sync);
    }
}
