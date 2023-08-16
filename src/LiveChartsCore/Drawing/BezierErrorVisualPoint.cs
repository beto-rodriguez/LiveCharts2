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
/// Defines a line bezier visual point.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <typeparam name="TVisual">The type of the visual.</typeparam>
/// <typeparam name="TErrorGeometry">The type of the error geometry.</typeparam>
public class BezierErrorVisualPoint<TDrawingContext, TVisual, TErrorGeometry>
    : BezierVisualPoint<TDrawingContext, TVisual>
        where TVisual : ISizedGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the y error geometry.
    /// </summary>
    public TErrorGeometry? YError { get; set; }

    /// <summary>
    /// Gets or sets the x error geometry.
    /// </summary>
    public TErrorGeometry? XError { get; set; }
}
