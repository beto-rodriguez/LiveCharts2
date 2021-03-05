﻿// The MIT License(MIT)

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
    public abstract class StackedBarSeries<TModel, TVisual, TDrawingContext> : CartesianSeries<TModel, TVisual, TDrawingContext>, IStackedBarSeries<TDrawingContext>
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        protected static float pivot = 0;
        protected int stackGroup;

        public StackedBarSeries(SeriesProperties properties)
            : base(properties)
        {
            HoverState = LiveCharts.StackedBarSeriesHoverKey;
        }

        public int StackGroup { get => stackGroup; set => stackGroup = value; }

        public double MaxColumnWidth { get; set; } = 30;

        Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>? IStackedBarSeries<TDrawingContext>.OnPointCreated
        {
            get => OnPointCreated as Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointCreated = value;
        }

        Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>? IStackedBarSeries<TDrawingContext>.OnPointAddedToState
        {
            get => OnPointAddedToState as Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointAddedToState = value;
        }

        Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>? IStackedBarSeries<TDrawingContext>.OnPointRemovedFromState
        {
            get => OnPointRemovedFromState as Action<ISizedGeometry<TDrawingContext>, IChartView<TDrawingContext>>;
            set => OnPointRemovedFromState = value;
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

        public override int GetStackGroup() => stackGroup;
    }
}
