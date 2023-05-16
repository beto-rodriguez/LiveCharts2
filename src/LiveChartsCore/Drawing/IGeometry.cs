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

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a geometry in the user interface.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <seealso cref="IDrawable{TDrawingContext}" />
public interface IGeometry<TDrawingContext> : IDrawable<TDrawingContext>, IPaintable<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the parent shape, if any the X and Y properties will be relative to the parent.
    /// </summary>
    IGeometry<TDrawingContext>? Parent { get; set; }

    /// <summary>
    /// Gets or sets the transform origin.
    /// </summary>
    LvcPoint TransformOrigin { get; set; }

    /// <summary>
    /// Gets or sets the translate transform.
    /// </summary>
    /// <value>
    /// The translate in coordinates.
    /// </value>
    LvcPoint TranslateTransform { get; set; }

    /// <summary>
    /// Gets or sets the rotation transform in degrees.
    /// </summary>
    /// <value>
    /// The rotation in degrees.
    /// </value>
    float RotateTransform { get; set; }

    /// <summary>
    /// Gets or sets the scale transform.
    /// </summary>
    /// <value>
    /// The scale to use on the X and Y axis.
    /// </value>
    LvcPoint ScaleTransform { get; set; }

    /// <summary>
    /// Gets or sets the skew transform.
    /// </summary>
    /// <value>
    /// The skew factor to use in the X and Y axis, both axes go from 0 to 1, where 0 is nothing and 1
    /// the length of the shape in the specified axis.
    /// </value>
    LvcPoint SkewTransform { get; set; }

    /// <summary>
    /// Gets or sets the x coordinate.
    /// When the parent is null the coordinates are relative to the canvas.
    /// When the parent is not null
    /// the setter coordinates are relative to the parent
    /// but
    /// the getter is relative to the canvas.
    /// </summary>
    /// <value>
    /// The x coordinate.
    /// </value>
    float X { get; set; }

    /// <summary>
    /// Gets or sets the y coordinate.
    /// When the parent is null the coordinates are relative to the canvas.
    /// When the parent is not null
    /// the setter coordinates are relative to the parent
    /// but
    /// the getter is relative to the canvas.
    /// </summary>
    /// <value>
    /// The y coordinate.
    /// </value>
    float Y { get; set; }

    /// <summary>
    /// Measures the specified drawable task.
    /// </summary>
    /// <param name="drawableTask">The drawable task.</param>
    /// <returns></returns>
    LvcSize Measure(IPaint<TDrawingContext> drawableTask);
}
