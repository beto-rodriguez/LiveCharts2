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

using LiveChartsCore.Context;
using System.ComponentModel;

namespace LiveChartsCore
{
    /// <summary>
    /// A point in a Cartesian Chart.
    /// </summary>
    public class ChartPoint<TModel> : ICartesianCoordinate
    {
        private float x;
        private float y;

        /// <summary>
        /// Initialized a new instance of the <see cref="ChartPoint"/> class.
        /// </summary>
        public ChartPoint()
        {

        }

        /// <summary>
        /// Initialized a new instance of the <see cref="ChartPoint"/> class with given coordinates.
        /// </summary>
        /// <param name="x">the x coordinate</param>
        /// <param name="y">the y coordinate</param>
        public ChartPoint(double x, double y, int index, TModel dataSource)
        {
            X = (float) x;
            Y = (float) y;
            Index = index;
            DataSource = dataSource;
        }

        /// <summary>
        /// The X coordinate value.
        /// </summary>
        public float X { get => x; set { x = value; OnPropertyChanged(nameof(X)); } }

        /// <summary>
        /// The Y coordinate value.
        /// </summary>
        public float Y { get => y; set { y = value; OnPropertyChanged(nameof(Y)); } }

        /// <inheritdoc/>
        public object Visual { get; set; }

        /// <inheritdoc/>
        public object DataSource { get; set; }

        public HoverArea HoverArea { get; set; }

        /// <inheritdoc/>
        public int Index { get; set; }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes INotifyPropertyChanged.PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">the name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
