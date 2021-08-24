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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LiveChartsCore.SkiaSharpView.WinUI
{
    /// <summary>
    /// Defines a win ui chart.
    /// </summary>
    public interface IWinUIChart
    {
        /// <summary>
        /// Gets the layout grid.
        /// </summary>
        /// <value>
        /// The layout grid.
        /// </value>
        Grid LayoutGrid { get; }

        /// <summary>
        /// Gets the canvas.
        /// </summary>
        /// <value>
        /// The canvas.
        /// </value>
        FrameworkElement Canvas { get; }

        /// <summary>
        /// Gets the legend.
        /// </summary>
        /// <value>
        /// The legend.
        /// </value>
        FrameworkElement Legend { get; }

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
        /// Gets or sets the color of the tool tip text.
        /// </summary>
        /// <value>
        /// The color of the tool tip text.
        /// </value>
        Brush TooltipTextBrush { get; set; }

        /// <summary>
        /// Gets or sets the color of the tool tip background.
        /// </summary>
        /// <value>
        /// The color of the tool tip background.
        /// </value>
        Brush TooltipBackground { get; set; }

        /// <summary>
        /// Gets or sets the legend font weight.
        /// </summary>
        /// <value>
        /// The legend font weight.
        /// </value>
        FontWeight TooltipFontWeight { get; set; }

        /// <summary>
        /// Gets or sets the legend font stretch.
        /// </summary>
        /// <value>
        /// The legend font stretch.
        /// </value>
        FontStretch TooltipFontStretch { get; set; }

        /// <summary>
        /// Gets or sets the legend font style.
        /// </summary>
        /// <value>
        /// The legend font style.
        /// </value>
        FontStyle TooltipFontStyle { get; set; }

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
        /// Gets or sets the color of the legend text.
        /// </summary>
        /// <value>
        /// The color of the legend text.
        /// </value>
        Brush LegendTextBrush { get; set; }

        /// <summary>
        /// Gets or sets the color of the legend background.
        /// </summary>
        /// <value>
        /// The color of the legend background.
        /// </value>
        Brush LegendBackground { get; set; }

        /// <summary>
        /// Gets or sets the legend font stretch.
        /// </summary>
        /// <value>
        /// The legend font stretch.
        /// </value>
        FontStretch LegendFontStretch { get; set; }

        /// <summary>
        /// Gets or sets the legend font style.
        /// </summary>
        /// <value>
        /// The legend font style.
        /// </value>
        FontStyle LegendFontStyle { get; set; }

        /// <summary>
        /// Gets or sets the legend font weight.
        /// </summary>
        /// <value>
        /// The legend font weight.
        /// </value>
        FontWeight LegendFontWeight { get; set; }

        /// <summary>
        /// Gets or sets the legend template.
        /// </summary>
        /// <value>
        /// The legend template.
        /// </value>
        DataTemplate? LegendTemplate { get; set; }
    }
}
