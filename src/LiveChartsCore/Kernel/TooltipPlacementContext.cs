﻿// The MIT License(MIT)
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

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines the tooltip placement context class.
    /// </summary>
    public class TooltipPlacementContext
    {
        /// <summary>
        /// Gets or sets the most top.
        /// </summary>
        /// <value>
        /// The most top.
        /// </value>
        public float MostTop { get; set; } = float.MaxValue;

        /// <summary>
        /// Gets or sets the most bottom.
        /// </summary>
        /// <value>
        /// The most bottom.
        /// </value>
        public float MostBottom { get; set; } = float.MinValue;

        /// <summary>
        /// Gets or sets the most right.
        /// </summary>
        /// <value>
        /// The most right.
        /// </value>
        public float MostRight { get; set; } = float.MinValue;

        /// <summary>
        /// Gets or sets the most left.
        /// </summary>
        /// <value>
        /// The most left.
        /// </value>
        public float MostLeft { get; set; } = float.MaxValue;

        /// <summary>
        /// Gets or sets the pie x.
        /// </summary>
        /// <value>
        /// The pie x.
        /// </value>
        public float PieX { get; set; } = 0;

        /// <summary>
        /// Gets or sets the pie y.
        /// </summary>
        /// <value>
        /// The pie y.
        /// </value>
        public float PieY { get; set; } = 0;
    }
}
