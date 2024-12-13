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

namespace LiveChartsCore.Drawing.Layouts;

/// <summary>
/// Defines a container geometry.
/// </summary>
/// <typeparam name="TShape">The type of the container shape.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class BaseContainer<TShape, TDrawingContext> : Layout<TDrawingContext>, IDrawnElement<TDrawingContext>
    where TShape : BoundedDrawnGeometry, IDrawnElement<TDrawingContext>, new()
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets or sets the content.
    /// </summary>
    public IDrawnElement<TDrawingContext>? Content { get; set; }

    /// <summary>
    /// Gets the container drawn geometry.
    /// </summary>
    public TShape Geometry { get; } = new();

    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)" />
    public void Draw(TDrawingContext context)
    {
        var content = GetContent();

        var contentSize = content.Measure();

        Geometry.Parent = this;
        Geometry.Width = contentSize.Width;
        Geometry.Height = contentSize.Height;

        context.Draw(Geometry);

        content.Parent = this;
        context.Draw(content);
    }

    /// <inheritdoc cref="IDrawnElement.Measure" />
    public override LvcSize Measure() =>
        GetContent().Measure();

    private IDrawnElement<TDrawingContext> GetContent() =>
        Content ?? throw new InvalidOperationException("Content not found");
}
