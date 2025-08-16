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

namespace LiveChartsCore.Measure;

/// <summary>
/// Defines the zooming and panning mode.
/// </summary>
[Flags]
public enum ZoomAndPanMode
{
    /// <summary>
    /// Disables zooming and panning.
    /// </summary>
    None = 0,

    /// <summary>
    /// Enables zooming and panning on the X axis and enables fitting to bounds.
    /// </summary>
    X = 1 << 0,

    /// <summary>
    /// Enables zooming and panning on the Y axis and enables fitting to bounds.
    /// </summary>
    Y = 1 << 1,

    /// <summary>
    /// Disables data bounds fitting when zooming or panning, this flag must be used in conjunction with
    /// <see cref="X"/>, <see cref="Y"/>, or <see cref="Both"/> to have an effect.
    /// </summary>
    NoFit = 1 << 2,

    /// <summary>
    /// Disables the "Zoom by section" feature, which allows zooming in on a specific section of the chart.
    /// </summary>
    NoZoomBySection = 1 << 3,

    /// <summary>
    /// When this flag is present the panning will be triggered using the right click on desktop devices and the touch-and-hold gesture on touch devices.
    /// The "Zoom by section" feature will be triggered to the left click on desktop devices and the touch-and-hold gesture on touch devices,
    /// this flag must be used in conjunction with
    /// <see cref="X"/>, <see cref="Y"/>, or <see cref="Both"/> to have an effect.
    /// </summary>
    InvertPanningPointerTrigger = 1 << 4,

    /// <summary>
    /// Enables zooming and panning on both axes and enables fitting to bounds.
    /// </summary>
    Both = X | Y
}
