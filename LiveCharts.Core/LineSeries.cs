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
    public class LineSeries<TModel, TPath, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TPath: IPathGeometry<TDrawingContext>, new()
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private IPathGeometry<TDrawingContext> fillPath;
        private IPathGeometry<TDrawingContext> strokePath;
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

        public override void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IGeometry<TDrawingContext>> drawBucket)
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
            //float p = view.Core.DrawMaringLocation.Y + view.Core.DrawMarginSize.Height;
            float p = yScale.ScaleToUi(unchecked((float)Pivot));

            if (Fill != null)
            {
                if (fillPath != null) Fill.RemoveGeometryFromPainTask(fillPath);
                fillPath = new TPath();
                Fill.AddGeometyToPaintTask(fillPath);
                drawBucket.Add(fillPath);
                view.CoreCanvas.AddPaintTask(Fill);
            }
            if (Stroke != null)
            {
                if (strokePath != null) Stroke.RemoveGeometryFromPainTask(strokePath);
                strokePath = new TPath();
                Stroke.AddGeometyToPaintTask(strokePath);
                drawBucket.Add(strokePath);
                view.CoreCanvas.AddPaintTask(Stroke);
            }

            foreach (var data in GetSpline(xScale, yScale))
            {
                var x = xScale.ScaleToUi(data.TargetCoordinate.X);
                var y = yScale.ScaleToUi(data.TargetCoordinate.Y);

                if (data.TargetCoordinate.Visual == null)
                {
                    var v = new LineSeriesVisualPoint<TDrawingContext, TVisual> { Geometry = new TVisual(), Bezier = data };
                    v.Geometry.X = x - hgs;
                    v.Geometry.Y = y - hgs;
                    v.Geometry.Width = gs;
                    v.Geometry.Height = gs;

                    data.TargetCoordinate.HoverArea = new HoverArea();
                    data.TargetCoordinate.Visual = v;
                    if (ShapesFill != null) ShapesFill.AddGeometyToPaintTask(v.Geometry);
                    if (ShapesStroke != null) ShapesStroke.AddGeometyToPaintTask(v.Geometry);
                }

                var visual = (LineSeriesVisualPoint<TDrawingContext, TVisual>)data.TargetCoordinate.Visual;

                if (Fill != null)
                {
                    if (data.IsFirst) 
                    { 
                        fillPath.MoveTo(data.X0, p);
                        fillPath.LineTo(data.X0, data.Y0); 
                    }
                    fillPath.CubicBezierTo(data.X0, data.Y0, data.X1, data.Y1, data.X2, data.Y2);
                    if (data.IsLast)
                    {
                        fillPath.LineTo(data.X0, p);
                    }
                }
                if (Stroke != null)
                {
                    if (data.IsFirst) strokePath.MoveTo(data.X0, data.Y0);
                    strokePath.CubicBezierTo(data.X0, data.Y0, data.X1, data.Y1, data.X2, data.Y2);
                }

                visual.Geometry.X = x - hgs;
                visual.Geometry.Y = y - hgs;
                visual.Geometry.Width = gs;
                visual.Geometry.Height = gs;

                data.TargetCoordinate.HoverArea.SetDimensions(x - huw, y - hgs - sw, uw, gs + 2*sw);
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
                        IsLast = i == points.Length -1,
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
            } else if (Fill != null)
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
            } else if (Stroke != null)
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
