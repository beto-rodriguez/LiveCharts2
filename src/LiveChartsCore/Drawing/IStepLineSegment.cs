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

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defiens a stepline segment.
/// </summary>
/// <typeparam name="TPathContext">The type of the path context.</typeparam>
/// <seealso cref="IPathCommand{TPathContext}" />
public interface IStepLineSegment<TPathContext> : IPathCommand<TPathContext>
{
    /// <summary>
    /// Gets or sets the X0 value.
    /// </summary>
    float X0 { get; set; }

    /// <summary>
    /// Gets or sets the Y0 value.
    /// </summary>
    float Y0 { get; set; }

    /// <summary>
    /// Gets or sets the X1 value.
    /// </summary>
    float X1 { get; set; }

    /// <summary>
    /// Gets or sets the Y1 value.
    /// </summary>
    float Y1 { get; set; }
}
