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

using LiveChartsCore.Transitions;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharp.Transitions
{
    public class MatrixTransition : Transition<SKMatrix>
    {
        public MatrixTransition()
        {
            fromValue = SKMatrix.Identity;
            toValue = SKMatrix.Identity;
        }

        public MatrixTransition(SKMatrix matrix)
        {
            fromValue = new SKMatrix(matrix.Values);
            toValue = new SKMatrix(matrix.Values);
        }

        protected override SKMatrix OnGetMovement(float progress)
        {
            var m = new SKMatrix();
            var f = fromValue;
            var t = toValue;

            m.Persp0 = f.Persp0 + progress * (t.Persp0 - f.Persp0);
            m.Persp1 = f.Persp1 + progress * (t.Persp1 - f.Persp1);
            m.Persp2 = f.Persp2 + progress * (t.Persp2 - f.Persp2);
            m.ScaleX = f.ScaleX + progress * (t.ScaleX - f.ScaleX);
            m.ScaleY = f.ScaleY + progress * (t.ScaleY - f.ScaleY);
            m.SkewX = f.SkewX + progress * (t.SkewX - f.SkewX);
            m.SkewY = f.SkewY + progress * (t.SkewY - f.SkewY);
            m.TransX = f.TransX + progress * (t.TransX - f.TransX);
            m.TransY = f.TransY + progress * (t.TransY - f.TransY);

            return m;
        }
    }
}
