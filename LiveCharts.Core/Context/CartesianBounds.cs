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

using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    /// <summary>
    /// Defines bounds for both, X and Y axes.
    /// </summary>
    public class CartesianBounds
    {
        private Bounds xAxisBounds;
        private Bounds yAxisBounds;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianBounds"/> class.
        /// </summary>
        public CartesianBounds()
        {
            XAxisBounds = new Bounds();
            YAxisBounds = new Bounds();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianBounds"/> class with given bounds.
        /// </summary>
        /// <param name="xBounds">The X axis bounds.</param>
        /// <param name="bounds">The Y axis bounds.</param>
        public CartesianBounds(Bounds xBounds, Bounds yBounds)
        {
            XAxisBounds = xBounds;
            YAxisBounds = yBounds;
        }

        /// <summary>
        /// Gets or sets the X axis bounds.
        /// </summary>
        public Bounds XAxisBounds { get => xAxisBounds; set {  xAxisBounds = value; } }

        /// <summary>
        /// Gets or sets the Y axis bounds.
        /// </summary>
        public Bounds YAxisBounds { get => yAxisBounds; set { yAxisBounds = value; } }

        internal HashSet<ICartesianCoordinate> XCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();

        internal HashSet<ICartesianCoordinate> YCoordinatesBounds { get; set; } = new HashSet<ICartesianCoordinate>();
    }
}
