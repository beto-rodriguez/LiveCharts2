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
using Avalonia;
using Avalonia.Markup.Xaml.Templates;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Painting;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

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

public abstract partial class ChartControl
{
    static LiveChartsSettings d = LiveCharts.DefaultSettings;

    /// <inheritdoc cref="IChartView.UpdateStarted"/>
    static XamlProperty<ICommand>                   updateStartedCommand;
    /// <inheritdoc cref="IChartView.DataPointerDown"/>
    static XamlProperty<ICommand>                   dataPointerDownCommand;
    /// <inheritdoc cref="IChartView.HoveredPointsChanged"/>
    static XamlProperty<ICommand>                   hoveredPointsChangedCommand;
    /// <inheritdoc cref="IChartView.ChartPointPointerDown"/>
    static XamlProperty<ICommand>                   chartPointPointerDownCommand;
    /// <inheritdoc cref="IChartView.VisualElementsPointerDown"/>
    static XamlProperty<ICommand>                   visualElementsPointerDownCommand;
    /// <summary>
    /// Ocurrs when the chart is pressed.
    /// </summary>
    static XamlProperty<ICommand>                   pointerPressedCommand;
    /// <summary>
    /// Ocurrs when the pointer is moved over the chart.
    /// </summary>
    static XamlProperty<ICommand>                   pointerMoveCommand;
    /// <summary>
    /// Ocurrs when the pointer is released over the chart.
    /// </summary>
    static XamlProperty<ICommand>                   pointerReleasedCommand;

    /// <inheritdoc cref="IChartView.AnimationsSpeed"/>
    static XamlProperty<TimeSpan>                   animationsSpeed         = new(d.AnimationsSpeed);
    /// <inheritdoc cref="IChartView.EasingFunction"/>
    static XamlProperty<Func<float, float>?>        easingFunction          = new(d.EasingFunction);
    /// <inheritdoc cref="IChartView.UpdaterThrottler"/>
    static XamlProperty<TimeSpan>                   updaterThrottler        = new(d.UpdateThrottlingTimeout);
    /// <inheritdoc cref="IChartView.AutoUpdateEnabled"/>
    static XamlProperty<bool>                       autoUpdateEnabled       = new(true);

    /// <inheritdoc cref="IChartView.DrawMargin"/>
    static XamlProperty<Margin>                     drawMargin              = new(null,                     OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.LegendPosition"/>
    static XamlProperty<LegendPosition>             legendPosition          = new(d.LegendPosition,         OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.TooltipPosition"/>
    static XamlProperty<TooltipPosition>            tooltipPosition         = new(d.TooltipPosition,        OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.LegendTextPaint"/>
    static XamlProperty<Paint?>                     legendTextPaint         = new(d.LegendTextPaint,        OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.LegendBackgroundPaint"/>
    static XamlProperty<Paint?>                     legendBackgroundPaint   = new(d.LegendBackgroundPaint,  OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.LegendTextSize"/>
    static XamlProperty<double>                     legendTextSize          = new(d.LegendTextSize,         OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.TooltipTextPaint"/>
    static XamlProperty<Paint?>                     tooltipTextPaint        = new(d.TooltipTextPaint,       OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.TooltipBackgroundPaint"/>
    static XamlProperty<Paint?>                     tooltipBackgroundPaint  = new(d.TooltipBackgroundPaint, OnChartPropertyChanged);
    /// <inheritdoc cref="IChartView.TooltipTextSize"/>
    static XamlProperty<double>                     tooltipTextSize         = new(d.TooltipTextSize,        OnChartPropertyChanged);

    /// <inheritdoc cref="IChartView.SyncContext"/>
    static XamlProperty<object>                     syncContext             = new(onChanged: OnSyncContextChanged);

    /// <inheritdoc cref="IChartView.Title"/>
    static XamlProperty<IChartElement?>             title                   = new(onChanged: OnObservedPropertyChanged(nameof(Title)));
    /// <inheritdoc cref="IChartView.VisualElements"/>
    static XamlProperty<ICollection<IChartElement>> visualElements          = new(onChanged: OnObservedPropertyChanged(nameof(VisualElements)));
    /// <inheritdoc cref="IChartView.Series"/>
    static XamlProperty<ICollection<ISeries>>       series                  = new(onChanged: OnObservedPropertyChanged(nameof(Series)));

    /// <summary>
    /// Gets or sets the source of the series.
    /// </summary>
    static XamlProperty<IEnumerable<object>>        seriesSource            = new(onChanged: OnSeriesSourceChanged);
    /// <summary>
    /// Gets or sets the template used to create series from the <see cref="SeriesSource"/>.
    /// </summary>
    static XamlProperty<DataTemplate>               seriesTemplate          = new(onChanged: OnSeriesSourceChanged);

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(IsPointerOver)) return;

        OnXamlPropertyChanged(change);

        if (change.Property.Name == nameof(ActualThemeVariant))
            CoreChart.ApplyTheme();
    }

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
