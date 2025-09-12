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
using System.Linq;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// A helper class that configures shared axes.
/// </summary>
public static class SharedAxes
{
    /// <summary>
    /// Sets the specified axes as shared.
    /// </summary>
    /// <param name="axes">The axes to share.</param>
    public static void Set(params ICartesianAxis[] axes)
    {
        // ToDo: unsubscribe events?

        var sharedInstance = new HashSet<CartesianChartEngine>();

        foreach (var axis in axes)
        {
            axis.SharedWith = [..
                axes
                    .Where(x => x != axis)
                    .Select(x => (ICartesianAxis)x.ChartElementSource)
                ];

            axis.MeasureStarted += (chart, obj) =>
            {
                var cc = (CartesianChartEngine)chart;
                cc.SubscribeSharedEvents(sharedInstance);
            };
        }
    }

    /// <summary>
    /// Matches the axes screen data ratio, it means that the axes will take the same amount of
    /// space in the screen per unit of data, note that when the view is diposed, <see cref="DisposeMatchAxesScreenDataRatio(ICartesianChartView)"/>
    /// will ensure the resources are released.
    /// </summary>
    /// <param name="chart">The chart.</param>
    public static void MatchAxesScreenDataRatio(this ICartesianChartView chart) =>
        ((CartesianChartEngine)chart.CoreChart).DrawMarginDefined += OnDrawMarginDefined;

    /// <summary>
    /// Disposes the match axes screen data ratio functionality.
    /// </summary>
    /// <param name="chart">The chart.</param>
    public static void DisposeMatchAxesScreenDataRatio(this ICartesianChartView chart) =>
        ((CartesianChartEngine)chart.CoreChart).DrawMarginDefined -= OnDrawMarginDefined;

    private static void OnDrawMarginDefined(CartesianChartEngine chart)
    {
        var drawMarginSize = chart.DrawMarginSize;
        ICartesianAxis source, target;
        float sourceDimension, targetDimension;

        if (chart.XAxes.Length > 1 || chart.YAxes.Length > 1)
        {
            throw new NotImplementedException(
                $"{nameof(MatchAxesScreenDataRatio)} only supports one axis for both X and Y. " +
                $"Why is this required? please open an issue at github explaining the need of this feature.");
        }

        var x = chart.XAxes[0];
        var y = chart.YAxes[0];

        var isFirstScale =
            x.MinLimit is null && x.MaxLimit is null && y.MinLimit is null && y.MaxLimit is null;
        var isXSource =
            isFirstScale
                ? drawMarginSize.Width < drawMarginSize.Height
                : x.DataBounds.Delta > y.DataBounds.Delta;

        if (isXSource)
        {
            source = x;
            target = y;
            sourceDimension = drawMarginSize.Width;
            targetDimension = drawMarginSize.Height;
        }
        else
        {
            source = y;
            target = x;
            sourceDimension = drawMarginSize.Height;
            targetDimension = drawMarginSize.Width;
        }

        var min = source.MinLimit ?? source.DataBounds.Min;
        var max = source.MaxLimit ?? source.DataBounds.Max;

        AxisLimit.ValidateLimits(ref min, ref max, source.MinStep);
        source.SetLimits(min, max, notify: false);

        var sourceScreenDataRatio = sourceDimension / (max - min);

        const int timesScale = 1;
        var targetDelta = targetDimension / (sourceScreenDataRatio * timesScale);
        var midTarget = GetMidPoint(target);

        target.SetLimits(
            midTarget - 0.5f * targetDelta,
            midTarget + 0.5f * targetDelta,
            notify: false);
    }

    private static double GetMidPoint(ICartesianAxis axis)
    {
        var min = axis.MinLimit ?? axis.DataBounds.Min;
        var max = axis.MaxLimit ?? axis.DataBounds.Max;
        return (min + max) * 0.5f;
    }
}
