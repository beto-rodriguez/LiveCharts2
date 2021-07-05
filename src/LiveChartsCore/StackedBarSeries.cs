// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Kernel.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the stacked bar series class.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="CartesianSeries{TModel, TVisual, TLabel, TDrawingContext}" />
    /// <seealso cref="IStackedBarSeries{TDrawingContext}" />
    public abstract class StackedBarSeries<TModel, TVisual, TLabel, TDrawingContext>
        : StrokeAndFillCartesianSeries<TModel, TVisual, TLabel, TDrawingContext>, IStackedBarSeries<TDrawingContext>
        where TVisual : class, IRoundedRectangleChartPoint<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        /// <summary>
        /// The stack group
        /// </summary>
        protected int stackGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackedBarSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        /// <param name="properties">The series properties.</param>
        protected StackedBarSeries(SeriesProperties properties)
            : base(properties)
        {
            HoverState = LiveCharts.StackedBarSeriesHoverKey;
        }

        /// <inheritdoc cref="IBarSeries{TDrawingContext}.GroupPadding"/>
        public double GroupPadding { get; set; } = 10;

        /// <inheritdoc cref="IStackedBarSeries{TDrawingContext}.StackGroup"/>
        public int StackGroup { get => stackGroup; set => stackGroup = value; }

        /// <inheritdoc cref="IStackedBarSeries{TDrawingContext}.MaxBarWidth"/>
        public double MaxBarWidth { get; set; } = 50;

        /// <inheritdoc cref="IStackedBarSeries{TDrawingContext}.Rx"/>
        public double Rx { get; set; }

        /// <inheritdoc cref="IStackedBarSeries{TDrawingContext}.Ry"/>
        public double Ry { get; set; }

        /// <summary>
        /// Called when the paint context changed.
        /// </summary>
        protected override void OnSeriesMiniatureChanged()
        {
            var context = new CanvasSchedule<TDrawingContext>();

            var w = LegendShapeSize;
            var sh = 0f;
            if (Stroke is not null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeThickness,
                    Y = strokeClone.StrokeThickness,
                    Height = (float)LegendShapeSize,
                    Width = (float)LegendShapeSize
                };
                sh = strokeClone.StrokeThickness;
                strokeClone.ZIndex = 1;
                w += 2 * strokeClone.StrokeThickness;
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(strokeClone, visual));
            }

            if (Fill is not null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = sh, Y = sh, Height = (float)LegendShapeSize, Width = (float)LegendShapeSize };
                context.PaintSchedules.Add(new PaintSchedule<TDrawingContext>(fillClone, visual));
            }

            context.Width = w;
            context.Height = w;

            canvaSchedule = context;
            OnPropertyChanged(nameof(CanvasSchedule));
        }

        /// <summary>
        /// Gets the stack group.
        /// </summary>
        /// <returns></returns>
        /// <inheritdoc />
        public override int GetStackGroup()
        {
            return stackGroup;
        }
    }
}
