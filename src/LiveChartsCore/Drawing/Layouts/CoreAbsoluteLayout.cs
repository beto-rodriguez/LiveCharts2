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

namespace LiveChartsCore.Drawing.Layouts;

/// <summary>
/// Defines an absolute layout.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public abstract class CoreAbsoluteLayout<TDrawingContext>
    : Layout<TDrawingContext>, IDrawnElement<TDrawingContext>
        where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the children.
    /// </summary>
    public List<IDrawnElement<TDrawingContext>> Children { get; set; } = [];

    /// <summary>
    /// Gets or sets the maximum width, default is <see cref="double.MaxValue"/>, and means that
    /// the <see cref="Measure"/> method will return the maximum width of all children.
    /// </summary>
    public double Width { get; set; } = double.MaxValue;

    /// <summary>
    /// Gets or sets the maximum height, default is <see cref="double.MaxValue"/>, and means that
    /// the <see cref="Measure"/> method will return the maximum height of all children.
    /// </summary>
    public double Height { get; set; } = double.MaxValue;

    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)"/>
    public void Draw(TDrawingContext context)
    {
        var activeOpacity = context.ActiveOpacity;

        foreach (var child in Children)
        {
            child.Parent = this;

            context.ActiveOpacity = activeOpacity * child.Opacity;
            context.Draw(child);
        }
    }

    /// <inheritdoc cref="IDrawnElement.Measure"/>
    public override LvcSize Measure()
    {
        float maxW = 0, maxH = 0;

        foreach (var child in Children)
        {
            var childSize = child.Measure();

            if (childSize.Width > maxW) maxW = childSize.Width;
            if (childSize.Height > maxH) maxH = childSize.Height;
        }

        if (maxW > Width) maxW = (float)Width;
        if (maxH > Height) maxH = (float)Height;

        return new LvcSize(maxW, maxH);
    }
}
