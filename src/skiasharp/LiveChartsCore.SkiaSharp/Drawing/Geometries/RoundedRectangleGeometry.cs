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

using LiveChartsCore.Motion;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    /// <summary>
    /// Defines a ropunded rectangle geometry.
    /// </summary>
    /// <seealso cref="LiveChartsCore.SkiaSharpView.Drawing.Geometries.SizedGeometry" />
    public class RoundedRectangleGeometry : SizedGeometry
    {
        private readonly FloatMotionProperty rx;
        private readonly FloatMotionProperty ry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundedRectangleGeometry"/> class.
        /// </summary>
        public RoundedRectangleGeometry()
        {
            rx = RegisterMotionProperty(new FloatMotionProperty(nameof(Rx), 8f));
            ry = RegisterMotionProperty(new FloatMotionProperty(nameof(Ry), 8f));
        }

        /// <summary>
        /// Gets or sets the rx, the rounding in the x axis.
        /// </summary>
        /// <value>
        /// The rx.
        /// </value>
        public float Rx { get => rx.GetMovement(this); set => rx.SetMovement(value, this); }

        /// <summary>
        /// Gets or sets the ry, the rounding in the axis.
        /// </summary>
        /// <value>
        /// The ry.
        /// </value>
        public float Ry { get => ry.GetMovement(this); set => ry.SetMovement(value, this); }

        /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
        public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawRoundRect(
                new SKRect { Top = Y, Left = X, Size = new SKSize { Height = Height, Width = Width } }, Rx, Ry, paint);
        }
    }
}
