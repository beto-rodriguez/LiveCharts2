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
using LiveChartsCore.Drawing;
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

            axis.MeasureStarted += (Chart chart, ICartesianAxis obj) =>
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
        ((CartesianChartEngine)chart.CoreChart).DrawMarginDefined += MatchAxesScreenDataRatioDelegate;

    /// <summary>
    /// Disposes the match axes screen data ratio functionality.
    /// </summary>
    /// <param name="chart">The chart.</param>
    public static void DisposeMatchAxesScreenDataRatio(this ICartesianChartView chart) =>
        ((CartesianChartEngine)chart.CoreChart).DrawMarginDefined -= MatchAxesScreenDataRatioDelegate;

    private static void MatchAxesScreenDataRatioDelegate(CartesianChartEngine chart)
    {
        var drawMarginSize = chart.DrawMarginSize;
        ICartesianAxis source, target;

        if (chart.XAxes.Length > 1 || chart.YAxes.Length > 1)
        {
            throw new NotImplementedException(
                $"{nameof(MatchAxesScreenDataRatio)} only supports one axis for both X and Y. " +
                $"Why is this required? please open an issue at github explaining the need of this feature.");
        }

        if (drawMarginSize.Height > drawMarginSize.Width)
        {
            source = chart.XAxes[0];
            target = chart.YAxes[0];
        }
        else
        {
            source = chart.YAxes[0];
            target = chart.XAxes[0];
        }

        MatchSlopes(drawMarginSize, source, target);
    }

    private static void MatchSlopes(LvcSize drawMarginSize, ICartesianAxis source, ICartesianAxis target)
    {
        var minSourceData = source.MinLimit ?? source.DataBounds.Min;
        var maxSourceData = source.MaxLimit ?? source.DataBounds.Max;

        source.SetLimits(
            minSourceData,
            maxSourceData,
            notify: false);

        var sourceDimension = source.Orientation == AxisOrientation.X
            ? drawMarginSize.Width
            : drawMarginSize.Height;

        var sourceScreenDataRatio = sourceDimension / (maxSourceData - minSourceData);

        var targetDimension = target.Orientation == AxisOrientation.X
            ? drawMarginSize.Width
            : drawMarginSize.Height;

        var desiredTargetDelta = targetDimension / sourceScreenDataRatio;

        var minTargetData = target.MinLimit ?? target.DataBounds.Min;
        var maxTargetData = target.MaxLimit ?? target.DataBounds.Max;
        var midTargetData = (minTargetData + maxTargetData) * 0.5f;

        target.SetLimits(
            midTargetData - 0.5f * desiredTargetDelta,
            midTargetData + 0.5f * desiredTargetDelta,
            notify: false);
    }
}
