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
using LiveChartsCore.Kernel;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <summary>
/// Defines the tool tip binding context.
/// </summary>
public class TooltipBindingContext
{
    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    /// <value>
    /// The points.
    /// </value>
    public IEnumerable<ChartPoint> Points { get; set; } = Enumerable.Empty<ChartPoint>();

    /// <summary>
    /// Gets or sets the tool tip font family.
    /// </summary>
    /// <value>
    /// The tool tip font family.
    /// </value>
    public FontFamily TooltipFontFamily { get; set; } = new("Trebuchet MS");

    /// <summary>
    /// Gets or sets the size of the tool tip font.
    /// </summary>
    /// <value>
    /// The size of the tool tip font.
    /// </value>
    public double TooltipFontSize { get; set; }

    /// <summary>
    /// Gets or sets the tool tip font weight.
    /// </summary>
    /// <value>
    /// The tool tip font weight.
    /// </value>
    public FontWeight TooltipFontWeight { get; set; }

    /// <summary>
    /// Gets or sets the tool tip font style.
    /// </summary>
    /// <value>
    /// The tool tip font style.
    /// </value>
    public FontStyle TooltipFontStyle { get; set; }

    /// <summary>
    /// Gets or sets the tool tip text brush.
    /// </summary>
    /// <value>
    /// The tool tip text brush.
    /// </value>
    public SolidColorBrush TooltipTextBrush { get; set; } = new(Color.FromRgb(35, 35, 35));

    /// <summary>
    /// Gets or sets the tool tip background.
    /// </summary>
    /// <value>
    /// The tool tip background.
    /// </value>
    public IBrush TooltipBackground { get; set; } = new SolidColorBrush(Color.FromRgb(250, 250, 250));
}
