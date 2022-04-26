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

namespace LiveChartsCore.Geo;

/// <summary>
/// Projects latitude and longitude coordinates using the Mercator projection.
/// </summary>
/// <seealso cref="MapProjector" />
public class MercatorProjector : MapProjector
{
    private readonly float _w;
    private readonly float _h;
    private readonly float _ox;
    private readonly float _oy;

    /// <summary>
    /// Initializes a new instance of the <see cref="MercatorProjector"/> class.
    /// </summary>
    /// <param name="mapWidth">Width of the map.</param>
    /// <param name="mapHeight">Height of the map.</param>
    /// <param name="offsetX">The offset x.</param>
    /// <param name="offsetY">The offset y.</param>
    public MercatorProjector(float mapWidth, float mapHeight, float offsetX, float offsetY)
    {
        _w = mapWidth;
        _h = mapHeight;
        _ox = offsetX;
        _oy = offsetY;
        XOffset = _ox;
        YOffset = _oy;
        MapWidth = mapWidth;
        MapHeight = mapHeight;
    }

    /// <summary>
    /// Gets the preferred ratio.
    /// </summary>
    /// <value>
    /// The preferred ratio.
    /// </value>
    public static float[] PreferredRatio => new[] { 1f, 1f };

    /// <inheritdoc cref="MapProjector.ToMap(double[])"/>
    public override float[] ToMap(double[] point)
    {
        var lat = point[1];
        var lon = point[0];

        var latRad = lat * Math.PI / 180d;
        var mercN = Math.Log(Math.Tan(Math.PI / 4d + latRad / 2d), Math.E);
        var y = _h / 2d - _h * mercN / (2 * Math.PI);

        return new[]
        {
            // x' =
            (float)((lon + 180) * (_w / 360d) + _ox),

            // y' =
            (float) y + _oy
        };
    }
}
