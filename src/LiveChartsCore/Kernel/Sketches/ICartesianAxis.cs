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

using System;
using LiveChartsCore.Measure;
using LiveChartsCore.Drawing.Common;
using System.ComponentModel;

namespace LiveChartsCore.Kernel.Sketches
{
    /// <summary>
    /// Defines an Axis in a Cartesian chart. 
    /// </summary>
    public interface ICartesianAxis : IPlane, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        AxisOrientation Orientation { get; }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>
        /// The padding.
        /// </value>
        Padding Padding { get; set; }

        /// <summary>
        /// Gets or sets the xo, a reference used internally to calculate the axis position.
        /// </summary>
        /// <value>
        /// The xo.
        /// </value>
        float Xo { get; set; }

        /// <summary>
        /// Gets or sets the yo, a reference used internally to calculate the axis position.
        /// </summary>
        /// <value>
        /// The yo.
        /// </value>
        float Yo { get; set; }

        /// <summary>
        /// Gets or sets the axis position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        AxisPosition Position { get; set; }

        /// <summary>
        /// Initializes the axis for the specified orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        void Initialize(AxisOrientation orientation);

        /// <summary>
        /// Occurs when the axis is initialized.
        /// </summary>
        event Action<ICartesianAxis>? Initialized;
    }
}
