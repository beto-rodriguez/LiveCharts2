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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using System;
using LiveChartsCore.Measure;
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a data series that has at least a <see cref="IDrawableTask{TDrawingContext}"/> to draw the data in the user interface.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="LiveChartsCore.Series{TModel, TVisual, TLabel, TDrawingContext}" />
    /// <seealso cref="LiveChartsCore.Kernel.IDrawableSeries{TDrawingContext}" />
    public abstract class DrawableSeries<TModel, TVisual, TLabel, TDrawingContext>
        : Series<TModel, TVisual, TLabel, TDrawingContext>, IDrawableSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        protected PaintContext<TDrawingContext> paintContext = new();
        private IDrawableTask<TDrawingContext>? stroke = null;
        private IDrawableTask<TDrawingContext>? fill = null;
        private double legendShapeSize = 15;
        private IDrawableTask<TDrawingContext>? dataLabelsDrawableTask;
        private double dataLabelsSize = 16;
        private DataLabelsPosition dataLabelsPosition;
        private Padding dataLabelsPadding = new Padding { Left = 6, Top = 8, Right = 6, Bottom = 8 };
        protected List<IDrawableTask<TDrawingContext>> deletingTasks = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawableSeries{TModel, TVisual, TLabel, TDrawingContext}"/> class.
        /// </summary>
        /// <param name="properties">The properties.</param>
        public DrawableSeries(SeriesProperties properties) : base(properties)
        {
        }

        List<IDrawableTask<TDrawingContext>> IDrawableSeries<TDrawingContext>.DeletingTasks => deletingTasks;

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
        public IDrawableTask<TDrawingContext>? Stroke
        {
            get => stroke;
            set
            {
                if (stroke != null) deletingTasks.Add(stroke);
                stroke = value;
                if (stroke != null)
                {
                    stroke.IsStroke = true;
                    stroke.IsFill = false;
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
        public IDrawableTask<TDrawingContext>? Fill
        {
            get => fill;
            set
            {
                if (fill != null) deletingTasks.Add(fill);
                fill = value;
                if (fill != null)
                {
                    fill.IsStroke = false;
                    fill.IsFill = true;
                    fill.StrokeThickness = 0;
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
        public IDrawableTask<TDrawingContext>? DataLabelsDrawableTask
        {
            get => dataLabelsDrawableTask;
            set
            {
                if (dataLabelsDrawableTask != null) deletingTasks.Add(dataLabelsDrawableTask);
                dataLabelsDrawableTask = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the size of the data labels.
        /// </summary>
        /// <value>
        /// The size of the data labels.
        /// </value>
        public double DataLabelsSize { get => dataLabelsSize; set { dataLabelsSize = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the data labels position.
        /// </summary>
        /// <value>
        /// The data labels position.
        /// </value>
        public DataLabelsPosition DataLabelsPosition { get => dataLabelsPosition; set { dataLabelsPosition = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the data labels padding.
        /// </summary>
        /// <value>
        /// The data labels padding.
        /// </value>
        public Padding DataLabelsPadding { get => dataLabelsPadding; set { dataLabelsPadding = value; OnPropertyChanged(); } }

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
        public double LegendShapeSize { get => legendShapeSize; set { legendShapeSize = value; OnPropertyChanged(); } }

        public override void Dispose()
        {
            foreach (var chart in subscribedTo)
            {
                var c = (Chart<TDrawingContext>)chart;
                if (fill != null) c.Canvas.RemovePaintTask(fill);
                if (stroke != null) c.Canvas.RemovePaintTask(stroke);
            }

            base.Dispose();
        }

        protected abstract void OnPaintContextChanged();

        protected void InitializeSeries()
        {
            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<TDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");

            initializer.ConstructSeries(this);

            var factory = LiveCharts.CurrentSettings.GetFactory<TDrawingContext>();
            dataProvider = factory.GetProvider<TModel>();
        }
    }
}
