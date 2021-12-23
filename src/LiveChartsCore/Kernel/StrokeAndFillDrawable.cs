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

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines the stroke and ill drawable class.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class StrokeAndFillDrawable<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrokeAndFillDrawable{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="stroke">The stroke.</param>
    /// <param name="fill">The fill.</param>
    /// <param name="isHoverState">is hover state?.</param>
    public StrokeAndFillDrawable(IPaint<TDrawingContext>? stroke, IPaint<TDrawingContext>? fill, bool isHoverState = false)
    {
        Stroke = stroke;
        if (stroke is not null)
        {
            stroke.IsStroke = true;
            stroke.IsFill = false;
        }

        Fill = fill;
        if (fill is not null)
        {
            fill.IsStroke = false;
            fill.IsFill = true;
            fill.StrokeThickness = 0;
        }
        IsHoverState = isHoverState;
    }

    /// <summary>
    /// Gets the stroke.
    /// </summary>
    /// <value>
    /// The stroke.
    /// </value>
    public IPaint<TDrawingContext>? Stroke { get; }

    /// <summary>
    /// Gets the fill.
    /// </summary>
    /// <value>
    /// The fill.
    /// </value>
    public IPaint<TDrawingContext>? Fill { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is hover state.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is hover state; otherwise, <c>false</c>.
    /// </value>
    public bool IsHoverState { get; set; }
}
