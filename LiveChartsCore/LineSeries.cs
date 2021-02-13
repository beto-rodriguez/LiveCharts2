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
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the data to plot as a line.
    /// </summary>
    public class LineSeries<TModel, TVisual, TDrawingContext, TGeometryPath, TLineSegment, TBezierSegment, TMoveTo, TPathContext>
        : Series<TModel, TVisual, TDrawingContext>
        where TGeometryPath : IPathGeometry<TDrawingContext, TPathContext>, new()
        where TLineSegment : ILinePathSegment<TPathContext>, new()
        where TBezierSegment : IBezierSegment<TPathContext>, new()
        where TMoveTo : IMoveToPathCommand<TPathContext>, new()
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveTo, TPathContext> fillPathHelper =
            new AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveTo, TPathContext>();
        private readonly AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveTo, TPathContext> strokePathHelper =
            new AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveTo, TPathContext>();

        private double lineSmoothness = 0.65;
        private double geometrySize = 18d;
        private IDrawableTask<TDrawingContext> shapesFill;
        private IDrawableTask<TDrawingContext> shapesStroke;

        public LineSeries()
        {

        }

        public IDrawableTask<TDrawingContext> ShapesFill
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

        public IDrawableTask<TDrawingContext> ShapesStroke
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
        public TransitionsSetterDelegate<LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext>> TransitionsSetter { get; set; }
        public TransitionsSetterDelegate<AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveTo, TPathContext>> PathTransitionsSetter { get; set; }

        public override void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IDrawable<TDrawingContext>> drawBucket)
        {
            var drawLocation = view.Core.DrawMaringLocation;
            var drawMarginSize = view.Core.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            var gs = unchecked((float)geometrySize);
            var hgs = gs / 2f;
            float uw = xScale.ScaleToUi(1f) - xScale.ScaleToUi(0f);
            float huw = uw * 0.5f;
            float sw = Stroke?.StrokeWidth ?? 0;
            float p = yScale.ScaleToUi(unchecked((float)Pivot));

            var wasFillInitialized = false;
            var wasStrokeInitialized = false;

            var chartAnimation = new Animation(view.EasingFunction, view.AnimationsSpeed);
            var pts = PathTransitionsSetter ?? SetDefaultPathTransitions;
            if (Fill != null)
            {
                wasFillInitialized = fillPathHelper.Initialize(pts, chartAnimation);
                Fill.AddGeometyToPaintTask(fillPathHelper.Path);
                drawBucket.Add(fillPathHelper.Path);
                view.CoreCanvas.AddPaintTask(Fill);
                fillPathHelper.Path.ClearCommands();
            }
            if (Stroke != null)
            {
                wasStrokeInitialized = strokePathHelper.Initialize(pts, chartAnimation);
                Stroke.AddGeometyToPaintTask(strokePathHelper.Path);
                drawBucket.Add(strokePathHelper.Path);
                view.CoreCanvas.AddPaintTask(Stroke);
                strokePathHelper.Path.ClearCommands();
            }
            var ts = TransitionsSetter ?? SetDefaultTransitions;

            foreach (var data in GetSpline(xScale, yScale))
            {
                var x = xScale.ScaleToUi(data.TargetCoordinate.X);
                var y = yScale.ScaleToUi(data.TargetCoordinate.Y);

                if (data.TargetCoordinate.Visual == null)
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

                    data.TargetCoordinate.HoverArea = new HoverArea();
                    data.TargetCoordinate.Visual = v;

                    if (ShapesFill != null) ShapesFill.AddGeometyToPaintTask(v.Geometry);
                    if (ShapesStroke != null) ShapesStroke.AddGeometyToPaintTask(v.Geometry);
                }

                var visual = (LineSeriesVisualPoint<TDrawingContext, TVisual, TGeometryPath, TPathContext>)data.TargetCoordinate.Visual;

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

                data.TargetCoordinate.HoverArea.SetDimensions(x - huw, y - hgs - sw, uw, gs + 2 * sw);
                OnPointMeasured(data.TargetCoordinate, visual.Geometry);
                drawBucket.Add(visual.Geometry);
            }

            if (HighlightFill != null) view.CoreCanvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) view.CoreCanvas.AddPaintTask(HighlightStroke);
            if (ShapesFill != null) view.CoreCanvas.AddPaintTask(ShapesFill);
            if (ShapesStroke != null) view.CoreCanvas.AddPaintTask(ShapesStroke);
        }

        public override CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(controlSize, x, y);

            var tick = y.GetTick(controlSize, baseBounds.YAxisBounds);

            return new CartesianBounds
            {
                XAxisBounds = new Bounds
                {
                    Max = baseBounds.XAxisBounds.Max,
                    Min = baseBounds.XAxisBounds.Min
                },
                YAxisBounds = new Bounds
                {
                    Max = baseBounds.YAxisBounds.Max + tick.Value,
                    min = baseBounds.YAxisBounds.min - tick.Value
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

        private IEnumerable<BezierData> GetSpline(ScaleContext xScale, ScaleContext yScale)
        {
            var points = GetPonts().ToArray();

            if (points.Length == 0) yield break;
            ICartesianCoordinate previous, current, next, next2;

            for (int i = 0; i < points.Length; i++)
            {
                previous = points[i - 1 < 0 ? 0 : i - 1];
                current = points[i];
                next = points[i + 1 > points.Length - 1 ? points.Length - 1 : i + 1];
                next2 = points[i + 2 > points.Length - 1 ? points.Length - 1 : i + 2];

                var xc1 = (previous.X + current.X) / 2.0;
                var yc1 = (previous.Y + current.Y) / 2.0;
                var xc2 = (current.X + next.X) / 2.0;
                var yc2 = (current.Y + next.Y) / 2.0;
                var xc3 = (next.X + next2.X) / 2.0;
                var yc3 = (next.Y + next2.Y) / 2.0;

                var len1 = Math.Sqrt((current.X - previous.X) * (current.X - previous.X) + (current.Y - previous.Y) * (current.Y - previous.Y));
                var len2 = Math.Sqrt((next.X - current.X) * (next.X - current.X) + (next.Y - current.Y) * (next.Y - current.Y));
                var len3 = Math.Sqrt((next2.X - next.X) * (next2.X - next.X) + (next2.Y - next.Y) * (next2.Y - next.Y));

                var k1 = len1 / (len1 + len2);
                var k2 = len2 / (len2 + len3);

                if (double.IsNaN(k1)) k1 = 0d;
                if (double.IsNaN(k2)) k2 = 0d;

                var xm1 = xc1 + (xc2 - xc1) * k1;
                var ym1 = yc1 + (yc2 - yc1) * k1;
                var xm2 = xc2 + (xc3 - xc2) * k2;
                var ym2 = yc2 + (yc3 - yc2) * k2;

                var c1X = xm1 + (xc2 - xm1) * lineSmoothness + current.X - xm1;
                var c1Y = ym1 + (yc2 - ym1) * lineSmoothness + current.Y - ym1;
                var c2X = xm2 + (xc2 - xm2) * lineSmoothness + next.X - xm2;
                var c2Y = ym2 + (yc2 - ym2) * lineSmoothness + next.Y - ym2;

                unchecked
                {
                    float x0, y0;

                    if (i == 0)
                    {
                        x0 = current.X;
                        y0 = current.Y;
                    }
                    else
                    {
                        x0 = (float)c1X;
                        y0 = (float)c1Y;
                    }

                    yield return new BezierData
                    {
                        IsFirst = i == 0,
                        IsLast = i == points.Length - 1,
                        TargetCoordinate = points[i],
                        X0 = xScale.ScaleToUi(x0),
                        Y0 = yScale.ScaleToUi(y0),
                        X1 = xScale.ScaleToUi((float)c2X),
                        Y1 = yScale.ScaleToUi((float)c2Y),
                        X2 = xScale.ScaleToUi(next.X),
                        Y2 = yScale.ScaleToUi(next.Y)
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
            AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveTo, TPathContext> areaHelper, Animation defaultAnimation)
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
    }
}
