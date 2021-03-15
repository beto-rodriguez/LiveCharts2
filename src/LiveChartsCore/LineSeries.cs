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

using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;

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
        private readonly List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>> fillPathHelperContainer =
            new List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>>();
        private readonly List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>> strokePathHelperContainer =
            new List<AreaHelper<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>>();
        private float lineSmoothness = 0.65f;
        private float geometrySize = 14f;
        private bool enableNullSplitting = true;
        private IDrawableTask<TDrawingContext>? shapesFill;
        private IDrawableTask<TDrawingContext>? shapesStroke;

        public LineSeries(bool isStacked = false)
            : base(SeriesProperties.Line | SeriesProperties.VerticalOrientation | (isStacked ? SeriesProperties.Stacked : 0))
        {
            HoverState = LiveCharts.LineSeriesHoverKey;
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometrySize"/>
        public double GeometrySize { get => geometrySize; set => geometrySize = (float)value; }

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
            }
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.EnableNullSplitting"/>
        public bool EnableNullSplitting { get => enableNullSplitting; set => enableNullSplitting = value; }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometryFill"/>
        public IDrawableTask<TDrawingContext>? GeometryFill
        {
            get => shapesFill;
            set
            {
                shapesFill = value;
                if (shapesFill != null)
                {
                    shapesFill.IsStroke = false;
                    shapesFill.StrokeThickness = 0;
                }

                OnPaintContextChanged();
            }
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.GeometrySize"/>
        public IDrawableTask<TDrawingContext>? GeometryStroke
        {
            get => shapesStroke;
            set
            {
                shapesStroke = value;
                if (shapesStroke != null)
                {
                    shapesStroke.IsStroke = true;
                }
                OnPaintContextChanged();
            }
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.OnPointCreated"/>
        Action<ILineBezierVisualChartPoint<TDrawingContext>, IChartView<TDrawingContext>>? ILineSeries<TDrawingContext>.OnPointCreated
        {
            get => OnPointCreated as Action<ILineBezierVisualChartPoint<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointCreated = value;
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.OnPointAddedToState"/>
        Action<ILineBezierVisualChartPoint<TDrawingContext>, IChartView<TDrawingContext>>? ILineSeries<TDrawingContext>.OnPointAddedToState
        {
            get => OnPointAddedToState as Action<ILineBezierVisualChartPoint<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointAddedToState = value;
        }

        /// <inheritdoc cref="ILineSeries{TDrawingContext}.OnPointRemovedFromState"/>
        Action<ILineBezierVisualChartPoint<TDrawingContext>, IChartView<TDrawingContext>>? ILineSeries<TDrawingContext>.OnPointRemovedFromState
        {
            get => OnPointRemovedFromState as Action<ILineBezierVisualChartPoint<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointRemovedFromState = value;
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.Measure(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})" />
        public override void Measure(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            var gs = geometrySize;
            var hgs = gs / 2f;
            float sw = Stroke?.StrokeThickness ?? 0;
            float p = yScale.ScaleToUi(pivot);

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);

            var points = Fetch(chart);

            var segments = enableNullSplitting
                ? SplitEachNull(points, xScale, yScale)
                : new ChartPoint<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>[][] { points };

            StackPosition<TDrawingContext>? stacker = (SeriesProperties & SeriesProperties.Stacked) == SeriesProperties.Stacked
                ? chart.SeriesContext.GetStackPosition(this, GetStackGroup())
                : null;

            if (stacker != null)
            {
                // easy workaround to set an automatic and valid z-index for stacked area series
                // the problem of this solution is that the user needs to set z-indexes above 1000
                // if the user needs to add more series to the chart.
                if (Fill != null) Fill.ZIndex = 1000 - stacker.Position;
                if (Stroke != null) Stroke.ZIndex = 1000 - stacker.Position;
            }

            var dls = unchecked((float)DataLabelsSize);

            var segmentI = 0;

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
                    chart.MeasuredDrawables.Add(fillPathHelper.Path);
                    chart.Canvas.AddDrawableTask(Fill);
                    fillPathHelper.Path.ClearCommands();
                }
                if (Stroke != null)
                {
                    wasStrokeInitialized = strokePathHelper.Initialize(SetDefaultPathTransitions, chartAnimation);
                    Stroke.AddGeometyToPaintTask(strokePathHelper.Path);
                    chart.MeasuredDrawables.Add(strokePathHelper.Path);
                    chart.Canvas.AddDrawableTask(Stroke);
                    strokePathHelper.Path.ClearCommands();
                }
                var ts = OnPointCreated ?? DefaultOnPointCreated;

                foreach (var data in GetSpline(segment, xScale, yScale, stacker))
                {
                    var s = 0f;
                    if (stacker != null)
                    {
                        s = stacker.GetStack(data.TargetPoint).Start;
                    }

                    var x = xScale.ScaleToUi(data.TargetPoint.SecondaryValue);
                    var y = yScale.ScaleToUi(data.TargetPoint.PrimaryValue + s);

                    if (data.TargetPoint.Context.Visual == null)
                    {
                        var v = new LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>();

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

                        ts(v, chart.View);

                        v.Geometry.CompleteAllTransitions();
                        v.Bezier.CompleteAllTransitions();

                        data.TargetPoint.Context.Visual = v;

                        if (GeometryFill != null) GeometryFill.AddGeometyToPaintTask(v.Geometry);
                        if (GeometryStroke != null) GeometryStroke.AddGeometyToPaintTask(v.Geometry);
                    }

                    var visual = data.TargetPoint.Context.Visual;

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
                            fillPathHelper.StartPoint.X = data.X0;
                            fillPathHelper.StartPoint.Y = p;
                            fillPathHelper.Path.AddCommand(fillPathHelper.StartPoint);

                            fillPathHelper.StartSegment.X = data.X0;
                            fillPathHelper.StartSegment.Y = data.Y0;
                            fillPathHelper.Path.AddCommand(fillPathHelper.StartSegment);

                            if (wasFillInitialized)
                            {
                                fillPathHelper.StartPoint.CompleteTransitions(
                                    nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                                fillPathHelper.StartSegment.CompleteTransitions(
                                    nameof(fillPathHelper.StartSegment.Y), nameof(fillPathHelper.StartSegment.X));
                            }
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
                            strokePathHelper.StartPoint.X = data.X0;
                            strokePathHelper.StartPoint.Y = data.Y0;
                            strokePathHelper.Path.AddCommand(strokePathHelper.StartPoint);

                            if (wasStrokeInitialized)
                            {
                                strokePathHelper.StartPoint.CompleteTransitions(
                                   nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                            }
                        }

                        strokePathHelper.Path.AddCommand(visual.Bezier);
                    }

                    visual.Geometry.X = x - hgs;
                    visual.Geometry.Y = y - hgs;
                    visual.Geometry.Width = gs;
                    visual.Geometry.Height = gs;

                    data.TargetPoint.Context.HoverArea = new RectangleHoverArea().SetDimensions(x - hgs, y - hgs + 2 * sw, gs, gs + 2 * sw);
                    OnPointMeasured(data.TargetPoint, visual);
                    chart.MeasuredDrawables.Add(visual.Geometry);

                    if (DataLabelsBrush != null)
                    {
                        if (data.TargetPoint.Context.Label == null)
                        {
                            var l = new TLabel { X = x - hgs, Y = p - hgs };

                            l.TransitionateProperties(nameof(l.X), nameof(l.Y))
                                .WithAnimation(a =>
                                    a.WithDuration(chart.AnimationsSpeed)
                                    .WithEasingFunction(chart.EasingFunction));

                            l.CompleteAllTransitions();
                            data.TargetPoint.Context.Label = l;
                            DataLabelsBrush.AddGeometyToPaintTask(l);
                        }

                        data.TargetPoint.Context.Label.Text = DataLabelFormatter(data.TargetPoint);
                        data.TargetPoint.Context.Label.TextSize = dls;
                        data.TargetPoint.Context.Label.Padding = DataLabelsPadding;
                        var labelPosition = GetLabelPosition(
                            x - hgs, y - hgs, gs, gs, data.TargetPoint.Context.Label.Measure(DataLabelsBrush), DataLabelsPosition,
                            SeriesProperties, data.TargetPoint.PrimaryValue > Pivot);
                        data.TargetPoint.Context.Label.X = labelPosition.X;
                        data.TargetPoint.Context.Label.Y = labelPosition.Y;

                        chart.MeasuredDrawables.Add(data.TargetPoint.Context.Label);
                    }
                }

                if (GeometryFill != null) chart.Canvas.AddDrawableTask(GeometryFill);
                if (GeometryStroke != null) chart.Canvas.AddDrawableTask(GeometryStroke);
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

            if (DataLabelsBrush != null) chart.Canvas.AddDrawableTask(DataLabelsBrush);
        }

        /// <inheritdoc cref="ICartesianSeries{TDrawingContext}.GetBounds(CartesianChart{TDrawingContext}, IAxis{TDrawingContext}, IAxis{TDrawingContext})"/>
        public override DimensinalBounds GetBounds(
            CartesianChart<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(chart, x, y);

            var tick = y.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);
            var xTick = x.GetTick(chart.ControlSize, baseBounds.SecondaryBounds);

            return new DimensinalBounds
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

        protected virtual void DefaultOnPointCreated(ILineBezierVisualChartPoint<TDrawingContext> visual, IChartView<TDrawingContext> chart)
        {
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

            if (shapesFill != null)
            {
                var fillClone = shapesFill.CloneTask();
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

            if (shapesStroke != null)
            {
                var strokeClone = shapesStroke.CloneTask();
                var visual = new TVisual
                {
                    X = shapesStroke.StrokeThickness,
                    Y = shapesStroke.StrokeThickness,
                    Height = lss,
                    Width = lss
                };
                w += 2 * shapesStroke.StrokeThickness;
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

        private IEnumerable<BezierData<LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>> GetSpline(
            ChartPoint<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>[] points,
            ScaleContext xScale,
            ScaleContext yScale,
            StackPosition<TDrawingContext>? stacker)
        {
            if (points.Length == 0) yield break;
            IChartPoint<LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext> previous, current, next, next2;


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

                yield return new BezierData<LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>(points[i])
                {
                    IsFirst = i == 0,
                    IsLast = i == points.Length - 1,
                    X0 = xScale.ScaleToUi(x0),
                    Y0 = yScale.ScaleToUi(y0),
                    X1 = xScale.ScaleToUi(c2X),
                    Y1 = yScale.ScaleToUi(c2Y),
                    X2 = xScale.ScaleToUi(next.SecondaryValue),
                    Y2 = yScale.ScaleToUi(next.PrimaryValue + nys)
                };
            }
        }

        private IEnumerable<ChartPoint<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>[]> SplitEachNull(
            ChartPoint<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>[] points,
            ScaleContext xScale,
            ScaleContext yScale)
        {
            List<ChartPoint<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>> l =
                new List<ChartPoint<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>>(points.Length);

            foreach (var point in points)
            {
                if (point.IsNull)
                {
                    if (point.Context.Visual != null)
                    {
                        var x = xScale.ScaleToUi(point.SecondaryValue);
                        var y = yScale.ScaleToUi(point.PrimaryValue);
                        var gs = geometrySize;
                        var hgs = gs / 2f;
                        float sw = Stroke?.StrokeThickness ?? 0;
                        float p = yScale.ScaleToUi(pivot);
                        point.Context.Visual.Geometry.X = x - hgs;
                        point.Context.Visual.Geometry.Y = p - hgs;
                        point.Context.Visual.Geometry.Width = gs;
                        point.Context.Visual.Geometry.Height = gs;
                        point.Context.Visual.Geometry.RemoveOnCompleted = true;
                        point.Context.Visual = null;
                    }

                    if (l.Count > 0) yield return l.ToArray();
                    l = new List<ChartPoint<TModel, LineBezierVisualPoint<TDrawingContext, TVisual, TBezierSegment, TPathArgs>, TLabel, TDrawingContext>>(points.Length);
                    continue;
                }

                l.Add(point);
            }

            if (l.Count > 0) yield return l.ToArray();
        }
    }
}
