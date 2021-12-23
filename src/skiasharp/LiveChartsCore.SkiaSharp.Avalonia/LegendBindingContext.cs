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

using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <summary>
/// The legend binding context.
/// </summary>
public class LegendBindingContext
{
    /// <summary>
    /// Gets or sets the series.
    /// </summary>
    /// <value>
    /// The series.
    /// </value>
    public IEnumerable<ISeries> Series { get; set; } = Enumerable.Empty<ISeries>();

    /// <summary>
    /// Gets or sets the font family.
    /// </summary>
    /// <value>
    /// The font family.
    /// </value>
    public FontFamily FontFamily { get; set; } = new("Trebuchet MS");

    /// <summary>
    /// Gets or sets the size of the font.
    /// </summary>
    /// <value>
    /// The size of the font.
    /// </value>
    public double FontSize { get; set; }

    /// <summary>
    /// Gets or sets the font weight.
    /// </summary>
    /// <value>
    /// The font weight.
    /// </value>
    public FontWeight FontWeight { get; set; }

    /// <summary>
    /// Gets or sets the font style.
    /// </summary>
    /// <value>
    /// The font style.
    /// </value>
    public FontStyle FontStyle { get; set; }

    /// <summary>
    /// Gets or sets the text brush.
    /// </summary>
    /// <value>
    /// The text brush.
    /// </value>
    public IBrush? TextBrush { get; set; } = new SolidColorBrush(Color.FromRgb(35, 35, 35));

    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    /// <value>
    /// The background.
    /// </value>
    public IBrush? Background { get; set; } = new SolidColorBrush(Color.FromRgb(250, 250, 250));
}
