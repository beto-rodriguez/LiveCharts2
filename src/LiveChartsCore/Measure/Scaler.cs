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

namespace LiveChartsCore.Measure
{
    public class Scaler
    {
        private readonly float m, mInv, minPx, maxPx, deltaPx, minVal, maxVal, deltaVal;

        public Scaler(
            PointF drawMaringLocation, SizeF drawMarginSize, AxisOrientation orientation, Bounds axisBounds, bool isInverted)
        {
            if (orientation == AxisOrientation.Unknown)
                throw new Exception("The axis is not ready to be scaled.");

            if (orientation == AxisOrientation.X)
            {
                minPx = drawMaringLocation.X;
                maxPx = drawMarginSize.Width;
                deltaPx = maxPx - minPx;

                maxVal = (float)(isInverted ? axisBounds.min : axisBounds.max);
                minVal = (float)(isInverted ? axisBounds.max : axisBounds.min);
                deltaVal = maxVal - minVal;
            }
            else
            {
                minPx = drawMaringLocation.Y;
                maxPx = drawMarginSize.Height;
                deltaPx = maxPx - minPx;

                maxVal = (float)(isInverted ? axisBounds.max : axisBounds.min);
                minVal = (float)(isInverted ? axisBounds.min : axisBounds.max);
                deltaVal = maxVal - minVal;
            }

            m = deltaPx / deltaVal;
            mInv = 1 / m;
        }

        public static Scaler GetDefaultScaler(AxisOrientation orientation) => new(new PointF(0, 0), new SizeF(0, 100), orientation, new Bounds(), false);

        public float ToPixels(float value) => minPx + (value - minVal) * m;

        public float ToChartValues(float pixels) => minVal + (pixels - minPx) * mInv;
    }
}
