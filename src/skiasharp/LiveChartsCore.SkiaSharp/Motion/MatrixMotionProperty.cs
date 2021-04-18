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

namespace LiveChartsCore.SkiaSharpView.Motion
{
    /// <summary>
    /// A wrapper to enable matrix/transform transitions for skia sharp.
    /// </summary>
    public class MatrixMotionProperty : MotionProperty<SKMatrix>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixMotionProperty"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public MatrixMotionProperty(string propertyName)
            : base(propertyName)
        {

        }

        /// <inheritdoc cref="MotionProperty{T}.OnGetMovement(float)" />
        protected override SKMatrix OnGetMovement(float progress)
        {
            return new SKMatrix
            {
                Persp0 = fromValue.Persp0 + progress * (toValue.Persp0 - fromValue.Persp0),
                Persp1 = fromValue.Persp1 + progress * (toValue.Persp1 - fromValue.Persp1),
                Persp2 = fromValue.Persp2 + progress * (toValue.Persp2 - fromValue.Persp2),
                ScaleX = fromValue.ScaleX + progress * (toValue.ScaleX - fromValue.ScaleX),
                ScaleY = fromValue.ScaleY + progress * (toValue.ScaleY - fromValue.ScaleY),
                SkewX = fromValue.SkewX + progress * (toValue.SkewX - fromValue.SkewX),
                SkewY = fromValue.SkewY + progress * (toValue.SkewY - fromValue.SkewY),
                TransX = fromValue.TransX + progress * (toValue.TransX - fromValue.TransX),
                TransY = fromValue.TransY + progress * (toValue.TransY - fromValue.TransY)
            };
        }
    }
}
