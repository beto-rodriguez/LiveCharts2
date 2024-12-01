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

using LiveChartsCore.Drawing;
using LiveChartsCore.Drawing.Layouts;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace LiveChartsCore.SkiaSharpView.Drawing.Layouts;

/// <inheritdoc cref="CoreStackLayout{TBackgroundGeometry, TDrawingContext}"/>
public class StackLayout : CoreStackLayout<RectangleGeometry, SkiaSharpDrawingContext>
{ }

/// <inheritdoc cref="CoreStackLayout{TBackgroundGeometry, TDrawingContext}"/>
public class StackLayout<TBackgroundGeometry>
    : CoreStackLayout<TBackgroundGeometry, SkiaSharpDrawingContext>
        where TBackgroundGeometry : CoreSizedGeometry, IDrawable<SkiaSharpDrawingContext>, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StackLayout{TBackgroundGeometry}"/> class.
    /// </summary>
    public StackLayout()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StackLayout{TBackgroundGeometry}"/> class,
    /// using the specified geometry as background.
    /// </summary>
    /// <param name="backgroundGeometry"></param>
    public StackLayout(TBackgroundGeometry backgroundGeometry)
        : base(backgroundGeometry)
    { }
}
