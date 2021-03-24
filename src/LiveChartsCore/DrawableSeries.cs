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
        private Func<ChartPoint, string> dataLabelFormatter = (point) => $"{point.PrimaryValue}";
        protected List<IDrawableTask<TDrawingContext>> deletingTasks = new ();

        public DrawableSeries(SeriesProperties properties) : base(properties)
        {
        }

        List<IDrawableTask<TDrawingContext>> IDrawableSeries<TDrawingContext>.DeletingTasks => deletingTasks;

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

        public double DataLabelsSize { get => dataLabelsSize; set { dataLabelsSize = value; OnPropertyChanged(); } }

        public DataLabelsPosition DataLabelsPosition { get => dataLabelsPosition; set { dataLabelsPosition = value; OnPropertyChanged(); } }

        public Padding DataLabelsPadding { get => dataLabelsPadding; set { dataLabelsPadding = value; OnPropertyChanged(); } }

        public Func<ChartPoint, string> DataLabelFormatter { get => dataLabelFormatter; set { dataLabelFormatter = value; OnPropertyChanged(); } }

        public PaintContext<TDrawingContext> DefaultPaintContext => paintContext;

        public double LegendShapeSize { get => legendShapeSize; set { legendShapeSize = value; OnPropertyChanged(); } }

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
