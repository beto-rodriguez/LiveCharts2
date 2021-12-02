
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
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Geo
{
    /// <summary>
    /// Defines a shape in a map.
    /// </summary>
    public abstract class MapShape<TDrawingContext> : IMapElement, INotifyPropertyChanged
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Called when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc cref="IMapElement.Measure(object)"/>
        public abstract void Measure(MapShapeContext<TDrawingContext> context);

        /// <inheritdoc cref="IMapElement.RemoveFromUI(object)"/>
        public abstract void RemoveFromUI(MapShapeContext<TDrawingContext> context);

        void IMapElement.Measure(object context)
        {
            Measure((MapShapeContext<TDrawingContext>)context);
        }

        void IMapElement.RemoveFromUI(object context)
        {
            RemoveFromUI((MapShapeContext<TDrawingContext>)context);
        }

        /// <summary>
        /// Called when a property changes.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
