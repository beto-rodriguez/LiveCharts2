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
/// Defines a the stack panel geometry.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public abstract class CoreStackLayout<TDrawingContext>
    : Layout, IDrawable<TDrawingContext>
        where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CoreStackLayout{TDrawingContext}"/> class.
    /// </summary>
    protected CoreStackLayout()
    { }

    /// <summary>
    /// Gets or sets the panel orientation.
    /// </summary>
    public ContainerOrientation Orientation { get; set; }

    /// <summary>
    /// Gets or sets the vertical alignment.
    /// </summary>
    public Align VerticalAlignment { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align HorizontalAlignment { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public Padding Padding { get; set; } = new();

    /// <summary>
    /// Gets or sets the maximum width. When the maximum with is reached, a new row is created.
    /// </summary>
    public double MaxWidth { get; set; } = double.MaxValue;

    /// <summary>
    /// Gets or sets the maximum height. When the maximum height is reached, a new column is created.
    /// </summary>
    public double MaxHeight { get; set; } = double.MaxValue;

    /// <inheritdoc cref="IDrawable{TDrawingContext}.Children"/>
    public IEnumerable<IDrawable<TDrawingContext>> Children { get; set; } = [];

    /// <inheritdoc cref="IDrawable{TDrawingContext}.Draw(TDrawingContext)"/>
    public void Draw(TDrawingContext context) =>
        _ = Measure();

    /// <inheritdoc cref="IDrawable.Measure"/>
    public override LvcSize Measure()
    {
        var xl = Padding.Left;
        var yl = Padding.Top;
        var rowHeight = -1f;
        var columnWidth = -1f;
        var mx = 0f;
        var my = 0f;

        List<MeasureResult> line = [];

        LvcSize alignCurrentLine()
        {
            var mx = -1f;
            var my = -1f;

            foreach (var child in line)
            {
                if (Orientation == ContainerOrientation.Horizontal)
                {
                    child.Drawable.Y = VerticalAlignment switch
                    {
                        Align.Start => yl,
                        Align.Middle => yl + (rowHeight - child.Size.Height) / 2f,
                        Align.End => yl + rowHeight - child.Size.Height,
                        _ => throw new System.NotImplementedException()
                    };
                }
                else
                {
                    child.Drawable.X = HorizontalAlignment switch
                    {
                        Align.Start => xl,
                        Align.Middle => xl + (columnWidth - child.Size.Width) / 2f,
                        Align.End => xl + columnWidth - child.Size.Width,
                        _ => throw new System.NotImplementedException()
                    };
                }

                var childSize = child.Drawable.Measure();

                if (childSize.Width > mx) mx = childSize.Width;
                if (childSize.Height > my) my = childSize.Height;
            }

            line = [];
            return new LvcSize(mx, my);
        }

        foreach (var child in Children)
        {
            var childSize = child.Measure();

            if (Orientation == ContainerOrientation.Horizontal)
            {
                if (xl + childSize.Width > MaxWidth)
                {
                    var lineSize = alignCurrentLine();
                    xl = Padding.Left;
                    yl += lineSize.Height;
                    rowHeight = -1f;
                }

                if (rowHeight < childSize.Height) rowHeight = childSize.Height;
                child.X = xl;

                xl += childSize.Width;
            }
            else
            {
                if (yl + childSize.Height > MaxHeight)
                {
                    var lineSize = alignCurrentLine();
                    yl = Padding.Top;
                    xl += lineSize.Width;
                    columnWidth = -1f;
                }

                if (columnWidth < childSize.Width) columnWidth = childSize.Width;
                child.Y = yl;

                yl += childSize.Height;
            }

            if (xl > mx) mx = xl;
            if (yl > my) my = yl;
            line.Add(new MeasureResult(child, childSize));
        }

        if (line.Count > 0)
        {
            var lineSize = alignCurrentLine();

            if (Orientation == ContainerOrientation.Horizontal)
            {
                yl += lineSize.Height;
            }
            else
            {
                xl += lineSize.Width;
            }

            if (xl > mx) mx = xl;
            if (yl > my) my = yl;
        }

        return new LvcSize(mx + Padding.Right, my + Padding.Bottom);
    }

    private class MeasureResult(IDrawable<TDrawingContext> visual, LvcSize size)
    {
        public IDrawable<TDrawingContext> Drawable { get; set; } = visual;
        public LvcSize Size { get; set; } = size;
    }
}
