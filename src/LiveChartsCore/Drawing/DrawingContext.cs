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

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// Defines a context that is able to draw 2D shapes in the user interface.
    /// </summary>
    public abstract class DrawingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingContext"/> class.
        /// </summary>
        public DrawingContext(bool lockOnDraw = false)
        {
            LockOnDraw = lockOnDraw;
        }

        /// <summary>
        /// Gets or sets a property indicating whether the canvas should be locked while the
        /// charts is being drawn. This property was created to prevent an Issue in Avalonia where
        /// the a custom renderer is called on multiple threads, this causes that the elements on
        /// composed geometries (like paths) could change their segments from multiple threads.
        /// </summary>
        public bool LockOnDraw { get; }

        /// <summary>
        /// Clears the canvas.
        /// </summary>
        public abstract void ClearCanvas();
    }
}
