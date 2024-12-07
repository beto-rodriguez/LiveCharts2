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
using LiveChartsCore.Drawing;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries;

/// <summary>
/// Defines a container geometry.
/// </summary>
/// <typeparam name="T">The type of the background geometry.</typeparam>
public class Container<T> : CoreSizedGeometry, ISkiaGeometry
    where T : CoreSizedGeometry, ISkiaGeometry, new()
{
    private readonly T _containerGeometry = new();

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    public IDrawable<SkiaSharpDrawingContext>? Content { get; set; }

    /// <inheritdoc cref="IDrawable{TDrawingContext}.Draw(TDrawingContext)" />
    public void Draw(SkiaSharpDrawingContext context) =>
        OnDraw(context, context.ActiveSkiaPaint);

    /// <inheritdoc cref="ISkiaGeometry.OnDraw(SkiaSharpDrawingContext, SKPaint)" />
    public void OnDraw(SkiaSharpDrawingContext context, SKPaint paint)
    {
        var content = Content ?? throw new InvalidOperationException("Content not found");

        var contentSize = content.Measure();

        _containerGeometry.X = X;
        _containerGeometry.Y = Y;
        _containerGeometry.Width = contentSize.Width;
        _containerGeometry.Height = contentSize.Height;

        _containerGeometry.OnDraw(context, paint);

        content.Parent = this;
        context.Draw(content);
    }
}
