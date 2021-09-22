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
using System.Collections.Generic;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;

namespace LiveChartsCore.SkiaSharpView
{
    /// <summary>
    /// Defines skia sharp heat extensions.
    /// </summary>
    public static class GeoExtensions
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{PathShape}"/> that defines the feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="projector">The projector.</param>
        /// <returns></returns>
        public static IEnumerable<HeatPathShape> AsPathShape(
            this GeoJsonFeature feature,
            MapProjector projector)
        {
            var d = new double[0][][][];

            foreach (var geometry in feature.Geometry?.Coordinates ?? d)
            {
                for (var i = 0; i < geometry.Length; i++)
                {
                    yield return new HeatPathShape { IsClosed = true };
                }
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{PathShape}"/> that defines the feature.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="projector">The projector.</param>
        /// <returns></returns>
        public static IEnumerable<PathCommand> GetPathCommands(
            this GeoJsonFeature feature,
            MapProjector projector)
        {
            var d = new double[0][][][];

            foreach (var geometry in feature.Geometry?.Coordinates ?? d)
            {
                foreach (var segment in geometry)
                {
                    var path = new HeatPathShape { IsClosed = true };

                    var isFirst = true;
                    foreach (var point in segment)
                    {
                        var p = projector.ToMap(point);

                        if (isFirst)
                        {
                            isFirst = false;
                            yield return new MoveToPathCommand { X = p[0], Y = p[1] };
                            continue;
                        }

                        yield return new LineSegment { X = p[0], Y = p[1] };
                    }
                }
            }
        }
    }
}
