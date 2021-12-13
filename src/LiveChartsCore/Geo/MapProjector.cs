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
/// The map projector class.
/// </summary>
public abstract class MapProjector
{
    /// <summary>
    /// Gets the map width.
    /// </summary>
    public float MapWidth { get; protected set; }

    /// <summary>
    /// Gets the map height.
    /// </summary>
    public float MapHeight { get; protected set; }

    /// <summary>
    /// Gets the x offset width.
    /// </summary>
    public float XOffset { get; protected set; }

    /// <summary>
    /// Gets the y offset.
    /// </summary>
    public float YOffset { get; protected set; }

    /// <summary>
    /// Projects the given point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns></returns>
    public abstract float[] ToMap(double[] point);
}
