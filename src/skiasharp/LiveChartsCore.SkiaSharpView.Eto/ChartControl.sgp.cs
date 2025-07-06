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
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;

// ==============================================================================================================
// the static fileds in this file generate bindable/dependency/avalonia or whatever properties...
// the disabled warning make it easier to maintain the code.
//
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0052 // Remove unread private member
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CS8618  // Non-nullable field must contain a non-null value when exiting constructor.
#pragma warning disable CS0169  // The field is never used
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0040 // Add accessibility modifiers
#pragma warning disable format
// ==============================================================================================================

namespace LiveChartsCore.SkiaSharpView.Eto;

public partial class ChartControl
{
    static LiveChartsSettings d = LiveCharts.DefaultSettings;

    /// <inheritdoc cref="IChartView.AnimationsSpeed"/>
    static UIProperty<TimeSpan>                   animationsSpeed         = new(d.AnimationsSpeed);
    /// <inheritdoc cref="IChartView.EasingFunction"/>
    static UIProperty<Func<float, float>?>        easingFunction          = new(d.EasingFunction);
    /// <inheritdoc cref="IChartView.UpdaterThrottler"/>
    static UIProperty<TimeSpan>                   updaterThrottler        = new(d.UpdateThrottlingTimeout);
    /// <inheritdoc cref="IChartView.AutoUpdateEnabled"/>
    static UIProperty<bool>                       autoUpdateEnabled       = new(true);

    /// <inheritdoc cref="IChartView.DrawMargin"/>
    static UIProperty<Margin>                     drawMargin              = new(null,                     OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.LegendPosition"/>
    static UIProperty<LegendPosition>             legendPosition          = new(d.LegendPosition,         OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.TooltipPosition"/>
    static UIProperty<TooltipPosition>            tooltipPosition         = new(d.TooltipPosition,        OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.LegendTextPaint"/>
    static UIProperty<Paint?>                     legendTextPaint         = new(d.LegendTextPaint,        OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.LegendBackgroundPaint"/>
    static UIProperty<Paint?>                     legendBackgroundPaint   = new(d.LegendBackgroundPaint,  OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.LegendTextSize"/>
    static UIProperty<double>                     legendTextSize          = new(d.LegendTextSize,         OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.TooltipTextPaint"/>
    static UIProperty<Paint?>                     tooltipTextPaint        = new(d.TooltipTextPaint,       OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.TooltipBackgroundPaint"/>
    static UIProperty<Paint?>                     tooltipBackgroundPaint  = new(d.TooltipBackgroundPaint, OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.TooltipTextSize"/>
    static UIProperty<double>                     tooltipTextSize         = new(d.TooltipTextSize,        OnChartPropertyChanged);

    /// <inheritdoc cref="IChartView.SyncContext"/>
    static UIProperty<object>                     syncContext             = new(onChanged: OnSyncContextChanged);

    /// <inheritdoc cref="IChartView.Title"/>
    static UIProperty<IChartElement?>             title                   = new(onChanged: OnObservedPropertyChanged(nameof(Title)));
    /// <inheritdoc cref="IChartView.VisualElements"/>
    static UIProperty<ICollection<IChartElement>> visualElements          = new(onChanged: OnObservedPropertyChanged(nameof(VisualElements)));
    /// <inheritdoc cref="IChartView.Series"/>
    static UIProperty<ICollection<ISeries>>       series                  = new(onChanged: OnObservedPropertyChanged(nameof(Series)));

    static void OnSyncContextChanged(ChartControl chart, object oldValue, object newValue)
    {
        chart.CoreCanvas.Sync = newValue;
        chart.CoreChart.Update();
    }

    static void OnChartPropertyChanged(ChartControl chart) => chart.CoreChart.Update();

#pragma warning disable IDE0060 // Remove unused parameter, hack for the source generator
    static Action<ChartControl, object, object> OnObservedPropertyChanged(
        string propertyName, object? a = null, object? b = null) =>
            (chart, o, n) =>
            {
                chart.Observe[propertyName].Initialize(n);
                chart.CoreChart.Update();
            };
#pragma warning restore IDE0060 // Remove unused parameter
}
