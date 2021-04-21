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

using LiveChartsCore.SkiaSharpView.Motion.Composed;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Motion
{
    /// <summary>
    /// A wrapper to enable linear gradient animations using skia sharp shaders.
    /// </summary>
    /// <seealso cref="Shader" />
    public class LinearGradientShader : Shader
    {
        private readonly SKPoint _start;
        private readonly SKPoint _end;
        private readonly SKColor[] _colors;
        private readonly float[] _colorPos;
        private readonly SKShaderTileMode _mode;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGradientShader"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="colors">The colors.</param>
        /// <param name="colorPos">The color position.</param>
        /// <param name="mode">The mode.</param>
        public LinearGradientShader(SKPoint start, SKPoint end, SKColor[] colors, float[] colorPos, SKShaderTileMode mode)
        {
            _start = start;
            _end = end;
            _colors = colors;
            _mode = mode;
            _colorPos = colorPos;
        }

        /// <inheritdoc cref="Shader.GetSKShader" />
        public override SKShader GetSKShader()
        {
            return SKShader.CreateLinearGradient(_start, _end, _colors, _mode);
        }

        /// <inheritdoc cref="Shader.InterpolateFrom(Shader, float)" />
        public override Shader InterpolateFrom(Shader from, float progress)
        {
            var f = (LinearGradientShader)from;

            if (_colors.Length != f._colors.Length) throw new System.Exception("Different colors length is not supported");

            var interpolatedColors = new SKColor[_colors.Length];
            for (var i = 0; i < _colors.Length; i++)
            {
                var fi = f._colors[i];
                var j = _colors[i];
                interpolatedColors[i] = new SKColor(
                    (byte)(fi.Red + progress * (j.Red - fi.Red)),
                    (byte)(fi.Green + progress * (j.Green - fi.Green)),
                    (byte)(fi.Blue + progress * (j.Blue - fi.Blue)),
                    (byte)(fi.Alpha + progress * (j.Alpha - fi.Alpha)));
            }

            return new LinearGradientShader(
                 new SKPoint(
                     f._start.X + progress * (_start.X - f._start.X),
                     f._start.Y + progress * (_start.Y - f._start.Y)),
                 new SKPoint(
                     f._end.X + progress * (_end.X - f._end.X),
                     f._end.Y + progress * (_end.Y - f._end.Y)),
                 interpolatedColors,
                 _colorPos,
                 _mode);
        }
    }
}
