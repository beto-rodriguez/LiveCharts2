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

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LiveChartsCore.Defaults
{
    /// <summary>
    /// Defines a point with a weighted coordinate.
    /// </summary>
    /// <seealso cref="INotifyPropertyChanged" />
    public class WeightedPointF : INotifyPropertyChanged
    {
        private float? _x;
        private float? _y;
        private float? _weight;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedPointF"/> class.
        /// </summary>
        public WeightedPointF()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightedPointF"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="weight">The weight.</param>
        public WeightedPointF(float? x, float? y, float? weight)
        {
            _x = x;
            _y = y;
            _weight = weight;
        }

        /// <summary>
        /// Gets or sets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public float? X { get => _x; set { _x = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public float? Y { get => _y; set { _y = value; OnPropertyChanged(); } }

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public float? Weight { get => _weight; set { _weight = value; OnPropertyChanged(); } }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));
        }
    }
}
