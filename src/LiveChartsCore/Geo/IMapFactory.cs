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

namespace LiveChartsCore.Geo
{
    /// <summary>
    /// Defines a map factory.
    /// </summary>
    public interface IMapFactory<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Fetches the map features.
        /// </summary>
        /// <param name="mapFile">The map file.</param>
        /// <param name="projector">The current projector.</param>
        IEnumerable<GeoJsonFeature> FetchFeatures(GeoJsonFile mapFile, MapProjector projector);

        /// <summary>
        /// Fetches the map elements.
        /// </summary>
        /// <param name="mapView">The map view.</param>
        IEnumerable<IMapElement> FetchMapElements(IGeoMapView<TDrawingContext> mapView);

        /// <summary>
        /// Converts the given feature into a path geometry.
        /// </summary>
        /// <param name="mapFile">The map file.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="projector">The current projector.</param>
        IEnumerable<IDrawable<TDrawingContext>> ConvertToPathShape(
            GeoJsonFile mapFile, GeoJsonFeature feature, MapProjector projector);
    }
}
