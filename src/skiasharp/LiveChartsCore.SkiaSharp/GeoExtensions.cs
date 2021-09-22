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
using LiveChartsCore.Drawing;
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
        /// <param name="predicate">The predicate, the action to apply to the built paths.</param>
        /// <returns></returns>
        public static IEnumerable<PathShape> AsPathShape(
            this GeoJsonFeature feature,
            MapProjector projector,
            Action<PathShape>? predicate = null)
        {
            var paths = new List<PathShape>();
            var d = new double[0][][][];

            foreach (var geometry in feature.Geometry?.Coordinates ?? d)
            {
                foreach (var segment in geometry)
                {
                    var path = new PathShape { IsClosed = true };
                    if (predicate is not null) predicate(path);

                    var isFirst = true;
                    foreach (var point in segment)
                    {
                        var p = projector.ToMap(point);

                        if (isFirst)
                        {
                            isFirst = false;
                            path.AddCommand(new MoveToPathCommand { X = p[0], Y = p[1] });
                            continue;
                        }

                        path.AddCommand(new LineSegment { X = p[0], Y = p[1] });
                    }

                    paths.Add(path);
                }
            }

            return paths;
        }

        /// <summary>
        /// Returns a geoJson map into a heat map.
        /// </summary>
        /// <param name="geoJson">The geoJson.</param>
        /// <param name="values">The values.</param>
        /// <param name="heatMap">The heat map.</param>
        /// <param name="heatStops">The heat stops.</param>
        /// <param name="stroke">The stroke.</param>
        /// <param name="fill">The fill.</param>
        /// <param name="thickness">The thickness.</param>
        /// <param name="projector">The projector.</param>
        /// <returns></returns>
        public static IEnumerable<PathShape> AsMapShapes(
            this GeoJsonFile geoJson,
            LvcColor stroke,
            LvcColor fill,
            float thickness,
            MapProjector projector)
        {
            var paths = new List<PathShape>();

            LvcColor? baseColor = null;

            foreach (var feature in geoJson.Features ?? new GeoJsonFeature[0])
            {
                paths.AddRange(
                    feature.AsPathShape(projector, p =>
                    {
                        p.StrokeColor = stroke;
                        p.FillColor = baseColor ?? fill;
                        p.StrokeThickness = thickness;
                    }));
            }

            return paths;
        }
    }
}
