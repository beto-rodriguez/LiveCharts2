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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines the scaler class, this class helps to scale from the data scale to the user interface scale and vise versa.
/// </summary>
public class Scaler
{
    private readonly double _deltaVal, _m, _mInv, _minPx, _maxPx, _deltaPx;
    private readonly AxisOrientation _orientation;

    /// <summary>
    /// Initializes a new instance of the <see cref="Scaler"/> class.
    /// </summary>
    /// <param name="drawMarginLocation">The draw margin location.</param>
    /// <param name="drawMarginSize">Size of the draw margin.</param>
    /// <param name="axis">The axis.</param>
    /// <param name="bounds">Indicates the bounds to use.</param>
    /// <exception cref="Exception">The axis is not ready to be scaled.</exception>
    public Scaler(
        LvcPoint drawMarginLocation,
        LvcSize drawMarginSize,
        ICartesianAxis axis,
        Bounds? bounds = null)
    {
        if (axis.Orientation == AxisOrientation.Unknown) throw new Exception("The axis is not ready to be scaled.");

        _orientation = axis.Orientation;

        var actualBounds = axis.DataBounds;
        var actualVisibleBounds = axis.VisibleDataBounds;
        var maxLimit = axis.MaxLimit;
        var minLimit = axis.MinLimit;

        if (bounds != null)
        {
            actualBounds = bounds;
            actualVisibleBounds = bounds;
            minLimit = null;
            maxLimit = null;
        }

        if (axis.Orientation == AxisOrientation.X)
        {
            _minPx = drawMarginLocation.X;
            _maxPx = drawMarginLocation.X + drawMarginSize.Width;
            _deltaPx = _maxPx - _minPx;

            MaxVal = axis.IsInverted ? actualBounds.Min : actualBounds.Max;
            MinVal = axis.IsInverted ? actualBounds.Max : actualBounds.Min;

            if (maxLimit is not null || minLimit is not null)
            {
                MaxVal = axis.IsInverted ? minLimit ?? MinVal : maxLimit ?? MaxVal;
                MinVal = axis.IsInverted ? maxLimit ?? MaxVal : minLimit ?? MinVal;
            }
            else
            {
                var visibleMax = axis.IsInverted ? actualVisibleBounds.Min : actualVisibleBounds.Max;
                var visibleMin = axis.IsInverted ? actualVisibleBounds.Max : actualVisibleBounds.Min;

                AxisLimit.ValidateLimits(ref visibleMin, ref visibleMax);

                if (visibleMax != MaxVal || visibleMin != MinVal)
                {
                    MaxVal = visibleMax;
                    MinVal = visibleMin;
                }
            }

            _deltaVal = MaxVal - MinVal;
        }
        else
        {
            _minPx = drawMarginLocation.Y;
            _maxPx = drawMarginLocation.Y + drawMarginSize.Height;
            _deltaPx = _maxPx - _minPx;

            MaxVal = axis.IsInverted ? actualBounds.Max : actualBounds.Min;
            MinVal = axis.IsInverted ? actualBounds.Min : actualBounds.Max;

            if (maxLimit is not null || minLimit is not null)
            {
                MaxVal = axis.IsInverted ? maxLimit ?? MinVal : minLimit ?? MaxVal;
                MinVal = axis.IsInverted ? minLimit ?? MaxVal : maxLimit ?? MinVal;
            }
            else
            {
                var visibleMax = axis.IsInverted ? actualVisibleBounds.Max : actualVisibleBounds.Min;
                var visibleMin = axis.IsInverted ? actualVisibleBounds.Min : actualVisibleBounds.Max;

                AxisLimit.ValidateLimits(ref visibleMax, ref visibleMin);

                if (visibleMax != MaxVal || visibleMin != MinVal)
                {
                    MaxVal = visibleMax;
                    MinVal = visibleMin;
                }
            }

            _deltaVal = MaxVal - MinVal;
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

        MaxVal = 0;
        MinVal = 100;
        _deltaVal = MaxVal - MinVal;

        _m = _deltaPx / _deltaVal;
        _mInv = 1 / _m;
    }

    /// <summary>
    /// Gets the maximum value.
    /// </summary>
    public double MaxVal { get; private set; }

    /// <summary>
    /// Gets the minimum value.
    /// </summary>
    public double MinVal { get; private set; }

    /// <summary>
    /// Measures an absolute value in pixels.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns></returns>
    public float MeasureInPixels(double value)
    {
        unchecked
        {
            return Math.Abs(
                _orientation == AxisOrientation.X
                    ? (float)(_minPx + (value - MinVal) * _m - (_minPx + (0 - MinVal) * _m))
                    : (float)(_minPx + (0 - MinVal) * _m - (_minPx + (value - MinVal) * _m)));
        }
    }

    /// <summary>
    /// Converts a given value (in chart values) to pixels.
    /// </summary>
    /// <param name="value">The value in chart values.</param>
    /// <returns></returns>
    public float ToPixels(double value)
    {
        return unchecked((float)(_minPx + (value - MinVal) * _m));
    }

    /// <summary>
    /// Converts a given value (in pixels) to chart values.
    /// </summary>
    /// <param name="pixels">The value in pixels.</param>
    /// <returns></returns>
    public double ToChartValues(double pixels)
    {
        return MinVal + (pixels - _minPx) * _mInv;
    }
}
