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
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines the polar scaler class, this class helps to scale from the data scale to the user interface scale and vise versa.
/// </summary>
public class PolarScaler
{
    private const double ToRadians = Math.PI / 180d;
    private readonly double _deltaRadius, _innerRadiusOffset, _outerRadiusOffset,
        _scalableRadius, _initialRotation, _deltaAngleVal, _circumference;

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarScaler"/> class.
    /// </summary>
    /// <param name="drawMarginLocation">The draw margin location.</param>
    /// <param name="drawMarginSize">Size of the draw margin.</param>
    /// <param name="radiusAxis">The radius axis.</param>
    /// <param name="angleAxis">The angle axis.</param>
    /// <param name="innerRadius">The inner radius.</param>
    /// <param name="initialRotation">The initial rotation.</param>
    /// <param name="totalAngle">The total angle.</param>
    /// <param name="usePreviousScale">Indicates if the scaler should be built based on the previous known data.</param>
    /// <exception cref="Exception">The axis is not ready to be scaled.</exception>
    public PolarScaler(
        LvcPoint drawMarginLocation,
        LvcSize drawMarginSize,
        IPolarAxis angleAxis,
        IPolarAxis radiusAxis,
        float innerRadius,
        float initialRotation,
        float totalAngle,
        bool usePreviousScale = false)
    {
        Bounds actualAngleBounds, actualAngleVisibleBounds, actualRadiusBounds, actualRadiusVisibleBounds;

        if (usePreviousScale)
        {
            actualAngleBounds = angleAxis.DataBounds;
            actualAngleVisibleBounds = angleAxis.VisibleDataBounds;
            actualRadiusBounds = radiusAxis.DataBounds;
            actualRadiusVisibleBounds = radiusAxis.VisibleDataBounds;
        }
        else
        {
            actualAngleBounds = angleAxis.DataBounds;
            actualAngleVisibleBounds = angleAxis.VisibleDataBounds;
            actualRadiusBounds = radiusAxis.DataBounds;
            actualRadiusVisibleBounds = radiusAxis.VisibleDataBounds;
        }

        //var actualAngleBounds = usePreviousScale ? angleAxis.PreviousDataBounds : angleAxis.DataBounds;
        //var actualAngleVisibleBounds = usePreviousScale ? angleAxis.PreviousVisibleDataBounds : angleAxis.VisibleDataBounds;

        //var actualRadiusBounds = usePreviousScale ? radiusAxis.PreviousDataBounds : radiusAxis.DataBounds;
        //var actualRadiusVisibleBounds = usePreviousScale ? radiusAxis.PreviousVisibleDataBounds : radiusAxis.VisibleDataBounds;

        if (actualAngleBounds is null || actualAngleVisibleBounds is null) throw new Exception("angle bounds not found");
        if (actualRadiusBounds is null || actualRadiusVisibleBounds is null) throw new Exception("radius bounds not found");

        CenterX = drawMarginLocation.X + drawMarginSize.Width * 0.5f;
        CenterY = drawMarginLocation.Y + drawMarginSize.Height * 0.5f;

        MinRadius = radiusAxis.MinLimit ?? actualRadiusVisibleBounds.Min;
        MaxRadius = radiusAxis.MaxLimit ?? actualRadiusVisibleBounds.Max;
        _deltaRadius = MaxRadius - MinRadius;

        var minDimension = drawMarginSize.Width < drawMarginSize.Height ? drawMarginSize.Width : drawMarginSize.Height;
        _innerRadiusOffset = innerRadius; // innerRadius;
        InnerRadius = innerRadius;
        _outerRadiusOffset = 0; //drawMagrinLocation.X; // We should also check for the top, right and bottom bounds.
        _scalableRadius = minDimension * 0.5 - _innerRadiusOffset - _outerRadiusOffset;

        MinAngle = angleAxis.MinLimit ?? actualAngleBounds.Min;
        MaxAngle = angleAxis.MaxLimit ?? actualAngleBounds.Max;
        _deltaAngleVal = MaxAngle - MinAngle;

        _initialRotation = initialRotation;
        _circumference = totalAngle;
    }

    /// <summary>
    /// Gets the center in the X axis.
    /// </summary>
    public float CenterX { get; private set; }

    /// <summary>
    /// Gets the center in the Y axis.
    /// </summary>
    public float CenterY { get; private set; }

    /// <summary>
    /// Gets the inner radius.
    /// </summary>
    public double InnerRadius { get; private set; }

    /// <summary>
    /// Gets the max radius in chart values scale.
    /// </summary>
    public double MaxRadius { get; private set; }

    /// <summary>
    /// Gets the min radius in chart values scale.
    /// </summary>
    public double MinRadius { get; private set; }

    /// <summary>
    /// Gets the max angle.
    /// </summary>
    public double MinAngle { get; private set; }

    /// <summary>
    /// Gets the min angle.
    /// </summary>
    public double MaxAngle { get; private set; }

    /// <summary>
    /// Converts to pixels.
    /// </summary>
    /// <param name="polarPoint">The polar point.</param>
    /// <returns></returns>
    public LvcPoint ToPixels(ChartPoint polarPoint)
    {
        var c = polarPoint.Coordinate;
        return ToPixels(c.SecondaryValue, c.PrimaryValue);
    }

    /// <summary>
    /// Converts to pixels.
    /// </summary>
    /// <param name="angle">The angle in chart values scale.</param>
    /// <param name="radius">The radius in chart values.</param>
    /// <returns></returns>
    public LvcPoint ToPixels(double angle, double radius)
    {
        var p = (radius - MinRadius) / _deltaRadius;
        var r = _innerRadiusOffset + _scalableRadius * p;
        var a = _circumference * angle / _deltaAngleVal;

        a += _initialRotation;
        a *= ToRadians;

        unchecked
        {
            return new LvcPoint(
                CenterX + (float)(Math.Cos(a) * r),
                CenterY + (float)(Math.Sin(a) * r));
        }
    }

    /// <summary>
    /// Converts to chart values.
    /// </summary>
    /// <param name="x">The x coordinate in pixels.</param>
    /// <param name="y">The y coordinate in pixels.</param>
    /// <returns></returns>
    public LvcPointD ToChartValues(double x, double y)
    {
        var dx = x - CenterX;
        var dy = y - CenterY;
        var hyp = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)) - _innerRadiusOffset;

        var r = hyp / _scalableRadius;

        var a = Math.Atan(dy / dx) * (1 / ToRadians);

        if (dx < 0 && dy > 0) a = 180 + a;
        if (dx < 0 && dy <= 0) a = 180 + a;
        if (dx > 0 && dy <= 0) a = 360 + a;

        a -= _initialRotation;
        if (a < 0) a = 360 - a;

        return new LvcPointD(
            MinAngle + _deltaAngleVal * a / _circumference,
            MinRadius + r * (MaxRadius - MinRadius));
    }

    /// <summary>
    /// Converts to pixels.
    /// </summary>
    /// <param name="angle">The angle in degrees.</param>
    /// <param name="radius">The radius.</param>
    /// <returns></returns>
    public LvcPoint ToPixelsWithAngleInDegrees(double angle, double radius)
    {
        var p = (radius - MinRadius) / _deltaRadius;
        var r = _innerRadiusOffset + _scalableRadius * p;
        var a = angle * ToRadians;

        unchecked
        {
            return new LvcPoint(
                CenterX + (float)(Math.Cos(a) * r),
                CenterY + (float)(Math.Sin(a) * r));
        }
    }

    /// <summary>
    /// Converts to degrees the given angle in chart values.
    /// </summary>
    /// <param name="angle">The angle in chart values.</param>
    /// <returns></returns>
    public float GetAngle(double angle)
    {
        return unchecked((float)(_initialRotation + _circumference * angle / _deltaAngleVal));
    }
}
