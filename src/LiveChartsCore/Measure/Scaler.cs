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

using System;
using System.Drawing;

namespace LiveChartsCore.Measure
{
    /// <summary>
    /// Defines the scaler class, this class helps to scale from the data scale to the user interface scale and vise versa.
    /// </summary>
    public class Scaler
    {
        private readonly float m, mInv, minPx, maxPx, deltaPx, minVal, maxVal, deltaVal;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scaler"/> class.
        /// </summary>
        /// <param name="drawMaringLocation">The draw maring location.</param>
        /// <param name="drawMarginSize">Size of the draw margin.</param>
        /// <param name="axis">The axis.</param>
        /// <exception cref="Exception">The axis is not ready to be scaled.</exception>
        public Scaler(PointF drawMaringLocation, SizeF drawMarginSize, IAxis axis)
        {
            if (axis.Orientation == AxisOrientation.Unknown)
                throw new Exception("The axis is not ready to be scaled.");

            if (axis.Orientation == AxisOrientation.X)
            {
                minPx = drawMaringLocation.X;
                maxPx = drawMaringLocation.X + drawMarginSize.Width;
                deltaPx = maxPx - minPx;

                maxVal = (float)(axis.IsInverted ? axis.DataBounds.Min : axis.DataBounds.Max);
                minVal = (float)(axis.IsInverted ? axis.DataBounds.Max : axis.DataBounds.Min);

                if (axis.MaxLimit != null || axis.MinLimit != null)
                {
                    maxVal = (float)(axis.IsInverted ? axis.MinLimit ?? minVal : axis.MaxLimit ?? maxVal);
                    minVal = (float)(axis.IsInverted ? axis.MaxLimit ?? maxVal : axis.MinLimit ?? minVal);
                }
                else
                {
                    var visibleMax = (float)(axis.IsInverted ? axis.VisibleDataBounds.Min : axis.VisibleDataBounds.Max);
                    var visibleMin = (float)(axis.IsInverted ? axis.VisibleDataBounds.Max : axis.VisibleDataBounds.Min);

                    if (visibleMax != maxVal || visibleMin != minVal)
                    {
                        maxVal = visibleMax;
                        minVal = visibleMin;
                    }
                }

                deltaVal = maxVal - minVal;
            }
            else
            {
                minPx = drawMaringLocation.Y;
                maxPx = drawMaringLocation.Y + drawMarginSize.Height;
                deltaPx = maxPx - minPx;

                maxVal = (float)(axis.IsInverted ? axis.DataBounds.Max : axis.DataBounds.Min);
                minVal = (float)(axis.IsInverted ? axis.DataBounds.Min : axis.DataBounds.Max);

                if (axis.MaxLimit != null || axis.MinLimit != null)
                {
                    maxVal = (float)(axis.IsInverted ? axis.MaxLimit ?? maxVal : axis.MinLimit ?? minVal);
                    minVal = (float)(axis.IsInverted ? axis.MinLimit ?? minVal : axis.MaxLimit ?? maxVal);
                }
                else
                {
                    var visibleMax = (float)(axis.IsInverted ? axis.VisibleDataBounds.Max : axis.VisibleDataBounds.Min);
                    var visibleMin = (float)(axis.IsInverted ? axis.VisibleDataBounds.Min : axis.VisibleDataBounds.Max);

                    if (visibleMax != maxVal || visibleMin != minVal)
                    {
                        maxVal = visibleMax;
                        minVal = visibleMin;
                    }
                }

                deltaVal = maxVal - minVal;
            }

            m = deltaPx / deltaVal;
            mInv = 1 / m;

            if (!double.IsNaN(m)) return;
            m = 0;
            mInv = 0;
        }

        internal Scaler()
        {
            minPx = 0;
            maxPx = 100;
            deltaPx = maxPx - minPx;

            maxVal = 0;
            minVal = 100;
            deltaVal = maxVal - minVal;

            m = deltaPx / deltaVal;
            mInv = 1 / m;
        }

        /// <summary>
        /// Converts to pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public float ToPixels(float value)
        {
            return minPx + (value - minVal) * m;
        }

        /// <summary>
        /// Converts to chartvalues.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <returns></returns>
        public float ToChartValues(float pixels)
        {
            return minVal + (pixels - minPx) * mInv;
        }
    }
}
