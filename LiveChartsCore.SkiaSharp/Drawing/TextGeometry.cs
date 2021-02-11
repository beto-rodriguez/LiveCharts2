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
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class TextGeometry : Geometry, ITextGeometry<SkiaDrawingContext>
    {
        private string text;

        public TextGeometry()
        {
        }

        public TextGeometry(string text, float x, float y)
            : base(x, y)
        {
            this.text = text;
        }

        public Align VerticalAlign { get; set; } = Align.Middle;

        public Align HorizontalAlign { get; set; } = Align.Middle;

        public string Text { get => text; set => text = value; }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            context.Canvas.DrawText(text ?? "", GetPosition(context, paint), paint);
        }

        public override SKSize Measure(SkiaDrawingContext context, SKPaint paint)
        {
            var bounds = new SKRect();
            paint.MeasureText(text, ref bounds);
            return bounds.Size;
        }

        public override SKPoint GetPosition(SkiaDrawingContext context, SKPaint paint)
        {
            var size = Measure(context, paint);
            float dx = 0f, dy = 0f;
            switch (VerticalAlign)
            {
                case Align.Start: dy = size.Height; break;
                case Align.Middle: dy = size.Height * 0.5f; break;
                case Align.End: dy = 0f; break;
            }
            switch (HorizontalAlign)
            {
                case Align.Start: dx = 0; break;
                case Align.Middle: dx = size.Width * 0.5f; break;
                case Align.End: dx = size.Width; break;
            }
            return new SKPoint(X - dx, Y + dy);
        }
    }
}
