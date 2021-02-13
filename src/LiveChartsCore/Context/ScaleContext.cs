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

using System;
using System.Drawing;

namespace LiveChartsCore.Context
{
    public class ScaleContext
    {
        private readonly float o, m, max, d;
        private readonly Func<float, float> scaler;

        public ScaleContext(PointF drawMaringLocation, SizeF drawMarginSize, AxisOrientation orientation, Bounds axisBounds)
        {
            if (orientation == AxisOrientation.Unknown) throw new System.Exception("The axis is not ready to be scaled.");

            if (orientation == AxisOrientation.X)
            {
                unchecked
                {
                    o = drawMaringLocation.X;
                    d = drawMarginSize.Width;
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));
                    max = (float)axisBounds.max;
                    scaler = ScaleXToUI;
                }
            }
            else
            {
                unchecked
                {
                    o = drawMaringLocation.Y;
                    d = drawMarginSize.Height;
                    m = (float)(-(d - 0) / (axisBounds.max - axisBounds.min));
                    max = (float)axisBounds.max;
                    scaler = ScaleYToUI;
                }
            }
        }

        public Func<float, float> ScaleToUi => scaler;

        private float ScaleXToUI(float value) => o + (m * (max - value) + d);
        private float ScaleYToUI(float value) => o + (d - (m * (max - value) + d));
    }
}
