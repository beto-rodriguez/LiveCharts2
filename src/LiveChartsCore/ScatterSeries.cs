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
using System.Collections.Generic;

namespace LiveChartsCore
{
    public class ScatterSeries<TModel, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private double geometrySize = 18d;

        public ScatterSeries()
            : base(SeriesProperties.Scatter)
        {

        }

        public double GeometrySize { get => geometrySize; set => geometrySize = value; }
        public TransitionsSetterDelegate<ISizedGeometry<TDrawingContext>> TransitionsSetter { get; set; }

        public override void Measure(
            IChartView<TDrawingContext> view, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis, SeriesContext<TDrawingContext> context, HashSet<IDrawable<TDrawingContext>> drawBucket)
        {
            var drawLocation = view.Core.DrawMaringLocation;
            var drawMarginSize = view.Core.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            if (Fill != null) view.CoreCanvas.AddPaintTask(Fill);
            if (Stroke != null) view.CoreCanvas.AddPaintTask(Stroke);

            var gs = unchecked((float)geometrySize);
            var hgs = gs / 2f;
            float sw = Stroke?.StrokeWidth ?? 0;

            var chartAnimation = new Animation(view.EasingFunction, view.AnimationsSpeed);
            var ts = TransitionsSetter ?? SetDefaultTransitions;

            foreach (var point in GetPonts())
            {
                var x = xScale.ScaleToUi(point.X);
                var y = yScale.ScaleToUi(point.Y);

                if (point.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = x - hgs,
                        Y = y - hgs,
                        Width = 0,
                        Height = 0
                    };

                    ts(r, chartAnimation);

                    point.HoverArea = new HoverArea();
                    point.Visual = r;
                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                }

                var sizedGeometry = (TVisual)point.Visual;

                sizedGeometry.X = x - hgs;
                sizedGeometry.Y = y - hgs;
                sizedGeometry.Width = gs;
                sizedGeometry.Height =  gs;

                point.HoverArea.SetDimensions(x - hgs, y - hgs, gs + 2 * sw, gs + 2 * sw);
                OnPointMeasured(point, sizedGeometry);
                drawBucket.Add(sizedGeometry);
            }

            if (HighlightFill != null) view.CoreCanvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) view.CoreCanvas.AddPaintTask(HighlightStroke);
        }

        public override CartesianBounds GetBounds(
            System.Drawing.SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y, SeriesContext<TDrawingContext> context)
        {
            var baseBounds = base.GetBounds(controlSize, x, y, context);

            var tick = y.GetTick(controlSize, baseBounds.YAxisBounds);

            return new CartesianBounds
            {
                XAxisBounds = new Bounds
                {
                    Max = baseBounds.XAxisBounds.Max + tick.Value,
                    Min = baseBounds.XAxisBounds.Min - tick.Value
                },
                YAxisBounds = new Bounds
                {
                    Max = baseBounds.YAxisBounds.Max + tick.Value,
                    min = baseBounds.YAxisBounds.min - tick.Value
                }
            };
        }

        protected virtual void SetDefaultTransitions(ISizedGeometry<TDrawingContext> visual, Animation defaultAnimation)
        {
            var defaultProperties = new string[]
            {
                nameof(visual.X),
                nameof(visual.Y),
                nameof(visual.Width),
                nameof(visual.Height),
            };
            visual.SetPropertyTransition(defaultAnimation, defaultProperties);
            visual.CompleteTransition(defaultProperties);
        }

        public override int GetStackGroup() => 0;
    }
}
