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
    /// Defines a geographic map for LiveCharts controls.
    /// </summary>
    public class CoreMap<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreMap{TDrawingContext}"/> class.
        /// </summary>
        public CoreMap()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreMap{TDrawingContext}"/> class, with the given layer.
        /// </summary>
        /// <param name="file">The geojson file for the default layer.</param>
        /// <param name="layerName">The layer name.</param>
        public CoreMap(GeoJsonFile file, string layerName)
        {
            _ = AddLayer(file, layerName);
        }

        /// <summary>
        /// Gets the map layers dictionary.
        /// </summary>
        public Dictionary<string, MapLayer<TDrawingContext>> Layers { get; protected set; } = new Dictionary<string, MapLayer<TDrawingContext>>();

        /// <summary>
        /// Finds a land by short name.
        /// </summary>
        /// <param name="shortName">The short name.</param>
        /// <param name="layerName">The layer name.</param>
        /// <returns>The land, null if not found.</returns>
        public LandDefinition? FindLand(string layerName, string shortName)
        {
            return Layers[layerName].Lands.TryGetValue(shortName, out var land) ? land : null;
        }

        /// <summary>
        /// Adds a layer to the map.
        /// </summary>
        /// <param name="file">The GeoJson file of the layer.</param>
        /// <param name="layerName">The layer name.</param>
        public CoreMap<TDrawingContext> AddLayer(GeoJsonFile file, string layerName)
        {
            Layers.Add(layerName, new MapLayer<TDrawingContext>(file, layerName));
            return this;
        }
    }
}
