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
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Drawing;

namespace LiveChartsCore.SkiaSharpView.Drawing
{
    public class LabelGeometry : Geometry, ILabelGeometry<SkiaSharpDrawingContext>
    {
        private string text;
        private FloatMotionProperty textSizeProperty;

        public LabelGeometry()
        {
            textSizeProperty = RegisterMotionProperty(new FloatMotionProperty(nameof(TextSize)));
        }

        public Align VerticalAlign { get; set; } = Align.Middle;

        public Align HorizontalAlign { get; set; } = Align.Middle;

        public string Text { get => text; set => text = value; }

        public float TextSize { get => textSizeProperty.GetMovement(this); set => textSizeProperty.SetMovement(value, this); }

        public Padding Padding { get; set; }

        public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
        {
            paint.TextSize = TextSize;
            context.Canvas.DrawText(text ?? "", GetPosition(context, paint), paint);
        }

        protected override SizeF OnMeasure(PaintTask drawable)
        {
            var p = new SKPaint
            {
                Color = drawable.Color,
                IsAntialias = drawable.IsAntialias,
                IsStroke = drawable.IsStroke,
                StrokeWidth = drawable.StrokeThickness,
                TextSize = TextSize
            };

            var bounds = new SKRect();

            p.MeasureText(text, ref bounds);
            return new SizeF(bounds.Size.Width + Padding.Left + Padding.Right, bounds.Size.Height + Padding.Top + Padding.Bottom);
        }

        protected override SKPoint GetPosition(SkiaSharpDrawingContext context, SKPaint paint)
        {
            var size = Measure(context.PaintTask);
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
            return new SKPoint(X - dx + Padding.Left, Y + dy - Padding.Top);
        }
    }
}
