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

using LiveChartsCore.Drawing;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines the stroke and ill drawable class.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    public class StrokeAndFillDrawable<TDrawingContext>
        where TDrawingContext: DrawingContext
    {
        private IDrawableTask<TDrawingContext>? stroke = null;
        private IDrawableTask<TDrawingContext>? fill = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="StrokeAndFillDrawable{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="stroke">The stroke.</param>
        /// <param name="fill">The fill.</param>
        public StrokeAndFillDrawable(IDrawableTask<TDrawingContext>? stroke, IDrawableTask<TDrawingContext>? fill)
        {
            this.stroke = stroke;
            if (stroke != null)
            {
                stroke.IsStroke = true;
                stroke.IsFill = false;
            }

            this.fill = fill;
            if (fill != null)
            {
                fill.IsStroke = false;
                fill.IsFill = true;
                fill.StrokeThickness = 0;
            }
        }

        /// <summary>
        /// Gets the stroke.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
        public IDrawableTask<TDrawingContext>? Stroke => stroke;

        /// <summary>
        /// Gets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
        public IDrawableTask<TDrawingContext>? Fill => fill;
    }
}
