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
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing;

/// <summary>
/// Defines an avalonia-skiasharp drawing context.
/// </summary>
/// <seealso cref="SkiaSharpDrawingContext" />
public class AvaloniaDrawingContext : SkiaSharpDrawingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AvaloniaDrawingContext"/> class.
    /// </summary>
    /// <param name="motionCanvas">The motion canvas.</param>
    /// <param name="info">The information.</param>
    /// <param name="surface">The surface.</param>
    /// <param name="canvas">The canvas.</param>
    public AvaloniaDrawingContext(MotionCanvas<SkiaSharpDrawingContext> motionCanvas, SKImageInfo info, SKSurface surface, SKCanvas canvas)
        : base(motionCanvas, info, surface, canvas, true)
    { }

    /// <summary>
    /// Gets or sets the color of the back.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    public SKColor BackColor { get; set; } = new(255, 255, 255, 255);

    /// <summary>
    /// Clears the canvas.
    /// </summary>
    public override void ClearCanvas() { }
}
