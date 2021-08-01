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

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using System.Collections.Generic;

namespace LiveChartsCore.Kernel
{
    /// <summary>
    /// Defines a series context.
    /// </summary>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    public class SeriesContext<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly IEnumerable<IChartSeries<TDrawingContext>> _series;

        private int _columnsCount = 0;
        private int _rowsCount = 0;
        private int _stackedColumnsCount = 0;
        private int _stackedRowsCount = 0;
        private bool _areBarsIndexed = false;

        private readonly Dictionary<IChartSeries<TDrawingContext>, int> _columnPositions = new();
        private readonly Dictionary<IChartSeries<TDrawingContext>, int> _rowPositions = new();
        private readonly Dictionary<int, int> _stackColumnPositions = new();
        private readonly Dictionary<int, int> _stackRowsPositions = new();

        private readonly Dictionary<string, Stacker<TDrawingContext>> _stackers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesContext{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        public SeriesContext(IEnumerable<IChartSeries<TDrawingContext>> series)
        {
            _series = series;
        }

        #region columns and rows

        /// <summary>
        /// Gets the column position.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetColumnPostion(IChartSeries<TDrawingContext> series)
        {
            if (_areBarsIndexed) return _columnPositions[series];
            IndexBars();
            return _columnPositions[series];
        }

        /// <summary>
        /// Gets the column series count.
        /// </summary>
        /// <returns></returns>
        public int GetColumnSeriesCount()
        {
            if (_areBarsIndexed) return _columnsCount;
            IndexBars();
            return _columnsCount;
        }

        /// <summary>
        /// Gets the row position.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetRowPostion(IChartSeries<TDrawingContext> series)
        {
            if (_areBarsIndexed) return _rowPositions[series];
            IndexBars();
            return _rowPositions[series];
        }

        /// <summary>
        /// Gets the row series count.
        /// </summary>
        /// <returns></returns>
        public int GetRowSeriesCount()
        {
            if (_areBarsIndexed) return _rowsCount;
            IndexBars();
            return _rowsCount;
        }

        /// <summary>
        /// Gets the stacked column position.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetStackedColumnPostion(IChartSeries<TDrawingContext> series)
        {
            if (_areBarsIndexed) return _stackColumnPositions[series.GetStackGroup()];
            IndexBars();
            return _stackColumnPositions[series.GetStackGroup()];
        }

        /// <summary>
        /// Gets the stacked column series count.
        /// </summary>
        /// <returns></returns>
        public int GetStackedColumnSeriesCount()
        {
            if (_areBarsIndexed) return _stackedColumnsCount;
            IndexBars();
            return _stackedColumnsCount;
        }

        /// <summary>
        /// Gets the stacked row position.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetStackedRowPostion(IChartSeries<TDrawingContext> series)
        {
            if (_areBarsIndexed) return _stackRowsPositions[series.GetStackGroup()];
            IndexBars();
            return _stackRowsPositions[series.GetStackGroup()];
        }

        /// <summary>
        /// Gets the stacked row series count.
        /// </summary>
        /// <returns></returns>
        public int GetStackedRowSeriesCount()
        {
            if (_areBarsIndexed) return _stackedRowsCount;
            IndexBars();
            return _stackedRowsCount;
        }

        private void IndexBars()
        {
            _columnsCount = 0;
            _rowsCount = 0;
            _stackedColumnsCount = 0;
            _stackedRowsCount = 0;

            foreach (var item in _series)
            {
                if (!item.IsBarSeries()) continue;

                if (item.IsColumnSeries())
                {
                    if (!item.IsStackedSeries())
                    {
                        _columnPositions[item] = _columnsCount++;
                        continue;
                    }

                    if (!_stackColumnPositions.ContainsKey(item.GetStackGroup()))
                        _stackColumnPositions[item.GetStackGroup()] = _stackedColumnsCount++;

                    continue;
                }

                if (item.IsRowSeries())
                {
                    if (!item.IsRowSeries())
                    {
                        _rowPositions[item] = _rowsCount++;
                        continue;
                    }

                    if (!_stackRowsPositions.ContainsKey(item.GetStackGroup()))
                        _stackRowsPositions[item.GetStackGroup()] = _stackedRowsCount++;

                    continue;
                }
            }

            _areBarsIndexed = true;
        }

        #endregion

        #region stacked

        /// <summary>
        /// Gets the stack position.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="stackGroup">The stack group.</param>
        /// <returns></returns>
        public StackPosition<TDrawingContext>? GetStackPosition(IChartSeries<TDrawingContext> series, int stackGroup)
        {
            if (!series.IsStackedSeries()) return null;

            var s = GetStacker(series, stackGroup);

            return s is null
                ? null
                : new StackPosition<TDrawingContext>
                {
                    Stacker = s,
                    Position = s.GetSeriesStackPosition(series)
                };
        }

        private Stacker<TDrawingContext> GetStacker(IChartSeries<TDrawingContext> series, int stackGroup)
        {
            var key = $"{series.SeriesProperties}.{stackGroup}";

            if (!_stackers.TryGetValue(key, out var stacker))
            {
                stacker = new Stacker<TDrawingContext>();
                _stackers.Add(key, stacker);
            }

            return stacker;
        }

        #endregion
    }
}
