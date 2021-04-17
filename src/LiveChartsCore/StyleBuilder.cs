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
using System;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defiens a style builder.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    public class StyleBuilder<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private Color[]? colors;
        private LiveChartsInitializer<TDrawingContext>? seriesInitializer;

        /// <summary>
        /// Gets the current colors.
        /// </summary>
        /// <value>
        /// The current colors.
        /// </value>
        public Color[]? CurrentColors => colors;

        /// <summary>
        /// Uses the colors.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <returns></returns>
        public StyleBuilder<TDrawingContext> UseColors(params Color[] colors)
        {
            this.colors = colors;
            return this;
        }

        /// <summary>
        /// Uses the series initializer.
        /// </summary>
        /// <param name="seriesInitializer">The series initializer.</param>
        /// <returns></returns>
        public StyleBuilder<TDrawingContext> UseSeriesInitializer(LiveChartsInitializer<TDrawingContext> seriesInitializer)
        {
            this.seriesInitializer = seriesInitializer;
            return this;
        }

        /// <summary>
        /// Gets the initializer.
        /// </summary>
        /// <returns></returns>
        public LiveChartsInitializer<TDrawingContext> GetInitializer()
        {
            if (seriesInitializer == null)
                throw new NullReferenceException(
                    $"An instance of {nameof(LiveChartsInitializer<TDrawingContext>)} is no configured yet, " +
                    $"please register an instance using {nameof(UseSeriesInitializer)}() method.");

            return seriesInitializer;
        }
    }
}
