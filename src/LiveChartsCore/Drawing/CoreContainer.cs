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

namespace LiveChartsCore.Drawing;

/// <summary>
/// Defines a container geometry.
/// </summary>
/// <typeparam name="TShape">The type of the container shape.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class CoreContainer<TShape, TDrawingContext> : CoreSizedGeometry, IDrawable<TDrawingContext>
    where TShape : CoreSizedGeometry, IDrawable<TDrawingContext>, new()
    where TDrawingContext : DrawingContext
{
    private readonly TShape _containerGeometry = new();

    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    public IDrawable<TDrawingContext>? Content { get; set; }

    /// <inheritdoc cref="IDrawable{TDrawingContext}.Draw(TDrawingContext)" />
    public void Draw(TDrawingContext context)
    {
        var content = Content ?? throw new InvalidOperationException("Content not found");

        var contentSize = content.Measure();

        _containerGeometry.X = X;
        _containerGeometry.Y = Y;
        _containerGeometry.Width = contentSize.Width;
        _containerGeometry.Height = contentSize.Height;

        context.Draw(_containerGeometry);

        content.Parent = this;
        context.Draw(content);
    }
}
