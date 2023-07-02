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
using System.IO;
using System.Threading.Tasks;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Geo;

/// <summary>
/// Defines a geographic map for LiveCharts controls.
/// </summary>
public class CoreMap<TDrawingContext> : IDisposable
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CoreMap{TDrawingContext}"/> class.
    /// </summary>
    public CoreMap() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreMap{TDrawingContext}"/> class, with the given layer.
    /// </summary>
    /// <param name="path">The path to the GeoJson file for the layer.</param>
    /// <param name="layerName">The layer name.</param>
    public CoreMap(string path, string layerName = "default") : this(new StreamReader(path), layerName)
    {
        _ = AddLayerFromDirectory(path, layerName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreMap{TDrawingContext}"/> class, with the given layer.
    /// </summary>
    /// <param name="streamReader">The stream reader instance of the GeoJson file for the layer.</param>
    /// <param name="layerName">The layer name.</param>
    public CoreMap(StreamReader streamReader, string layerName = "default")
    {
        _ = AddLayerFromStreamReader(streamReader, layerName);
    }

    /// <summary>
    /// Gets the map layers dictionary.
    /// </summary>
    public Dictionary<string, MapLayer<TDrawingContext>> Layers { get; protected set; } = new();

    /// <summary>
    /// Finds a land by short name.
    /// </summary>
    /// <param name="shortName">The short name.</param>
    /// <param name="layerName">The layer name.</param>
    /// <returns>The land, null if not found.</returns>
    public LandDefinition? FindLand(string shortName, string layerName = "default")
    {
        return Layers[layerName].Lands.TryGetValue(shortName, out var land) ? land : null;
    }

    /// <summary>
    /// Adds a layer to the map from a directory.
    /// </summary>
    /// <param name="path">The path to the GeoJson file for the layer.</param>
    /// <param name="layerName">The layer name.</param>
    /// <param name="stroke">The stroke.</param>
    /// <param name="fill">The fill.</param>
    /// <returns>The added layer.</returns>
    public MapLayer<TDrawingContext> AddLayerFromDirectory(
        string path, IPaint<TDrawingContext> stroke, IPaint<TDrawingContext> fill, string layerName = "default")
    {
        using var sr = new StreamReader(path);
        return AddLayerFromStreamReader(sr, stroke, fill, layerName);
    }

    /// <summary>
    /// Adds a layer to the map from a directory.
    /// </summary>
    /// <param name="path">The path to the GeoJson file for the layer.</param>
    /// <param name="layerName">The layer name.</param>
    /// <returns>The added layer.</returns>
    public MapLayer<TDrawingContext> AddLayerFromDirectory(string path, string layerName = "default")
    {
        var provider = LiveCharts.DefaultSettings.GetProvider<TDrawingContext>();
        var stroke = provider.GetSolidColorPaint(new(33, 150, 243));
        var fill = provider.GetSolidColorPaint(new(33, 150, 243, 50));

        return AddLayerFromDirectory(path, stroke, fill, layerName);
    }

    /// <summary>
    /// Adds a layer to the map from a stream reader.
    /// </summary>
    /// <param name="streamReader">The path to the stream reader.</param>
    /// <param name="layerName">The layer name.</param>
    /// <param name="stroke">The stroke.</param>
    /// <param name="fill">The fill.</param>
    /// <returns>The added layer.</returns>
    public MapLayer<TDrawingContext> AddLayerFromStreamReader(
        StreamReader streamReader, IPaint<TDrawingContext> stroke, IPaint<TDrawingContext> fill, string layerName = "default")
    {
        if (!Layers.TryGetValue(layerName, out var layer))
        {
            layer = new MapLayer<TDrawingContext>(layerName, stroke, fill);
            Layers.Add(layerName, layer);
        }

        GeoJsonFile? geoJson;

#if NET5_0_OR_GREATER
        geoJson = System.Text.Json.JsonSerializer.Deserialize<GeoJsonFile>(
            streamReader.ReadToEnd(),
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            });
#else
        geoJson = Newtonsoft.Json.JsonConvert.DeserializeObject<GeoJsonFile>(streamReader.ReadToEnd());
#endif

        if (geoJson is null) throw new Exception("Map not found");
        layer.AddFile(geoJson);
        return layer;
    }

    /// <summary>
    /// Adds a layer to the map from a stream reader.
    /// </summary>
    /// <param name="streamReader">The path to the stream reader.</param>
    /// <param name="layerName">The layer name.</param>
    /// <returns>The added layer.</returns>
    public MapLayer<TDrawingContext> AddLayerFromStreamReader(StreamReader streamReader, string layerName = "default")
    {
        var provider = LiveCharts.DefaultSettings.GetProvider<TDrawingContext>();
        var stroke = provider.GetSolidColorPaint(new(33, 150, 243));
        var fill = provider.GetSolidColorPaint(new(33, 150, 243, 50));

        return AddLayerFromStreamReader(streamReader, stroke, fill, layerName);
    }

    /// <summary>
    /// Adds a layer to the map from a directory asynchronously.
    /// </summary>
    /// <param name="path">The path to the GeoJson file for the layer.</param>
    /// <param name="layerName">The layer name.</param>
    /// <param name="stroke">The stroke.</param>
    /// <param name="fill">The fill.</param>
    /// <returns>The added layer as await-able task.</returns>
    public Task<MapLayer<TDrawingContext>> AddLayerFromDirectoryAsync(
        string path, IPaint<TDrawingContext> stroke, IPaint<TDrawingContext> fill, string layerName = "default")
    {
        return Task.Run(() => AddLayerFromDirectory(path, stroke, fill, layerName));
    }

    /// <summary>
    /// Adds a layer to the map from a directory asynchronously.
    /// </summary>
    /// <param name="path">The path to the GeoJson file for the layer.</param>
    /// <param name="layerName">The layer name.</param>
    /// <returns>The added layer as await-able task.</returns>
    public Task<MapLayer<TDrawingContext>> AddLayerFromDirectoryAsync(string path, string layerName = "default")
    {
        var provider = LiveCharts.DefaultSettings.GetProvider<TDrawingContext>();
        var stroke = provider.GetSolidColorPaint(new(33, 150, 243));
        var fill = provider.GetSolidColorPaint(new(33, 150, 243, 50));

        return Task.Run(() => AddLayerFromDirectory(path, stroke, fill, layerName));
    }

    /// <summary>
    /// Adds a layer to the map from a stream reader asynchronously.
    /// </summary>
    /// <param name="streamReader">The path to the stream reader.</param>
    /// <param name="layerName">The layer name.</param>
    /// <param name="stroke">The stroke.</param>
    /// <param name="fill">The fill.</param>
    /// <returns>The added layer as await-able task.</returns>
    public Task<MapLayer<TDrawingContext>> AddLayerFromStreamReaderAsync(
        StreamReader streamReader, IPaint<TDrawingContext> stroke, IPaint<TDrawingContext> fill, string layerName = "default")
    {
        return Task.Run(() => AddLayerFromStreamReader(streamReader, stroke, fill, layerName));
    }

    /// <summary>
    /// Adds a layer to the map from a stream reader asynchronously.
    /// </summary>
    /// <param name="streamReader">The path to the stream reader.</param>
    /// <param name="layerName">The layer name.</param>
    /// <returns>The added layer as await-able task.</returns>
    public Task<MapLayer<TDrawingContext>> AddLayerFromStreamReaderAsync(StreamReader streamReader, string layerName = "default")
    {
        var provider = LiveCharts.DefaultSettings.GetProvider<TDrawingContext>();
        var stroke = provider.GetSolidColorPaint(new(33, 150, 243));
        var fill = provider.GetSolidColorPaint(new(33, 150, 243, 50));

        return Task.Run(() => AddLayerFromStreamReader(streamReader, stroke, fill, layerName));
    }

    /// <summary>
    /// Disposes the map.
    /// </summary>
    public void Dispose()
    {
        Layers.Clear();
    }
}
