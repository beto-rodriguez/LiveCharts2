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
public class LineSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TVisualPoint>
    : StrokeAndFillCartesianSeries<TModel, TVisualPoint, TLabel, TDrawingContext>, ILineSeries<TDrawingContext>
        where TVisualPoint : BezierVisualPoint<TDrawingContext, TVisual>, new()
        where TPathGeometry : IVectorGeometry<CubicBezierSegment, TDrawingContext>, new()
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
{
    internal readonly Dictionary<object, List<TPathGeometry>> _fillPathHelperDictionary = new();
    internal readonly Dictionary<object, List<TPathGeometry>> _strokePathHelperDictionary = new();
    private float _lineSmoothness = 0.65f;
    private float _geometrySize = 14f;
    private bool _enableNullSplitting = true;
    private IPaint<TDrawingContext>? _geometryFill;
    private IPaint<TDrawingContext>? _geometryStroke;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="LineSeries{TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TBezierVisual}"/>
    /// class.
    /// </summary>
    /// <param name="isStacked">if set to <c>true</c> [is stacked].</param>
    public LineSeries(bool isStacked = false)
        : base(
              SeriesProperties.Line | SeriesProperties.PrimaryAxisVerticalOrientation |
              (isStacked ? SeriesProperties.Stacked : 0) | SeriesProperties.Sketch | SeriesProperties.PrefersXStrategyTooltips)
    {
        DataPadding = new LvcPoint(0.5f, 1f);
    }

    /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometrySize"/>
    public double GeometrySize { get => _geometrySize; set => SetProperty(ref _geometrySize, (float)value); }

    /// <inheritdoc cref="ILineSeries{TDrawingContext}.LineSmoothness"/>
    public double LineSmoothness
    {
        get => _lineSmoothness;
        set
        {
            var v = value;
            if (value > 1) v = 1;
            if (value < 0) v = 0;
            SetProperty(ref _lineSmoothness, (float)v);
        }
    }

    /// <inheritdoc cref="ILineSeries{TDrawingContext}.EnableNullSplitting"/>
    public bool EnableNullSplitting { get => _enableNullSplitting; set => SetProperty(ref _enableNullSplitting, value); }

    /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometryFill"/>
    public IPaint<TDrawingContext>? GeometryFill
    {
        get => _geometryFill;
        set => SetPaintProperty(ref _geometryFill, value);
    }

    /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometryStroke"/>
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

        // Note #240222
        // the following cases probably have a similar performance impact
        // this options were necessary at some older point when _enableNullSplitting = false could improve performance
        // ToDo: Check this out, maybe this is unnecessary now and we should just go for the first approach all the times.
        var segments = _enableNullSplitting
            ? Fetch(cartesianChart).SplitByNullGaps(point => DeleteNullPoint(point, secondaryScale, primaryScale)) // calling this method is probably as expensive as the line bellow
            : new List<IEnumerable<ChartPoint>>() { Fetch(cartesianChart) };

        var stacker = (SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked
            ? cartesianChart.SeriesContext.GetStackPosition(this, GetStackGroup())
            : null;

        var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;

        if (stacker is not null)
        {
            // Note# 010621
            // easy workaround to set an automatic and valid z-index for stacked area series
            // the problem of this solution is that the user needs to set z-indexes above 1000
            // if the user needs to add more series to the chart.
            actualZIndex = 1000 - stacker.Position;
            if (Fill is not null) Fill.ZIndex = actualZIndex;
            if (Stroke is not null) Stroke.ZIndex = actualZIndex;
        }

        var dls = (float)DataLabelsSize;
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

        var segmentI = 0;

        foreach (var segment in segments)
        {
            TPathGeometry fillPath;
            TPathGeometry strokePath;
            var isNew = false;

            if (segmentI >= fillPathHelperContainer.Count)
            {
                isNew = true;
                fillPath = new TPathGeometry { ClosingMethod = VectorClosingMethod.CloseToPivot };
                fillPathHelperContainer.Add(fillPath);
            }
            else
            {
                fillPath = fillPathHelperContainer[segmentI];
            }

            if (segmentI >= strokePathHelperContainer.Count)
            {
                isNew = true;
                strokePath = new TPathGeometry { ClosingMethod = VectorClosingMethod.NotClosed };
                strokePathHelperContainer.Add(strokePath);
            }
            else
            {
                strokePath = strokePathHelperContainer[segmentI];
            }

            var strokeVector = new VectorManager<CubicBezierSegment, TDrawingContext>(strokePath);
            var fillVector = new VectorManager<CubicBezierSegment, TDrawingContext>(fillPath);

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

            var isSegmentEmpty = true;

            foreach (var data in GetSpline(segment, stacker))
            {
                isSegmentEmpty = false;
                var s = 0d;
                if (stacker is not null)
                    s = data.TargetPoint.PrimaryValue > 0
                        ? stacker.GetStack(data.TargetPoint).Start
                        : stacker.GetStack(data.TargetPoint).NegativeStart;

                var visual = (TVisualPoint?)data.TargetPoint.Context.Visual;

                if (visual is null)
                {
                    var v = new TVisualPoint();
                    visual = v;

                    if (IsFirstDraw)
                    {
                        v.Geometry.X = secondaryScale.ToPixels(data.TargetPoint.SecondaryValue);
                        v.Geometry.Y = p;
                        v.Geometry.Width = 0;
                        v.Geometry.Height = 0;

                        v.Bezier.Xi = secondaryScale.ToPixels(data.X0);
                        v.Bezier.Xm = secondaryScale.ToPixels(data.X1);
                        v.Bezier.Xj = secondaryScale.ToPixels(data.X2);
                        v.Bezier.Yi = p;
                        v.Bezier.Ym = p;
                        v.Bezier.Yj = p;
                    }

                    data.TargetPoint.Context.Visual = v;
                    OnPointCreated(data.TargetPoint);
                }

                _ = everFetched.Add(data.TargetPoint);

                GeometryFill?.AddGeometryToPaintTask(cartesianChart.Canvas, visual.Geometry);
                GeometryStroke?.AddGeometryToPaintTask(cartesianChart.Canvas, visual.Geometry);

                visual.Bezier.Id = data.TargetPoint.Context.Entity.EntityIndex;

                if (Fill is not null) fillVector.AddConsecutiveSegment(visual.Bezier, !IsFirstDraw);
                if (Stroke is not null) strokeVector.AddConsecutiveSegment(visual.Bezier, !IsFirstDraw);

                visual.Bezier.Xi = secondaryScale.ToPixels(data.X0);
                visual.Bezier.Xm = secondaryScale.ToPixels(data.X1);
                visual.Bezier.Xj = secondaryScale.ToPixels(data.X2);
                visual.Bezier.Yi = primaryScale.ToPixels(data.Y0);
                visual.Bezier.Ym = primaryScale.ToPixels(data.Y1);
                visual.Bezier.Yj = primaryScale.ToPixels(data.Y2);

                var x = secondaryScale.ToPixels(data.TargetPoint.SecondaryValue);
                var y = primaryScale.ToPixels(data.TargetPoint.PrimaryValue + s);

                visual.Geometry.MotionProperties[nameof(visual.Geometry.X)]
                    .CopyFrom(visual.Bezier.MotionProperties[nameof(visual.Bezier.Xj)]);
                visual.Geometry.MotionProperties[nameof(visual.Geometry.Y)]
                    .CopyFrom(visual.Bezier.MotionProperties[nameof(visual.Bezier.Yj)]);
                visual.Geometry.TranslateTransform = new LvcPoint(-hgs, -hgs);

                visual.Geometry.Width = gs;
                visual.Geometry.Height = gs;
                visual.Geometry.RemoveOnCompleted = false;

                visual.FillPath = fillPath;
                visual.StrokePath = strokePath;

                var hags = gs < 8 ? 8 : gs;

                if (data.TargetPoint.Context.HoverArea is not RectangleHoverArea ha)
                    data.TargetPoint.Context.HoverArea = ha = new RectangleHoverArea();

                _ = ha.SetDimensions(x - uwx * 0.5f, y - hgs, uwx, gs)
                    .CenterXToolTip()
                    .CenterYToolTip();

                pointsCleanup.Clean(data.TargetPoint);

                if (DataLabelsPaint is not null)
                {
                    var label = (TLabel?)data.TargetPoint.Context.Label;

                    if (label is null)
                    {
                        var l = new TLabel { X = x - hgs, Y = p - hgs, RotateTransform = (float)DataLabelsRotation };
                        l.Animate(EasingFunction ?? cartesianChart.EasingFunction, AnimationsSpeed ?? cartesianChart.AnimationsSpeed);
                        label = l;
                        data.TargetPoint.Context.Label = l;
                    }

                    DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);
                    label.Text = DataLabelsFormatter(new ChartPoint<TModel, TVisualPoint, TLabel>(data.TargetPoint));
                    label.TextSize = dls;
                    label.Padding = DataLabelsPadding;
                    var m = label.Measure(DataLabelsPaint);
                    var labelPosition = GetLabelPosition(
                        x - hgs, y - hgs, gs, gs, m, DataLabelsPosition,
                        SeriesProperties, data.TargetPoint.PrimaryValue > Pivot, drawLocation, drawMarginSize);
                    if (DataLabelsTranslate is not null) label.TranslateTransform =
                        new LvcPoint(m.Width * DataLabelsTranslate.Value.X, m.Height * DataLabelsTranslate.Value.Y);

                    label.X = labelPosition.X;
                    label.Y = labelPosition.Y;
                }

                OnPointMeasured(data.TargetPoint);
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

            if (!isSegmentEmpty) segmentI++;
        }

        var maxSegment = fillPathHelperContainer.Count > strokePathHelperContainer.Count
            ? fillPathHelperContainer.Count
            : strokePathHelperContainer.Count;

        for (var i = maxSegment - 1; i >= segmentI; i--)
        {
            if (i < fillPathHelperContainer.Count)
            {
                var segmentFill = fillPathHelperContainer[i];
                Fill?.RemoveGeometryFromPainTask(cartesianChart.Canvas, segmentFill);
                segmentFill.ClearCommands();
                fillPathHelperContainer.RemoveAt(i);
            }

            if (i < strokePathHelperContainer.Count)
            {
                var segmentStroke = strokePathHelperContainer[i];
                Stroke?.RemoveGeometryFromPainTask(cartesianChart.Canvas, segmentStroke);
                segmentStroke.ClearCommands();
                strokePathHelperContainer.RemoveAt(i);
            }
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

    /// <inheritdoc cref="IChartSeries{TDrawingContext}.MiniatureEquals(IChartSeries{TDrawingContext})"/>
    public override bool MiniatureEquals(IChartSeries<TDrawingContext> series)
    {
        return series is LineSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TVisualPoint> lineSeries &&
            Name == series.Name &&
            !((ISeries)this).PaintsChanged &&
            Fill == lineSeries.Fill && Stroke == lineSeries.Stroke &&
            GeometryFill == lineSeries.GeometryFill && GeometryStroke == lineSeries.GeometryStroke;
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

    /// <summary>
    /// Builds an spline from the given points.
    /// </summary>
    /// <param name="points">The points.</param>
    /// <param name="stacker">The stacker.</param>
    /// <returns></returns>
    protected internal IEnumerable<BezierData> GetSpline(
        IEnumerable<ChartPoint> points,
        StackPosition<TDrawingContext>? stacker)
    {
        foreach (var item in points.AsSplineData())
        {
            if (item.IsFirst)
            {
                var c = item.Current;

                var sc = (item.Current.PrimaryValue > 0
                    ? stacker?.GetStack(c).Start
                    : stacker?.GetStack(c).NegativeStart) ?? 0;

                yield return new BezierData(item.Next)
                {
                    X0 = c.SecondaryValue,
                    Y0 = c.PrimaryValue + sc,
                    X1 = c.SecondaryValue,
                    Y1 = c.PrimaryValue + sc,
                    X2 = c.SecondaryValue,
                    Y2 = c.PrimaryValue + sc
                };

                continue;
            }

            var pys = 0d;
            var cys = 0d;
            var nys = 0d;
            var nnys = 0d;

            if (stacker is not null)
            {
                pys = item.Previous.PrimaryValue > 0 ? stacker.GetStack(item.Previous).Start : stacker.GetStack(item.Previous).NegativeStart;
                cys = item.Current.PrimaryValue > 0 ? stacker.GetStack(item.Current).Start : stacker.GetStack(item.Current).NegativeStart;
                nys = item.Next.PrimaryValue > 0 ? stacker.GetStack(item.Next).Start : stacker.GetStack(item.Next).NegativeStart;
                nnys = item.AfterNext.PrimaryValue > 0 ? stacker.GetStack(item.AfterNext).Start : stacker.GetStack(item.AfterNext).NegativeStart;
            }

            var xc1 = (item.Previous.SecondaryValue + item.Current.SecondaryValue) / 2.0f;
            var yc1 = (item.Previous.PrimaryValue + pys + item.Current.PrimaryValue + cys) / 2.0f;
            var xc2 = (item.Current.SecondaryValue + item.Next.SecondaryValue) / 2.0f;
            var yc2 = (item.Current.PrimaryValue + cys + item.Next.PrimaryValue + nys) / 2.0f;
            var xc3 = (item.Next.SecondaryValue + item.AfterNext.SecondaryValue) / 2.0f;
            var yc3 = (item.Next.PrimaryValue + nys + item.AfterNext.PrimaryValue + nnys) / 2.0f;

            var len1 = (float)Math.Sqrt(
                (item.Current.SecondaryValue - item.Previous.SecondaryValue) *
                (item.Current.SecondaryValue - item.Previous.SecondaryValue) +
                (item.Current.PrimaryValue + cys - item.Previous.PrimaryValue + pys) * (item.Current.PrimaryValue + cys - item.Previous.PrimaryValue + pys));
            var len2 = (float)Math.Sqrt(
                (item.Next.SecondaryValue - item.Current.SecondaryValue) *
                (item.Next.SecondaryValue - item.Current.SecondaryValue) +
                (item.Next.PrimaryValue + nys - item.Current.PrimaryValue + cys) * (item.Next.PrimaryValue + nys - item.Current.PrimaryValue + cys));
            var len3 = (float)Math.Sqrt(
                (item.AfterNext.SecondaryValue - item.Next.SecondaryValue) *
                (item.AfterNext.SecondaryValue - item.Next.SecondaryValue) +
                (item.AfterNext.PrimaryValue + nnys - item.Next.PrimaryValue + nys) * (item.AfterNext.PrimaryValue + nnys - item.Next.PrimaryValue + nys));

            var k1 = len1 / (len1 + len2);
            var k2 = len2 / (len2 + len3);

            if (float.IsNaN(k1)) k1 = 0f;
            if (float.IsNaN(k2)) k2 = 0f;

            var xm1 = xc1 + (xc2 - xc1) * k1;
            var ym1 = yc1 + (yc2 - yc1) * k1;
            var xm2 = xc2 + (xc3 - xc2) * k2;
            var ym2 = yc2 + (yc3 - yc2) * k2;

            var c1X = xm1 + (xc2 - xm1) * _lineSmoothness + item.Current.SecondaryValue - xm1;
            var c1Y = ym1 + (yc2 - ym1) * _lineSmoothness + item.Current.PrimaryValue + cys - ym1;
            var c2X = xm2 + (xc2 - xm2) * _lineSmoothness + item.Next.SecondaryValue - xm2;
            var c2Y = ym2 + (yc2 - ym2) * _lineSmoothness + item.Next.PrimaryValue + nys - ym2;

            yield return new BezierData(item.Next)
            {
                X0 = c1X,
                Y0 = c1Y,
                X1 = c2X,
                Y1 = c2Y,
                X2 = item.Next.SecondaryValue,
                Y2 = item.Next.PrimaryValue + nys
            };
        }
    }

    /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
    protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
    {
        var chart = chartPoint.Context.Chart;

        if (chartPoint.Context.Visual is not TVisualPoint visual)
            throw new Exception("Unable to initialize the point instance.");

        visual.Geometry.Animate(EasingFunction ?? chart.EasingFunction, AnimationsSpeed ?? chart.AnimationsSpeed);
        visual.Bezier.Animate(EasingFunction ?? chart.EasingFunction, AnimationsSpeed ?? chart.AnimationsSpeed);
    }

    /// <inheritdoc cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}.SoftDeleteOrDisposePoint(ChartPoint, Scaler, Scaler)"/>
    protected internal override void SoftDeleteOrDisposePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
    {
        var visual = (TVisualPoint?)point.Context.Visual;
        if (visual is null) return;
        if (DataFactory is null) throw new Exception("Data provider not found");

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

    private class SplineData
    {
        public SplineData(ChartPoint start)
        {
            Previous = start;
            Current = start;
            Next = start;
            AfterNext = start;
        }

        public ChartPoint Previous { get; set; }

        public ChartPoint Current { get; set; }

        public ChartPoint Next { get; set; }

        public ChartPoint AfterNext { get; set; }

        public bool IsFirst { get; set; } = true;

        public void GoNext(ChartPoint point)
        {
            Previous = Current;
            Current = Next;
            Next = AfterNext;
            AfterNext = point;
        }
    }
}
