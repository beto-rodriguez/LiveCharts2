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
using LiveChartsCore.Drawing.Common;
using System;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a chart data series.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="Series{TModel, TVisual, TLabel, TDrawingContext}" />
    /// <seealso cref="IChartSeries{TDrawingContext}" />
    public abstract class ChartSeries<TModel, TVisual, TLabel, TDrawingContext> : Series<TModel, TVisual, TLabel, TDrawingContext>, IChartSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        /// <summary>
        /// The paint context.
        /// </summary>
        protected CanvasSchedule<TDrawingContext> canvaSchedule = new();

        private double _legendShapeSize = 15;
        private IPaintTask<TDrawingContext>? _dataLabelsPaint;
        private double _dataLabelsSize = 16;
        private Padding _dataLabelsPadding = new() { Left = 6, Top = 8, Right = 6, Bottom = 8 };

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        protected ChartSeries(SeriesProperties properties) : base(properties) { }

        /// <inheritdoc cref="IChartSeries{TDrawingContext}.DataLabelsPaint"/>
        public IPaintTask<TDrawingContext>? DataLabelsPaint
        {
            get => _dataLabelsPaint;
            set => SetPaintProperty(ref _dataLabelsPaint, value);
        }

        /// <summary>
        /// Gets or sets the data labels drawable task.
        /// </summary>
        /// <value>
        /// The data labels drawable task.
        /// </value>
        [Obsolete("Renamed to DataLabelsPaint")]
        public IPaintTask<TDrawingContext>? DataLabelsDrawableTask { get => DataLabelsPaint; set => DataLabelsPaint = value; }

        /// <inheritdoc cref="IChartSeries{TDrawingContext}.DataLabelsSize"/>
        public double DataLabelsSize { get => _dataLabelsSize; set { _dataLabelsSize = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartSeries{TDrawingContext}.DataLabelsPadding"/>
        public Padding DataLabelsPadding { get => _dataLabelsPadding; set { _dataLabelsPadding = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IChartSeries{TDrawingContext}.CanvasSchedule"/>
        public CanvasSchedule<TDrawingContext> CanvasSchedule => canvaSchedule;

        /// <summary>
        /// Gets or sets the size of the legend shape.
        /// </summary>
        /// <value>
        /// The size of the legend shape.
        /// </value>
        public double LegendShapeSize { get => _legendShapeSize; set { _legendShapeSize = value; OnPropertyChanged(); } }

        /// <summary>
        /// Called when the paint context changed.
        /// </summary>
        /// <returns></returns>
        protected abstract void OnSeriesMiniatureChanged();

        /// <summary>
        /// Initializes the series.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Default colors are not valid</exception>
        protected void InitializeSeries()
        {
            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<TDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");

            initializer.ApplyStyleToSeries(this);

            var factory = LiveCharts.CurrentSettings.GetFactory<TDrawingContext>();
            dataProvider = factory.GetProvider<TModel>();
        }

        /// <inheritdoc cref="IChartSeries{TDrawingContext}.MiniatureEquals(IChartSeries{TDrawingContext})"/>
        public abstract bool MiniatureEquals(IChartSeries<TDrawingContext> instance);
    }
}
