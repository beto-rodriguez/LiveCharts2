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

using LiveChartsCore.SkiaSharp.Motion.Composed;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Motion
{
    public class LinearGradientShader : Shader
    {
        private readonly SKPoint start;
        private readonly SKPoint end;
        private readonly SKColor[] colors;
        private readonly float[] colorPos;
        private readonly SKShaderTileMode mode;

        public LinearGradientShader(SKPoint start, SKPoint end, SKColor[] colors, float[] colorPos, SKShaderTileMode mode)
        {
            this.start = start;
            this.end = end;
            this.colors = colors;
            this.mode = mode;
            this.colorPos = colorPos;
        }

        public override SKShader GetSKShader()
        {
            return SKShader.CreateLinearGradient(start, end, colors, mode);
        }

        public override Shader InterpolateFrom(Shader from, float progress)
        {
            var f = (LinearGradientShader)from;

            if (colors.Length != f.colors.Length) throw new System.Exception("Different colors length is not supported");

            var interpolatedColors = new SKColor[colors.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                var fi = f.colors[i];
                var j = colors[i];
                interpolatedColors[i] = new SKColor(
                    (byte)(fi.Red + progress * (j.Red - fi.Red)),
                    (byte)(fi.Green + progress * (j.Green - fi.Green)),
                    (byte)(fi.Blue + progress * (j.Blue - fi.Blue)),
                    (byte)(fi.Alpha + progress * (j.Alpha - fi.Alpha)));
            }

            return new LinearGradientShader(
                 new SKPoint(
                     f.start.X + progress * (start.X - f.start.X),
                     f.start.Y + progress * (start.Y - f.start.Y)),
                 new SKPoint(
                     f.end.X + progress * (end.X - f.end.X),
                     f.end.Y + progress * (end.Y - f.end.Y)),
                 interpolatedColors,
                 colorPos,
                 mode);
        }
    }
}
