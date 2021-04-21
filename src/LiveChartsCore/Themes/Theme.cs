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

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using System;
using System.Drawing;

namespace LiveChartsCore.Themes
{
    /// <summary>
    /// Defines a style builder.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    public class Theme<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private VisualsInitializer<TDrawingContext>? _theme;

        /// <summary>
        /// Gets the current colors.
        /// </summary>
        /// <value>
        /// The current colors.
        /// </value>
        public Color[]? CurrentColors { get; private set; }

        /// <summary>
        /// Gets or sets the series default resolver.
        /// </summary>
        /// <value>
        /// The series default resolver.
        /// </value>
        public Action<Color[], IDrawableSeries<TDrawingContext>>? SeriesDefaultsResolver { get; set; }

        /// <summary>
        /// Gets or sets the axis default resolver.
        /// </summary>
        /// <value>
        /// The axis default resolver.
        /// </value>
        public Action<IAxis<TDrawingContext>>? AxisDefaultResolver { get; set; }

        /// <summary>
        /// Uses the colors.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <returns>The current theme instance</returns>
        public Theme<TDrawingContext> WithColors(params Color[] colors)
        {
            CurrentColors = colors;
            return this;
        }

        /// <summary>
        /// Creates a new styles builder and configures it using the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The current theme instance</returns>
        public Theme<TDrawingContext> WithVisualsInitializer(Action<VisualsInitializer<TDrawingContext>> predicate)
        {
            predicate(_theme = new VisualsInitializer<TDrawingContext>());
            return this;
        }

        /// <summary>
        /// Sets the series defaults resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        public Theme<TDrawingContext> WithSeriesDefaultsResolver(Action<Color[], IDrawableSeries<TDrawingContext>> resolver)
        {
            SeriesDefaultsResolver = resolver;
            return this;
        }

        /// <summary>
        /// Sets the axis defaults resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        public Theme<TDrawingContext> WithAxisDefaultsResolver(Action<IAxis<TDrawingContext>> resolver)
        {
            AxisDefaultResolver = resolver;
            return this;
        }

        /// <summary>
        /// Gets the objects builder.
        /// </summary>
        /// <returns>The current theme instance</returns>
        public VisualsInitializer<TDrawingContext> GetVisualsInitializer()
        {
            return _theme ?? throw new NullReferenceException(
                    $"An instance of {nameof(VisualsInitializer<TDrawingContext>)} is no configured yet, " +
                    $"please register an instance using {nameof(WithVisualsInitializer)}() method.");
        }

        /// <summary>
        /// Resolves the series defaults.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <param name="series">The series.</param>
        public virtual void ResolveSeriesDefaults(Color[] colors, IDrawableSeries<TDrawingContext> series)
        {
            SeriesDefaultsResolver?.Invoke(colors, series);
        }

        /// <summary>
        /// Resolves the axis defaults.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public virtual void ResolveAxisDefaults(IAxis<TDrawingContext> axis)
        {
            AxisDefaultResolver?.Invoke(axis);
        }
    }
}
