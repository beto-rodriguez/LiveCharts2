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
using LiveChartsCore.Kernel.Drawing;

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines the tool tip finding strategy.
/// </summary>
[Obsolete($"Renamed to {nameof(FindingStrategy)}")]
public enum TooltipFindingStrategy
{
    /// <summary>
    /// The automatic mode, it will be calculated automatically based on the series and the chart.
    /// </summary>
    Automatic,

    /// <summary>
    /// Compares whether the pointer is inside the <see cref="HoverArea"/> in both X and Y axis.
    /// </summary>
    CompareAll,

    /// <summary>
    /// Compares whether the pointer is inside the <see cref="HoverArea"/> in the X axis.
    /// </summary>
    CompareOnlyX,

    /// <summary>
    /// Compares whether the pointer is inside the <see cref="HoverArea"/> in the Y axis.
    /// </summary>
    CompareOnlyY,

    /// <summary>
    /// Compares whether the pointer is inside the <see cref="HoverArea"/> in both X and Y axis,
    /// if overlapped then takes the closest to the pointer in each series.
    /// </summary>
    CompareAllTakeClosest,

    /// <summary>
    /// Compares whether the pointer is inside the <see cref="HoverArea"/> in the X axis,
    /// if overlapped then takes the closest to the pointer in each series.
    /// </summary>
    CompareOnlyXTakeClosest,

    /// <summary>
    /// Compares whether the pointer is inside the <see cref="HoverArea"/> in the Y axis,
    /// if overlapped then takes the closest to the pointer in each series.
    /// </summary>
    CompareOnlyYTakeClosest,

    /// <summary>
    /// Compares whether the pointer is inside the drawn shape.
    /// </summary>
    ExactMatch,

    /// <summary>
    /// Compares whether the pointer is inside the drawn shape,
    /// if overlapped then takes the closest to the pointer in each series.
    /// </summary>
    ExactMatchTakeClosest
}

internal static class ObsoleteMapper
{
    public static FindingStrategy AsNew(this TooltipFindingStrategy oldName)
        => oldName switch
        {
            TooltipFindingStrategy.Automatic => FindingStrategy.Automatic,
            TooltipFindingStrategy.CompareAll => FindingStrategy.CompareAll,
            TooltipFindingStrategy.CompareOnlyX => FindingStrategy.CompareOnlyX,
            TooltipFindingStrategy.CompareOnlyY => FindingStrategy.CompareOnlyY,
            TooltipFindingStrategy.CompareAllTakeClosest => FindingStrategy.CompareAllTakeClosest,
            TooltipFindingStrategy.CompareOnlyXTakeClosest => FindingStrategy.CompareOnlyXTakeClosest,
            TooltipFindingStrategy.CompareOnlyYTakeClosest => FindingStrategy.CompareOnlyYTakeClosest,
            TooltipFindingStrategy.ExactMatch => FindingStrategy.ExactMatch,
            TooltipFindingStrategy.ExactMatchTakeClosest => FindingStrategy.ExactMatchTakeClosest,
            _ => throw new NotImplementedException()
        };

    public static TooltipFindingStrategy AsOld(this FindingStrategy newName)
        => newName switch
        {
            FindingStrategy.Automatic => TooltipFindingStrategy.Automatic,
            FindingStrategy.CompareAll => TooltipFindingStrategy.CompareAll,
            FindingStrategy.CompareOnlyX => TooltipFindingStrategy.CompareOnlyX,
            FindingStrategy.CompareOnlyY => TooltipFindingStrategy.CompareOnlyY,
            FindingStrategy.CompareAllTakeClosest => TooltipFindingStrategy.CompareAllTakeClosest,
            FindingStrategy.CompareOnlyXTakeClosest => TooltipFindingStrategy.CompareOnlyXTakeClosest,
            FindingStrategy.CompareOnlyYTakeClosest => TooltipFindingStrategy.CompareOnlyYTakeClosest,
            FindingStrategy.ExactMatch => TooltipFindingStrategy.ExactMatch,
            FindingStrategy.ExactMatchTakeClosest => TooltipFindingStrategy.ExactMatchTakeClosest,
            _ => throw new NotImplementedException()
        };
}
