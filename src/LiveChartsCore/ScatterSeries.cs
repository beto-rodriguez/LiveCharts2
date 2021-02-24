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
namespace LiveChartsCore
{
    public class ScatterSeries<TModel, TVisual, TDrawingContext> : CartesianSeries<TModel, TVisual, TDrawingContext>
        where TVisual : class, ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private double geometrySize = 18d;

        public ScatterSeries()
            : base(SeriesProperties.Scatter)
        {

        }

        public double GeometrySize { get => geometrySize; set => geometrySize = value; }
        public TransitionsSetterDelegate<ISizedGeometry<TDrawingContext>>? TransitionsSetter { get; set; }

        public override void Measure(
            CartesianChartCore<TDrawingContext> chart, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis)
        {
            var drawLocation = chart.DrawMaringLocation;
            var drawMarginSize = chart.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            if (Fill != null) chart.Canvas.AddPaintTask(Fill);
            if (Stroke != null) chart.Canvas.AddPaintTask(Stroke);

            var gs = unchecked((float)geometrySize);
            var hgs = gs / 2f;
            float sw = Stroke?.StrokeWidth ?? 0;

            var chartAnimation = new Animation(chart.EasingFunction, chart.AnimationsSpeed);
            var ts = TransitionsSetter ?? SetDefaultTransitions;

            foreach (var point in Fetch(chart))
            {
                var x = xScale.ScaleToUi(point.SecondaryValue);
                var y = yScale.ScaleToUi(point.PrimaryValue);

                if (point.PointContext.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = x - hgs,
                        Y = y - hgs,
                        Width = 0,
                        Height = 0
                    };

                    ts(r, chartAnimation);

                    point.PointContext.Visual = r;
                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                }

                var sizedGeometry = point.PointContext.Visual;

                sizedGeometry.X = x - hgs;
                sizedGeometry.Y = y - hgs;
                sizedGeometry.Width = gs;
                sizedGeometry.Height =  gs;

                point.PointContext.HoverArea.SetDimensions(x - hgs, y - hgs, gs + 2 * sw, gs + 2 * sw);
                OnPointMeasured(point, sizedGeometry);
                chart.MeasuredDrawables.Add(sizedGeometry);
            }

            if (HighlightFill != null) chart.Canvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) chart.Canvas.AddPaintTask(HighlightStroke);
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
                    Max = baseBounds.SecondaryBounds.Max + tick.Value,
                    Min = baseBounds.SecondaryBounds.Min - tick.Value
                },
                PrimaryBounds = new Bounds
                {
                    Max = baseBounds.PrimaryBounds.Max + tick.Value,
                    min = baseBounds.PrimaryBounds.min - tick.Value
                }
            };
        }

        protected virtual void SetDefaultTransitions(ISizedGeometry<TDrawingContext> visual, Animation defaultAnimation)
        {
            visual
                .DefinePropertyTransitions(nameof(visual.X), nameof(visual.Y), nameof(visual.Width), nameof(visual.Height))
                .WithAnimation(defaultAnimation)
                .CompleteCurrentTransitions();
        }

        protected override void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = (float)LegendShapeSize, Width = (float)LegendShapeSize };
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            var w = LegendShapeSize;
            if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeWidth,
                    Y = strokeClone.StrokeWidth,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize
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
