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

using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Drawing
{
    public class SVGPathGeometry : SizedGeometry
    {
        private string svg;
        private SKPath svgPath;

        public SVGPathGeometry() : base()
        {

        }

        public SVGPathGeometry(SKPath svgPath)
        {
            this.svgPath = svgPath;
        }

        public SVGPathGeometry(float x, float y, float width, float height, string svg)
            : base(x, y, width, height)
        {
            this.svg = svg;
        }

        public string SVG { get => svg; set { svg = value; OnSVGPropertyChanged(); } }

        public override void OnDraw(SkiaDrawingContext context, SKPaint paint)
        {
            if (svgPath == null && svg == null)
                throw new System.NullReferenceException(
                    $"{nameof(SVG)} property is null and there is not a defined path to draw.");

            context.Canvas.Save();

            var canvas = context.Canvas;
            svgPath.GetTightBounds(out SKRect bounds);

            canvas.Translate(X + Width / 2, Y + Height / 2);
            canvas.Scale(Width / (bounds.Width + paint.StrokeWidth),
                         Height / (bounds.Height + paint.StrokeWidth));
            canvas.Translate(-bounds.MidX, -bounds.MidY);

            canvas.DrawPath(svgPath, paint);

            context.Canvas.Restore();
        }

        private void OnSVGPropertyChanged()
        {
            svgPath = SKPath.ParseSvgPathData(svg);
        }
    }
}
