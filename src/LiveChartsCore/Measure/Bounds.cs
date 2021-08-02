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

namespace LiveChartsCore.Measure
{
    /// <summary>
    /// Represents the maximum and minimum values in a set.
    /// </summary>
    public class Bounds
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Bounds"/> class.
        /// </summary>
        public Bounds()
        {

        }

        /// <summary>
        /// Gets whether the bounds are empty.
        /// </summary>
        public bool IsEmpty { get; private set; } = true;

        /// <summary>
        /// Gets or sets the maximum value in the data set.
        /// </summary>
        public double Max { get; set; } = float.MinValue;

        /// <summary>
        /// Gets or sets the minimum value in the data set.
        /// </summary>
        public double Min { get; set; } = float.MaxValue;

        /// <summary>
        /// Gets the delta, the absolute range in the axis.
        /// </summary>
        /// <value>
        /// The delta.
        /// </value>
        public double Delta => Max - Min;

        /// <summary>
        /// Gets or sets the minimum delta.
        /// </summary>
        /// <value>
        /// The minimum delta.
        /// </value>
        public double MinDelta { get; set; }

        /// <summary>
        /// Compares the current bounds with a given value,
        /// if the given value is greater than the current instance <see cref="Max"/> property then the given value is set at <see cref="Max"/> property,
        /// if the given value is less than the current instance <see cref="Min"/> property then the given value is set at <see cref="Min"/> property.
        /// </summary>
        /// <param name="value">the value to append</param>
        /// <returns>Whether the value affected the current bounds, true if it affected, false if did not.</returns>
        public void AppendValue(double value)
        {
            if (Max <= value) Max = value;
            if (Min >= value) Min = value;
            IsEmpty = false;
        }

        /// <summary>
        /// Determines whether the current instance has the same limit to the given instance.
        /// </summary>
        /// <param name="bounds">The bounds to compate.</param>
        public bool HasSameLimitTo(Bounds bounds)
        {
            return Max == bounds.Max && Min == bounds.Min;
        }
    }
}
