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

using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    /// <summary>
    /// Defines a geometry that is built using from a svg path.
    /// </summary>
    /// <seealso cref="SizedGeometry" />
    public class SVGPathGeometry : SizedGeometry
    {
        private string _svg = string.Empty;
        internal SKPath? _svgPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="SVGPathGeometry"/> class.
        /// </summary>
        public SVGPathGeometry() : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SVGPathGeometry"/> class.
        /// </summary>
        /// <param name="svgPath">The SVG path.</param>
        public SVGPathGeometry(SKPath svgPath)
        {
            _svgPath = svgPath;
        }

        /// <summary>
        /// Gets or sets the SVG path.
        /// </summary>
        /// <value>
        /// The SVG.
        /// </value>
        public string SVG { get => _svg; set { _svg = value; OnSVGPropertyChanged(); } }

        /// <summary>
        /// Gets or sets whether the path should be fitted to the size of the geometry.
        /// </summary>
        public bool FitToSize { get; set; } = false;

        /// <inheritdoc cref="Geometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
        public override void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
        {
            if (_svgPath is null)
                throw new System.NullReferenceException(
                    $"{nameof(SVG)} property is null and there is not a defined path to draw.");

            using var tp = new SKPaint { Color = SKColors.LightGray };
            context.Canvas.DrawRect(X, Y, Width, Height, tp);

            _ = context.Canvas.Save();

            var canvas = context.Canvas;
            _ = _svgPath.GetTightBounds(out var bounds);

            if (FitToSize)
            {
                // fit to both axis
                canvas.Translate(X + Width / 2f, Y + Height / 2f);
                canvas.Scale(
                    Width / (bounds.Width + paint.StrokeWidth),
                    Height / (bounds.Height + paint.StrokeWidth));
                canvas.Translate(-bounds.MidX, -bounds.MidY);
            }
            else
            {
                // fit to the max dimension
                // preserve the corresponding scale in the min axis.
                var maxB = bounds.Width < bounds.Height ? bounds.Height : bounds.Width;

                canvas.Translate(X + Width / 2f, Y + Height / 2f);
                canvas.Scale(
                    Width / (maxB + paint.StrokeWidth),
                    Height / (maxB + paint.StrokeWidth));
                canvas.Translate(-bounds.MidX, -bounds.MidY);
            }

            canvas.DrawPath(_svgPath, paint);


            context.Canvas.Restore();
        }

        private void OnSVGPropertyChanged()
        {
            _svgPath = SKPath.ParseSvgPathData(_svg);
        }
    }
}
