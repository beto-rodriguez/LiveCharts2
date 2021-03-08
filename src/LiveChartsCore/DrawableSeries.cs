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
using LiveChartsCore.Drawing.Common;
using System;

namespace LiveChartsCore
{
    public abstract class DrawableSeries<TModel, TVisual, TLabel, TDrawingContext> 
        : Series<TModel, TVisual, TLabel, TDrawingContext>, IDrawableSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
    {
        protected PaintContext<TDrawingContext> paintContext = new PaintContext<TDrawingContext>();
        private IDrawableTask<TDrawingContext>? stroke = null;
        private IDrawableTask<TDrawingContext>? fill = null;
        private double legendShapeSize = 15;

        public DrawableSeries(SeriesProperties properties) : base(properties)
        {
        }

        public IDrawableTask<TDrawingContext>? Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                if (stroke != null)
                {
                    stroke.IsStroke = true;
                    stroke.IsFill = false;
                }
                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext>? Fill
        {
            get => fill;
            set
            {
                fill = value;
                if (fill != null)
                {
                    fill.IsStroke = false;
                    fill.IsFill = true;
                    fill.StrokeWidth = 0;
                }
                OnPaintContextChanged();
            }
        }
        public IDrawableTask<TDrawingContext>? DataLabelsBrush { get; set; }

        public double DataLabelsSize { get; set; } = 16;

        public DataLabelsPosition DataLabelsPosition { get; set; }

        public Padding DataLabelsPadding { get; set; } = new Padding { Left = 6, Top = 8, Right = 6, Bottom = 8 };

        public Func<IChartPoint, string> DataLabelFormatter { get; set; } = (point) => $"{point.PrimaryValue}";

        public PaintContext<TDrawingContext> DefaultPaintContext => paintContext;

        public double LegendShapeSize { get => legendShapeSize; set => legendShapeSize = value; }

        protected abstract void OnPaintContextChanged();

        protected void InitializeSeries()
        {
            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<TDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");

            initializer.ConstructSeries(this);
        }
    }
}
