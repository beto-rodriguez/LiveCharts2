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
using LiveChartsCore.Painting;

namespace LiveChartsCore.Drawing;

/// <inheritdoc cref="CoreGeometry" />
/// <summary>
/// Initializes a new instance of the <see cref="CoreGeometry"/> class.
/// </summary>
public abstract class CoreGeometry(bool hasGeometryTransform = false)
    : Animatable(hasGeometryTransform), IDrawable
{
    /// <summary>
    /// Measures the geometry.
    /// </summary>
    /// <param name="drawableTask">The drawable task.</param>
    /// <returns>the size of the geometry.</returns>
    public LvcSize Measure(Paint drawableTask)
    {
        var measure = OnMeasure(drawableTask);

        var r = RotateTransform;
        if (Math.Abs(r) > 0)
        {
            const double toRadians = Math.PI / 180;

            r %= 360;
            if (r < 0) r += 360;

            if (r > 180) r = 360 - r;
            if (r is > 90 and <= 180) r = 180 - r;

            var rRadians = r * toRadians;

            var w = (float)(Math.Cos(rRadians) * measure.Width + Math.Sin(rRadians) * measure.Height);
            var h = (float)(Math.Sin(rRadians) * measure.Width + Math.Cos(rRadians) * measure.Height);

            measure = new LvcSize(w, h);
        }

        return measure;
    }

    /// <summary>
    /// Called when the geometry is measured.
    /// </summary>
    /// <param name="paintTasks">The paint task.</param>
    /// <returns>the size of the geometry</returns>
    public abstract LvcSize OnMeasure(Paint paintTasks);
}
