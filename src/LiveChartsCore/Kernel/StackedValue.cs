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

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines the a stacked value.
/// </summary>
public class StackedValue
{
    /// <summary>
    /// Gets or sets the start.
    /// </summary>
    /// <value>
    /// The start.
    /// </value>
    public double Start { get; set; }

    /// <summary>
    /// Gets or sets the end.
    /// </summary>
    /// <value>
    /// The end.
    /// </value>
    public double End { get; set; }

    /// <summary>
    /// Gets or sets the total stacked.
    /// </summary>
    /// <value>
    /// The total.
    /// </value>
    public double Total { get; set; }

    /// <summary>
    /// Gets the share in the total stack.
    /// </summary>
    public double Share => (End - Start) / Total;

    /// <summary>
    /// Gets or sets the start.
    /// </summary>
    /// <value>
    /// The start.
    /// </value>
    public double NegativeStart { get; set; }

    /// <summary>
    /// Gets or sets the end.
    /// </summary>
    /// <value>
    /// The end.
    /// </value>
    public double NegativeEnd { get; set; }

    /// <summary>
    /// Gets or sets the total stacked.
    /// </summary>
    /// <value>
    /// The total.
    /// </value>
    public double NegativeTotal { get; set; }
}
