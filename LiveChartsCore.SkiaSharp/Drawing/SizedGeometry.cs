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

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public abstract class SizedGeometry : Geometry, ISizedGeometry<SkiaDrawingContext>
    {
        protected readonly FloatTransitionProperty width;
        protected readonly FloatTransitionProperty height;
        protected bool matchDimensions = false;

        public SizedGeometry() : base()
        {
            width = RegisterTransitionProperty(new FloatTransitionProperty(nameof(Width), 0));
            height = RegisterTransitionProperty(new FloatTransitionProperty(nameof(Height), 0));
        }

        public float Width { get => width.GetCurrentMovement(this); set => width.MoveTo(value, this); }

        public float Height
        {
            get
            {
                if (matchDimensions) return width.GetCurrentMovement(this);
                return height.GetCurrentMovement(this);
            }
            set
            {
                if (matchDimensions)
                {
                    width.MoveTo(value, this);
                    return;
                }
                height.MoveTo(value, this);
            }
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            return new SKSize(Width, Height);
        }
    }
}
