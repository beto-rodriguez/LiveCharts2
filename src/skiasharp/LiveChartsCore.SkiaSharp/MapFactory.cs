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

using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;

namespace LiveChartsCore.SkiaSharpView
{
    /// <summary>
    /// Defines a map builder.
    /// </summary>
    public class MapFactory : IMapFactory<SkiaSharpDrawingContext>
    {
        /// <summary>
        /// Fetches the map features.
        /// </summary>
        /// <param name="mapFile">The map file.</param>
        /// <param name="projector">The current projector.</param>
        public IEnumerable<GeoJsonFeature> FetchFeatures(GeoJsonFile mapFile, MapProjector projector)
        {
            foreach (var feature in mapFile.Features ?? new GeoJsonFeature[0])
                yield return feature;
        }

        /// <summary>
        /// Fetches the map elements.
        /// </summary>
        /// <param name="mapView">The map view.</param>
        public IEnumerable<IMapElement> FetchMapElements(IGeoMapView<SkiaSharpDrawingContext> mapView)
        {
            foreach (var shape in mapView.Shapes)
                yield return shape;
        }

        /// <summary>
        /// Converts the given feature into a path geometry.
        /// </summary>
        /// <param name="mapFile">The map file.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="projector">The current projector.</param>
        public IEnumerable<IDrawable<SkiaSharpDrawingContext>> ConvertToPathShape(
            GeoJsonFile mapFile, GeoJsonFeature feature, MapProjector projector)
        {
            var paths = new List<HeatPathShape>();
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
    }
}
