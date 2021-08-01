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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines a visual element in a chart.
    /// </summary>
    public abstract class ChartElement<TDrawingContext> : IChartElement<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly List<IPaintTask<TDrawingContext>> _deletingTasks = new();

        /// <inheritdoc cref="IChartElement{TDrawingContext}.Measure(Chart{TDrawingContext})" />
        public abstract void Measure(Chart<TDrawingContext> chart);

        /// <inheritdoc cref="IChartElement{TDrawingContext}.RemoveOldPaints(IChartView{TDrawingContext})" />
        public void RemoveOldPaints(IChartView<TDrawingContext> chart)
        {
            if (_deletingTasks.Count == 0) return;

            foreach (var item in _deletingTasks)
            {
                chart.CoreCanvas.RemovePaintTask(item);
                item.Dispose();
            }

            _deletingTasks.Clear();
        }

        /// <inheritdoc cref="IChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})" />
        public virtual void RemoveFromUI(Chart<TDrawingContext> chart)
        {
            foreach (var item in GetPaintTasks())
            {
                if (item is null) continue;
                chart.Canvas.RemovePaintTask(item);
                item.ClearGeometriesFromPaintTask(chart.Canvas);
            }
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="reference">The referenced paint task.</param>
        /// <param name="value">The value</param>
        /// <param name="isStroke">if set to <c>true</c> [is stroke].</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected virtual void SetPaintProperty(
            ref IPaintTask<TDrawingContext>? reference,
            IPaintTask<TDrawingContext>? value,
            bool isStroke = false,
            [CallerMemberName] string? propertyName = null)
        {
            if (reference is not null) ScheduleDeleteFor(reference);
            reference = value;
            if (reference is not null)
            {
                reference.IsStroke = isStroke;
                reference.IsFill = !isStroke; // seems unnecessary ????
                if (!isStroke) reference.StrokeThickness = 0;
            }

            OnPaintChanged(propertyName);
        }

        /// <summary>
        /// Schedules the delete for thew given <see cref="IPaintTask{TDrawingContext}"/> instance.
        /// </summary>
        /// <returns></returns>
        protected void ScheduleDeleteFor(IPaintTask<TDrawingContext> paintTask)
        {
            _deletingTasks.Add(paintTask);
        }

        /// <summary>
        /// Called when the fill changes.
        /// </summary>
        /// <returns></returns>
        protected virtual void OnPaintChanged(string? propertyName) { }

        /// <summary>
        /// Gets the paint tasks.
        /// </summary>
        /// <returns></returns>
        protected abstract IPaintTask<TDrawingContext>?[] GetPaintTasks();
    }
}
