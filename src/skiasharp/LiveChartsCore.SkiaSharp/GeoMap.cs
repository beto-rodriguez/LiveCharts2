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

using System.IO;
using System.Threading.Tasks;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView;

/// <inheritdoc cref="CoreMap{TDrawingContext}"/>.
public class GeoMapCore : CoreMap<SkiaSharpDrawingContext>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeoMapCore"/> class from the given core map.
    /// </summary>
    public GeoMapCore()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoMapCore"/> class from the given core map.
    /// </summary>
    /// <param name="coreMap"></param>
    public GeoMapCore(CoreMap<SkiaSharpDrawingContext> coreMap)
    {
        Layers = coreMap.Layers;
    }

    /// <inheritdoc cref="Maps.GetWorldMap{TDrawingContext}"/>.
    public static GeoMapCore GetWorldMap()
    {
        return new GeoMapCore(Maps.GetWorldMap<SkiaSharpDrawingContext>());
    }

    /// <inheritdoc cref="Maps.GetMapFromDirectory{TDrawingContext}(string)"/>.
    public static GeoMapCore GetMapFromDirectory(string path)
    {
        return new GeoMapCore(Maps.GetMapFromDirectory<SkiaSharpDrawingContext>(path));
    }

    /// <inheritdoc cref="Maps.GetMapFromStreamReader{TDrawingContext}(StreamReader)"/>.
    public static GeoMapCore GetMapFromStreamReader(StreamReader stream)
    {
        return new GeoMapCore(Maps.GetMapFromStreamReader<SkiaSharpDrawingContext>(stream));
    }

    /// <inheritdoc cref="Maps.GetMapFromDirectory{TDrawingContext}(string)"/>.
    public static Task<GeoMapCore> GetMapFromDirectoryAsync(string path)
    {
        return Task.Run(() => new GeoMapCore(Maps.GetMapFromDirectory<SkiaSharpDrawingContext>(path)));
    }

    /// <inheritdoc cref="Maps.GetMapFromStreamReader{TDrawingContext}(StreamReader)"/>.
    public static Task<GeoMapCore> GetMapFromStreamReaderAsync(StreamReader stream)
    {
        return Task.Run(() => new GeoMapCore(Maps.GetMapFromStreamReader<SkiaSharpDrawingContext>(stream)));
    }
}
