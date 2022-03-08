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

namespace LiveChartsCore.Drawing.Segments;

/// <summary>
/// Defines a path segment that is part of a sequence.
/// </summary>
public interface IConsecutivePathSegment : IAnimatable
{
    /// <summary>
    /// Gets or sets the segment id, a unique and consecutive integer.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Gets the start point in the X axis.
    /// </summary>
    float Xi { get; }

    /// <summary>
    /// Gets the end point in the X axis.
    /// </summary>
    float Xj { get; }

    /// <summary>
    /// Gets the start point in the Y axis.
    /// </summary>
    float Yi { get; }

    /// <summary>
    /// Gets the end point in the Y axis.
    /// </summary>
    float Yj { get; }

    /// <summary>
    /// Copies the segment to the end of the given segment.
    /// </summary>
    /// <param name="segment">The segment.</param>
    void Follows(IConsecutivePathSegment segment);
}
