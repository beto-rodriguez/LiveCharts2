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

using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LiveChartsCore.SkiaSharpView.Maui.Handlers;

/// <summary>
/// Just a view that does nothing, but helps to make the XAML work.
/// </summary>
public class EmptyContentView : View, IContentView
{
    /// <summary>
    /// Gets the content.
    /// </summary>
    public object? Content => null;

    /// <summary>
    /// Gets the presented content.
    /// </summary>
    public IView? PresentedContent => null;

    /// <summary>
    /// Gets the padding.
    /// </summary>
    Thickness IPadding.Padding => new();

    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public Size CrossPlatformArrange(Rect bounds) => new();

    /// <summary>
    /// Measures the specified width constraint.
    /// </summary>
    /// <param name="widthConstraint"></param>
    /// <param name="heightConstraint"></param>
    /// <returns></returns>
    public Size CrossPlatformMeasure(double widthConstraint, double heightConstraint) => new();
}
