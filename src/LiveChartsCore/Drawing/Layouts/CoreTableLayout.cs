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
using System.Collections.Generic;
using System.Linq;

namespace LiveChartsCore.Drawing.Layouts;

/// <summary>
/// Defines a the stack panel geometry.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public abstract class CoreTableLayout<TDrawingContext>
    : Layout<TDrawingContext>, IDrawnElement<TDrawingContext>
        where TDrawingContext : DrawingContext
{
    private LvcSize[,] _measuredSizes = new LvcSize[0, 0];
    private readonly Dictionary<int, Dictionary<int, TableCell>> _positions = [];
    private int _maxRow = 0;
    private int _maxColumn = 0;

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align HorizontalAlignment { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align VerticalAlignment { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the cells.
    /// </summary>
    public TableCell[] Cells
    {
        get => _positions.Values.SelectMany(x => x.Values).ToArray();
        set
        {
            foreach (var cell in value)
                _ = AddChild(
                    cell.Drawable, cell.Row, cell.Column, cell.HorizontalAlign, cell.VerticalAlign);
        }
    }

    /// <summary>
    /// Adds a child to the layout.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="column">The column index.</param>
    /// <param name="child">The visual to add.</param>
    /// <param name="horizontalAlign">The cell horizontal alignment, if null the alignment will be defined by the layout.</param>
    /// <param name="verticalAlign">The cell vertical alignment, if null the alignment will be defined by the layout.</param>
    public CoreTableLayout<TDrawingContext> AddChild(
        IDrawnElement<TDrawingContext> child,
        int row,
        int column,
        Align? horizontalAlign = null,
        Align? verticalAlign = null)
    {
        if (!_positions.TryGetValue(row, out var r))
            _positions.Add(row, r = []);

        r[column] = new(row, column, child, verticalAlign, horizontalAlign);

        if (row > _maxRow) _maxRow = row;
        if (column > _maxColumn) _maxColumn = column;

        return this;
    }

    /// <summary>
    /// Removes a child from the layout.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    public void RemoveChildAt(int row, int column)
    {
        var r = _positions[row];
        _ = r.Remove(column);
        if (r.Count == 0) _ = _positions.Remove(row);
    }

    /// <inheritdoc cref="IDrawnElement{TDrawingContext}.Draw(TDrawingContext)"/>
    public void Draw(TDrawingContext context)
    {
        _ = Measure();

        float w, h = Padding.Top;

        for (var r = 0; r <= _maxRow; r++)
        {
            if (!_positions.TryGetValue(r, out var row)) continue;

            var rowHeight = _measuredSizes[r, _maxColumn + 1].Height;
            w = Padding.Left;

            for (var c = 0; c <= _maxColumn; c++)
            {
                var columnWidth = _measuredSizes[_maxRow + 1, c].Width;
                if (!row.TryGetValue(c, out var cell))
                {
                    w += columnWidth;
                    continue;
                }

                cell.Drawable.X = (cell.HorizontalAlign ?? HorizontalAlignment) switch
                {
                    Align.Start => w,
                    Align.Middle => w + (columnWidth - _measuredSizes[r, c].Width) * 0.5f,
                    Align.End => w + columnWidth - _measuredSizes[r, c].Width,
                    _ => throw new NotImplementedException(),
                };
                cell.Drawable.Y = (cell.VerticalAlign ?? VerticalAlignment) switch
                {
                    Align.Start => h,
                    Align.Middle => h + (rowHeight - _measuredSizes[r, c].Height) * 0.5f,
                    Align.End => h + rowHeight - _measuredSizes[r, c].Height,
                    _ => throw new NotImplementedException(),
                };

                cell.Drawable.Parent = this;
                w += columnWidth;
            }

            h += rowHeight;
        }

        var activeOpacity = context.ActiveOpacity;
        foreach (var child in GetChildren())
        {
            child.Parent = this;

            context.ActiveOpacity = activeOpacity * child.Opacity;
            context.Draw(child);
        }
    }

    /// <inheritdoc cref="IDrawnElement.Measure"/>
    public override LvcSize Measure()
    {
        var maxH = Padding.Top;
        _measuredSizes = new LvcSize[_maxRow + 2, _maxColumn + 2];

        var mr = _maxRow + 1;
        var mc = _maxColumn + 1;

        for (var r = 0; r <= _maxRow; r++)
        {
            if (!_positions.TryGetValue(r, out var row)) continue;

            for (var c = 0; c <= _maxColumn; c++)
            {
                var cellSize = _measuredSizes[r, c];
                var rowSize = _measuredSizes[r, mc];
                var columnSize = _measuredSizes[mr, c];

                if (!row.TryGetValue(c, out var cell)) continue;

                var childSize = cell.Drawable.Measure();

                if (cellSize.Width < childSize.Width)
                    _measuredSizes[r, c] = new(childSize.Width, _measuredSizes[r, c].Height);
                if (cellSize.Height < childSize.Height)
                    _measuredSizes[r, c] = new(_measuredSizes[r, c].Width, childSize.Height);
                if (rowSize.Height < childSize.Height)
                    _measuredSizes[r, mc] = new(0, childSize.Height);
                if (columnSize.Width < childSize.Width)
                    _measuredSizes[mr, c] = new(childSize.Width, 0);
            }

            maxH += _measuredSizes[r, _maxColumn + 1].Height;
        }

        var maxW = Padding.Left;
        for (var c = 0; c <= _maxColumn; c++)
            maxW += _measuredSizes[_maxRow + 1, c].Width;

        return new(maxW + Padding.Right, maxH + Padding.Bottom);
    }
    private IEnumerable<IDrawnElement<TDrawingContext>> GetChildren() =>
        _positions.Values.SelectMany(x => x.Values.Select(y => y.Drawable));

    /// <summary>
    /// Defines a table cell.
    /// </summary>
    /// <param name="row">the row index.</param>
    /// <param name="column">the column index.</param>
    /// <param name="drawable">The drawable element.</param>
    /// <param name="verticalAlign">the vertical alignment.</param>
    /// <param name="horizontalAlign">The horizontal alignment.</param>
    public class TableCell(
        int row,
        int column,
        IDrawnElement<TDrawingContext> drawable,
        Align? verticalAlign = null,
        Align? horizontalAlign = null)
    {
        /// <summary>
        /// Gets the row.
        /// </summary>
        public int Row { get; } = row;

        /// <summary>
        /// Gets the column.
        /// </summary>
        public int Column { get; } = column;

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        public Align? VerticalAlign { get; } = verticalAlign;

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        public Align? HorizontalAlign { get; } = horizontalAlign;

        /// <summary>
        /// Gets the visual element.
        /// </summary>
        public IDrawnElement<TDrawingContext> Drawable { get; } = drawable;
    }
}
