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
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.Kernel.Sketches;

/// <summary>
/// Defines a chart view
/// </summary>
public interface IChartView
{
    /// <summary>
    /// Gets the core.
    /// </summary>
    /// <value>
    /// The core.
    /// </value>
    IChart CoreChart { get; }

    /// <summary>
    /// Gets whether the control is in designer mode.
    /// </summary>
    bool DesignerMode { get; }

    /// <summary>
    /// Sets the back color of the control.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    LvcColor BackColor { get; set; }

    /// <summary>
    /// Gets the size of the control.
    /// </summary>
    /// <value>
    /// The size of the control.
    /// </value>
    LvcSize ControlSize { get; }

    /// <summary>
    /// Gets or sets the draw margin, if this property is null, the library will calculate a margin, this margin is the distance 
    /// between the view bounds and the drawable area.
    /// </summary>
    /// <value>
    /// The draw margin.
    /// </value>
    Margin? DrawMargin { get; set; }

    /// <summary>
    /// Gets or sets the animations speed.
    /// </summary>
    /// <value>
    /// The animations speed.
    /// </value>
    TimeSpan AnimationsSpeed { get; set; }

    /// <summary>
    /// Gets or sets the easing function, the library already provides many easing functions in the 
    /// LiveCharts.EasingFunction static class.
    /// </summary>
    /// <value>
    /// The easing function.
    /// </value>
    Func<float, float>? EasingFunction { get; set; }

    /// <summary>
    /// Gets or sets the updater throttler, this property controls the interval where the user interface updates.
    /// </summary>
    /// <value>
    /// The updater throttler.
    /// </value>
    TimeSpan UpdaterThrottler { get; set; }

    /// <summary>
    /// Gets or sets the legend position.
    /// </summary>
    /// <value>
    /// The legend position.
    /// </value>
    LegendPosition LegendPosition { get; set; }

    /// <summary>
    /// Gets or sets the tooltip position.
    /// </summary>
    /// <value>
    /// The tooltip position.
    /// </value>
    TooltipPosition TooltipPosition { get; set; }

    /// <summary>
    /// Occurs when the pointer goes down over a chart point(s).
    /// </summary>
    event ChartPointsHandler? DataPointerDown;

    /// <summary>
    /// Occurs when the pointer goes down over a chart point, if there are multiple points, the closest one will be selected.
    /// </summary>
    event ChartPointHandler? ChartPointPointerDown;

    /// <summary>
    /// Called when the pointer goes down on a data point or points.
    /// </summary>
    /// <param name="points">The found points.</param>
    /// <param name="pointer">The pointer location.</param>
    void OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer);

    /// <summary>
    /// Gets or sets the Synchronization Context, use this property to
    /// use an external object to handle multi threading synchronization.
    /// </summary>
    object SyncContext { get; set; }

    /// <summary>
    /// Invokes an action in the UI thread.
    /// </summary>
    /// <param name="action"></param>
    void InvokeOnUIThread(Action action);

    /// <summary>
    /// Invalidates the control.
    /// </summary>
    void Invalidate();
}

/// <summary>
/// Defines a chart view.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public interface IChartView<TDrawingContext> : IChartView
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the legend default text paint, when null the library will use the default text paint.
    /// </summary>
    IPaint<TDrawingContext>? LegendTextPaint { get; set; }

    /// <summary>
    /// Gets or sets the legend background paint, when null the library will use the default background paint.
    /// </summary>
    IPaint<TDrawingContext>? LegendBackgroundPaint { get; set; }

    /// <summary>
    /// Gets or sets the legend text size, when null the library will use the default text size.
    /// </summary>
    double? LegendTextSize { get; set; }

    /// <summary>
    /// Gets or sets the tooltip default text paint, when null the library will use the default text paint.
    /// </summary>
    IPaint<TDrawingContext>? TooltipTextPaint { get; set; }

    /// <summary>
    /// Gets or sets the tooltip background paint, when null the library will use the default background paint.
    /// </summary>
    IPaint<TDrawingContext>? TooltipBackgroundPaint { get; set; }

    /// <summary>
    /// Gets or sets the tooltip text size, when null the library will use the default text size.
    /// </summary>
    double? TooltipTextSize { get; set; }

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    VisualElement<TDrawingContext>? Title { get; set; }

    /// <summary>
    /// Occurs before the chart is measured, this is the first step before the chart updates.
    /// </summary>
    event ChartEventHandler<TDrawingContext>? Measuring;

    /// <summary>
    /// Occurs when the chart started an update, just when the drawing loop started.
    /// </summary>
    event ChartEventHandler<TDrawingContext>? UpdateStarted;

    /// <summary>
    /// Occurs when a chart update finished, just when the drawing loop finished.
    /// </summary>
    event ChartEventHandler<TDrawingContext>? UpdateFinished;

    /// <summary>
    /// Occurs when the pointer goes down over a visual element.
    /// </summary>
    event VisualElementsHandler<TDrawingContext> VisualElementsPointerDown;

    /// <summary>
    /// Gets or sets a value indicating whether the automatic updates are enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if automatic update are enabled; otherwise, <c>false</c>.
    /// </value>
    bool AutoUpdateEnabled { get; set; }

    /// <summary>
    /// Gets the core canvas.
    /// </summary>
    /// <value>
    /// The core canvas.
    /// </value>
    MotionCanvas<TDrawingContext> CoreCanvas { get; }

    /// <summary>
    /// Gets or sets the legend.
    /// </summary>
    /// <value>
    /// The legend.
    /// </value>
    IChartLegend<TDrawingContext>? Legend { get; set; }

    /// <summary>
    /// Gets or sets the tooltip.
    /// </summary>
    /// <value>
    /// The tooltip.
    /// </value>
    IChartTooltip<TDrawingContext>? Tooltip { get; set; }

    /// <summary>
    /// Gets or sets the visual elements.
    /// </summary>
    IEnumerable<ChartElement<TDrawingContext>> VisualElements { get; set; }

    /// <summary>
    /// Called when the pointer goes down on a visual element(s).
    /// </summary>
    /// <param name="visualElements">The visual elements.</param>
    /// <param name="pointer">The pointer location.</param>
    void OnVisualElementPointerDown(IEnumerable<VisualElement<TDrawingContext>> visualElements, LvcPoint pointer);

    /// <summary>
    /// Gets all the <see cref="ChartPoint"/> that contain the given point.
    /// </summary>
    /// <param name="point">The given point.</param>
    /// <param name="strategy">The finding strategy, default is <see cref="TooltipFindingStrategy.Automatic"/>.</param>
    /// <returns>An enumerable of <see cref="ChartPoint"/>.</returns>
    IEnumerable<ChartPoint> GetPointsAt(LvcPoint point, TooltipFindingStrategy strategy = TooltipFindingStrategy.Automatic);

    /// <summary>
    /// Gets all the <see cref="VisualElement{TDrawingContext}"/> that contain the given point.
    /// </summary>
    /// <param name="point">The given point.</param>
    /// <returns>An enumerable of <see cref="VisualElement{TDrawingContext}"/>.</returns>
    IEnumerable<VisualElement<TDrawingContext>> GetVisualsAt(LvcPoint point);
}
