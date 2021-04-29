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
using LiveChartsCore.Kernel;

namespace LiveChartsCore.Measure
{
    /// <summary>
    /// Defines the scaler class, this class helps to scale from the data scale to the user interface scale and vise versa.
    /// </summary>
    public class Scaler
    {
        private readonly float _m, _mInv, _minPx, _maxPx, _deltaPx, _minVal, _maxVal, _deltaVal;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scaler"/> class.
        /// </summary>
        /// <param name="drawMaringLocation">The draw margin location.</param>
        /// <param name="drawMarginSize">Size of the draw margin.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="actualBounds">The bounds to use, if null then use the axis bounds.</param>
        /// <param name="actualVisibleBounds">The previous visible bounds to use, if null then use the axis bounds.</param>
        /// <exception cref="Exception">The axis is not ready to be scaled.</exception>
        public Scaler(
            PointF drawMaringLocation, SizeF drawMarginSize, IAxis axis,
            Bounds? actualBounds = null, Bounds? actualVisibleBounds = null)
        {
            if (axis.Orientation == AxisOrientation.Unknown)
                throw new Exception("The axis is not ready to be scaled.");

            actualBounds ??= axis.DataBounds;
            actualVisibleBounds ??= axis.VisibleDataBounds;

            if (axis.Orientation == AxisOrientation.X)
            {
                _minPx = drawMaringLocation.X;
                _maxPx = drawMaringLocation.X + drawMarginSize.Width;
                _deltaPx = _maxPx - _minPx;

                _maxVal = (float)(axis.IsInverted ? actualBounds.Min : actualBounds.Max);
                _minVal = (float)(axis.IsInverted ? actualBounds.Max : actualBounds.Min);

                if (axis.MaxLimit != null || axis.MinLimit != null)
                {
                    _maxVal = (float)(axis.IsInverted ? axis.MinLimit ?? _minVal : axis.MaxLimit ?? _maxVal);
                    _minVal = (float)(axis.IsInverted ? axis.MaxLimit ?? _maxVal : axis.MinLimit ?? _minVal);
                }
                else
                {
                    var visibleMax = (float)(axis.IsInverted ? actualVisibleBounds.Min : actualVisibleBounds.Max);
                    var visibleMin = (float)(axis.IsInverted ? actualVisibleBounds.Max : actualVisibleBounds.Min);

                    if (visibleMax != _maxVal || visibleMin != _minVal)
                    {
                        _maxVal = visibleMax;
                        _minVal = visibleMin;
                    }
                }

                _deltaVal = _maxVal - _minVal;
            }
            else
            {
                _minPx = drawMaringLocation.Y;
                _maxPx = drawMaringLocation.Y + drawMarginSize.Height;
                _deltaPx = _maxPx - _minPx;

                _maxVal = (float)(axis.IsInverted ? actualBounds.Max : actualBounds.Min);
                _minVal = (float)(axis.IsInverted ? actualBounds.Min : actualBounds.Max);

                if (axis.MaxLimit != null || axis.MinLimit != null)
                {
                    _maxVal = (float)(axis.IsInverted ? axis.MaxLimit ?? _maxVal : axis.MinLimit ?? _minVal);
                    _minVal = (float)(axis.IsInverted ? axis.MinLimit ?? _minVal : axis.MaxLimit ?? _maxVal);
                }
                else
                {
                    var visibleMax = (float)(axis.IsInverted ? actualVisibleBounds.Max : actualVisibleBounds.Min);
                    var visibleMin = (float)(axis.IsInverted ? actualVisibleBounds.Min : actualVisibleBounds.Max);

                    if (visibleMax != _maxVal || visibleMin != _minVal)
                    {
                        _maxVal = visibleMax;
                        _minVal = visibleMin;
                    }
                }

                _deltaVal = _maxVal - _minVal;
            }

            _m = _deltaPx / _deltaVal;
            _mInv = 1 / _m;

            if (!double.IsNaN(_m)) return;
            _m = 0;
            _mInv = 0;
        }

        internal Scaler()
        {
            _minPx = 0;
            _maxPx = 100;
            _deltaPx = _maxPx - _minPx;

            _maxVal = 0;
            _minVal = 100;
            _deltaVal = _maxVal - _minVal;

            _m = _deltaPx / _deltaVal;
            _mInv = 1 / _m;
        }

        /// <summary>
        /// Converts to pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public float ToPixels(float value)
        {
            return _minPx + (value - _minVal) * _m;
        }

        /// <summary>
        /// Converts to chart values.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <returns></returns>
        public float ToChartValues(float pixels)
        {
            return _minVal + (pixels - _minPx) * _mInv;
        }
    }
}
