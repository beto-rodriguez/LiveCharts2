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
using LiveChartsCore.Motion;

namespace LiveChartsCore.Geo;

/// <summary>
/// Defines a geographic map.
/// </summary>
public interface IGeoMapView<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the active map.
    /// </summary>
    CoreMap<TDrawingContext> ActiveMap { get; set; }

    /// <summary>
    /// Gets the motion canvas.
    /// </summary>
    MotionCanvas<TDrawingContext> Canvas { get; }

    /// <summary>
    /// Gets the control width.
    /// </summary>
    float Width { get; }

    /// <summary>
    /// Gets the control height.
    /// </summary>
    float Height { get; }

    /// <summary>
    /// Gets or sets the stroke.
    /// </summary>
    IPaint<TDrawingContext>? Stroke { get; set; }

    /// <summary>
    /// Gets or sets the fill.
    /// </summary>
    IPaint<TDrawingContext>? Fill { get; set; }

    /// <summary>
    /// Gets or sets whether the chart auto-updates are enabled.
    /// </summary>
    bool AutoUpdateEnabled { get; set; }

    /// <summary>
    /// Gets or sets the projection.
    /// </summary>
    MapProjection MapProjection { get; set; }

    /// <summary>
    /// Gets whether the control is in designer mode.
    /// </summary>
    bool DesignerMode { get; }

    /// <summary>
    /// Gets or sets the Synchronization Context, use this property to
    /// use an external object to handle multi threading synchronization.
    /// </summary>
    object SyncContext { get; set; }

    /// <summary>
    /// Gets or sets the view command.
    /// </summary>
    object? ViewCommand { get; set; }

    /// <summary>
    /// Invokes an action in the UI thread.
    /// </summary>
    /// <param name="action"></param>
    void InvokeOnUIThread(Action action);

    /// <summary>
    /// Gets or sets the series.
    /// </summary>
    IEnumerable<IGeoSeries> Series { get; set; }
}
