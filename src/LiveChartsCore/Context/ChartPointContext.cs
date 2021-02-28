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

namespace LiveChartsCore.Context
{
    /// <summary>
    /// Defines all the involved variables a point inside a chart requires.
    /// </summary>
    public class ChartPointContext<TVisual, TDrawingContext>: IChartPointContext
        where TDrawingContext : DrawingContext
        where TVisual : class, IVisualChartPoint<TDrawingContext>, new()
    {
        /// <summary>
        /// Gets the position of the point the collection that was used when the point was drawn.
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Gets the DataSource.
        /// </summary>
        public object? DataSource { get; internal set; }

        /// <summary>
        /// Gets the visual element in the UI.
        /// </summary>
        public TVisual? Visual { get; internal set; }

        object? IChartPointContext.Visual => Visual;

        /// <summary>
        /// Gets or sets the area that triggers the ToolTip.
        /// </summary>
        public HoverArea? HoverArea { get; internal set; }
    }
}
