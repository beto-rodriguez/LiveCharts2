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
using System.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;

namespace LiveChartsCore.SkiaSharpView
{
    /// <summary>
    /// Defines skia sharp heat extensions.
    /// </summary>
    public static class HeatExtensions
    {
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
        public static IEnumerable<PathShape> AsHeatMapShapes(
            this GeoJsonFile geoJson,
            Dictionary<string, double> values,
            Color[] heatMap, List<Tuple<double, Color>> heatStops,
            Color stroke,
            Color fill,
            float thickness,
            MapProjector projector)
        {
            var paths = new List<PathShape>();

            var weightBounds = new Bounds();
            foreach (var value in values)
            {
                weightBounds.AppendValue(value.Value);
            }

            var d = new double[0][][][];

            foreach (var feature in geoJson.Features ?? new GeoJsonFeature[0])
            {
                var name = feature.Properties is not null ? feature.Properties["shortName"] : "";
                Color? baseColor = values.TryGetValue(name, out var weight)
                    ? HeatFunctions.InterpolateColor((float)weight, weightBounds, heatMap, heatStops)
                    : null;

                foreach (var geometry in feature.Geometry?.Coordinates ?? d)
                {
                    foreach (var segment in geometry)
                    {
                        var path = new PathShape
                        {
                            StrokeColor = stroke,
                            FillColor = baseColor ?? fill,
                            StrokeThickness = thickness,
                            IsClosed = true
                        };
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
            }

            return paths;
        }
    }
}
