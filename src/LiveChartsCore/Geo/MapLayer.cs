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
using System.Linq;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Geo;

/// <summary>
/// Defines a map layer.
/// </summary>
public class MapLayer<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Initializes a new <see cref="MapLayer{TDrawingContext}"/> from the given <see cref="GeoJsonFile"/>.
    /// </summary>
    /// <param name="layerName">The layer name.</param>
    /// <param name="stroke">The stroke.</param>
    /// <param name="fill">The fill.</param>
    public MapLayer(string layerName, IPaint<TDrawingContext> stroke, IPaint<TDrawingContext> fill)
    {
        Name = layerName;
        Stroke = stroke;
        Fill = fill;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the layer process index.
    /// </summary>
    public int ProcessIndex { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this layer is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    public IPaint<TDrawingContext>? Stroke { get; set; }

    /// <summary>
    /// Gets or sets the fill.
    /// </summary>
    public IPaint<TDrawingContext>? Fill { get; set; }

    /// <summary>
    /// Gets or sets the X bounds.
    /// </summary>
    public double[] Max { get; set; } = Array.Empty<double>();

    /// <summary>
    /// Gets or sets the Y bounds.
    /// </summary>
    public double[] Min { get; set; } = Array.Empty<double>();

    /// <summary>
    /// Gets the lands.
    /// </summary>
    public Dictionary<string, LandDefinition> Lands { get; private set; } = new();

    /// <summary>
    /// Gets or sets the land condition, it must return true if the land is required.
    /// </summary>
    public Func<LandDefinition, CoreMap<TDrawingContext>, bool>? AddLandWhen { get; set; }

    /// <summary>
    /// Adds a GeoJson file to the layer.
    /// </summary>
    /// <param name="file"></param>
    public void AddFile(GeoJsonFile file)
    {
        if (file.Features is null)
            throw new Exception(
                $"The {nameof(GeoJsonFile.Features)} property is required to build a {nameof(CoreMap<TDrawingContext>)} instance. " +
                $"Ensure the property is not null.");

        foreach (var feature in file.Features)
        {
            if (feature.Geometry is null || feature.Geometry.Coordinates is null) continue;

            var name = (feature.Properties?["name"] ?? "?").ToLowerInvariant();
            var shortName = (feature.Properties?["shortName"] ?? "?").ToLowerInvariant();
            var setOf = (feature.Properties?["setOf"] ?? "?").ToLowerInvariant();

            var definition = new LandDefinition(shortName, name, setOf);

            var dataCollection = new List<LandData>();

            foreach (var geometry in feature.Geometry.Coordinates)
            {
                foreach (var segment in geometry)
                {
                    var data = new LandData(segment);

                    if (data.MaxBounds[0] > definition.MaxBounds[0]) definition.MaxBounds[0] = data.MaxBounds[0];
                    if (data.MinBounds[0] < definition.MinBounds[0]) definition.MinBounds[0] = data.MinBounds[0];

                    if (data.MaxBounds[1] > definition.MaxBounds[1]) definition.MaxBounds[1] = data.MaxBounds[1];
                    if (data.MinBounds[1] < definition.MinBounds[1]) definition.MinBounds[1] = data.MinBounds[1];

                    dataCollection.Add(data);
                }
            }

            definition.Data = dataCollection.OrderByDescending(x => x.BoundsHypotenuse).ToArray();
            Lands.Add(shortName, definition);
        }
    }
}
