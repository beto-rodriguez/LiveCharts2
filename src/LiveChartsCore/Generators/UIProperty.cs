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

namespace LiveChartsCore.Generators;

/// <summary>
/// Defines a xaml property, used to generate bindable/dependency/avalonia or whatever properties.
/// </summary>
/// <typeparam name="T">The type of the property.</typeparam>
/// <param name="defaultValue">The default value.</param>
/// <param name="onChanged">The method to call when the property changes.</param>
public class UIProperty<T>(
    T? defaultValue = default,
    Delegate? onChanged = null)
{
    /// <summary>
    /// Gets the default value for the property.
    /// </summary>
    public T? DefaultValue { get; } = defaultValue;

    /// <summary>
    /// Gets the method to call when the property changes.
    /// </summary>
    public Delegate? OnChanged { get; } = onChanged;
}
