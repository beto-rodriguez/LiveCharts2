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
        internal double max = double.MinValue;
        internal double min = double.MaxValue;

        /// <summary>
        /// Creates a new instance of the <see cref="Bounds"/> class.
        /// </summary>
        public Bounds()
        {

        }

        /// <summary>
        /// Gets or sets the maximum value in the set.
        /// </summary>
        public double Max { get => max; set => max = value; }

        /// <summary>
        /// Gets or sets the minimum value in the set.
        /// </summary>
        public double Min { get => min; set => min = value; }

        /// <summary>
        /// Compares the current bounds with a given value,
        /// if the given value is greater than the current instance <see cref="Max"/> property then the given value is set at <see cref="Max"/> property,
        /// if the given value is less than the current instance <see cref="Min"/> property then the given value is set at <see cref="Min"/> property.
        /// </summary>
        /// <param name="value">the value to append</param>
        /// <returns>Whether the value affected the current bounds, true if it affected, false if did not.</returns>
        public AffectedBound AppendValue(double value)
        {
            var ab = AffectedBound.None;
            // the equals comparison is important, we need to register also the coordinates that are equal to the current limit.
            if (max <= value) { max = value; ab |= AffectedBound.Max; }
            if (min >= value) { min = value; ab |= AffectedBound.Min; }

            return ab;
        }
    }
}
