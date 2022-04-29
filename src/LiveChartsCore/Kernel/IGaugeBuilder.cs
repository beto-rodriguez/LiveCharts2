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
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;

namespace LiveChartsCore.Kernel;

/// <summary>
/// Defines a gauge builder
/// </summary>
public interface IGaugeBuilder<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the inner radius.
    /// </summary>
    /// <value>
    /// The inner radius.
    /// </value>
    double? InnerRadius { get; set; }

    /// <summary>
    /// Gets or sets the offset radius, the separation between each gauge if multiple gauges are nested.
    /// </summary>
    /// <value>
    /// The relative inner radius.
    /// </value>
    double? OffsetRadius { get; set; }

    /// <summary>
    /// Gets or sets the maximum width of the radial column.
    /// </summary>
    /// <value>
    /// The maximum width of the radial column.
    /// </value>
    double? MaxColumnWidth { get; set; }

    /// <summary>
    /// Gets or sets the corner radius.
    /// </summary>
    /// <value>
    /// The corner radius.
    /// </value>
    double? CornerRadius { get; set; }

    /// <summary>
    /// Gets or sets the radial align.
    /// </summary>
    /// <value>
    /// The radial align.
    /// </value>
    RadialAlignment? RadialAlign { get; set; }

    /// <summary>
    /// Gets or sets the background inner radius.
    /// </summary>
    /// <value>
    /// The background inner radius.
    /// </value>
    double? BackgroundInnerRadius { get; set; }

    /// <summary>
    /// Gets or sets the background offset radius, the separation between each gauge if multiple gauges are nested.
    /// </summary>
    /// <value>
    /// The background relative inner radius.
    /// </value>
    double? BackgroundOffsetRadius { get; set; }

    /// <summary>
    /// Gets or sets the width of the background maximum radial column.
    /// </summary>
    /// <value>
    /// The width of the background maximum radial column.
    /// </value>
    double? BackgroundMaxRadialColumnWidth { get; set; }

    /// <summary>
    /// Gets or sets the background corner radius.
    /// </summary>
    /// <value>
    /// The background corner radius.
    /// </value>
    double? BackgroundCornerRadius { get; set; }

    /// <summary>
    /// Gets or sets the background.
    /// </summary>
    /// <value>
    /// The background.
    /// </value>
    IPaint<TDrawingContext>? Background { get; set; }

    /// <summary>
    /// Gets or sets the size of the labels.
    /// </summary>
    /// <value>
    /// The size of the labels.
    /// </value>
    double? LabelsSize { get; set; }

    /// <summary>
    /// Gets or sets the labels position.
    /// </summary>
    /// <value>
    /// The labels position.
    /// </value>
    PolarLabelsPosition? LabelsPosition { get; set; }

    /// <summary>
    /// Gets or sets the label formatter.
    /// </summary>
    /// <value>
    /// The label formatter.
    /// </value>
    Func<ChartPoint, string> LabelFormatter { get; set; }
}
