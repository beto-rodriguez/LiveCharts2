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

/// <inheritdoc cref="CoreTableLayout{TBackgroundGeometry, TDrawingContext}"/>
public class TableLayout : CoreTableLayout<RectangleGeometry, SkiaSharpDrawingContext>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableLayout"/> class.
    /// </summary>
    public TableLayout()
    { }

    /// <inheritdoc cref="TableLayout{TBackgroundGeometry}.AddChild(IDrawable{SkiaSharpDrawingContext}, int, int, Align?, Align?)"/>
    public new TableLayout AddChild(
        IDrawable<SkiaSharpDrawingContext> drawable, int row,
        int column,
        Align? horizontalAlign = null,
        Align? verticalAlign = null)
    {
        _ = base.AddChild(drawable, row, column, horizontalAlign, verticalAlign);
        return this;
    }
}

/// <inheritdoc cref="CoreTableLayout{TBackgroundGeometry, TDrawingContext}"/>
public class TableLayout<TBackgroundGeometry> : CoreTableLayout<TBackgroundGeometry, SkiaSharpDrawingContext>
    where TBackgroundGeometry : CoreSizedGeometry, IDrawable<SkiaSharpDrawingContext>, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TableLayout{TBackgroundGeometry}"/> class.
    /// </summary>
    public TableLayout()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableLayout{TBackgroundGeometry}"/> class,
    /// </summary>
    /// <param name="backgroundGeometry">The background geometry.</param>
    public TableLayout(TBackgroundGeometry backgroundGeometry)
        : base(backgroundGeometry)
    { }

    /// <inheritdoc cref="TableLayout{TBackgroundGeometry}.AddChild(IDrawable{SkiaSharpDrawingContext}, int, int, Align?, Align?)"/>
    public new TableLayout<TBackgroundGeometry> AddChild(
        IDrawable<SkiaSharpDrawingContext> drawable, int row,
        int column,
        Align? horizontalAlign = null,
        Align? verticalAlign = null)
    {
        _ = base.AddChild(drawable, row, column, horizontalAlign, verticalAlign);
        return this;
    }
}
