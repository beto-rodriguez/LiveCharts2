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
using System;

namespace LiveChartsCore.Kernel.Drawing
{
    /// <summary>
    /// Defines the area helper class.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <typeparam name="TGeometryPath">The type of the geometry path.</typeparam>
    /// <typeparam name="TLineSegment">The type of the line segment.</typeparam>
    /// <typeparam name="TMoveTo">The type of the move to.</typeparam>
    /// <typeparam name="TPathContext">The type of the path context.</typeparam>
    public class AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveTo, TPathContext>
        where TGeometryPath : IPathGeometry<TDrawingContext, TPathContext>, new()
        where TLineSegment : ILinePathSegment<TPathContext>, new()
        where TMoveTo : IMoveToPathCommand<TPathContext>, new()
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public IPathGeometry<TDrawingContext, TPathContext> Path { get; set; } = new TGeometryPath();

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>
        /// The start point.
        /// </value>
        public IMoveToPathCommand<TPathContext> StartPoint { get; set; } = new TMoveTo();

        /// <summary>
        /// Gets or sets the start segment.
        /// </summary>
        /// <value>
        /// The start segment.
        /// </value>
        public ILinePathSegment<TPathContext> StartSegment { get; set; } = new TLineSegment();

        /// <summary>
        /// Gets or sets the end segment.
        /// </summary>
        /// <value>
        /// The end segment.
        /// </value>
        public ILinePathSegment<TPathContext> EndSegment { get; set; } = new TLineSegment();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized { get; set; }

        /// <summary>
        /// Initializes the helper with the specified transition setter.
        /// </summary>
        /// <param name="transitionSetter">The transition setter.</param>
        /// <param name="defaultAnimation">The default animation.</param>
        /// <returns></returns>
        public bool Initialize(
            Action<AreaHelper<TDrawingContext, TGeometryPath, TLineSegment, TMoveTo, TPathContext>, Animation> transitionSetter,
            Animation defaultAnimation)
        {
            if (IsInitialized) return false;

            IsInitialized = true;
            transitionSetter(this, defaultAnimation);

            return true;
        }

        /// <summary>
        /// Clears the limits.
        /// </summary>
        public void ClearLimits()
        {
            Path.RemoveCommand(StartPoint);
            Path.RemoveCommand(StartSegment);
            Path.RemoveCommand(EndSegment);
        }
    }
}
