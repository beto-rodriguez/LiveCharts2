// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

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

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the data to plot as a line.
    /// </summary>
    public class LineSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs>
        : CartesianSeries<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>, ILineSeries<TDrawingContext>
        where TPathGeometry : IPathGeometry<TDrawingContext, TPathArgs>, new()
        where TLineSegment : ILinePathSegment<TPathArgs>, new()
        where TBezierSegment : IBezierSegment<TPathArgs>, new()
        where TMoveToCommand : IMoveToPathCommand<TPathArgs>, new()
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>> fillPathHelperContainer = new();
        private readonly List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>> strokePathHelperContainer = new();
        private float lineSmoothness = 0.65f;
        private float geometrySize = 14f;
        private bool enableNullSplitting = true;
        private IDrawableTask<TDrawingContext>? geometryFill;
        private IDrawableTask<TDrawingContext>? geometryStroke;

        public LineSeries(bool isStacked = false)
            : base(SeriesProperties.Line | SeriesProperties.VerticalOrientation | (isStacked ? SeriesProperties.Stacked : 0))
        {
            HoverState = LiveCharts.LineSeriesHoverKey;
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometrySize"/>
        public double GeometrySize { get => geometrySize; set { geometrySize = (float)value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.LineSmoothness"/>
        public double LineSmoothness
        {
            get => lineSmoothness;
            set
            {
                var v = value;
                if (value > 1) v = 1;
                if (value < 0) v = 0;
                lineSmoothness = (float)v;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.EnableNullSplitting"/>
        public bool EnableNullSplitting { get => enableNullSplitting; set { enableNullSplitting = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometryFill"/>
        public IDrawableTask<TDrawingContext>? GeometryFill
        {
            get => geometryFill;
            set
            {
                if (geometryFill != null) deletingTasks.Add(geometryFill);
                geometryFill = value;
                if (geometryFill != null)
                {
                    geometryFill.IsStroke = false;
                    geometryFill.StrokeThickness = 0;
                }

                OnPaintContextChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometrySize"/>
        public IDrawableTask<TDrawingContext>? GeometryStroke
        {
            get => geometryStroke;
            set
            {
                if (geometryStroke != null) deletingTasks.Add(geometryStroke);
                geometryStroke = value;
                if (geometryStroke != null)
                {
                    geometryStroke.IsStroke = true;
                }
                OnPaintContextChanged();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.Measure(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})" />
        public override void Measure(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var secondaryScale = new Scaler(drawLocation, drawMarginSize, xAxis);
            var primaryScale = new Scaler(drawLocation, drawMarginSize, yAxis);

            var gs = geometrySize;
            var hgs = gs / 2f;
            float sw = Stroke?.StrokeThickness ?? 0;
            float p = primaryScale.ToPixels(pivot);

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);

            var fetched = Fetch(chart);
            if (fetched is not ChartPoint[] points) points = fetched.ToArray();

            var segments = enableNullSplitting
                ? SplitEachNull(points, secondaryScale, primaryScale)
                : new ChartPoint[][] { points };

            StackPosition<TDrawingContext>? stacker = (SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked
                ? chart.SeriesContext.GetStackPosition(this, GetStackGroup())
                : null;

            var actualZIndex = ZIndex == 0 ? ((ISeries)this).SeriesId : ZIndex;

            if (stacker != null)
            {
                // easy workaround to set an automatic and valid z-index for stacked area series
                // the problem of this solution is that the user needs to set z-indexes above 1000
                // if the user needs to add more series to the chart.
                actualZIndex = 1000 - stacker.Position;
                if (Fill != null) Fill.ZIndex = actualZIndex;
                if (Stroke != null) Stroke.ZIndex = actualZIndex;
            }

            var dls = unchecked((float)DataLabelsSize);

            var segmentI = 0;
            var toDeletePoints = new HashSet<ChartPoint>(everFetched);

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

                if (Fill != null)
                {
                    wasFillInitialized = fillPathHelper.Initialize(SetDefaultPathTransitions, chartAnimation);
                    Fill.AddGeometyToPaintTask(fillPathHelper.Path);
                    chart.Canvas.AddDrawableTask(Fill);
                    Fill.ZIndex = actualZIndex + 0.1;
                    Fill.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                    fillPathHelper.Path.ClearCommands();
                }
                if (Stroke != null)
                {
                    wasStrokeInitialized = strokePathHelper.Initialize(SetDefaultPathTransitions, chartAnimation);
                    Stroke.AddGeometyToPaintTask(strokePathHelper.Path);
                    chart.Canvas.AddDrawableTask(Stroke);
                    Stroke.ZIndex = actualZIndex + 0.2;
                    Stroke.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                    strokePathHelper.Path.ClearCommands();
                }

                foreach (var data in GetSpline(segment, secondaryScale, primaryScale, stacker))
                {
                    var s = 0f;
                    if (stacker != null)
                    {
                        s = stacker.GetStack(data.TargetPoint).Start;
                    }

                    var x = secondaryScale.ToPixels(data.TargetPoint.SecondaryValue);
                    var y = primaryScale.ToPixels(data.TargetPoint.PrimaryValue + s);

                    var visual = (LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>?)data.TargetPoint.Context.Visual;

                    if (visual == null)
                    {
                        var v = new LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>();

                        visual = v;

                        v.Geometry.X = x - hgs;
                        v.Geometry.Y = p - hgs;
                        v.Geometry.Width = gs;
                        v.Geometry.Height = gs;

                        v.Bezier.X0 = data.X0;
                        v.Bezier.Y0 = p - hgs;
                        v.Bezier.X1 = data.X1;
                        v.Bezier.Y1 = p - hgs;
                        v.Bezier.X2 = data.X2;
                        v.Bezier.Y2 = p - hgs;

                        data.TargetPoint.Context.Visual = v;
                        OnPointCreated(data.TargetPoint);
                        v.Geometry.CompleteAllTransitions();
                        v.Bezier.CompleteAllTransitions();
                        everFetched.Add(data.TargetPoint);
                    }

                    if (GeometryFill != null) GeometryFill.AddGeometyToPaintTask(visual.Geometry);
                    if (GeometryStroke != null) GeometryStroke.AddGeometyToPaintTask(visual.Geometry);

                    visual.Bezier.X0 = data.X0;
                    visual.Bezier.Y0 = data.Y0;
                    visual.Bezier.X1 = data.X1;
                    visual.Bezier.Y1 = data.Y1;
                    visual.Bezier.X2 = data.X2;
                    visual.Bezier.Y2 = data.Y2;

                    if (Fill != null)
                    {
                        if (data.IsFirst)
                        {
                            if (wasFillInitialized)
                            {
                                fillPathHelper.StartPoint.X = data.X0;
                                fillPathHelper.StartPoint.Y = p;

                                fillPathHelper.StartSegment.X = data.X0;
                                fillPathHelper.StartSegment.Y = p;

                                fillPathHelper.StartPoint.CompleteTransitions(
                                    nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                                fillPathHelper.StartSegment.CompleteTransitions(
                                    nameof(fillPathHelper.StartSegment.Y), nameof(fillPathHelper.StartSegment.X));
                            }

                            fillPathHelper.StartPoint.X = data.X0;
                            fillPathHelper.StartPoint.Y = p;
                            fillPathHelper.Path.AddCommand(fillPathHelper.StartPoint);

                            fillPathHelper.StartSegment.X = data.X0;
                            fillPathHelper.StartSegment.Y = data.Y0;
                            fillPathHelper.Path.AddCommand(fillPathHelper.StartSegment);
                        }

                        fillPathHelper.Path.AddCommand(visual.Bezier);

                        if (data.IsLast)
                        {
                            fillPathHelper.EndSegment.X = data.X2;
                            fillPathHelper.EndSegment.Y = p;
                            fillPathHelper.Path.AddCommand(fillPathHelper.EndSegment);

                            if (wasFillInitialized)
                                fillPathHelper.EndSegment.CompleteTransitions(
                                    nameof(fillPathHelper.EndSegment.Y), nameof(fillPathHelper.EndSegment.X));
                        }
                    }
                    if (Stroke != null)
                    {
                        if (data.IsFirst)
                        {
                            if (wasStrokeInitialized)
                            {
                                strokePathHelper.StartPoint.X = data.X0;
                                strokePathHelper.StartPoint.Y = p;

                                strokePathHelper.StartPoint.CompleteTransitions(
                                   nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                            }

                            strokePathHelper.StartPoint.X = data.X0;
                            strokePathHelper.StartPoint.Y = data.Y0;
                            strokePathHelper.Path.AddCommand(strokePathHelper.StartPoint);
                        }

                        strokePathHelper.Path.AddCommand(visual.Bezier);
                    }

                    visual.Geometry.X = x - hgs;
                    visual.Geometry.Y = y - hgs;
                    visual.Geometry.Width = gs;
                    visual.Geometry.Height = gs;

                    data.TargetPoint.Context.HoverArea = new RectangleHoverArea().SetDimensions(x - hgs, y - hgs + 2 * sw, gs, gs + 2 * sw);

                    OnPointMeasured(data.TargetPoint);
                    toDeletePoints.Remove(data.TargetPoint);

                    if (DataLabelsDrawableTask != null)
                    {
                        var label = (TLabel?)data.TargetPoint.Context.Label;

                        if (label == null)
                        {
                            var l = new TLabel { X = x - hgs, Y = p - hgs };

                            l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                                .WithAnimation(a =>
                                    a.WithDuration(chart.AnimationsSpeed)
                                    .WithEasingFunction(chart.EasingFunction));

                            l.CompleteAllTransitions();
                            label = l;
                            data.TargetPoint.Context.Label = l;
                            DataLabelsDrawableTask.AddGeometyToPaintTask(l);
                        }

                        label.Text = DataLabelFormatter(data.TargetPoint);
                        label.TextSize = dls;
                        label.Padding = DataLabelsPadding;
                        var labelPosition = GetLabelPosition(
                            x - hgs, y - hgs, gs, gs, label.Measure(DataLabelsDrawableTask), DataLabelsPosition,
                            SeriesProperties, data.TargetPoint.PrimaryValue > Pivot);
                        label.X = labelPosition.X;
                        label.Y = labelPosition.Y;
                    }
                }

                if (GeometryFill != null)
                {
                    chart.Canvas.AddDrawableTask(GeometryFill);
                    GeometryFill.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                    GeometryFill.ZIndex = actualZIndex + 0.3;
                }
                if (GeometryStroke != null) {
                    chart.Canvas.AddDrawableTask(GeometryStroke);
                    GeometryStroke.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                    GeometryStroke.ZIndex = actualZIndex + 0.4;
                }
                segmentI++;
            }

            while (segmentI > fillPathHelperContainer.Count)
            {
                var iFill = fillPathHelperContainer.Count - 1;
                var fillHelper = fillPathHelperContainer[iFill];
                if (Fill != null) Fill.RemoveGeometryFromPainTask(fillHelper.Path);
                fillPathHelperContainer.RemoveAt(iFill);

                var iStroke = strokePathHelperContainer.Count - 1;
                var strokeHelper = strokePathHelperContainer[iStroke];
                if (Stroke != null) Stroke.RemoveGeometryFromPainTask(strokeHelper.Path);
                strokePathHelperContainer.RemoveAt(iStroke);
            }

            if (DataLabelsDrawableTask != null)
            {
                chart.Canvas.AddDrawableTask(DataLabelsDrawableTask);
                DataLabelsDrawableTask.ClipRectangle = new RectangleF(drawLocation, drawMarginSize);
                DataLabelsDrawableTask.ZIndex = actualZIndex + 0.5;
            }

            foreach (var point in toDeletePoints)
            {
                if (point.Context.Chart != chart.View) continue;
                SoftDeletePoint(point, primaryScale, secondaryScale);
                everFetched.Remove(point);
            }
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override DimensionalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(chart, x, y);

            var tick = y.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);
            var xTick = x.GetTick(chart.ControlSize, baseBounds.SecondaryBounds);

            return new DimensionalBounds
            {
                SecondaryBounds = new Bounds
                {
                    Max = baseBounds.SecondaryBounds.Max + 0.5f * xTick.Value,
                    Min = baseBounds.SecondaryBounds.Min - 0.5f * xTick.Value
                },
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tick.Value,
                    min = baseBounds.PrimaryBounds.min - tick.Value
                }
            };
        }

        protected virtual void SetDefaultPathTransitions(
            ISizedGeometry<TDrawingContext> geometry, Animation defaultAnimation)
        {
            var defaultProperties = new string[]
            {
                nameof(geometry.X),
                nameof(geometry.Y),
                nameof(geometry.Width),
                nameof(geometry.Height)
            };
            geometry.SetPropertiesTransitions(defaultAnimation, defaultProperties);
            geometry.CompleteTransitions(defaultProperties);
        }

        protected virtual void SetDefaultPathTransitions(
            AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs> areaHelper, Animation defaultAnimation)
        {
            areaHelper.StartPoint
                .TransitionateProperties(nameof(areaHelper.StartPoint.X), nameof(areaHelper.StartPoint.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();

            areaHelper.StartSegment
                .TransitionateProperties(nameof(areaHelper.StartSegment.X), nameof(areaHelper.StartSegment.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();

            areaHelper.EndSegment
                .TransitionateProperties(nameof(areaHelper.EndSegment.X), nameof(areaHelper.EndSegment.Y))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();
        }

        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();
            var lss = unchecked((float)LegendShapeSize);
            var w = LegendShapeSize;

            if (geometryFill != null)
            {
                var fillClone = geometryFill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = lss, Width = lss };
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }
            else if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = lss, Width = lss };
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            if (geometryStroke != null)
            {
                var strokeClone = geometryStroke.CloneTask();
                var visual = new TVisual
                {
                    X = geometryStroke.StrokeThickness,
                    Y = geometryStroke.StrokeThickness,
                    Height = lss,
                    Width = lss
                };
                w += 2 * geometryStroke.StrokeThickness;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }
            else if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeThickness,
                    Y = strokeClone.StrokeThickness,
                    Height = lss,
                    Width = lss
                };
                w += 2 * strokeClone.StrokeThickness;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
        }

        private IEnumerable<BezierData> GetSpline(
            ChartPoint[] points,
            Scaler xScale,
            Scaler yScale,
            StackPosition<TDrawingContext>? stacker)
        {
            if (points.Length == 0) yield break;

            ChartPoint previous, current, next, next2;

            for (int i = 0; i < points.Length; i++)
            {
                previous = points[i - 1 < 0 ? 0 : i - 1];
                current = points[i];
                next = points[i + 1 > points.Length - 1 ? points.Length - 1 : i + 1];
                next2 = points[i + 2 > points.Length - 1 ? points.Length - 1 : i + 2];

                var pys = 0f;
                var cys = 0f;
                var nys = 0f;
                var nnys = 0f;

                if (stacker != null)
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

                var c1X = xm1 + (xc2 - xm1) * lineSmoothness + current.SecondaryValue - xm1;
                var c1Y = ym1 + (yc2 - ym1) * lineSmoothness + current.PrimaryValue + cys - ym1;
                var c2X = xm2 + (xc2 - xm2) * lineSmoothness + next.SecondaryValue - xm2;
                var c2Y = ym2 + (yc2 - ym2) * lineSmoothness + next.PrimaryValue + nys - ym2;

                float x0, y0;

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
                    Y2 = yScale.ToPixels(next.PrimaryValue + nys)
                };
            }
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
                    var visual = point.Context.Visual as LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>;
                    if (visual != null)
                    {
                        var x = xScale.ToPixels(point.SecondaryValue);
                        var y = yScale.ToPixels(point.PrimaryValue);
                        var gs = geometrySize;
                        var hgs = gs / 2f;
                        float sw = Stroke?.StrokeThickness ?? 0;
                        float p = yScale.ToPixels(pivot);
                        visual.Geometry.X = x - hgs;
                        visual.Geometry.Y = p - hgs;
                        visual.Geometry.Width = gs;
                        visual.Geometry.Height = gs;
                        visual.Geometry.RemoveOnCompleted = true;
                        visual = null;
                    }

                    if (l.Count > 0) yield return l.ToArray();
                    l = new List<ChartPoint>(points.Length);
                    continue;
                }

                l.Add(point);
            }

            if (l.Count > 0) yield return l.ToArray();
        }

        protected override void SetDefaultPointTransitions(ChartPoint chartPoint)
        {
            var visual = chartPoint.Context.Visual as LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>;
            var chart = chartPoint.Context.Chart;

            if (visual == null) throw new Exception("Unable to initialize the point instance.");

            visual.Geometry
                .TransitionateProperties(
                    nameof(visual.Geometry.X),
                    nameof(visual.Geometry.Y),
                    nameof(visual.Geometry.Width),
                    nameof(visual.Geometry.Height))
                .WithAnimation(animation =>
                    animation
                        .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));

            visual.Bezier
                .TransitionateProperties(
                    nameof(visual.Bezier.X0),
                    nameof(visual.Bezier.Y0),
                    nameof(visual.Bezier.X1),
                    nameof(visual.Bezier.Y1),
                    nameof(visual.Bezier.X2),
                    nameof(visual.Bezier.Y2))
                .WithAnimation(animation =>
                    animation
                         .WithDuration(chart.AnimationsSpeed)
                        .WithEasingFunction(chart.EasingFunction));
        } 

        protected override void SoftDeletePoint(ChartPoint point, Scaler primaryScale, Scaler secondaryScale)
        {
            var visual = (LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>?)point.Context.Visual;
            if (visual == null) return;

            //float p = primaryScale.ToPixels(pivot);

            //var secondary = secondaryScale.ToPixels(point.SecondaryValue);
            //var x = secondary;// - uwm + cp; // we cant know those values... the series does not have a position now...

            //visual.X = 0;
            //visual.Y = 0;
            visual.Geometry.Height = 0;
            visual.Geometry.Width = 0;
            visual.Geometry.RemoveOnCompleted = true;
        }

        public override void Delete(IChartView chart)
        {
            base.Delete(chart);

            if (Fill != null)
                foreach (var pathHelper in fillPathHelperContainer.ToArray())
                    Fill.RemoveGeometryFromPainTask(pathHelper.Path);

            if (Stroke != null)
                foreach (var pathHelper in strokePathHelperContainer.ToArray())
                    Stroke.RemoveGeometryFromPainTask(pathHelper.Path);
        }

        public override void Dispose()
        {
            foreach (var chart in subscribedTo)
            {
                var c = (Chart<TDrawingContext>)chart;
                if (geometryFill != null) c.Canvas.RemovePaintTask(geometryFill);
                if (geometryStroke != null) c.Canvas.RemovePaintTask(geometryStroke);
            }

            base.Dispose();
        }
    }
}
