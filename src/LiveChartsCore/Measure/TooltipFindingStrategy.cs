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

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines the tool tip finding strategy.
/// </summary>
public enum TooltipFindingStrategy
{
    /// <summary>
    /// The automatic mode, it will be calculated automatically based on the series and the chart.
    /// </summary>
    Automatic,

    /// <summary>
    /// Looks for all the points that contain the pointer positon.
    /// </summary>
    CompareAll,

    /// <summary>
    /// Looks for all the points that contain the pointer positon ignoring the Y axis.
    /// </summary>
    CompareOnlyX,

    /// <summary>
    /// Looks for all the points that contain the pointer positon ignoring the X axis.
    /// </summary>
    CompareOnlyY,

    /// <summary>
    /// Looks for the closest point (to the pointer) per series that contains the pointer positon.
    /// </summary>
    CompareAllTakeClosest,

    /// <summary>
    /// Looks for the closest point (to the pointer) per series that contains the pointer positon ignoring the Y axis.
    /// </summary>
    CompareOnlyXTakeClosest,

    /// <summary>
    /// Looks for the closest point (to the pointer) per series that contains the pointer positon ignoring the X axis.
    /// </summary>
    CompareOnlyYTakeClosest
}
