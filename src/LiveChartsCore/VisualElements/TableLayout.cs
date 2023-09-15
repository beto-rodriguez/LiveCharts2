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
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;

namespace LiveChartsCore.VisualElements;

/// <summary>
/// Defines the table panel class.
/// </summary>
/// <typeparam name="TBackgroundGeometry">The type of the background geometry.</typeparam>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public class TableLayout<TBackgroundGeometry, TDrawingContext> : VisualElement<TDrawingContext>
    where TDrawingContext : DrawingContext
    where TBackgroundGeometry : ISizedGeometry<TDrawingContext>, new()
{
    private IPaint<TDrawingContext>? _backgroundPaint;
    private readonly Dictionary<int, Dictionary<int, TableCell>> _positions = new();
    private LvcSize[,] _measuredSizes = new LvcSize[0, 0];
    private int _maxRow = 0;
    private int _maxColumn = 0;
    private Padding _padding = new();
    private Align _horizontalAlignment = Align.Middle;
    private Align _verticalAlignment = Align.Middle;

    /// <summary>
    /// Initializes a new instance of the <see cref="TableLayout{TBackgroundGeometry, TDrawingContext}"/> class.
    /// </summary>
    public TableLayout()
    {
        ClippingMode = ClipMode.None;
    }

    /// <summary>
    /// Gets or sets the padding.
    /// </summary>
    public Padding Padding { get => _padding; set => SetProperty(ref _padding, value); }

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align HorizontalAlignment { get => _horizontalAlignment; set => SetProperty(ref _horizontalAlignment, value); }

    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public Align VerticalAlignment { get => _verticalAlignment; set => SetProperty(ref _verticalAlignment, value); }

    /// <summary>
    /// Gets or sets the background paint.
    /// </summary>
    public IPaint<TDrawingContext>? BackgroundPaint
    {
        get => _backgroundPaint;
        set => SetPaintProperty(ref _backgroundPaint, value);
    }

    /// <summary>
    /// Gets the background geometry.
    /// </summary>
    public TBackgroundGeometry BackgroundGeometry { get; } = new();

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
    public override LvcSize Measure(Chart<TDrawingContext> chart)
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

                var childSize = cell.VisualElement.Measure(chart);

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

    /// <inheritdoc cref="ChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})"/>
    public override void RemoveFromUI(Chart<TDrawingContext> chart)
    {
        foreach (var child in EnumerateChildren())
            chart.RemoveVisual(child.VisualElement);

        base.RemoveFromUI(chart);
    }

    /// <summary>
    /// Adds a child to the layout.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="column">The column index.</param>
    /// <param name="child">The visual to add.</param>
    /// <param name="horizontalAlign">The cell horizontal alignment, if null the alignment will be defined by the layout.</param>
    /// <param name="verticalAlign">The cell vertical alignment, if null the alignment will be defined by the layout.</param>
    public void AddChild(
        VisualElement<TDrawingContext> child,
        int row, int column,
        Align? horizontalAlign = null,
        Align? verticalAlign = null)
    {
        if (!_positions.TryGetValue(row, out var r))
            _positions.Add(row, r = new Dictionary<int, TableCell>());
        r[column] = new(row, column, child, verticalAlign, horizontalAlign);

        if (row > _maxRow) _maxRow = row;
        if (column > _maxColumn) _maxColumn = column;
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

    /// <summary>
    /// Enumerates the children in the layout.
    /// </summary>
    public IEnumerable<TableCell> EnumerateChildren()
    {
        foreach (var r in _positions.Keys)
            foreach (var c in _positions[r].Keys)
                yield return _positions[r][c];
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.OnInvalidated(Chart{TDrawingContext})"/>
    protected internal override void OnInvalidated(Chart<TDrawingContext> chart)
    {
        var controlSize = Measure(chart);
        var clipping = Clipping.GetClipRectangle(ClippingMode, chart);

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

                cell.VisualElement._x = (cell.HorizontalAlign ?? HorizontalAlignment) switch
                {
                    Align.Start => w,
                    Align.Middle => w + (columnWidth - _measuredSizes[r, c].Width) * 0.5f,
                    Align.End => w + columnWidth - _measuredSizes[r, c].Width,
                    _ => throw new System.NotImplementedException(),
                };
                cell.VisualElement._y = (cell.VerticalAlign ?? VerticalAlignment) switch
                {
                    Align.Start => h,
                    Align.Middle => h + (rowHeight - _measuredSizes[r, c].Height) * 0.5f,
                    Align.End => h + rowHeight - _measuredSizes[r, c].Height,
                    _ => throw new System.NotImplementedException(),
                };

                cell.VisualElement.OnInvalidated(chart);
                cell.VisualElement.SetParent(BackgroundGeometry);
                w += columnWidth;
            }

            h += rowHeight;
        }

        // NOTE #20231605
        // force the background to have at least an invisible geometry
        // we use this geometry in the motion canvas to track the position
        // of the stack panel as the time and animations elapse.
        BackgroundPaint ??= LiveCharts.DefaultSettings
            .GetProvider<TDrawingContext>()
            .GetSolidColorPaint(new LvcColor(0, 0, 0, 0));

        chart.Canvas.AddDrawableTask(BackgroundPaint);
        BackgroundGeometry.X = (float)X;
        BackgroundGeometry.Y = (float)Y;
        BackgroundGeometry.Width = controlSize.Width;
        BackgroundGeometry.Height = controlSize.Height;
        BackgroundGeometry.RotateTransform = (float)Rotation;
        BackgroundGeometry.TranslateTransform = Translate;

        BackgroundPaint.AddGeometryToPaintTask(chart.Canvas, BackgroundGeometry);
        BackgroundPaint.SetClipRectangle(chart.Canvas, clipping);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.SetParent(IGeometry{TDrawingContext})"/>
    protected internal override void SetParent(IGeometry<TDrawingContext> parent)
    {
        if (BackgroundGeometry is null) return;
        BackgroundGeometry.Parent = parent;
    }

    internal override IAnimatable?[] GetDrawnGeometries()
    {
        return new IAnimatable?[] { BackgroundGeometry };
    }

    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _backgroundPaint };
    }

    internal override IEnumerable<VisualElement<TDrawingContext>> IsHitBy(Chart<TDrawingContext> chart, LvcPoint point)
    {
        var location = GetActualCoordinate();
        var size = Measure(chart);

        // it returns an enumerable because there are more complex types where a visual can contain more than one element
        if (point.X >= location.X && point.X <= location.X + size.Width &&
            point.Y >= location.Y && point.Y <= location.Y + size.Height)
        {
            yield return this;
        }

        var relativePoint = new LvcPoint(point.X - location.X, point.Y - location.Y);
        foreach (var child in EnumerateChildren())
        {
            foreach (var element in child.VisualElement.IsHitBy(chart, relativePoint))
            {
                yield return element;
            }
        }
    }

    /// <summary>
    /// Defines a cell in the <see cref="TableLayout{TBackgroundGeometry, TDrawingContext}"/>.
    /// </summary>
    public class TableCell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCell"/> class.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="visualElement">The visual to add.</param>
        /// <param name="verticalAlign">The cell vertical alignment, if null the alignment will be defined by the layout.</param>
        /// <param name="horizontalAlign">The cell horizontal alignment, if null the alignment will be defined by the layout.</param>
        public TableCell(
            int row,
            int column,
            VisualElement<TDrawingContext> visualElement,
            Align? verticalAlign = null,
            Align? horizontalAlign = null)
        {
            Row = row;
            Column = column;
            VisualElement = visualElement;
            VerticalAlign = verticalAlign;
            HorizontalAlign = horizontalAlign;
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        public int Row { get; }

        /// <summary>
        /// Gets the column.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Gets or sets the vertical alignment.
        /// </summary>
        public Align? VerticalAlign { get; }

        /// <summary>
        /// Gets or sets the horizontal alignment.
        /// </summary>
        public Align? HorizontalAlign { get; }

        /// <summary>
        /// Gets the visual element.
        /// </summary>
        public VisualElement<TDrawingContext> VisualElement { get; }
    }
}

