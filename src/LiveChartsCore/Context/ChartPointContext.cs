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

namespace LiveChartsCore.Context
{
    /// <summary>
    /// Defines all the involved variables a point inside a chart requires.
    /// </summary>
    public class ChartPointContext
    {
        private readonly int index;
        private readonly object dataSource;
        private readonly SeriesProperties seriesProperties;
        private readonly bool isInitialized;
        private HoverArea hoverArea = new HoverArea();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartPointContext"/> class.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="dataSource"></param>
        /// <param name="seriesProperties"></param>
        internal ChartPointContext(int index, object dataSource, SeriesProperties seriesProperties, bool isInitialized)
        {
            this.index = index;
            this.dataSource = dataSource;
            this.seriesProperties = seriesProperties;
            this.isInitialized = isInitialized;
        }

        /// <summary>
        /// Gets the position of the point the collection that was used when the point was drawn.
        /// </summary>
        public int Index { get => index; }

        /// <summary>
        /// Gets the DataSource.
        /// </summary>
        public object DataSource { get => dataSource; }

        /// <summary>
        /// Gets the series properties.
        /// </summary>
        public SeriesProperties SeriesProperties { get => seriesProperties; }

        /// <summary>
        /// Gets or sets (must not be set) the visual element in the UI.
        /// </summary>
        public object? Visual { get; set; }

        /// <summary>
        /// Gets or sets the area that triggers the ToolTip.
        /// </summary>
        public HoverArea HoverArea { get => hoverArea; set => hoverArea = value; }

        /// <summary>
        /// Gets weather the instance is initialized or not.
        /// </summary>
        public bool IsInitialized => isInitialized;
    }
}
