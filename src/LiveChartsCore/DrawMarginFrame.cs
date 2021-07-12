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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a draw margin frame visual in a chart.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    public abstract class DrawMarginFrame<TDrawingContext> : ChartElement<TDrawingContext>, INotifyPropertyChanged
        where TDrawingContext : DrawingContext
    {
        private IPaintTask<TDrawingContext>? _stroke = null;
        private IPaintTask<TDrawingContext>? _fill = null;

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
        public IPaintTask<TDrawingContext>? Stroke
        {
            get => _stroke;
            set => SetPaintProperty(ref _stroke, value, true);
        }

        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
        public IPaintTask<TDrawingContext>? Fill
        {
            get => _fill;
            set => SetPaintProperty(ref _fill, value);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the paint tasks.
        /// </summary>
        /// <returns></returns>
        protected override IPaintTask<TDrawingContext>?[] GetPaintTasks()
        {
            return new[] { _stroke, _fill };
        }

        /// <summary>
        /// Called when the fill changes.
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPaintChanged(string? propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Called when a property changes.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Defines a draw margin frame visual in a chart.
    /// </summary>
    /// <typeparam name="TSizedGeometry">The type of the sized geometry.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    public abstract class DrawMarginFrame<TSizedGeometry, TDrawingContext> : DrawMarginFrame<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TSizedGeometry : ISizedGeometry<TDrawingContext>, new()
    {
        private TSizedGeometry? _fillSizedGeometry;
        private TSizedGeometry? _strokeSizedGeometry;

        /// <summary>
        /// Measures the specified chart.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public override void Measure(Chart<TDrawingContext> chart)
        {
            var drawLocation = chart.DrawMarginLocation;
            var drawMarginSize = chart.DrawMarginSize;

            if (Fill is not null)
            {
                Fill.ZIndex = -3;

                _fillSizedGeometry ??= new TSizedGeometry();

                _fillSizedGeometry.X = drawLocation.X;
                _fillSizedGeometry.Y = drawLocation.Y;
                _fillSizedGeometry.Width = drawMarginSize.Width;
                _fillSizedGeometry.Height = drawMarginSize.Height;

                Fill.AddGeometryToPaintTask(chart.Canvas, _fillSizedGeometry);
                chart.Canvas.AddDrawableTask(Fill);
            }

            if (Stroke is not null)
            {
                Stroke.ZIndex = -2;

                _strokeSizedGeometry ??= new TSizedGeometry();

                _strokeSizedGeometry.X = drawLocation.X;
                _strokeSizedGeometry.Y = drawLocation.Y;
                _strokeSizedGeometry.Width = drawMarginSize.Width;
                _strokeSizedGeometry.Height = drawMarginSize.Height;

                Stroke.AddGeometryToPaintTask(chart.Canvas, _strokeSizedGeometry);
                chart.Canvas.AddDrawableTask(Stroke);
            }
        }
    }
}
