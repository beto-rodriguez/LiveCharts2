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
using LiveChartsCore.Measure;

namespace LiveChartsCore.Geo;

/// <summary>
/// Defines the map shape context class.
/// </summary>
public class MapShapeContext<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapShapeContext{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="chart">The chart.</param>
    /// <param name="heatPaint">The heat paint.</param>
    /// <param name="heatStops">The heat stops.</param>
    /// <param name="bounds">The bounds.</param>
    public MapShapeContext(
        IGeoMapView<TDrawingContext> chart,
        IPaint<TDrawingContext> heatPaint,
        List<Tuple<double, LvcColor>> heatStops,
        Bounds bounds)
    {
        Chart = chart;
        HeatPaint = heatPaint;
        HeatStops = heatStops;
        Bounds = bounds;
    }

    /// <summary>
    /// Gets the chart.
    /// </summary>
    public IGeoMapView<TDrawingContext> Chart { get; }

    /// <summary>
    /// Gets the heat paint.
    /// </summary>
    public IPaint<TDrawingContext> HeatPaint { get; }

    /// <summary>
    /// Gets the heat stops.
    /// </summary>
    public List<Tuple<double, LvcColor>> HeatStops { get; }

    /// <summary>
    /// Gets the bounds dictionary.
    /// </summary>
    public Bounds Bounds { get; }
}
