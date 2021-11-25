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

using Xamarin.Forms;

namespace LiveChartsCore.SkiaSharpView.XamarinForms
{
    /// <summary>
    /// A chart for Xamarin.
    /// </summary>
    public interface IMobileChart
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
        BindableObject Canvas { get; }

        /// <summary>
        /// Gets the legend.
        /// </summary>
        /// <value>
        /// The legend.
        /// </value>
        BindableObject Legend { get; }

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>
        /// The tool tip template.
        /// </value>
        DataTemplate TooltipTemplate { get; set; }

        /// <summary>
        /// Gets or sets the tool tip font family.
        /// </summary>
        /// <value>
        /// The tool tip font family.
        /// </value>
        string TooltipFontFamily { get; set; }

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
        Color TooltipTextBrush { get; set; }

        /// <summary>
        /// Gets or sets the color of the tool tip background.
        /// </summary>
        /// <value>
        /// The color of the tool tip background.
        /// </value>
        Color TooltipBackground { get; set; }

        /// <summary>
        /// Gets or sets the tool tip font attributes.
        /// </summary>
        /// <value>
        /// The tool tip font attributes.
        /// </value>
        FontAttributes TooltipFontAttributes { get; set; }

        /// <summary>
        /// Gets or sets the legend template.
        /// </summary>
        /// <value>
        /// The legend template.
        /// </value>
        DataTemplate LegendTemplate { get; set; }

        /// <summary>
        /// Gets or sets the legend font family.
        /// </summary>
        /// <value>
        /// The legend font family.
        /// </value>
        string LegendFontFamily { get; set; }

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
        Color LegendTextBrush { get; set; }

        /// <summary>
        /// Gets or sets the color of the legend background.
        /// </summary>
        /// <value>
        /// The color of the legend background.
        /// </value>
        Color LegendBackground { get; set; }

        /// <summary>
        /// Gets or sets the legend font attributes.
        /// </summary>
        /// <value>
        /// The legend font attributes.
        /// </value>
        FontAttributes LegendFontAttributes { get; set; }
    }
}
