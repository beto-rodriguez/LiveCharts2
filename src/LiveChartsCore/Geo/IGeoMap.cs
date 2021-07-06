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
using System.Drawing;

namespace LiveChartsCore.Geo
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    public interface IGeoMap
    {
        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        Projection Projection { get; set; }

        /// <summary>
        /// Gets or sets the heat map.
        /// </summary>
        Color[] HeatMap { get; set; }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        double[]? ColorStops { get; set; }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        Color StrokeColor { get; set; }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        double StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        Color FillColor { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        Dictionary<string, double> Values { get; set; }
    }
}
