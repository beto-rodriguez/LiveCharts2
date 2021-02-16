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
    public class StackedColumnSeries<TModel, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly static float pivot = 0;
        private int stackGroup;

        public StackedColumnSeries()
            : base(SeriesType.StackedColumn, SeriesDirection.Vertical, true)
        {

        }

        public int StackGroup { get => stackGroup; set => stackGroup = value; }
        public double MaxColumnWidth { get; set; } = 30;
        public TransitionsSetterDelegate<ISizedGeometry<TDrawingContext>> TransitionsSetter { get; set; }

        public override void Measure(IChartView<TDrawingContext> view, IAxis<TDrawingContext> xAxis, IAxis<TDrawingContext> yAxis, SeriesContext<TDrawingContext> context, HashSet<IDrawable<TDrawingContext>> drawBucket)
        {
            var drawLocation = view.Core.DrawMaringLocation;
            var drawMarginSize = view.Core.DrawMarginSize;
            var xScale = new ScaleContext(drawLocation, drawMarginSize, xAxis.Orientation, xAxis.DataBounds);
            var yScale = new ScaleContext(drawLocation, drawMarginSize, yAxis.Orientation, yAxis.DataBounds);

            float uw = xScale.ScaleToUi(1f) - xScale.ScaleToUi(0f);
            float uwm = 0.5f * uw;
            float sw = Stroke?.StrokeWidth ?? 0;
            float p = yScale.ScaleToUi(pivot);

            var pos = context.GetStackedColumnPostion(this);
            var count = context.GetStackedColumnSeriesCount();
            float cp = 0f;

            if (count > 1)
            {
                uw = uw / count;
                uwm = 0.5f * uw;
                cp = (pos - (count / 2f)) * uw + uwm;
            }

            if (uw > MaxColumnWidth)
            {
                uw = unchecked((float)MaxColumnWidth);
                uwm = uw / 2f;
            }

            if (Fill != null) view.CoreCanvas.AddPaintTask(Fill);
            if (Stroke != null) view.CoreCanvas.AddPaintTask(Stroke);

            var chartAnimation = new Animation(view.EasingFunction, view.AnimationsSpeed);
            var ts = TransitionsSetter ?? SetDefaultTransitions;

            var stacker = context.GetStackPosition(this, GetStackGroup());

            foreach (var point in GetPonts())
            {
                var x = xScale.ScaleToUi(point.X);

                if (point.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = x - uwm + cp,
                        Y = p,
                        Width = uw,
                        Height = 0
                    };

                    ts(r, chartAnimation);

                    point.HoverArea = new HoverArea();
                    point.Visual = r;
                    if (Fill != null) Fill.AddGeometyToPaintTask(r);
                    if (Stroke != null) Stroke.AddGeometyToPaintTask(r);
                }

                var sizedGeometry = (TVisual)point.Visual;

                var sy = stacker.GetStack(point);
                var yi = yScale.ScaleToUi(sy.Start);
                var yj = yScale.ScaleToUi(sy.End);

                sizedGeometry.X = x - uwm + cp;
                sizedGeometry.Y = yj;
                sizedGeometry.Width = uw;
                sizedGeometry.Height = yi - yj;

                point.HoverArea.SetDimensions(x - uwm + cp, yj, uw, yi - yj);
                OnPointMeasured(point, sizedGeometry);
                drawBucket.Add(sizedGeometry);
            }
        }

        public override CartesianBounds GetBounds(
            System.Drawing.SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y, SeriesContext<TDrawingContext> seriesContext)
        {
            var baseBounds = base.GetBounds(controlSize, x, y, seriesContext);

            var tick = y.GetTick(controlSize, baseBounds.YAxisBounds);

            return new CartesianBounds
            {
                XAxisBounds = new Bounds
                {
                    Max = baseBounds.XAxisBounds.Max + 0.5,
                    Min = baseBounds.XAxisBounds.Min - 0.5
                },
                YAxisBounds = new Bounds
                {
                    Max = baseBounds.YAxisBounds.Max + tick.Value,
                    min = 0
                }
            };
        }

        protected virtual void SetDefaultTransitions(ISizedGeometry<TDrawingContext> visual, Animation defaultAnimation)
        {
            var defaultProperties = new string[]
            {
                nameof(visual.X),
                nameof(visual.Width)
            };
            visual.SetPropertyTransition(defaultAnimation, defaultProperties);
            visual.CompleteTransition(defaultProperties);

            var bounceProperties = new string[]
            {
                nameof(visual.Y),
                nameof(visual.Height),
            };
            visual.SetPropertyTransition(
                new Animation(EasingFunctions.BounceOut, (long)(defaultAnimation.duration * 1.5), defaultAnimation.RepeatTimes),
                bounceProperties);
            visual.CompleteTransition(bounceProperties);
        }

        public override int GetStackGroup() => stackGroup;
    }
}
