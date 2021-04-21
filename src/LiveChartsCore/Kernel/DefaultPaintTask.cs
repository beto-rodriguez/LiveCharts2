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

using LiveChartsCore.Drawing;
using LiveChartsCore.Motion;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines a default paint task.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="IDrawableTask{TDrawingContext}" />
    public class DefaultPaintTask<TDrawingContext> : IDrawableTask<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is stroke.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is stroke; otherwise, <c>false</c>.
        /// </value>
        public bool IsStroke { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fill.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is fill; otherwise, <c>false</c>.
        /// </value>
        public bool IsFill { get; set; }

        /// <summary>
        /// Gets or sets the index of the z.
        /// </summary>
        /// <value>
        /// The index of the z.
        /// </value>
        public double ZIndex { get; set; }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
        /// </value>
        public float StrokeThickness { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is completed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public long CurrentTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the element is removed when all the transitions complete.
        /// </summary>
        /// <value>
        ///   <c>true</c> if remove on completed; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveOnCompleted { get; set; }

        /// <summary>
        /// Gets or sets the clip rectangle.
        /// </summary>
        /// <value>
        /// The clip rectangle.
        /// </value>
        public RectangleF ClipRectangle { get; set; }

        /// <summary>
        /// Adds the geomety to paint task.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        public void AddGeometyToPaintTask(IDrawable<TDrawingContext> geometry)
        {

        }

        /// <summary>
        /// Clones the task.
        /// </summary>
        /// <returns></returns>
        public IDrawableTask<TDrawingContext> CloneTask()
        {
            return this;
        }

        /// <summary>
        /// Completes the transitions.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void CompleteTransitions(params string[] propertyName)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets the geometries.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDrawable<TDrawingContext>> GetGeometries()
        {
            return Enumerable.Empty<IDrawable<TDrawingContext>>();
        }

        /// <summary>
        /// Gets the transition property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IMotionProperty GetTransitionProperty(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Initializes the task.
        /// </summary>
        /// <param name="context">The context.</param>
        public void InitializeTask(TDrawingContext context)
        {
        }

        /// <summary>
        /// Removes the geometry from pain task.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        public void RemoveGeometryFromPainTask(IDrawable<TDrawingContext> geometry)
        {
        }

        /// <summary>
        /// Removes a property transition.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RemovePropertyTransition(string propertyName)
        {
        }

        /// <summary>
        /// Sets the geometries.
        /// </summary>
        /// <param name="geometries">The geometries.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SetGeometries(HashSet<IDrawable<TDrawingContext>> geometries)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sets the properties transitions.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void SetPropertiesTransitions(Animation animation, params string[] propertyName)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Completes all transitions.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CompleteAllTransitions()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Removes all the current transitions.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public void RemoveTransitions()
        {
            throw new System.NotImplementedException();
        }
    }
}
