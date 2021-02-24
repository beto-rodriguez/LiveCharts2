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

namespace LiveChartsCore
{
    public class PieSeries<TModel, TVisual, TDrawingContext> : DrawableSeries<TModel, TVisual, TDrawingContext>, IDisposable, IPieSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IDoughnutGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
    {
        private Bounds bounds = new Bounds();
        private Stacker<TDrawingContext> stacker = new Stacker<TDrawingContext>();

        public PieSeries() : base(SeriesProperties.PieSeries) { }

        public TransitionsSetterDelegate<IDoughnutGeometry<TDrawingContext>>? TransitionsSetter { get; set; }

        public void Measure(PieChart<TDrawingContext> chart)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var minDimension = drawMarginSize.Width < drawMarginSize.Height ? drawMarginSize.Width : drawMarginSize.Height;

            if (Fill != null) chart.Canvas.AddPaintTask(Fill);
            if (Stroke != null) chart.Canvas.AddPaintTask(Stroke);

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);
            var ts = TransitionsSetter ?? SetDefaultTransitions;

            var cx = drawLocation.X + drawMarginSize.Width * 0.5f;
            var cy = drawLocation.Y + drawMarginSize.Height * 0.5f;

            var stackPosition = GetStackGroup();

            foreach (var point in Fetch(chart))
            {
                if (point.PointContext.Visual == null)
                {
                    var p = new TVisual 
                    {
                        X = cx,
                        Y = cy,
                        Width = 0,
                        Height = 0,
                        SweepAngle = 0,
                        StartAngle = 0
                    };

                    ts(p, chartAnimation);

                    point.PointContext.Visual = p;
                    if (Fill != null) Fill.AddGeometyToPaintTask(p);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(p);
                }

                var dougnutGeometry = point.PointContext.Visual;

                var s = stacker.GetStack(point, stackPosition);

                dougnutGeometry.X = (drawMarginSize.Width - minDimension) * 0.5f;
                dougnutGeometry.Y = (drawMarginSize.Height - minDimension) * 0.5f;
                dougnutGeometry.Width = minDimension;
                dougnutGeometry.Height = minDimension;
                var start = (s.Start / s.Total) * 360;
                var end = (s.End / s.Total) * 360;
                dougnutGeometry.StartAngle = start;
                dougnutGeometry.SweepAngle = end;

                point.PointContext.HoverArea = new SemicircleHoverArea().SetDimensions(cx, cy, start, end, minDimension * 0.5f);
                OnPointMeasured(point, dougnutGeometry);
                chart.MeasuredDrawables.Add(dougnutGeometry);
            }
        }

        public Bounds GetBounds(PieChart<TDrawingContext> chart)
        {
            bounds = new Bounds();
            stacker = new Stacker<TDrawingContext>();
            var stackPosition = GetStackGroup();

            foreach (var point in Fetch(chart))
            {
                var secondary = point.SecondaryValue;
                var primary = stacker.StackPoint(point, stackPosition);

                bounds.AppendValue(secondary);
                bounds.AppendValue(primary);
            }

            return bounds;
        }

        protected virtual void SetDefaultTransitions(IDoughnutGeometry<TDrawingContext> visual, Animation defaultAnimation)
        {
            visual
                .DefinePropertyTransitions(
                    nameof(visual.X), nameof(visual.Y), nameof(visual.Width), nameof(visual.Height),
                    nameof(visual.StartAngle), nameof(visual.SweepAngle))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();
        }

        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            //if (Fill != null)
            //{
            //    var fillClone = Fill.CloneTask();
            //    var visual = new TVisual { X = 0, Y = 0, Height = (float)LegendShapeSize, Width = (float)LegendShapeSize };
            //    fillClone.AddGeometyToPaintTask(visual);
            //    context.PaintTasks.Add(fillClone);
            //}

            //var w = LegendShapeSize;
            //if (Stroke != null)
            //{
            //    var strokeClone = Stroke.CloneTask();
            //    var visual = new TVisual
            //    {
            //        X = strokeClone.StrokeWidth,
            //        Y = strokeClone.StrokeWidth,
            //        Height = (float)LegendShapeSize,
            //        Width = (float)LegendShapeSize
            //    };
            //    w += 2 * strokeClone.StrokeWidth;
            //    strokeClone.AddGeometyToPaintTask(visual);
            //    context.PaintTasks.Add(strokeClone);
            //}

            //context.Width = w;
            //context.Height = w;

            paintContext = context;
        }

        // for now multiple stack levels at a pie chart is not supported.
        public override int GetStackGroup() => 0;
    }
}
