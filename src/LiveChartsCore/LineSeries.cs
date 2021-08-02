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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel.Data;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the data to plot as a line.
    /// </summary>
    public class LineSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs>
        : StrokeAndFillCartesianSeries<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>, ILineSeries<TDrawingContext>
        where TPathGeometry : IPathGeometry<TDrawingContext, TPathArgs>, new()
        where TLineSegment : ILinePathSegment<TPathArgs>, new()
        where TBezierSegment : IBezierSegment<TPathArgs>, new()
        where TMoveToCommand : IMoveToPathCommand<TPathArgs>, new()
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly Dictionary<object, List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>>> _fillPathHelperDictionary = new();
        private readonly Dictionary<object, List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>>> _strokePathHelperDictionary = new();
        private float _lineSmoothness = 0.65f;
        private float _geometrySize = 14f;
        private bool _enableNullSplitting = true;
        private IPaintTask<TDrawingContext>? _geometryFill;
        private IPaintTask<TDrawingContext>? _geometryStroke;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSeries{TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs}"/> class.
        /// </summary>
        /// <param name="isStacked">if set to <c>true</c> [is stacked].</param>
        public LineSeries(bool isStacked = false)
            : base(
                  SeriesProperties.Line | SeriesProperties.PrimaryAxisVerticalOrientation |
                  (isStacked ? SeriesProperties.Stacked : 0) | SeriesProperties.Sketch | SeriesProperties.PrefersXStrategyTooltips)
        {
            DataPadding = new PointF(0.5f, 1f);
            HoverState = LiveCharts.LineSeriesHoverKey;
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometrySize"/>
        public double GeometrySize { get => _geometrySize; set { _geometrySize = (float)value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.LineSmoothness"/>
        public double LineSmoothness
        {
            get => _lineSmoothness;
            set
            {
                var v = value;
                if (value > 1) v = 1;
                if (value < 0) v = 0;
                _lineSmoothness = (float)v;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.EnableNullSplitting"/>
        public bool EnableNullSplitting { get => _enableNullSplitting; set { _enableNullSplitting = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometryFill"/>
        public IPaintTask<TDrawingContext>? GeometryFill
        {
            get => _geometryFill;
            set => SetPaintProperty(ref _geometryFill, value);
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometryStroke"/>
        public IPaintTask<TDrawingContext>? GeometryStroke
        {
            get => _geometryStroke;
            set => SetPaintProperty(ref _geometryStroke, value, true);
        }

        /// <inheritdoc cref="ChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
        public override void Measure(Chart<TDrawingContext> chart)
        {
            var cartesianChart = (CartesianChart<TDrawingContext>)chart;
            var primaryAxis = cartesianChart.YAxes[ScalesYAt];
            var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

            var drawLocation = cartesianChart.DrawMarginLocation;
            var drawMarginSize = cartesianChart.DrawMarginSize;
            var secondaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
            var primaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);
            var previousPrimaryScale =
                primaryAxis.PreviousDataBounds is null ? null : new Scaler(drawLocation, drawMarginSize, primaryAxis, true);
            var previousSecondaryScale =
                secondaryAxis.PreviousDataBounds is null ? null : new Scaler(drawLocation, drawMarginSize, secondaryAxis, true);

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
                strokePathHelperContainer = new List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>>();
                _strokePathHelperDictionary[chart.Canvas.Sync] = strokePathHelperContainer;
            }

            if (!_fillPathHelperDictionary.TryGetValue(chart.Canvas.Sync, out var fillPathHelperContainer))
            {
                fillPathHelperContainer = new List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>>();
                _fillPathHelperDictionary[chart.Canvas.Sync] = fillPathHelperContainer;
            }

            foreach (var item in strokePathHelperContainer) item.Path.ClearCommands();
            foreach (var item in fillPathHelperContainer) item.Path.ClearCommands();

            foreach (var segment in segments)
            {
                var wasFillInitialized = false;
                var wasStrokeInitialized = false;

                AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs> fillPathHelper;
                AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs> strokePathHelper;

                if (segmentI >= fillPathHelperContainer.Count)
                {
                    fillPathHelper = new AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>();
                    strokePathHelper = new AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>();
                    fillPathHelperContainer.Add(fillPathHelper);
                    strokePathHelperContainer.Add(strokePathHelper);
                }
                else
                {
                    fillPathHelper = fillPathHelperContainer[segmentI];
                    strokePathHelper = strokePathHelperContainer[segmentI];
                }

                if (Fill is not null)
                {
                    wasFillInitialized = fillPathHelper.Initialize(SetDefaultPathTransitions, chartAnimation);
                    Fill.AddGeometryToPaintTask(cartesianChart.Canvas, fillPathHelper.Path);
                    cartesianChart.Canvas.AddDrawableTask(Fill);
                    Fill.ZIndex = actualZIndex + 0.1;
                    Fill.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                }
                if (Stroke is not null)
                {
                    wasStrokeInitialized = strokePathHelper.Initialize(SetDefaultPathTransitions, chartAnimation);
                    Stroke.AddGeometryToPaintTask(cartesianChart.Canvas, strokePathHelper.Path);
                    cartesianChart.Canvas.AddDrawableTask(Stroke);
                    Stroke.ZIndex = actualZIndex + 0.2;
                    Stroke.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                }

                foreach (var data in GetSpline(segment, secondaryScale, primaryScale, stacker))
                {
                    var s = 0d;
                    if (stacker is not null)
                    {
                        s = stacker.GetStack(data.TargetPoint).Start;
                    }

                    var x = secondaryScale.ToPixels(data.TargetPoint.SecondaryValue);
                    var y = primaryScale.ToPixels(data.TargetPoint.PrimaryValue + s);

                    var visual = (LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>?)data.TargetPoint.Context.Visual;

                    if (visual is null)
                    {
                        var v = new LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>();

                        visual = v;

                        var pg = p;
                        var xg = x - hgs;
                        var yg = p - hgs;

                        var x0b = data.X0;
                        var x1b = data.X1;
                        var x2b = data.X2;
                        var y0b = p - hgs;
                        var y1b = p - hgs;
                        var y2b = p - hgs;

                        if (previousSecondaryScale is not null && previousPrimaryScale is not null)
                        {
                            pg = previousPrimaryScale.ToPixels(pivot);
                            xg = previousSecondaryScale.ToPixels(data.TargetPoint.SecondaryValue) - hgs;
                            yg = previousPrimaryScale.ToPixels(data.TargetPoint.PrimaryValue + s) - hgs;

                            if (data.OriginalData is null) throw new Exception("Original data not found");

                            x0b = previousSecondaryScale.ToPixels(data.OriginalData.X0);
                            x1b = previousSecondaryScale.ToPixels(data.OriginalData.X1);
                            x2b = previousSecondaryScale.ToPixels(data.OriginalData.X2);
                            y0b = previousPrimaryScale.ToPixels(data.OriginalData.Y0); //chart.IsZoomingOrPanning ? previousPrimaryScale.ToPixels(data.OriginalData.Y0) : pg - hgs;
                            y1b = previousPrimaryScale.ToPixels(data.OriginalData.Y1); //chart.IsZoomingOrPanning ? previousPrimaryScale.ToPixels(data.OriginalData.Y1) : pg - hgs;
                            y2b = previousPrimaryScale.ToPixels(data.OriginalData.Y2); //chart.IsZoomingOrPanning ? previousPrimaryScale.ToPixels(data.OriginalData.Y2) : pg - hgs;
                        }

                        v.Geometry.X = xg;
                        v.Geometry.Y = yg;
                        v.Geometry.Width = gs;
                        v.Geometry.Height = gs;

                        v.Bezier.X0 = (float)x0b;
                        v.Bezier.Y0 = y0b;
                        v.Bezier.X1 = (float)x1b;
                        v.Bezier.Y1 = y1b;
                        v.Bezier.X2 = (float)x2b;
                        v.Bezier.Y2 = y2b;

                        data.TargetPoint.Context.Visual = v;
                        OnPointCreated(data.TargetPoint);
                        v.Geometry.CompleteAllTransitions();
                        v.Bezier.CompleteAllTransitions();
                    }

                    _ = everFetched.Add(data.TargetPoint);

                    if (GeometryFill is not null) GeometryFill.AddGeometryToPaintTask(cartesianChart.Canvas, visual.Geometry);
                    if (GeometryStroke is not null) GeometryStroke.AddGeometryToPaintTask(cartesianChart.Canvas, visual.Geometry);

                    visual.Bezier.X0 = (float)data.X0;
                    visual.Bezier.Y0 = (float)data.Y0;
                    visual.Bezier.X1 = (float)data.X1;
                    visual.Bezier.Y1 = (float)data.Y1;
                    visual.Bezier.X2 = (float)data.X2;
                    visual.Bezier.Y2 = (float)data.Y2;

                    if (Fill is not null)
                    {
                        if (data.IsFirst)
                        {
                            if (wasFillInitialized)
                            {
                                fillPathHelper.StartPoint.X = (float)data.X0;
                                fillPathHelper.StartPoint.Y = p;

                                fillPathHelper.StartSegment.X = (float)data.X0;
                                fillPathHelper.StartSegment.Y = p;

                                fillPathHelper.StartPoint.CompleteTransitions(
                                    nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                                fillPathHelper.StartSegment.CompleteTransitions(
                                    nameof(fillPathHelper.StartSegment.Y), nameof(fillPathHelper.StartSegment.X));
                            }

                            fillPathHelper.StartPoint.X = (float)data.X0;
                            fillPathHelper.StartPoint.Y = p;
                            fillPathHelper.Path.AddCommand(fillPathHelper.StartPoint);

                            fillPathHelper.StartSegment.X = (float)data.X0;
                            fillPathHelper.StartSegment.Y = (float)data.Y0;
                            fillPathHelper.Path.AddCommand(fillPathHelper.StartSegment);
                        }

                        fillPathHelper.Path.AddCommand(visual.Bezier);

                        if (data.IsLast)
                        {
                            fillPathHelper.EndSegment.X = (float)data.X2;
                            fillPathHelper.EndSegment.Y = p;
                            fillPathHelper.Path.AddCommand(fillPathHelper.EndSegment);

                            if (wasFillInitialized)
                                fillPathHelper.EndSegment.CompleteTransitions(
                                    nameof(fillPathHelper.EndSegment.Y), nameof(fillPathHelper.EndSegment.X));
                        }
                    }
                    if (Stroke is not null)
                    {
                        if (data.IsFirst)
                        {
                            if (wasStrokeInitialized || cartesianChart.IsZoomingOrPanning)
                            {
                                if (cartesianChart.IsZoomingOrPanning && previousPrimaryScale is not null && previousSecondaryScale is not null)
                                {
                                    strokePathHelper.StartPoint.X = previousSecondaryScale.ToPixels(data.OriginalData?.X0 ?? 0);
                                    strokePathHelper.StartPoint.Y = previousPrimaryScale.ToPixels(data.OriginalData?.Y0 ?? 0);
                                }
                                else
                                {
                                    strokePathHelper.StartPoint.X = (float)data.X0;
                                    strokePathHelper.StartPoint.Y = p;
                                }

                                strokePathHelper.StartPoint.CompleteTransitions(
                                   nameof(strokePathHelper.StartPoint.Y), nameof(strokePathHelper.StartPoint.X));
                            }

                            if (!cartesianChart.IsFirstDraw && previousSecondaryScale is not null && previousPrimaryScale is not null)
                            {
                                strokePathHelper.StartPoint.X = previousSecondaryScale.ToPixels(data.OriginalData?.X0 ?? 0);
                                strokePathHelper.StartPoint.Y = previousPrimaryScale.ToPixels(data.OriginalData?.Y0 ?? 0);
                                strokePathHelper.StartPoint.CompleteTransitions(
                                   nameof(strokePathHelper.StartPoint.Y), nameof(strokePathHelper.StartPoint.X));
                            }

                            strokePathHelper.StartPoint.X = (float)data.X0;
                            strokePathHelper.StartPoint.Y = (float)data.Y0;
                            strokePathHelper.Path.AddCommand(strokePathHelper.StartPoint);
                        }

                        strokePathHelper.Path.AddCommand(visual.Bezier);
                    }

                    visual.Geometry.X = x - hgs;
                    visual.Geometry.Y = y - hgs;
                    visual.Geometry.Width = gs;
                    visual.Geometry.Height = gs;
                    visual.Geometry.RemoveOnCompleted = false;

                    visual.FillPath = fillPathHelper.Path;
                    visual.StrokePath = strokePathHelper.Path;

                    var hags = gs < 8 ? 8 : gs;

                    data.TargetPoint.Context.HoverArea = new RectangleHoverArea()
                        .SetDimensions(x - hgs, y - hgs + 2 * sw, hags, hags + 2 * sw);

                    OnPointMeasured(data.TargetPoint);
                    _ = toDeletePoints.Remove(data.TargetPoint);

                    if (DataLabelsPaint is not null)
                    {
                        var label = (TLabel?)data.TargetPoint.Context.Label;

                        if (label is null)
                        {
                            var l = new TLabel { X = x - hgs, Y = p - hgs };

                            _ = l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                                .WithAnimation(animation =>
                                    animation
                                        .WithDuration(AnimationsSpeed ?? cartesianChart.AnimationsSpeed)
                                        .WithEasingFunction(EasingFunction ?? cartesianChart.EasingFunction));

                            l.CompleteAllTransitions();
                            label = l;
                            data.TargetPoint.Context.Label = l;
                        }

                        DataLabelsPaint.AddGeometryToPaintTask(cartesianChart.Canvas, label);
                        label.Text = DataLabelsFormatter(
                            new TypedChartPoint<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>(data.TargetPoint));
                        label.TextSize = dls;
                        label.Padding = DataLabelsPadding;
                        var labelPosition = GetLabelPosition(
                            x - hgs, y - hgs, gs, gs, label.Measure(DataLabelsPaint), DataLabelsPosition,
                            SeriesProperties, data.TargetPoint.PrimaryValue > Pivot);
                        label.X = labelPosition.X;
                        label.Y = labelPosition.Y;
                    }
                }

                if (GeometryFill is not null)
                {
                    cartesianChart.Canvas.AddDrawableTask(GeometryFill);
                    GeometryFill.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                    GeometryFill.ZIndex = actualZIndex + 0.3;
                }
                if (GeometryStroke is not null)
                {
                    cartesianChart.Canvas.AddDrawableTask(GeometryStroke);
                    GeometryStroke.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                    GeometryStroke.ZIndex = actualZIndex + 0.4;
                }
                segmentI++;
            }

            while (segmentI > fillPathHelperContainer.Count)
            {
                var iFill = fillPathHelperContainer.Count - 1;
                var fillHelper = fillPathHelperContainer[iFill];
                if (Fill is not null) Fill.RemoveGeometryFromPainTask(cartesianChart.Canvas, fillHelper.Path);
                fillPathHelperContainer.RemoveAt(iFill);

                var iStroke = strokePathHelperContainer.Count - 1;
                var strokeHelper = strokePathHelperContainer[iStroke];
                if (Stroke is not null) Stroke.RemoveGeometryFromPainTask(cartesianChart.Canvas, strokeHelper.Path);
                strokePathHelperContainer.RemoveAt(iStroke);
            }

            if (DataLabelsPaint is not null)
            {
                cartesianChart.Canvas.AddDrawableTask(DataLabelsPaint);
                DataLabelsPaint.SetClipRectangle(cartesianChart.Canvas, new RectangleF(drawLocation, drawMarginSize));
                DataLabelsPaint.ZIndex = actualZIndex + 0.5;
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != cartesianChart.View) continue;
                SoftDeletePoint(point, primaryScale, secondaryScale);
                _ = everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override SeriesBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> secondaryAxis, IAxis<TDrawingContext> primaryAxis)
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

            return
                new SeriesBounds(
                    new DimensionalBounds
                    {
                        SecondaryBounds = new Bounds
                        {
                            Max = baseBounds.SecondaryBounds.Max + ts,
                            Min = baseBounds.SecondaryBounds.Min - ts
                        },
                        PrimaryBounds = new Bounds
                        {
                            Max = baseBounds.PrimaryBounds.Max + tp,
                            Min = baseBounds.PrimaryBounds.Min - tp
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
                        },
                        MinDeltaPrimary = baseBounds.MinDeltaPrimary,
                        MinDeltaSecondary = baseBounds.MinDeltaSecondary
                    },
                    false);
        }

        /// <summary>
        /// Sets the default path transitions.
        /// </summary>
        /// <param name="areaHelper">The area helper.</param>
        /// <param name="defaultAnimation">The default animation.</param>
        /// <returns></returns>
        protected virtual void SetDefaultPathTransitions(
            AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs> areaHelper,
            Animation defaultAnimation)
        {
            _ = areaHelper.StartPoint
                .TransitionateProperties(nameof(areaHelper.StartPoint.X), nameof(areaHelper.StartPoint.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();

            _ = areaHelper.StartSegment
                .TransitionateProperties(nameof(areaHelper.StartSegment.X), nameof(areaHelper.StartSegment.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();

            _ = areaHelper.EndSegment
                .TransitionateProperties(nameof(areaHelper.EndSegment.X), nameof(areaHelper.EndSegment.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();
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
                var visual = new TVisual
                {
                    X = _geometryStroke.StrokeThickness,
                    Y = _geometryStroke.StrokeThickness,
                    Height = lss,
                    Width = lss
                };
                sh = _geometryStroke.StrokeThickness;
                strokeClone.ZIndex = 1;
                w += 2 * _geometryStroke.StrokeThickness;
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
            }
            else if (Stroke is not null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeThickness,
                    Y = strokeClone.StrokeThickness,
                    Height = lss,
                    Width = lss
                };
                sh = strokeClone.StrokeThickness;
                strokeClone.ZIndex = 1;
                w += 2 * strokeClone.StrokeThickness;
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
            }

            if (_geometryFill is not null)
            {
                var fillClone = _geometryFill.CloneTask();
                var visual = new TVisual { X = sh, Y = sh, Height = lss, Width = lss };
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
            }
            else if (Fill is not null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = sh, Y = sh, Height = lss, Width = lss };
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
            }

            context.Width = w;
            context.Height = w;

            canvaSchedule = context;
            OnPropertyChanged(nameof(CanvasSchedule));
        }

        private IEnumerable<BezierData> GetSpline(
            ChartPoint[] points,
            Scaler xScale,
            Scaler yScale,
            StackPosition<TDrawingContext>? stacker)
        {
            if (points.Length == 0) yield break;

            ChartPoint previous, current, next, next2;

            for (var i = 0; i < points.Length; i++)
            {
                previous = points[i - 1 < 0 ? 0 : i - 1];
                current = points[i];
                next = points[i + 1 > points.Length - 1 ? points.Length - 1 : i + 1];
                next2 = points[i + 2 > points.Length - 1 ? points.Length - 1 : i + 2];

                var pys = 0d;
                var cys = 0d;
                var nys = 0d;
                var nnys = 0d;

                if (stacker is not null)
                {
                    pys = stacker.GetStack(previous).Start;
                    cys = stacker.GetStack(current).Start;
                    nys = stacker.GetStack(next).Start;
                    nnys = stacker.GetStack(next2).Start;
                }

                var xc1 = (previous.SecondaryValue + current.SecondaryValue) / 2.0f;
                var yc1 = (previous.PrimaryValue + pys + current.PrimaryValue + cys) / 2.0f;
                var xc2 = (current.SecondaryValue + next.SecondaryValue) / 2.0f;
                var yc2 = (current.PrimaryValue + cys + next.PrimaryValue + nys) / 2.0f;
                var xc3 = (next.SecondaryValue + next2.SecondaryValue) / 2.0f;
                var yc3 = (next.PrimaryValue + nys + next2.PrimaryValue + nnys) / 2.0f;

                var len1 = (float)Math.Sqrt(
                    (current.SecondaryValue - previous.SecondaryValue) *
                    (current.SecondaryValue - previous.SecondaryValue) +
                    (current.PrimaryValue + cys - previous.PrimaryValue + pys) * (current.PrimaryValue + cys - previous.PrimaryValue + pys));
                var len2 = (float)Math.Sqrt(
                    (next.SecondaryValue - current.SecondaryValue) *
                    (next.SecondaryValue - current.SecondaryValue) +
                    (next.PrimaryValue + nys - current.PrimaryValue + cys) * (next.PrimaryValue + nys - current.PrimaryValue + cys));
                var len3 = (float)Math.Sqrt(
                    (next2.SecondaryValue - next.SecondaryValue) *
                    (next2.SecondaryValue - next.SecondaryValue) +
                    (next2.PrimaryValue + nnys - next.PrimaryValue + nys) * (next2.PrimaryValue + nnys - next.PrimaryValue + nys));

                var k1 = len1 / (len1 + len2);
                var k2 = len2 / (len2 + len3);

                if (float.IsNaN(k1)) k1 = 0f;
                if (float.IsNaN(k2)) k2 = 0f;

                var xm1 = xc1 + (xc2 - xc1) * k1;
                var ym1 = yc1 + (yc2 - yc1) * k1;
                var xm2 = xc2 + (xc3 - xc2) * k2;
                var ym2 = yc2 + (yc3 - yc2) * k2;

                var c1X = xm1 + (xc2 - xm1) * _lineSmoothness + current.SecondaryValue - xm1;
                var c1Y = ym1 + (yc2 - ym1) * _lineSmoothness + current.PrimaryValue + cys - ym1;
                var c2X = xm2 + (xc2 - xm2) * _lineSmoothness + next.SecondaryValue - xm2;
                var c2Y = ym2 + (yc2 - ym2) * _lineSmoothness + next.PrimaryValue + nys - ym2;

                double x0, y0;

                if (i == 0)
                {
                    x0 = current.SecondaryValue;
                    y0 = current.PrimaryValue + cys;
                }
                else
                {
                    x0 = c1X;
                    y0 = c1Y;
                }

                yield return new BezierData(points[i])
                {
                    IsFirst = i == 0,
                    IsLast = i == points.Length - 1,
                    X0 = xScale.ToPixels(x0),
                    Y0 = yScale.ToPixels(y0),
                    X1 = xScale.ToPixels(c2X),
                    Y1 = yScale.ToPixels(c2Y),
                    X2 = xScale.ToPixels(next.SecondaryValue),
                    Y2 = yScale.ToPixels(next.PrimaryValue + nys),
                    OriginalData = new BezierData(points[i])
                    {
                        X0 = x0,
                        Y0 = y0,
                        X1 = c2X,
                        Y1 = c2Y,
                        X2 = next.SecondaryValue,
                        Y2 = next.PrimaryValue + nys,
                    }
                };
            }
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SetDefaultPointTransitions(ChartPoint)"/>
        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var chart = chartPoint.Context.Chart;

            if (chartPoint.Context.Visual is not LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs> visual)
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
                        .WithEasingFunction(EasingFunction ?? chart.EasingFunction));

            _ = visual.Bezier
                .TransitionateProperties(
                    nameof(visual.Bezier.X0),
                    nameof(visual.Bezier.Y0),
                    nameof(visual.Bezier.X1),
                    nameof(visual.Bezier.Y1),
                    nameof(visual.Bezier.X2),
                    nameof(visual.Bezier.Y2))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(AnimationsSpeed ?? chart.AnimationsSpeed)
                        .WithEasingFunction(EasingFunction ?? chart.EasingFunction));
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SoftDeletePoint(ChartPoint, Scaler, Scaler)"/>
        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>?)point.Context.Visual;
            if (visual is null) return;

            var chartView = (ICartesianChartView<TDrawingContext>)point.Context.Chart;
            if (chartView.Core.IsZoomingOrPanning)
            {
                visual.Geometry.CompleteAllTransitions();
                visual.Geometry.RemoveOnCompleted = true;
                return;
            }

            var gs = _geometrySize;
            var hgs = gs / 2f;

            var x = secondaryScale.ToPixels(point.SecondaryValue) - hgs;
            var y = primaryScale.ToPixels(point.PrimaryValue) - hgs;

            visual.Geometry.X = x;
            visual.Geometry.Y = y;
            visual.Geometry.Height = 0;
            visual.Geometry.Width = 0;
            visual.Geometry.RemoveOnCompleted = true;

            if (dataProvider is null) throw new Exception("Data provider not found");
            dataProvider.DisposePoint(point);

            var label = (TLabel?)point.Context.Label;
            if (label is null) return;

            label.TextSize = 1;
            label.RemoveOnCompleted = true;
        }

        /// <inheritdoc cref="Series{TModel, TVisual, TLabel, TDrawingContext}.SoftDelete(IChartView)"/>
        public override void SoftDelete(IChartView chart)
        {
            base.SoftDelete(chart);
            var canvas = ((ICartesianChartView<TDrawingContext>)chart).CoreCanvas;

            if (Fill is not null)
            {
                foreach (var activeChartContainer in _fillPathHelperDictionary.ToArray())
                    foreach (var pathHelper in activeChartContainer.Value.ToArray())
                        Fill.RemoveGeometryFromPainTask(canvas, pathHelper.Path);
            }

            if (Stroke is not null)
            {
                foreach (var activeChartContainer in _strokePathHelperDictionary.ToArray())
                    foreach (var pathHelper in activeChartContainer.Value.ToArray())
                        Stroke.RemoveGeometryFromPainTask(canvas, pathHelper.Path);
            }

            if (GeometryFill is not null) canvas.RemovePaintTask(GeometryFill);
            if (GeometryStroke is not null) canvas.RemovePaintTask(GeometryStroke);
        }

        /// <summary>
        /// Gets the paint tasks.
        /// </summary>
        /// <returns></returns>
        protected override IPaintTask<TDrawingContext>?[] GetPaintTasks()
        {
            return new[] { Stroke, Fill, _geometryFill, _geometryStroke, DataLabelsPaint };
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
                    if (point.Context.Visual is LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs> visual)
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
    }
}
