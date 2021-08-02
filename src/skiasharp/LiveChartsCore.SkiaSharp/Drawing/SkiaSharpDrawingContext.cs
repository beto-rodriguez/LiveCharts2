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
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing
{
    /// <summary>
    /// Defines a skia sharp drawing context.
    /// </summary>
    /// <seealso cref="DrawingContext" />
    public class SkiaSharpDrawingContext : DrawingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpDrawingContext"/> class.
        /// </summary>
        /// <param name="motionCanvas">The motion canvas.</param>
        /// <param name="info">The information.</param>
        /// <param name="surface">The surface.</param>
        /// <param name="canvas">The canvas.</param>
        /// <param name="lockOnDraw">Locks the canvas while drawing it (workaround for avalonia).</param>
        public SkiaSharpDrawingContext(
            MotionCanvas<SkiaSharpDrawingContext> motionCanvas,
            SKImageInfo info,
            SKSurface surface,
            SKCanvas canvas,
            bool lockOnDraw = false) : base(lockOnDraw)
        {
            MotionCanvas = motionCanvas;
            Info = info;
            Surface = surface;
            Canvas = canvas;
        }

        /// <summary>
        /// Gets or sets the motion canvas.
        /// </summary>
        /// <value>
        /// The motion canvas.
        /// </value>
        public MotionCanvas<SkiaSharpDrawingContext> MotionCanvas { get; set; }

        /// <summary>
        /// Gets or sets the information.
        /// </summary>
        /// <value>
        /// The information.
        /// </value>
        public SKImageInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the surface.
        /// </summary>
        /// <value>
        /// The surface.
        /// </value>
        public SKSurface Surface { get; set; }

        /// <summary>
        /// Gets or sets the canvas.
        /// </summary>
        /// <value>
        /// The canvas.
        /// </value>
        public SKCanvas Canvas { get; set; }

        /// <summary>
        /// Gets or sets the paint task.
        /// </summary>
        /// <value>
        /// The paint task.
        /// </value>
        public PaintTask PaintTask { get; set; }

        /// <summary>
        /// Gets or sets the paint.
        /// </summary>
        /// <value>
        /// The paint.
        /// </value>
        public SKPaint Paint { get; set; }

        /// <summary>
        /// Gets or sets the color of the clear.
        /// </summary>
        /// <value>
        /// The color of the clear.
        /// </value>
        public SKColor ClearColor { get; set; } = SKColor.Empty;

        /// <summary>
        /// Clears the canvas.
        /// </summary>
        public override void ClearCanvas()
        {
            Canvas.Clear(ClearColor);
        }
    }
}
