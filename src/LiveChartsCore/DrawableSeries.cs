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

namespace LiveChartsCore
{
    public abstract class DrawableSeries<TModel, TVisual, TDrawingContext> : Series<TModel, TVisual, TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : class, IHighlightableGeometry<TDrawingContext>, new()
    {
        protected PaintContext<TDrawingContext> paintContext = new PaintContext<TDrawingContext>();
        private IDrawableTask<TDrawingContext>? stroke = null;
        private IDrawableTask<TDrawingContext>? fill = null;
        private IDrawableTask<TDrawingContext>? highlightStroke = null;
        private IDrawableTask<TDrawingContext>? highlightFill = null;
        private double legendShapeSize = 15;

        public DrawableSeries(SeriesProperties properties) : base(properties) { }

        public IDrawableTask<TDrawingContext>? Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                if (stroke != null)
                {
                    stroke.IsStroke = true;
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
                    fill.StrokeWidth = 0;
                }
                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext>? HighlightStroke
        {
            get => highlightStroke;
            set
            {
                highlightStroke = value;
                if (highlightStroke != null)
                {
                    highlightStroke.IsStroke = true;
                    highlightStroke.ZIndex = 1;
                }
                OnPaintContextChanged();
            }
        }

        public IDrawableTask<TDrawingContext>? HighlightFill
        {
            get => highlightFill;
            set
            {
                highlightFill = value;
                if (highlightFill != null)
                {
                    highlightFill.IsStroke = false;
                    highlightFill.StrokeWidth = 0;
                    highlightFill.ZIndex = 1;
                }
                OnPaintContextChanged();
            }
        }

        public PaintContext<TDrawingContext> DefaultPaintContext => paintContext;

        public double LegendShapeSize { get => legendShapeSize; set => legendShapeSize = value; }

        protected abstract void OnPaintContextChanged();
    }
}
