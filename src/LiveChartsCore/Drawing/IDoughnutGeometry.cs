﻿// The MIT License(MIT)
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
    /// Defines a doughnut geometry.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <seealso cref="IDrawable{TDrawingContext}" />
    public interface IDoughnutGeometry<TDrawingContext> : IGeometry<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Gets or sets the center x.
        /// </summary>
        /// <value>
        /// The center x.
        /// </value>
        float CenterX { get; set; }

        /// <summary>
        /// Gets or sets the center y.
        /// </summary>
        /// <value>
        /// The center y.
        /// </value>
        float CenterY { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        float Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        float Height { get; set; }

        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        /// <value>
        /// The start angle.
        /// </value>
        float StartAngle { get; set; }

        /// <summary>
        /// Gets or sets the sweep angle.
        /// </summary>
        /// <value>
        /// The sweep angle.
        /// </value>
        float SweepAngle { get; set; }

        /// <summary>
        /// Gets or sets the push out.
        /// </summary>
        /// <value>
        /// The push out.
        /// </value>
        float PushOut { get; set; }

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        /// <value>
        /// The inner radius.
        /// </value>
        float InnerRadius { get; set; }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>
        /// The corner radius.
        /// </value>
        float CornerRadius { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the direction of the corner radius is inverted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the direction is inverted; otherwise, <c>false</c>.
        /// </value>
        bool InvertedCornerRadius { get; set; }
    }
}
