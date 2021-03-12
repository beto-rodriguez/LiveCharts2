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

using Avalonia.Media;
using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Common;
using LiveChartsCore.Motion;
using System;
using System.Drawing;

namespace LiveChartsCore.AvaloniaView.Drawing
{
    public class LabelGeometry : Geometry, ILabelGeometry<AvaloniaDrawingContext>
    {
        private string text = string.Empty;
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

        public override void OnDraw(AvaloniaDrawingContext context)
        {
            if (context.DrawableTask == null) throw new Exception($"{nameof(context.DrawableTask)} is misssing");
            var ft = new FormattedText(
               text, Typeface.Default, TextSize, TextAlignment.Left, TextWrapping.NoWrap,
               new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
            var location = GetPosition(context, context.DrawableTask);
            context.AvaloniaContext.DrawText(context.Brush, new Avalonia.Point(location.X, location.Y), ft);
        }

        protected override SizeF OnMeasure(IDrawableTask<AvaloniaDrawingContext> paintTaks)
        {
            return new SizeF(50, 13);

            // throws null reference exception.

            var ft = new FormattedText() { Typeface = new Typeface("Arial"), FontSize = 18 };

            //var ft = new FormattedText(
            //    text, Typeface.Default, TextSize, TextAlignment.Left, TextWrapping.NoWrap,
            //    new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));

            return new SizeF((float)ft.Bounds.Width + Padding.Left + Padding.Right, (float)ft.Bounds.Height + Padding.Top + Padding.Bottom);
        }

        protected override PointF GetPosition(AvaloniaDrawingContext context, IDrawableTask<AvaloniaDrawingContext> drawableTask)
        {
            var size = Measure(drawableTask);
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

            return new PointF(X - dx + Padding.Left, Y + dy - Padding.Top);
        }

    }
}
