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
using System.Globalization;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.Maui.Controls;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <summary>
/// A converter value helper
/// </summary>
public class PaintTasksValueConverter : IValueConverter
{
    /// <summary>
    /// Implement this method to convert <paramref name="value" /> to <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The type to which to convert the value.</param>
    /// <param name="parameter">A parameter to use during the conversion.</param>
    /// <param name="culture">The culture to use during the conversion.</param>
    /// <returns>
    /// To be added.
    /// </returns>
    /// <remarks>
    /// To be added.
    /// </remarks>
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is IChartSeries<SkiaSharpDrawingContext> v
            ? v.CanvasSchedule.PaintSchedules
            : null;
    }

    /// <summary>
    /// Implement this method to convert <paramref name="value" /> back from <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="targetType">The type to which to convert the value.</param>
    /// <param name="parameter">A parameter to use during the conversion.</param>
    /// <param name="culture">The culture to use during the conversion.</param>
    /// <returns>
    /// To be added.
    /// </returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <remarks>
    /// To be added.
    /// </remarks>
    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
