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

using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore.Drawing
{
    /// <summary>
    /// Defines a set of geometries that will be drawn according to this instance specifications.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    public interface IDrawableTask<TDrawingContext> : IAnimatable, IDisposable
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is stroke.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is stroke; otherwise, <c>false</c>.
        /// </value>
        bool IsStroke { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fill.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fill; otherwise, <c>false</c>.
        /// </value>
        bool IsFill { get; set; }

        /// <summary>
        /// Gets or sets the index of the z.
        /// </summary>
        /// <value>
        /// The index of the z.
        /// </value>
        double ZIndex { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        float StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets the clip rectangle.
        /// </summary>
        /// <value>
        /// The clip rectangle.
        /// </value>
        RectangleF ClipRectangle { get; set; }

        /// <summary>
        /// Initializes the task.
        /// </summary>
        /// <param name="context">The context.</param>
        void InitializeTask(TDrawingContext context);

        /// <summary>
        /// Gets the geometries.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDrawable<TDrawingContext>> GetGeometries();

        /// <summary>
        /// Sets the geometries.
        /// </summary>
        /// <param name="geometries">The geometries.</param>
        void SetGeometries(HashSet<IDrawable<TDrawingContext>> geometries);

        /// <summary>
        /// Adds the geomety to paint task.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        void AddGeometyToPaintTask(IDrawable<TDrawingContext> geometry);

        /// <summary>
        /// Removes the geometry from pain task.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        void RemoveGeometryFromPainTask(IDrawable<TDrawingContext> geometry);

        /// <summary>
        /// Clones the task.
        /// </summary>
        /// <returns></returns>
        IDrawableTask<TDrawingContext> CloneTask();
    }
}
