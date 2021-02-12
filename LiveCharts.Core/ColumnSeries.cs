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

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the data to plot as columns.
    /// </summary>
    public class ColumnSeries<TModel, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
        where TDrawingContext: DrawingContext
    {
        public ColumnSeries()
        {

        }

        public double Pivot { get; set; }
        public double ColumnPadding { get; set; }

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
            var chartAnimation = new Animation(view.EasingFunction, view.AnimationsSpeed);

            float uw = xScale.ScaleToUi(1f) - xScale.ScaleToUi(0f);
            float uwm = 0.5f * uw;
            float sw = Stroke?.StrokeWidth ?? 0;
            float p = yScale.ScaleToUi(unchecked((float)Pivot));
            float colPad = unchecked((float)ColumnPadding);

            if (Fill != null) view.CoreCanvas.AddPaintTask(Fill);
            if (Stroke != null) view.CoreCanvas.AddPaintTask(Stroke);

            var ts = transitionSetter ?? SetDefaultTransitions;

            foreach (var point in GetPonts())
            {
                var x = xScale.ScaleToUi(point.X);
                var y = yScale.ScaleToUi(point.Y);
                float b = Math.Abs(y - p);

                if (point.Visual == null)
                {
                    var r = new TVisual
                    {
                        X = x - uwm,
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

                var rectangle = (TVisual)point.Visual;

                var cy = point.Y > Pivot ? y : y - b;

                rectangle.X = x - uwm;
                rectangle.Y = cy;
                rectangle.Width = uw;
                rectangle.Height = b;

                OnPointMeasured(point, rectangle);
                drawBucket.Add(rectangle);
            }

            if (HighlightFill != null) view.CoreCanvas.AddPaintTask(HighlightFill);
            if (HighlightStroke != null) view.CoreCanvas.AddPaintTask(HighlightStroke);
        }

        public override CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            var baseBounds = base.GetBounds(controlSize, x, y);

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
                    min = baseBounds.YAxisBounds.min - tick.Value
                }
            };
        }

        protected override void SetDefaultTransitions(ISizedGeometry<TDrawingContext> geometry, Animation defaultAnimation)
        {
            var defaultProperties = new string[]
            {
                    nameof(ISizedGeometry<TDrawingContext>.X),
                    nameof(ISizedGeometry<TDrawingContext>.Width)
            };
            geometry.SetPropertyTransition(defaultAnimation, defaultProperties);
            geometry.CompleteTransition(defaultProperties);

            var bounceProperties = new string[]
            {
                nameof(ISizedGeometry<TDrawingContext>.Y),
                nameof(ISizedGeometry<TDrawingContext>.Height),
            };

            geometry.SetPropertyTransition(
                new Animation(EasingFunctions.BounceOut, defaultAnimation.duration * 2, defaultAnimation.RepeatTimes),
                bounceProperties);
            geometry.CompleteTransition(bounceProperties);
        }
    }
}
