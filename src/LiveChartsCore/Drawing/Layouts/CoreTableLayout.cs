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

namespace LiveChartsCore.Drawing.Layouts;

/// <summary>
/// Defines a the stack panel geometry.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
/// <typeparam name="TBackgroundGeometry">The type of the background geometry.</typeparam>
public abstract class CoreTableLayout<TBackgroundGeometry, TDrawingContext>
    : CoreGeometry, IDrawable<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TBackgroundGeometry : CoreSizedGeometry, IDrawable<TDrawingContext>, new()
{
    private LvcSize[,] _measuredSizes = new LvcSize[0, 0];
    private readonly Dictionary<int, Dictionary<int, TableCell>> _positions = [];
    private int _maxRow = 0;
    private int _maxColumn = 0;
    private readonly TBackgroundGeometry _backgroundGeometry = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreStackLayout{TBackgroundGeometry, TDrawingContext}"/> class.
    /// </summary>
    protected CoreTableLayout()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreStackLayout{TBackgroundGeometry, TDrawingContext}"/> class,
    /// and uses the specified geometry as background.
    /// </summary>
    /// <param name="backgroundGeometry">The background gemetry.</param>
    protected CoreTableLayout(TBackgroundGeometry backgroundGeometry)
    {
        _backgroundGeometry = backgroundGeometry;
    }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public Padding Padding { get; set; } = new();

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align HorizontalAlignment { get; set; } = Align.Middle;

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align VerticalAlignment { get; set; } = Align.Middle;

    /// <summary>
    /// Adds a child to the layout.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="column">The column index.</param>
    /// <param name="child">The visual to add.</param>
    /// <param name="horizontalAlign">The cell horizontal alignment, if null the alignment will be defined by the layout.</param>
    /// <param name="verticalAlign">The cell vertical alignment, if null the alignment will be defined by the layout.</param>
    public CoreTableLayout<TBackgroundGeometry, TDrawingContext> AddChild(
        IDrawable<TDrawingContext> child,
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

    /// <inheritdoc cref="IDrawable{TDrawingContext}.Draw(TDrawingContext)"/>
    public void Draw(TDrawingContext context)
    {
        var controlSize = Measure();

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

        _backgroundGeometry.X = X;
        _backgroundGeometry.Y = Y;
        _backgroundGeometry.Height = controlSize.Height;
        _backgroundGeometry.Width = controlSize.Width;
        _backgroundGeometry.Fill = Fill;
        _backgroundGeometry.Stroke = Stroke;

        _backgroundGeometry.Draw(context);
    }

    /// <inheritdoc cref="IDrawable.Measure"/>
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

    private class TableCell(
        int row,
        int column,
        IDrawable<TDrawingContext> visualElement,
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
        public IDrawable<TDrawingContext> Drawable { get; } = visualElement;
    }
}
