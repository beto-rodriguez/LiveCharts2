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

using System;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Themes;

/// <summary>
/// Defines a style builder.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class Theme<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private VisualsStyle<TDrawingContext>? _style;

    /// <summary>
    /// Gets the current colo9r pallete.
    /// </summary>
    /// <value>
    /// The current colors.
    /// </value>
    public LvcColor[] ColorPallete { get; private set; } = Array.Empty<LvcColor>();

    /// <summary>
    /// Gets the style.
    /// </summary>
    /// <value>
    /// The style.
    /// </value>
    public VisualsStyle<TDrawingContext> Style
    {
        get => _style ?? throw new Exception("There is no style defined yet.");
        private set => _style = value;
    }

    /// <summary>
    /// Gets or sets the series default resolver.
    /// </summary>
    /// <value>
    /// The series default resolver.
    /// </value>
    public Action<LvcColor[], IChartSeries<TDrawingContext>, bool>? SeriesDefaultsResolver { get; set; }

    /// <summary>
    /// Uses the colors.
    /// </summary>
    /// <param name="colors">The colors.</param>
    /// <returns>The current theme instance</returns>
    public Theme<TDrawingContext> WithColors(params LvcColor[] colors)
    {
        ColorPallete = colors;
        return this;
    }

    /// <summary>
    /// Creates a new styles builder and configures it using the given predicate.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The current theme instance</returns>
    public Theme<TDrawingContext> WithStyle(Action<VisualsStyle<TDrawingContext>> predicate)
    {
        predicate(Style = new VisualsStyle<TDrawingContext>());
        return this;
    }

    /// <summary>
    /// Sets the series defaults resolver.
    /// </summary>
    /// <param name="resolver">The resolver.</param>
    /// <returns></returns>
    public Theme<TDrawingContext> WithSeriesDefaultsResolver(Action<LvcColor[], IChartSeries<TDrawingContext>, bool> resolver)
    {
        SeriesDefaultsResolver = resolver;
        return this;
    }

    /// <summary>
    /// Gets the objects builder.
    /// </summary>
    /// <returns>The current theme instance</returns>
    public VisualsStyle<TDrawingContext> GetVisualsInitializer()
    {
        return Style ?? throw new NullReferenceException(
                $"An instance of {nameof(VisualsStyle<TDrawingContext>)} is no configured yet, " +
                $"please register an instance using {nameof(WithStyle)}() method.");
    }
}
