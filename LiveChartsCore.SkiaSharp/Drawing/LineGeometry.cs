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

using LiveChartsCore.Drawing;
using LiveChartsCore.Transitions;
using SkiaSharp;
using System;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class LineGeometry : Geometry, ILineGeometry<SkiaDrawingContext>
    {
        private readonly FloatTransition x1 = new FloatTransition(0f);
        private readonly FloatTransition y1 = new FloatTransition(0f);

        public LineGeometry()
        {
        }

        public LineGeometry(float x, float y, float x1, float y1)
            : base(x, y)
        {
            this.x1 = new FloatTransition(x1);
            this.y1 = new FloatTransition(y1);
        }

        public float X1 { get => x1.GetCurrentMovement(this); set => x1.MoveTo(value, this); }

        public float Y1 { get => y1.GetCurrentMovement(this); set => y1.MoveTo(value, this); }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawLine(X, Y, X1, Y1, paint);
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Math.Abs(X1 - X), Math.Abs(Y1 - Y));
        }
    }
}
