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

namespace LiveChartsCore.Themes;

/// <summary>
/// Defines an object that must initialize live charts visual objects, this object defines how things will 
/// be drawn by default, it is highly related to themes.
/// </summary>
public class Theme<TDrawingContext> where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the color palette.
    /// </summary>
    public LvcColor[]? ColorPalette { get; private set; }

    /// <summary>
    /// Gets the builders.
    /// </summary>
    public Dictionary<Type, List<object>> Builders { get; } = new();

    /// <summary>
    /// Adds a rule for the specified type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="predicate">The rule.</param>
    /// <returns></returns>
    public Theme<TDrawingContext> SetRuleFor<T>(Action<T> predicate)
    {
        if (!Builders.TryGetValue(typeof(T), out var list))
            Builders[typeof(T)] = list = new();

        list.Add(predicate);

        return this;
    }

    /// <summary>
    /// Applies the rules to the specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    public void ApplyRuleTo<T>(T target)
    {
        if (!Builders.TryGetValue(typeof(T), out var builder)) return;
        foreach (var rule in builder) ((Action<T>)rule)(target);
    }
}
