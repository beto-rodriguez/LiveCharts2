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

namespace LiveChartsCore.Geo;

/// <summary>
/// Defines a map context.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="MapContext"/> class.
/// </remarks>
public class MapContext(
    GeoMapChart core,
    IGeoMapView view,
    DrawnMap map,
    MapProjector projector)
{
    /// <summary>
    /// Gets the core map.
    /// </summary>
    public GeoMapChart CoreMap { get; } = core;

    /// <summary>
    /// Gets the map file.
    /// </summary>
    public DrawnMap MapFile { get; } = map;

    /// <summary>
    /// Gets the map projector.
    /// </summary>
    public MapProjector Projector { get; } = projector;

    /// <summary>
    /// Gets the map view.
    /// </summary>
    public IGeoMapView View { get; } = view;
}
