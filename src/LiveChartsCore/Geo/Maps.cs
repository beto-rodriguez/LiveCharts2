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
using System.IO;
using System.Reflection;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.Geo;

/// <summary>
/// Defines the maps
/// </summary>
public static class Maps
{
    /// <summary>
    /// Gets the world map.
    /// </summary>
    /// <returns>The map.</returns>
    /// <exception cref="Exception">Map not found</exception>
    public static CoreMap<TDrawingContext> GetWorldMap<TDrawingContext>()
        where TDrawingContext : DrawingContext
    {
        var a = Assembly.GetExecutingAssembly();
        var map = "LiveChartsCore.Geo.world.geojson";
        using var reader = new StreamReader(a.GetManifestResourceStream(map) ?? throw new Exception("file not found"));

        return GetMapFromStreamReader<TDrawingContext>(reader);
    }

    /// <summary>
    /// Gets a map from a specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The map.</returns>
    public static CoreMap<TDrawingContext> GetMapFromDirectory<TDrawingContext>(string path)
        where TDrawingContext : DrawingContext
    {
        using var sr = new StreamReader(path);

        return GetMapFromStreamReader<TDrawingContext>(sr);
    }

    /// <summary>
    /// Gets a map from a specified stream.
    /// </summary>
    /// <param name="stream">The stream.</param>
    /// <returns>The map.</returns>
    public static CoreMap<TDrawingContext> GetMapFromStreamReader<TDrawingContext>(StreamReader stream)
        where TDrawingContext : DrawingContext
    {
        return new CoreMap<TDrawingContext>(stream, "default");
    }

    /// <summary>
    /// Builds a projector with the given parameters.
    /// </summary>
    /// <param name="projection">The projection.</param>
    /// <param name="mapSize">Size of the map.</param>
    /// <returns></returns>
    public static MapProjector BuildProjector(MapProjection projection, float[] mapSize)
    {
        var mapRatio =
            projection == MapProjection.Default
            ? ControlCoordinatesProjector.PreferredRatio
            : MercatorProjector.PreferredRatio;

        var normalizedW = mapSize[0] / mapRatio[0];
        var normalizedH = mapSize[1] / mapRatio[1];
        float ox = 0f, oy = 0f;

        if (normalizedW < normalizedH)
        {
            var h = mapSize[0] * mapRatio[1] / mapRatio[0];
            oy = (float)(mapSize[1] - h) * 0.5f;
            mapSize[1] = h;
        }
        else
        {
            var w = mapSize[1] * mapRatio[0] / mapRatio[1];
            ox = (float)(mapSize[0] - w) * 0.5f;
            mapSize[0] = w;
        }

        return
            projection == MapProjection.Default
            ? new ControlCoordinatesProjector(mapSize[0], mapSize[1], ox, oy)
            : new MercatorProjector(mapSize[0], mapSize[1], ox, oy);
    }
}
