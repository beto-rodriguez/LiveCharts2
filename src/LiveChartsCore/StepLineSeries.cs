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
        where TPathGeometry : IVectorGeometry<StepLineSegment, TDrawingContext>, new()
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
    public bool EnableNullSplitting { get => _enableNullSplitting; set => SetProperty(ref _enableNullSplitting, value); }

    /// <inheritdoc cref="IStepLineSeries{TDrawingContext}.GeometrySize"/>
    public double GeometrySize { get => _geometrySize; set => SetProperty(ref _geometrySize, (float)value); }

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
        var actualSecondaryScale = secondaryAxis.GetActualScaler(cartesianChart);
        var actualPrimaryScale = primaryAxis.GetActualScaler(cartesianChart);

        var gs = _geometrySize;
        var hgs = gs / 2f;
        var sw = Stroke?.StrokeThickness ?? 0;
        var p = primaryScale.ToPixels(pivot);

        // see note #240222
        var segments = _enableNullSplitting
            ? Fetch(cartesianChart).SplitByNullGaps(point => DeleteNullPoint(point, secondaryScale, primaryScale))
            : new List<IEnumerable<ChartPoint>>() { Fetch(cartesianChart) };

        var stacker = (SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked
            ? cartesianChart.SeriesContext.GetStackPosition(this, GetStackGroup())
            : null;

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;

        if (stacker is not null)
        {
            // see note #010621
            actualZIndex = 1000 - stacker.Position;
            if (Fill is not null) Fill.ZIndex = actualZIndex;
            if (Stroke is not null) Stroke.ZIndex = actualZIndex;
        }

        var dls = (float)DataLabelsSize;

        var segmentI = 0;
        var pointsCleanup = ChartPointCleanupContext.For(everFetched);

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

        var uwx = secondaryScale.MeasureInPixels(secondaryAxis.UnitWidth);
        uwx = uwx < gs ? gs : uwx;

        foreach (var segment in segments)
        {
            TPathGeometry fillPath;
            TPathGeometry strokePath;
            var isNew = false;

            if (segmentI >= fillPathHelperContainer.Count)
            {
                isNew = true;
                fillPath = new TPathGeometry { ClosingMethod = VectorClosingMethod.CloseToPivot };
                strokePath = new TPathGeometry { ClosingMethod = VectorClosingMethod.NotClosed };
                fillPathHelperContainer.Add(fillPath);
                strokePathHelperContainer.Add(strokePath);
            }
            else
            {
                fillPath = fillPathHelperContainer[segmentI];
                strokePath = strokePathHelperContainer[segmentI];
            }

            var strokeVector = new VectorManager<StepLineSegment, TDrawingContext>(strokePath);
            var fillVector = new VectorManager<StepLineSegment, TDrawingContext>(fillPath);

            if (Fill is not null)
            {
                Fill.AddGeometryToPaintTask(cartesianChart.Canvas, fillPath);
                cartesianChart.Canvas.AddDrawableTask(Fill);
                Fill.ZIndex = actualZIndex + 0.1;
                Fill.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
                fillPath.Pivot = p;
                if (isNew)
                {
                    fillPath.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
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
                    strokePath.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
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

                    if (IsFirstDraw)
                    {
                        v.Geometry.X = secondaryScale.ToPixels(point.SecondaryValue);
                        v.Geometry.Y = p;
                        v.Geometry.Width = 0;
                        v.Geometry.Height = 0;

                        v.StepSegment.Xi = secondaryScale.ToPixels(point.SecondaryValue - ds);
                        v.StepSegment.Xj = secondaryScale.ToPixels(point.SecondaryValue);
                        v.StepSegment.Yi = p;
                        v.StepSegment.Yj = p;
                    }

                    point.Context.Visual = v;
                    OnPointCreated(point);
                }

                _ = everFetched.Add(point);

                GeometryFill?.AddGeometryToPaintTask(cartesianChart.Canvas, visual.Geometry);
                GeometryStroke?.AddGeometryToPaintTask(cartesianChart.Canvas, visual.Geometry);

                visual.StepSegment.Id = point.Context.Entity.EntityIndex;

                if (Fill is not null) fillVector.AddConsecutiveSegment(visual.StepSegment, !IsFirstDraw);
                if (Stroke is not null) strokeVector.AddConsecutiveSegment(visual.StepSegment, !IsFirstDraw);

                visual.StepSegment.Xi = secondaryScale.ToPixels(point.SecondaryValue - ds);
                visual.StepSegment.Xj = secondaryScale.ToPixels(point.SecondaryValue);
                visual.StepSegment.Yi = primaryScale.ToPixels(point.PrimaryValue + s - dp);
                visual.StepSegment.Yj = primaryScale.ToPixels(point.PrimaryValue + s);

                var x = secondaryScale.ToPixels(point.SecondaryValue);
                var y = primaryScale.ToPixels(point.PrimaryValue + s);

                visual.Geometry.MotionProperties[nameof(visual.Geometry.X)].CopyFrom(visual.StepSegment.MotionProperties[nameof(visual.StepSegment.Xj)]);
                visual.Geometry.MotionProperties[nameof(visual.Geometry.Y)].CopyFrom(visual.StepSegment.MotionProperties[nameof(visual.StepSegment.Yj)]);
                visual.Geometry.TranslateTransform = new LvcPoint(-hgs, -hgs);

                visual.Geometry.Width = gs;
                visual.Geometry.Height = gs;
                visual.Geometry.RemoveOnCompleted = false;

                visual.FillPath = fillPath;
                visual.StrokePath = strokePath;

                var hags = gs < 8 ? 8 : gs;

                if (point.Context.HoverArea is not RectangleHoverArea ha)
                    point.Context.HoverArea = ha = new RectangleHoverArea();
                _ = ha.SetDimensions(x - uwx * 0.5f, y - hgs, uwx, gs);

                pointsCleanup.Clean(point);

                if (DataLabelsPaint is not null)
                {
                    var label = (TLabel?)point.Context.Label;

                    if (label is null)
                    {
                        var l = new TLabel { X = x - hgs, Y = p - hgs, RotateTransform = (float)DataLabelsRotation };
                        l.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
                        label = l;
                        point.Context.Label = l;
                    }

                    DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);
                    label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisualPoint, TLabel>(point));
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var m = label.Measure(DataLabelsPaint);
                    var labelPosition = GetLabelPosition(
                        x - hgs, y - hgs, gs, gs, m, DataLabelsPosition,
                        SeriesProperties, point.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                    if (DataLabelsTranslate is not null) label.TranslateTransform =
                        new LvcPoint(m.Width * DataLabelsTranslate.Value.X, m.Height * DataLabelsTranslate.Value.Y);

                    label.X = labelPosition.X; 
                    label.Y = labelPosition.Y;
                }

                OnPointMeasured(point);
                previousPrimary = point.PrimaryValue + s;
                previousSecondary = point.SecondaryValue;
            }

            strokeVector.End();
            fillVector.End();

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
            Fill?.RemoveGeometryFromPainTask(cartesianChart.Canvas, fillHelper);
            fillPathHelperContainer.RemoveAt(iFill);

            var iStroke = strokePathHelperContainer.Count - 1;
            var strokeHelper = strokePathHelperContainer[iStroke];
            Stroke?.RemoveGeometryFromPainTask(cartesianChart.Canvas, strokeHelper);
            strokePathHelperContainer.RemoveAt(iStroke);
        }

        if (DataLabelsPaint is not null)
        {
            cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
            //DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new LvcRectangle(drawLocation, drawMarginSize));
            DataLabelsPaint.ZIndex = actualZIndex + 0.5;
        }

        pointsCleanup.CollectPoints(
            everFetched, cartesianChart.View, primaryScale, secondaryScale, SoftDeleteOrDisposePoint);

        IsFirstDraw = false;
    }

    /// <inheritdoc cref="GetRequestedGeometrySize"/>
    protected override double GetRequestedGeometrySize()
    {
        return (GeometrySize + (GeometryStroke?.StrokeThickness ?? 0)) * 0.5f;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.GetMiniatresSketch"/>
    public override Sketch<TDrawingContext> GetMiniatresSketch()
    {
        var schedules = new List<PaintSchedule<TDrawingContext>>();

        if (GeometryFill is not null) schedules.Add(BuildMiniatureSchedule(GeometryFill, new TVisual()));
        else if (Fill is not null) schedules.Add(BuildMiniatureSchedule(Fill, new TVisual()));

        if (GeometryStroke is not null) schedules.Add(BuildMiniatureSchedule(GeometryStroke, new TVisual()));
        else if (Stroke is not null) schedules.Add(BuildMiniatureSchedule(Stroke, new TVisual()));

        return new Sketch<TDrawingContext>()
        {
            Height = MiniatureShapeSize,
            Width = MiniatureShapeSize,
            PaintSchedules = schedules
        };
    }

    /// <inheritdoc cref="IChartSeries{TDrawingContext}.MiniatureEquals(IChartSeries{TDrawingContext})"/>
    public override bool MiniatureEquals(IChartSeries<TDrawingContext> series)
    {
        return series is StepLineSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TVisualPoint> stepSeries &&
            Name == series.Name &&
            !((ISeries)this).PaintsChanged &&
            Fill == stepSeries.Fill && Stroke == stepSeries.Stroke &&
            GeometryFill == stepSeries.GeometryFill && GeometryStroke == stepSeries.GeometryStroke;
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        var chart = chartPoint.Context.Chart;

        if (chartPoint.Context.Visual is not TVisualPoint visual)
            throw new Exception("Unable to initialize the point instance.");

        visual.Geometry.Animate(EasingFunction ?? chart.EasingFunction, AnimationsSpeed ?? chart.AnimationsSpeed);
        visual.StepSegment.Animate(EasingFunction ?? chart.EasingFunction, AnimationsSpeed ?? chart.AnimationsSpeed);
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
            visual.Geometry.CompleteTransition(null);
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

    /// <inheritdoc/>
    public override void RemoveFromUI(Chart<TDrawingContext> chart)
    {
        base.RemoveFromUI(chart);

        _ = _fillPathHelperDictionary.Remove(chart.Canvas.Sync);
        _ = _strokePathHelperDictionary.Remove(chart.Canvas.Sync);
    }

    /// <summary>
    /// Gets the paint tasks.
    /// </summary>
    /// <returns></returns>
    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { Stroke, Fill, _geometryFill, _geometryStroke, DataLabelsPaint, hoverPaint };
    }

    private void DeleteNullPoint(ChartPoint point, Scaler xScale, Scaler yScale)
    {
        if (point.Context.Visual is not TVisualPoint visual) return;

        var x = xScale.ToPixels(point.SecondaryValue);
        var y = yScale.ToPixels(point.PrimaryValue);
        var gs = _geometrySize;
        var hgs = gs / 2f;

        visual.Geometry.X = x - hgs;
        visual.Geometry.Y = y - hgs;
        visual.Geometry.Width = gs;
        visual.Geometry.Height = gs;
        visual.Geometry.RemoveOnCompleted = true;
        point.Context.Visual = null;
    }
}
