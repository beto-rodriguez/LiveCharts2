﻿// The MIT License(MIT)
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

using LiveChartsCore.Kernel;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <summary>
/// The Uwp poing class, just used to bind the tooltips.
/// </summary>
public class BindingPoint
{
    /// <summary>
    /// Ges the chart point.
    /// </summary>
    public ChartPoint ChartPoint { get; init; } = null!; // <- how to force the init.. so the nullables do not complain?

    /// <summary>
    /// Gets the font family.
    /// </summary>
    public FontFamily FontFamily { get; init; } = null!;

    /// <summary>
    /// Gets the foreground.
    /// </summary>
    public Brush Foreground { get; init; } = null!;

    /// <summary>
    /// Gets the font size.
    /// </summary>
    public double FontSize { get; init; }

    /// <summary>
    /// Gets the font weight.
    /// </summary>
    public FontWeight FontWeight { get; init; }

    /// <summary>
    /// Gets the font style.
    /// </summary>
    public FontStyle FontStyle { get; init; }

    /// <summary>
    /// Gets the font stretch.
    /// </summary>
    public FontStretch FontStretch { get; init; }
}
