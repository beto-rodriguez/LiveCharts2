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
using System.Windows.Input;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using Microsoft.Maui.Controls;
using Paint = LiveChartsCore.Painting.Paint;

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

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <inheritdoc cref="IChartView"/>
public abstract partial class ChartControl
{
    static LiveChartsSettings d = LiveCharts.DefaultSettings;

    static XamlProperty<ICommand>                   updateStartedCommand;
    static XamlProperty<ICommand>                   dataPointerDownCommand;
    static XamlProperty<ICommand>                   hoveredPointsChangedCommand;
    static XamlProperty<ICommand>                   chartPointPointerDownCommand;
    static XamlProperty<ICommand>                   visualElementsPointerDownCommand;
    static XamlProperty<ICommand>                   pressedCommand;
    static XamlProperty<ICommand>                   movedCommand;
    static XamlProperty<ICommand>                   releasedCommand;

    static XamlProperty<TimeSpan>                   animationsSpeed         = new(d.AnimationsSpeed);
    static XamlProperty<Func<float, float>?>        easingFunction          = new(d.EasingFunction);
    static XamlProperty<TimeSpan>                   updaterThrottler        = new(d.UpdateThrottlingTimeout);
    static XamlProperty<bool>                       autoUpdateEnabled       = new(true);

    static XamlProperty<Margin>                     drawMargin              = new(null,                     OnChartPropertyChanged);
    static XamlProperty<LegendPosition>             legendPosition          = new(d.LegendPosition,         OnChartPropertyChanged);
    static XamlProperty<TooltipPosition>            tooltipPosition         = new(d.TooltipPosition,        OnChartPropertyChanged);
    static XamlProperty<Paint?>                     legendTextPaint         = new(d.LegendTextPaint,        OnChartPropertyChanged);
    static XamlProperty<Paint?>                     legendBackgroundPaint   = new(d.LegendBackgroundPaint,  OnChartPropertyChanged);
    static XamlProperty<double>                     legendTextSize          = new(d.LegendTextSize,         OnChartPropertyChanged);
    static XamlProperty<Paint?>                     tooltipTextPaint        = new(d.TooltipTextPaint,       OnChartPropertyChanged);
    static XamlProperty<Paint?>                     tooltipBackgroundPaint  = new(d.TooltipBackgroundPaint, OnChartPropertyChanged);
    static XamlProperty<double>                     tooltipTextSize         = new(d.TooltipTextSize,        OnChartPropertyChanged);

    static XamlProperty<object>                     syncContext             = new(onChanged: OnSyncContextChanged);

    static XamlProperty<IChartElement?>             title                   = new(onChanged: OnObservedPropertyChanged(nameof(Title)));
    static XamlProperty<ICollection<IChartElement>> visualElements          = new(onChanged: OnObservedPropertyChanged(nameof(VisualElements)));
    static XamlProperty<ICollection<ISeries>>       series                  = new(onChanged: OnObservedPropertyChanged(nameof(Series)));

    static XamlProperty<IEnumerable<object>>        seriesSource            = new(onChanged: OnSeriesSourceChanged);
    static XamlProperty<DataTemplate>               seriesTemplate          = new(onChanged: OnSeriesSourceChanged);

    static void OnChartPropertyChanged(ChartControl chart) => chart.CoreChart.Update();

    static void OnSyncContextChanged(ChartControl chart, object oldValue, object newValue)
    {
        chart.CoreCanvas.Sync = newValue;
        chart.CoreChart.Update();
    }

    static void OnSeriesSourceChanged(ChartControl chart)
    {
        var seriesObserver = (SeriesSourceObserver)chart.Observe[nameof(SeriesSource)];
        seriesObserver.Initialize(chart.SeriesSource);

        if (seriesObserver.Series is not null)
            chart.Series = seriesObserver.Series;
    }

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
