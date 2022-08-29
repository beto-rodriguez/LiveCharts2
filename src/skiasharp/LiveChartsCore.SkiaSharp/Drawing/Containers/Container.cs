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

using System.Collections.Generic;
using LiveChartsCore.Drawing;

namespace LiveChartsCore.SkiaSharpView.Drawing.Containers;

/// <summary>
/// Defines a drawable container.
/// </summary>
public class Container : Animatable, IDrawable<SkiaSharpDrawingContext>
{
    private readonly HashSet<IPaint<SkiaSharpDrawingContext>> _children = new();

    /// <inheritdoc cref="IDrawable{TDrawingContext}.Draw(TDrawingContext)"/>
    public void Draw(SkiaSharpDrawingContext context)
    {
        //context.Canvas.Translate(new SKPoint(100, 100));
        context.Canvas.Translate(100, 100);
    }

    /// <summary>
    /// Adds a child to the container.
    /// </summary>
    /// <param name="child">The child.</param>
    public void AddChild(IPaint<SkiaSharpDrawingContext> child)
    {
        _ = _children.Add(child);
    }

    /// <summary>
    /// Removes a child from the container.
    /// </summary>
    /// <param name="child">The child.</param>
    public void RemoveChild(IPaint<SkiaSharpDrawingContext> child)
    {
        _ = _children.Remove(child);
    }
}
