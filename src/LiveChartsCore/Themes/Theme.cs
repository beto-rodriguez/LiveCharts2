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
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Themes;

/// <summary>
/// Defines an object that must initialize live charts visual objects, this object defines how things will 
/// be drawn by default, it is highly related to themes.
/// </summary>
public class Theme<TDrawingContext> where TDrawingContext : DrawingContext
{
    private static readonly Dictionary<string, List<string>> s_assignableTypes = new();
    private static readonly Type[] s_stylableTypes = new Type[]
    {
        typeof(IChartView<TDrawingContext>),

        typeof(ICartesianChartView<TDrawingContext>),
        typeof(IPieChartView<TDrawingContext>),
        typeof(IPolarChartView<TDrawingContext>),

        typeof(ISeries<TDrawingContext>),
        typeof(IChartSeries<TDrawingContext>),
        typeof(ICartesianSeries<TDrawingContext>),

        typeof(IBarSeries<TDrawingContext>),
        typeof(IFinancialSeries<TDrawingContext>),
        typeof(IHeatSeries<TDrawingContext>),
        typeof(ILineSeries<TDrawingContext>),
        typeof(IPieSeries<TDrawingContext>),
        typeof(IPolarLineSeries<TDrawingContext>),
        typeof(IScatterSeries<TDrawingContext>),
        typeof(IStackedBarSeries<TDrawingContext>),
        typeof(IStepLineSeries<TDrawingContext>),
        typeof(IStrokedAndFilled<TDrawingContext>),

        typeof(IPlane<TDrawingContext>),

        typeof(ICartesianAxis<TDrawingContext>),
        //typeof(IPolarAxis<TDrawingContext>).GetType().Name
    };

    /// <summary>
    /// Gets the builders.
    /// </summary>
    public Dictionary<string, List<object>> Builders { get; } = new();

    /// <summary>
    /// Adds a rule for the specified type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="predicate">The rule.</param>
    /// <returns></returns>
    public Theme<TDrawingContext> SetRuleFor<T>(Action<T> predicate)
    {
        var key = typeof(T).Name;

        if (!Builders.TryGetValue(key, out var list))
            Builders[key] = list = new();

        list.Add((object o) => predicate((T)o));

        return this;
    }

    /// <summary>
    /// Applies the rules to the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the target.</typeparam>
    /// <param name="target">The target.</param>
    public void ApplyRuleTo<T>(T target)
        where T : notnull
    {
        var t = target.GetType();

        if (!s_assignableTypes.TryGetValue(t.Name, out var assignableTypes))
        {
            assignableTypes = new List<string>();

            foreach (var stylableType in s_stylableTypes)
            {
                if (stylableType.IsAssignableFrom(t))
                    assignableTypes.Add(stylableType.Name);
            }

            s_assignableTypes[t.Name] = assignableTypes;
        }

        foreach (var assignableType in assignableTypes)
        {
            if (Builders.TryGetValue(assignableType, out var list))
            {
                foreach (var rule in list)
                {
                    ((Action<object>)rule)(target);
                }
            }
        }
    }
}
