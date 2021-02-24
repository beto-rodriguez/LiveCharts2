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
using System.Linq;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the data to plot as a line.
    /// </summary>
    public class LineSeries<TModel, TVisual, TDrawingContext, TGeometryPath, TLineSegment, TBezierSegment, TMoveToCommand, TPathContext>
        : CartesianSeries<TModel, LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext>, TDrawingContext>
        where TGeometryPath : IPathGeometry<TDrawingContext, TPathContext>, new()
        where TLineSegment : ILinePathSegment<TPathContext>, new()
        where TBezierSegment : IBezierSegment<TPathContext>, new()
        where TMoveToCommand : IMoveToPathCommand<TPathContext>, new()
        where TVisual : class, ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveToCommand, TPathContext> fillPathHelper =
            new AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveToCommand, TPathContext>();
        private readonly AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveToCommand, TPathContext> strokePathHelper =
            new AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveToCommand, TPathContext>();
        private double lineSmoothness = 0.65;
        private double geometrySize = 18d;
        private IDrawableTask<TDrawingContext>? shapesFill;
        private IDrawableTask<TDrawingContext>? shapesStroke;

        public LineSeries()
            : base(SeriesProperties.Line | SeriesProperties.VerticalOrientation)
        {

        }

        public IDrawableTask<TDrawingContext>? ShapesFill
        {
            get => shapesFill;
            set
            {
                shapesFill = value;
                if (shapesFill != null)
                {
                    shapesFill.IsStroke = false;
                    shapesFill.StrokeWidth = 0;
                }

                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext>? ShapesStroke
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

        public double Pivot { get; set; }

        public double GeometrySize { get => geometrySize; set => geometrySize = value; }

        public double LineSmoothness { get => lineSmoothness; set => lineSmoothness = value; }

        public TransitionsSetterDelegate<LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext>>? TransitionsSetter { get; set; }

        public TransitionsSetterDelegate<AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveToCommand, TPathContext>>? PathTransitionsSetter { get; set; }

        public override void Measure(
            CartesianChartCore<TDrawingContext> chart, IAxis<TDrawingContext>xAxis, IAxis<TDrawingContext> yAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            var gs = unchecked((float)geometrySize);
            var hgs = gs / 2f;
            float sw = Stroke?.StrokeWidth ?? 0;
            float p = yScale.ScaleToUi(unchecked((float)Pivot));

            var wasFillInitialized = false;
            var wasStrokeInitialized = false;

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);
            var pts = PathTransitionsSetter ?? SetDefaultPathTransitions;
            if (Fill != null)
            {
                wasFillInitialized = fillPathHelper.Initialize(pts, chartAnimation);
                Fill.AddGeometyToPaintTask(fillPathHelper.Path);
                chart.MeasuredDrawables.Add(fillPathHelper.Path);
                chart.Canvas.AddPaintTask(Fill);
                fillPathHelper.Path.ClearCommands();
            }
            if (Stroke != null)
            {
                wasStrokeInitialized = strokePathHelper.Initialize(pts, chartAnimation);
                Stroke.AddGeometyToPaintTask(strokePathHelper.Path);
                chart.MeasuredDrawables.Add(strokePathHelper.Path);
                chart.Canvas.AddPaintTask(Stroke);
                strokePathHelper.Path.ClearCommands();
            }
            var ts = TransitionsSetter ?? SetDefaultTransitions;

            foreach (var data in GetSpline(chart, xScale, yScale))
            {
                var x = xScale.ScaleToUi(data.TargetPoint.SecondaryValue);
                var y = yScale.ScaleToUi(data.TargetPoint.PrimaryValue);

                if (data.TargetPoint.PointContext.Visual == null)
                {
                    var v = new LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext>
                    {
                        Geometry = new TVisual(),
                        Bezier = new TBezierSegment()
                    };

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

                    ts(v, chartAnimation);

                    data.TargetPoint.PointContext.Visual = v;

                    if (ShapesFill != null) ShapesFill.AddGeometyToPaintTask(v.Geometry);
                    if (ShapesStroke != null) ShapesStroke.AddGeometyToPaintTask(v.Geometry);
                }

                var visual = data.TargetPoint.PointContext.Visual;

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
                            fillPathHelper.StartPoint.CompleteTransition(
                                nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                            fillPathHelper.StartSegment.CompleteTransition(
                                nameof(fillPathHelper.StartSegment.Y), nameof(fillPathHelper.StartSegment.X));
                        }
                    }

                    fillPathHelper.Path.AddCommand(visual.Bezier);

                    if (data.IsLast)
                    {
                        fillPathHelper.EndSegment.X = data.X0;
                        fillPathHelper.EndSegment.Y = p;
                        fillPathHelper.Path.AddCommand(fillPathHelper.EndSegment);

                        if (wasFillInitialized)
                            fillPathHelper.EndSegment.CompleteTransition(
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
                            strokePathHelper.StartPoint.CompleteTransition(
                               nameof(fillPathHelper.StartPoint.Y), nameof(fillPathHelper.StartPoint.X));
                        }
                    }

                    strokePathHelper.Path.AddCommand(visual.Bezier);
                }

                visual.Geometry.X = x - hgs;
                visual.Geometry.Y = y - hgs;
                visual.Geometry.Width = gs;
                visual.Geometry.Height = gs;

                data.TargetPoint.PointContext.HoverArea.SetDimensions(x - hgs, y - hgs + 2 * sw, gs, gs + 2 * sw);
                OnPointMeasured(data.TargetPoint, visual);
                chart.MeasuredDrawables.Add(visual.Geometry);
            }

            if (HighlightFill != null) chart.Canvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) chart.Canvas.AddPaintTask(HighlightStroke);
            if (ShapesFill != null) chart.Canvas.AddPaintTask(ShapesFill);
            if (ShapesStroke != null) chart.Canvas.AddPaintTask(ShapesStroke);
        }

        public override CartesianBounds GetBounds(
            CartesianChartCore<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(chart, x, y);

            var tick = y.GetTick(chart.ControlSize, baseBounds.PrimaryBounds);

            return new CartesianBounds
            {
                SecondaryBounds = new Bounds
                {
                    Max = baseBounds.SecondaryBounds.Max,
                    Min = baseBounds.SecondaryBounds.Min
                },
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tick.Value,
                    min = baseBounds.PrimaryBounds.min - tick.Value
                }
            };
        }

        protected virtual void SetDefaultTransitions(
            LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext> visual,
            Animation defaultAnimation)
        {
            var geometryProperties = new string[]
            {
                nameof(visual.Geometry.X),
                nameof(visual.Geometry.Y),
                nameof(visual.Geometry.Width),
                nameof(visual.Geometry.Height)
            };
            visual.Geometry.SetPropertyTransition(defaultAnimation, geometryProperties);
            visual.Geometry.CompleteTransition(geometryProperties);

            var cubicBezierProperties = new string[]
            {
                nameof(visual.Bezier.X0),
                nameof(visual.Bezier.Y0),
                nameof(visual.Bezier.X1),
                nameof(visual.Bezier.Y1),
                nameof(visual.Bezier.X2),
                nameof(visual.Bezier.Y2)
            };
            visual.Bezier.SetPropertyTransition(defaultAnimation, cubicBezierProperties);
            visual.Bezier.CompleteTransition(cubicBezierProperties);
        }

        private IEnumerable<BezierData<LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext>, TDrawingContext>> GetSpline(
            CartesianChartCore<TDrawingContext> chart, ScaleContext xScale, ScaleContext yScale)
        {
            var points = Fetch(chart).ToArray();

            if (points.Length == 0) yield break;
            IChartPoint<LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext>, TDrawingContext> previous, current, next, next2;

            for (int i = 0; i < points.Length; i++)
            {
                previous = points[i - 1 < 0 ? 0 : i - 1];
                current = points[i];
                next = points[i + 1 > points.Length - 1 ? points.Length - 1 : i + 1];
                next2 = points[i + 2 > points.Length - 1 ? points.Length - 1 : i + 2];

                var xc1 = (previous.SecondaryValue + current.SecondaryValue) / 2.0;
                var yc1 = (previous.PrimaryValue + current.PrimaryValue) / 2.0;
                var xc2 = (current.SecondaryValue + next.SecondaryValue) / 2.0;
                var yc2 = (current.PrimaryValue + next.PrimaryValue) / 2.0;
                var xc3 = (next.SecondaryValue + next2.SecondaryValue) / 2.0;
                var yc3 = (next.PrimaryValue + next2.PrimaryValue) / 2.0;

                var len1 = Math.Sqrt(
                    (current.SecondaryValue - previous.SecondaryValue) * 
                    (current.SecondaryValue - previous.SecondaryValue) + 
                    (current.PrimaryValue - previous.PrimaryValue) * (current.PrimaryValue - previous.PrimaryValue));
                var len2 = Math.Sqrt(
                    (next.SecondaryValue - current.SecondaryValue) * 
                    (next.SecondaryValue - current.SecondaryValue) + 
                    (next.PrimaryValue - current.PrimaryValue) * (next.PrimaryValue - current.PrimaryValue));
                var len3 = Math.Sqrt(
                    (next2.SecondaryValue - next.SecondaryValue) * 
                    (next2.SecondaryValue - next.SecondaryValue) + 
                    (next2.PrimaryValue - next.PrimaryValue) * (next2.PrimaryValue - next.PrimaryValue));

                var k1 = len1 / (len1 + len2);
                var k2 = len2 / (len2 + len3);

                if (double.IsNaN(k1)) k1 = 0d;
                if (double.IsNaN(k2)) k2 = 0d;

                var xm1 = xc1 + (xc2 - xc1) * k1;
                var ym1 = yc1 + (yc2 - yc1) * k1;
                var xm2 = xc2 + (xc3 - xc2) * k2;
                var ym2 = yc2 + (yc3 - yc2) * k2;

                var c1X = xm1 + (xc2 - xm1) * lineSmoothness + current.SecondaryValue - xm1;
                var c1Y = ym1 + (yc2 - ym1) * lineSmoothness + current.PrimaryValue - ym1;
                var c2X = xm2 + (xc2 - xm2) * lineSmoothness + next.SecondaryValue - xm2;
                var c2Y = ym2 + (yc2 - ym2) * lineSmoothness + next.PrimaryValue - ym2;

                unchecked
                {
                    float x0, y0;

                    if (i == 0)
                    {
                        x0 = current.SecondaryValue;
                        y0 = current.PrimaryValue;
                    }
                    else
                    {
                        x0 = (float)c1X;
                        y0 = (float)c1Y;
                    }

                    yield return new BezierData<LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext>, TDrawingContext>(points[i])
                    {
                        IsFirst = i == 0,
                        IsLast = i == points.Length - 1,
                        X0 = xScale.ScaleToUi(x0),
                        Y0 = yScale.ScaleToUi(y0),
                        X1 = xScale.ScaleToUi((float)c2X),
                        Y1 = yScale.ScaleToUi((float)c2Y),
                        X2 = xScale.ScaleToUi(next.SecondaryValue),
                        Y2 = yScale.ScaleToUi(next.PrimaryValue)
                    };
                }
            }
        }

        protected virtual void SetDefaultTransitions(
            ISizedGeometry<TDrawingContext> geometry, Animation defaultAnimation)
        {
            var defaultProperties = new string[]
            {
                nameof(geometry.X),
                nameof(geometry.Y),
                nameof(geometry.Width),
                nameof(geometry.Height)
            };
            geometry.SetPropertyTransition(defaultAnimation, defaultProperties);
            geometry.CompleteTransition(defaultProperties);
        }

        protected virtual void SetDefaultPathTransitions(
            AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveToCommand, TPathContext> areaHelper, Animation defaultAnimation)
        {
            var startPointProperties = new string[]
            {
                nameof(areaHelper.StartPoint.X),
                nameof(areaHelper.StartPoint.Y)
            };
            areaHelper.StartPoint.SetPropertyTransition(defaultAnimation, startPointProperties);
            areaHelper.StartPoint.CompleteTransition(startPointProperties);

            var startSegmentProperties = new string[]
            {
                nameof(areaHelper.StartSegment.X),
                nameof(areaHelper.StartSegment.Y)
            };
            areaHelper.StartSegment.SetPropertyTransition(defaultAnimation, startSegmentProperties);
            areaHelper.StartSegment.CompleteTransition(startSegmentProperties);

            var endSegmentProperties = new string[]
            {
                nameof(areaHelper.EndSegment.X),
                nameof(areaHelper.EndSegment.Y)
            };
            areaHelper.EndSegment.SetPropertyTransition(defaultAnimation, endSegmentProperties);
            areaHelper.EndSegment.CompleteTransition(endSegmentProperties);
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
                    X = shapesStroke.StrokeWidth,
                    Y = shapesStroke.StrokeWidth,
                    Height = lss,
                    Width = lss
                };
                w += 2 * shapesStroke.StrokeWidth;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }
            else if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeWidth,
                    Y = strokeClone.StrokeWidth,
                    Height = lss,
                    Width = lss
                };
                w += 2 * strokeClone.StrokeWidth;
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            context.Width = w;
            context.Height = w;

            paintContext = context;
        }

        public override int GetStackGroup() => 0;
    }
}
