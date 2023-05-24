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
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;

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
    private readonly TBackgroundGeometry _boundsGeometry = new();
    internal readonly Dictionary<int, Dictionary<int, VisualElement<TDrawingContext>?>> _positions = new();
    private LvcSize[,] _measuredSizes = new LvcSize[0, 0];
    private int _maxRow = 0;
    private int _maxColumn = 0;

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
    /// Gets or sets the background paint.
    /// </summary>
    public IPaint<TDrawingContext>? BackgroundPaint
    {
        get => _backgroundPaint;
        set => SetPaintProperty(ref _backgroundPaint, value);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure(Chart{TDrawingContext})"/>
    public override LvcSize Measure(Chart<TDrawingContext> chart)
    {
        float maxW = 0f, maxH = Padding.Top;
        _measuredSizes = new LvcSize[_maxRow + 2, _maxColumn + 2];

        foreach (var r in _positions.Keys)
        {
            if (!_positions.TryGetValue(r, out var row)) continue;

            var w = Padding.Left;
            foreach (var c in row.Keys)
            {
                if (!row.TryGetValue(c, out var visualElement) || visualElement is null) continue;

                var childSize = visualElement.Measure(chart);
                var cellSize = _measuredSizes[r, c];
                var rowSize = _measuredSizes[r, _maxColumn + 1];
                var columnSize = _measuredSizes[_maxRow + 1, c];

                if (cellSize.Width < childSize.Width) _measuredSizes[r, c] = new(childSize.Width, _measuredSizes[r, c].Height);
                if (cellSize.Height < childSize.Height) _measuredSizes[r, c] = new(_measuredSizes[r, c].Width, childSize.Height);
                if (rowSize.Height < childSize.Height) _measuredSizes[r, _maxColumn + 1] = new(0, childSize.Height);
                if (columnSize.Width < childSize.Width) _measuredSizes[_maxRow + 1, c] = new(childSize.Width, 0);

                w += _measuredSizes[r, c].Width;
                if (maxW < w) maxW = w;
            }

            maxH += _measuredSizes[r, _maxColumn + 1].Height;
        }

        return new(maxW + Padding.Right, maxH + Padding.Bottom);
    }

    /// <inheritdoc cref="ChartElement{TDrawingContext}.RemoveFromUI(Chart{TDrawingContext})"/>
    public override void RemoveFromUI(Chart<TDrawingContext> chart)
    {
        foreach (var r in _positions.Keys.ToArray())
        {
            if (!_positions.TryGetValue(r, out var row)) continue;
            foreach (var c in row.Keys)
            {
                if (!row.TryGetValue(c, out var child) || child is null) continue;
                RemoveChildAt(r, c);
            }
        }

        base.RemoveFromUI(chart);
    }

    /// <summary>
    /// Adds a child to the layout.
    /// </summary>
    /// <param name="child">The child.</param>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    public void AddChild(VisualElement<TDrawingContext> child, int row, int column)
    {
        if (!_positions.TryGetValue(row, out var r))
            _positions.Add(row, r = new Dictionary<int, VisualElement<TDrawingContext>?>());
        r[column] = child;
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
        var col = _positions[column];
        _ = col.Remove(row);
        if (col.Count == 0) _ = _positions.Remove(column);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.OnInvalidated(Chart{TDrawingContext})"/>
    protected internal override void OnInvalidated(Chart<TDrawingContext> chart)
    {
        var controlSize = Measure(chart);
        float w, h = Padding.Top;

        foreach (var r in _positions.Keys)
        {
            if (!_positions.TryGetValue(r, out var row)) continue;

            var rowHeight = _measuredSizes[r, _maxColumn + 1].Height;
            w = Padding.Left;
            foreach (var c in row.Keys)
            {
                if (!row.TryGetValue(c, out var visualElement) || visualElement is null) continue;
                var columnWidth = _measuredSizes[_maxRow + 1, c].Width;

                visualElement._x = HorizontalAlignment switch
                {
                    Align.Start => w,
                    Align.Middle => w + (columnWidth - _measuredSizes[r, c].Width) * 0.5f,
                    Align.End => w + columnWidth - _measuredSizes[r, c].Width,
                    _ => throw new System.NotImplementedException(),
                };
                visualElement._y = VerticalAlignment switch
                {
                    Align.Start => h,
                    Align.Middle => h + (rowHeight - _measuredSizes[r, c].Height) * 0.5f,
                    Align.End => h + rowHeight - _measuredSizes[r, c].Height,
                    _ => throw new System.NotImplementedException(),
                };

                visualElement.OnInvalidated(chart);
                visualElement.SetParent(_boundsGeometry);
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
        _boundsGeometry.X = (float)X;
        _boundsGeometry.Y = (float)Y;
        _boundsGeometry.Width = controlSize.Width;
        _boundsGeometry.Height = controlSize.Height;

        BackgroundPaint.AddGeometryToPaintTask(chart.Canvas, _boundsGeometry);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.SetParent(IGeometry{TDrawingContext})"/>
    protected internal override void SetParent(IGeometry<TDrawingContext> parent)
    {
        if (_boundsGeometry is null) return;
        _boundsGeometry.Parent = parent;
    }

    internal override IAnimatable?[] GetDrawnGeometries()
    {
        return new IAnimatable?[] { _boundsGeometry };
    }

    internal override IPaint<TDrawingContext>?[] GetPaintTasks()
    {
        return new[] { _backgroundPaint };
    }

    private class TablePosition
    {
        public TablePosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; set; }
        public int Column { get; set; }
    }
}

