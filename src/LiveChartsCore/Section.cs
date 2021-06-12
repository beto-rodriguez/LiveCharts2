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
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a visual section in a chart.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="ChartElement{TDrawingContext}" />
    public abstract class Section<TDrawingContext> : ChartElement<TDrawingContext>, INotifyPropertyChanged
        where TDrawingContext : DrawingContext
    {
        private IPaintTask<TDrawingContext>? _stroke = null;
        private IPaintTask<TDrawingContext>? _fill = null;
        private double? _xi;
        private double? _xj;
        private double? _yi;
        private double? _yj;
        private int _scalesXAt;
        private int _scalesYAt;

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
        /// Gets or sets the xi, the value where the section starts at the X axis,
        /// set the property to null to indicate that the section must start at the beginning of the X axis, default is null.
        /// </summary>
        /// <value>
        /// The xi.
        /// </value>
        public double? Xi { get => _xi; set { _xi = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the xj, the value where the section ends and the X axis.
        /// set the property to null to indicate that the section must go to the end of the X axis, default is null.
        /// </summary>
        /// <value>
        /// The xj.
        /// </value>
        public double? Xj { get => _xj; set { _xj = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the yi, the value where the section starts and the Y axis.
        /// set the property to null to indicate that the section must start at the beginning of the Y axis, default is null.
        /// </summary>
        /// <value>
        /// The yi.
        /// </value>
        public double? Yi { get => _yi; set { _yi = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the yj, the value where the section ends and the Y axis.
        /// set the property to null to indicate that the section must go to the end of the Y axis, default is null.
        /// </summary>
        /// <value>
        /// The yj.
        /// </value>
        public double? Yj { get => _yj; set { _yj = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the axis index where the section is scaled in the X plane, the index must exist 
        /// in the <see cref="ICartesianChartView{TDrawingContext}.XAxes"/> collection.
        /// </summary>
        /// <value>
        /// The index of the axis.
        /// </value>
        public int ScalesXAt { get => _scalesXAt; set { _scalesXAt = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the axis index where the section is scaled in the Y plane, the index must exist 
        /// in the <see cref="ICartesianChartView{TDrawingContext}.YAxes"/> collection.
        /// </summary>
        /// <value>
        /// The index of the axis.
        /// </value>
        public int ScalesYAt { get => _scalesYAt; set { _scalesYAt = value; OnPropertyChanged(); } }

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
    /// Defines a visual section in a chart.
    /// </summary>
    /// <typeparam name="TSizedGeometry">The type of the sized geometry.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="ChartElement{TDrawingContext}" />
    public abstract class Section<TSizedGeometry, TDrawingContext> : Section<TDrawingContext>
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

            var cartesianChart = (CartesianChart<TDrawingContext>)chart;
            var primaryAxis = cartesianChart.YAxes[ScalesYAt];
            var secondaryAxis = cartesianChart.XAxes[ScalesXAt];

            var secondaryScale = new Scaler(drawLocation, drawMarginSize, secondaryAxis);
            var primaryScale = new Scaler(drawLocation, drawMarginSize, primaryAxis);

            if (Fill != null)
            {
                Fill.ZIndex = -0.5;

                if (_fillSizedGeometry == null)
                {
                    _fillSizedGeometry = new TSizedGeometry();


                }

                _fillSizedGeometry.X = drawLocation.X;
                _fillSizedGeometry.Y = drawLocation.Y;
                _fillSizedGeometry.Width = drawMarginSize.Width;
                _fillSizedGeometry.Height = drawMarginSize.Height;

                Fill.AddGeometryToPaintTask(chart.Canvas, _fillSizedGeometry);
                chart.Canvas.AddDrawableTask(Fill);
            }

            if (Stroke != null)
            {
                Stroke.ZIndex = -0.5;

                if (_strokeSizedGeometry == null)
                {
                    _strokeSizedGeometry = new TSizedGeometry();

                }

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
