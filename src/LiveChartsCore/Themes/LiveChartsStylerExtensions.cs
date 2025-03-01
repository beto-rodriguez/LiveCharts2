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
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.Themes;

/// <summary>
/// Defines the LiveCharts styler extensions.
/// </summary>
public static class LiveChartsStylerExtensions
{
    /// <summary>
    ///  Defines a style builder for <see cref="IChartView"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForAxes(this Theme styler, Action<IPlane> predicate)
    {
        styler.AxisBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    /// Defines a style builder for <see cref="CoreDrawMarginFrame"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="getter">The getter.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForDrawMarginFrame(this Theme styler, Func<CoreDrawMarginFrame?> getter, Action<CoreDrawMarginFrame> predicate)
    {
        styler.DrawMarginFrameGetter = getter;
        styler.DrawMarginFrameBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="ISeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForAnySeries(this Theme styler, Action<ISeries> predicate)
    {
        styler.SeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IPieSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForPieSeries(this Theme styler, Action<IPieSeries> predicate)
    {
        styler.PieSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IPieSeries"/> objects when used as gauges.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForGaugeSeries(this Theme styler, Action<IPieSeries> predicate)
    {
        styler.GaugeSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IPieSeries"/> objects when used as gauges fills.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForGaugeFillSeries(this Theme styler, Action<IPieSeries> predicate)
    {
        styler.GaugeFillSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="ILineSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForLineSeries(this Theme styler, Action<ILineSeries> predicate)
    {
        styler.LineSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IStepLineSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForStepLineSeries(this Theme styler, Action<IStepLineSeries> predicate)
    {
        styler.StepLineSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for stacked <see cref="IStepLineSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForStackedStepLineSeries(this Theme styler, Action<IStepLineSeries> predicate)
    {
        styler.StackedStepLineSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for stacked <see cref="IStepLineSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForBoxSeries(this Theme styler, Action<IBoxSeries> predicate)
    {
        styler.BoxSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for tacked <see cref="ILineSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForStackedLineSeries(this Theme styler, Action<ILineSeries> predicate)
    {
        styler.StackedLineSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IBarSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForBarSeries(this Theme styler, Action<IBarSeries> predicate)
    {
        styler.BarSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for vertical <see cref="IBarSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForColumnSeries(this Theme styler, Action<IBarSeries> predicate)
    {
        styler.ColumnSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for horizontal <see cref="IBarSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForRowSeries(this Theme styler, Action<IBarSeries> predicate)
    {
        styler.ColumnSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for stacked <see cref="IBarSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForStackedBarSeries(this Theme styler, Action<IStackedBarSeries> predicate)
    {
        styler.StackedBarSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for stacked vertical <see cref="IBarSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForStackedColumnSeries(this Theme styler, Action<IStackedBarSeries> predicate)
    {
        styler.StackedColumnSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for stacked horizontal <see cref="IBarSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForStackedRowSeries(this Theme styler, Action<IStackedBarSeries> predicate)
    {
        styler.StackedRowSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IScatterSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForScatterSeries(this Theme styler, Action<IScatterSeries> predicate)
    {
        styler.ScatterSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IHeatSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForHeatSeries(this Theme styler, Action<IHeatSeries> predicate)
    {
        styler.HeatSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IFinancialSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForFinancialSeries(this Theme styler, Action<IFinancialSeries> predicate)
    {
        styler.FinancialSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IPolarSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForPolaSeries(this Theme styler, Action<IPolarSeries> predicate)
    {
        styler.PolarSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    ///  Defines a style builder for <see cref="IPolarLineSeries"/> objects.
    /// </summary>
    /// <param name="styler">The styler.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleForPolarLineSeries(this Theme styler, Action<IPolarLineSeries> predicate)
    {
        styler.PolarLineSeriesBuilder.Add(predicate);
        return styler;
    }

    /// <summary>
    /// Defines the default tooltip.
    /// </summary>
    /// <param name="styler"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Theme HasDefaultTooltip(this Theme styler, Func<IChartTooltip> predicate)
    {
        styler.DefaultTooltip = predicate;
        return styler;
    }

    /// <summary>
    /// Defines the default legend.
    /// </summary>
    /// <param name="styler"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Theme HasDefaultLegend(this Theme styler, Func<IChartLegend> predicate)
    {
        styler.DefaultLegend = predicate;
        return styler;
    }

    /// <summary>
    /// Defines a style builder for <see cref= "VisualElement" /> objects.
    /// </summary>
    /// <typeparam name="TChartElement">The type of the chart element.</typeparam>
    /// <param name="styler">the styler.</param>
    /// <param name="predicate">the predicate.</param>
    /// <returns></returns>
    public static Theme HasRuleFor<TChartElement>(this Theme styler, Action<TChartElement> predicate)
        where TChartElement : ChartElement
    {
        styler.ChartElementElementBuilder.Add(typeof(TChartElement), predicate);
        return styler;
    }

    /// <summary>
    /// Defines the initialized action.
    /// </summary>
    /// <param name="styler"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Theme OnInitialized(this Theme styler, Action predicate)
    {
        styler.Initialized = predicate;
        return styler;
    }
}
