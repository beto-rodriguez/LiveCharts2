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

using Avalonia;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <summary>
/// Defines a chart only for the avalonia ui framework
/// </summary>
public interface IAvaloniaChart
{
    /// <summary>
    /// Gets or sets the tool tip template.
    /// </summary>
    /// <value>
    /// The tool tip template.
    /// </value>
    DataTemplate? TooltipTemplate { get; set; }

    /// <summary>
    /// Gets or sets the tool tip font family.
    /// </summary>
    /// <value>
    /// The tool tip font family.
    /// </value>
    FontFamily TooltipFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the size of the tool tip font.
    /// </summary>
    /// <value>
    /// The size of the tool tip font.
    /// </value>
    double TooltipFontSize { get; set; }

    /// <summary>
    /// Gets or sets the tool tip font weight.
    /// </summary>
    /// <value>
    /// The tool tip font weight.
    /// </value>
    FontWeight TooltipFontWeight { get; set; }

    /// <summary>
    /// Gets or sets the tool tip font style.
    /// </summary>
    /// <value>
    /// The tool tip font style.
    /// </value>
    FontStyle TooltipFontStyle { get; set; }

    /// <summary>
    /// Gets or sets the tool tip text brush.
    /// </summary>
    /// <value>
    /// The tool tip text brush.
    /// </value>
    IBrush TooltipTextBrush { get; set; }

    /// <summary>
    /// Gets or sets the tool tip background.
    /// </summary>
    /// <value>
    /// The tool tip background.
    /// </value>
    IBrush TooltipBackground { get; set; }

    /// <summary>
    /// Gets or sets the legend template.
    /// </summary>
    /// <value>
    /// The legend template.
    /// </value>
    DataTemplate? LegendTemplate { get; set; }

    /// <summary>
    /// Gets or sets the legend font family.
    /// </summary>
    /// <value>
    /// The legend font family.
    /// </value>
    FontFamily LegendFontFamily { get; set; }

    /// <summary>
    /// Gets or sets the size of the legend font.
    /// </summary>
    /// <value>
    /// The size of the legend font.
    /// </value>
    double LegendFontSize { get; set; }

    /// <summary>
    /// Gets or sets the legend font weight.
    /// </summary>
    /// <value>
    /// The legend font weight.
    /// </value>
    FontWeight LegendFontWeight { get; set; }

    /// <summary>
    /// Gets or sets the legend font style.
    /// </summary>
    /// <value>
    /// The legend font style.
    /// </value>
    FontStyle LegendFontStyle { get; set; }

    /// <summary>
    /// Gets or sets the legend text brush.
    /// </summary>
    /// <value>
    /// The legend text brush.
    /// </value>
    IBrush LegendTextBrush { get; set; }

    /// <summary>
    /// Gets or sets the legend background.
    /// </summary>
    /// <value>
    /// The legend background.
    /// </value>
    IBrush LegendBackground { get; set; }

    /// <summary>
    /// Gets the canvas position relative to the control.
    /// </summary>
    /// <returns>the postion.</returns>
    Point GetCanvasPosition();
}
