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
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a data series that has at least a <see cref="IPaintTask{TDrawingContext}"/> to draw the data in the user interface.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="Series{TModel, TVisual, TLabel, TDrawingContext}" />
    /// <seealso cref="IDrawableSeries{TDrawingContext}" />
    public abstract class DrawableSeries<TModel, TVisual, TLabel, TDrawingContext>
        : Series<TModel, TVisual, TLabel, TDrawingContext>, IDrawableSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        /// <summary>
        /// The paint context.
        /// </summary>
        protected PaintContext<TDrawingContext> paintContext = new();

        /// <summary>
        /// The pending to delete tasks.
        /// </summary>
        protected List<IPaintTask<TDrawingContext>> deletingTasks = new();
        private IPaintTask<TDrawingContext>? _stroke = null;
        private IPaintTask<TDrawingContext>? _fill = null;
        private double _legendShapeSize = 15;
        private IPaintTask<TDrawingContext>? _dataLabelsPaint;
        private double _dataLabelsSize = 16;
        private Padding _dataLabelsPadding = new() { Left = 6, Top = 8, Right = 6, Bottom = 8 };

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawableSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public DrawableSeries(SeriesProperties properties) : base(properties)
        {
        }

        List<IPaintTask<TDrawingContext>> IDrawableSeries<TDrawingContext>.DeletingTasks => deletingTasks;

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
        public IPaintTask<TDrawingContext>? Stroke
        {
            get => _stroke;
            set
            {
                if (_stroke != null) deletingTasks.Add(_stroke);
                _stroke = value;
                if (_stroke != null)
                {
                    _stroke.IsStroke = true;
                    _stroke.IsFill = false;
                }
                OnPaintContextChanged();
                OnPropertyChanged();
            }
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
            set
            {
                if (_fill != null) deletingTasks.Add(_fill);
                _fill = value;
                if (_fill != null)
                {
                    _fill.IsStroke = false;
                    _fill.IsFill = true;
                    _fill.StrokeThickness = 0;
                }
                OnPaintContextChanged();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the data labels drawable task.
        /// </summary>
        /// <value>
        /// The data labels drawable task.
        /// </value>
        public IPaintTask<TDrawingContext>? DataLabelsPaint
        {
            get => _dataLabelsPaint;
            set
            {
                if (_dataLabelsPaint != null) deletingTasks.Add(_dataLabelsPaint);
                _dataLabelsPaint = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the data labels drawable task.
        /// </summary>
        /// <value>
        /// The data labels drawable task.
        /// </value>
        [Obsolete("Renamed to DataLabelsPaint")]
        public IPaintTask<TDrawingContext>? DataLabelsDrawableTask { get => DataLabelsPaint; set => DataLabelsPaint = value; }

        /// <summary>
        /// Gets or sets the size of the data labels.
        /// </summary>
        /// <value>
        /// The size of the data labels.
        /// </value>
        public double DataLabelsSize { get => _dataLabelsSize; set { _dataLabelsSize = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the data labels padding.
        /// </summary>
        /// <value>
        /// The data labels padding.
        /// </value>
        public Padding DataLabelsPadding { get => _dataLabelsPadding; set { _dataLabelsPadding = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets the default paint context.
        /// </summary>
        /// <value>
        /// The default paint context.
        /// </value>
        public PaintContext<TDrawingContext> DefaultPaintContext => paintContext;

        /// <summary>
        /// Gets or sets the size of the legend shape.
        /// </summary>
        /// <value>
        /// The size of the legend shape.
        /// </value>
        public double LegendShapeSize { get => _legendShapeSize; set { _legendShapeSize = value; OnPropertyChanged(); } }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <returns></returns>
        public override void Dispose()
        {
            foreach (var chart in subscribedTo)
            {
                var c = (Chart<TDrawingContext>)chart;
                if (_fill != null) c.Canvas.RemovePaintTask(_fill);
                if (_stroke != null) c.Canvas.RemovePaintTask(_stroke);
            }

            base.Dispose();
        }

        /// <summary>
        /// Called when the paint context changed.
        /// </summary>
        /// <returns></returns>
        protected abstract void OnPaintContextChanged();

        /// <summary>
        /// Initializes the series.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Default colors are not valid</exception>
        protected void InitializeSeries()
        {
            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<TDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");

            initializer.ApplyStyleToSeries(this);

            var factory = LiveCharts.CurrentSettings.GetFactory<TDrawingContext>();
            dataProvider = factory.GetProvider<TModel>();
        }
    }
}
