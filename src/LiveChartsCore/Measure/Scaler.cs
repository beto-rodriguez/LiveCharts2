﻿// The MIT License(MIT)
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
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Measure
{
    /// <summary>
    /// Defines the scaler class, this class helps to scale from the data scale to the user interface scale and vise versa.
    /// </summary>
    public class Scaler
    {
        private readonly double _minVal, _maxVal, _deltaVal, _m, _mInv, _minPx, _maxPx, _deltaPx;
        private readonly AxisOrientation _orientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scaler"/> class.
        /// </summary>
        /// <param name="drawMagrinLocation">The draw margin location.</param>
        /// <param name="drawMarginSize">Size of the draw margin.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="usePreviousScale">Indicates if the scaler should be built based on the previous known data.</param>
        /// <exception cref="Exception">The axis is not ready to be scaled.</exception>
        public Scaler(
            PointF drawMagrinLocation, SizeF drawMarginSize, IAxis axis, bool usePreviousScale = false)
        {
            if (axis.Orientation == AxisOrientation.Unknown) throw new Exception("The axis is not ready to be scaled.");

            _orientation = axis.Orientation;

            var actualBounds = usePreviousScale ? axis.PreviousDataBounds : axis.DataBounds;
            var actualVisibleBounds = usePreviousScale ? axis.PreviousVisibleDataBounds : axis.VisibleDataBounds;
            var maxLimit = usePreviousScale ? axis.PreviousMaxLimit : axis.MaxLimit;
            var minLimit = usePreviousScale ? axis.PreviousMinLimit : axis.MinLimit;

            if (actualBounds is null || actualVisibleBounds is null) throw new Exception("bounds not found");

            if (double.IsInfinity(actualBounds.Delta) || double.IsInfinity(actualVisibleBounds.Delta))
            {
                _maxVal = 0;
                _minVal = 0;
                _deltaVal = 0;

                if (axis.Orientation == AxisOrientation.X)
                {
                    _minPx = drawMagrinLocation.X;
                    _maxPx = drawMagrinLocation.X + drawMarginSize.Width;
                    _deltaPx = _maxPx - _minPx;
                }
                else
                {
                    _minPx = drawMagrinLocation.Y;
                    _maxPx = drawMagrinLocation.Y + drawMarginSize.Height;
                    _deltaPx = _maxPx - _minPx;
                }

                _m = 0;
                _mInv = 0;

                return;
            }

            if (axis.Orientation == AxisOrientation.X)
            {
                _minPx = drawMagrinLocation.X;
                _maxPx = drawMagrinLocation.X + drawMarginSize.Width;
                _deltaPx = _maxPx - _minPx;

                _maxVal = axis.IsInverted ? actualBounds.Min : actualBounds.Max;
                _minVal = axis.IsInverted ? actualBounds.Max : actualBounds.Min;

                if (maxLimit is not null || minLimit is not null)
                {
                    _maxVal = axis.IsInverted ? minLimit ?? _minVal : maxLimit ?? _maxVal;
                    _minVal = axis.IsInverted ? maxLimit ?? _maxVal : minLimit ?? _minVal;
                }
                else
                {
                    var visibleMax = axis.IsInverted ? actualVisibleBounds.Min : actualVisibleBounds.Max;
                    var visibleMin = axis.IsInverted ? actualVisibleBounds.Max : actualVisibleBounds.Min;

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
                _minPx = drawMagrinLocation.Y;
                _maxPx = drawMagrinLocation.Y + drawMarginSize.Height;
                _deltaPx = _maxPx - _minPx;

                _maxVal = axis.IsInverted ? actualBounds.Max : actualBounds.Min;
                _minVal = axis.IsInverted ? actualBounds.Min : actualBounds.Max;

                if (maxLimit is not null || minLimit is not null)
                {
                    _maxVal = axis.IsInverted ? maxLimit ?? _maxVal : minLimit ?? _minVal;
                    _minVal = axis.IsInverted ? minLimit ?? _minVal : maxLimit ?? _maxVal;
                }
                else
                {
                    var visibleMax = axis.IsInverted ? actualVisibleBounds.Max : actualVisibleBounds.Min;
                    var visibleMin = axis.IsInverted ? actualVisibleBounds.Min : actualVisibleBounds.Max;

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

            if (!double.IsNaN(_m) && !double.IsInfinity(_m)) return;
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
        public float MeasureInPixels(double value)
        {
            unchecked
            {
                return _orientation == AxisOrientation.X
                    ? (float)(_minPx + (value - _minVal) * _m - (_minPx + (0 - _minVal) * _m))
                    : (float)(_minPx + (0 - _minVal) * _m - (_minPx + (value - _minVal) * _m));
            }
        }

        /// <summary>
        /// Converts to pixels.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public float ToPixels(double value)
        {
            return unchecked((float)(_minPx + (value - _minVal) * _m));
        }

        /// <summary>
        /// Converts to chart values.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <returns></returns>
        public double ToChartValues(double pixels)
        {
            return _minVal + (pixels - _minPx) * _mInv;
        }
    }
}
