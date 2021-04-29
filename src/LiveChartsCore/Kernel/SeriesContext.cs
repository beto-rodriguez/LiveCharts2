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
        private readonly IEnumerable<IDrawableSeries<TDrawingContext>> series;

        private int columnsCount = 0;
        private int rowsCount = 0;
        private int stackedColumnsCount = 0;
        private int stackedRowsCount = 0;
        private bool areBarsIndexed = false;

        private readonly Dictionary<IDrawableSeries<TDrawingContext>, int> columnPositions = new Dictionary<IDrawableSeries<TDrawingContext>, int>();
        private readonly Dictionary<IDrawableSeries<TDrawingContext>, int> rowPositions = new Dictionary<IDrawableSeries<TDrawingContext>, int>();
        private readonly Dictionary<int, int> stackColumnPositions = new Dictionary<int, int>();
        private readonly Dictionary<int, int> stackRowsPositions = new Dictionary<int, int>();

        private readonly Dictionary<string, Stacker<TDrawingContext>> stackers = new Dictionary<string, Stacker<TDrawingContext>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesContext{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        public SeriesContext(IEnumerable<IDrawableSeries<TDrawingContext>> series)
        {
            this.series = series;
        }

        #region columns and rows

        /// <summary>
        /// Gets the column postion.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetColumnPostion(IDrawableSeries<TDrawingContext> series)
        {
            if (areBarsIndexed) return columnPositions[series];
            IndexBars();
            return columnPositions[series];
        }

        /// <summary>
        /// Gets the column series count.
        /// </summary>
        /// <returns></returns>
        public int GetColumnSeriesCount()
        {
            if (areBarsIndexed) return columnsCount;
            IndexBars();
            return columnsCount;
        }

        /// <summary>
        /// Gets the row postion.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetRowPostion(IDrawableSeries<TDrawingContext> series)
        {
            if (areBarsIndexed) return rowPositions[series];
            IndexBars();
            return rowPositions[series];
        }

        /// <summary>
        /// Gets the row series count.
        /// </summary>
        /// <returns></returns>
        public int GetRowSeriesCount()
        {
            if (areBarsIndexed) return rowsCount;
            IndexBars();
            return rowsCount;
        }

        /// <summary>
        /// Gets the stacked column postion.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetStackedColumnPostion(IDrawableSeries<TDrawingContext> series)
        {
            if (areBarsIndexed) return stackColumnPositions[series.GetStackGroup()];
            IndexBars();
            return stackColumnPositions[series.GetStackGroup()];
        }

        /// <summary>
        /// Gets the stacked column series count.
        /// </summary>
        /// <returns></returns>
        public int GetStackedColumnSeriesCount()
        {
            if (areBarsIndexed) return stackedColumnsCount;
            IndexBars();
            return stackedColumnsCount;
        }

        /// <summary>
        /// Gets the stacked row postion.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns></returns>
        public int GetStackedRowPostion(IDrawableSeries<TDrawingContext> series)
        {
            if (areBarsIndexed) return stackRowsPositions[series.GetStackGroup()];
            IndexBars();
            return stackRowsPositions[series.GetStackGroup()];
        }

        /// <summary>
        /// Gets the stacked row series count.
        /// </summary>
        /// <returns></returns>
        public int GetStackedRowSeriesCount()
        {
            if (areBarsIndexed) return stackedRowsCount;
            IndexBars();
            return stackedRowsCount;
        }

        private void IndexBars()
        {
            columnsCount = 0;
            rowsCount = 0;
            stackedColumnsCount = 0;
            stackedRowsCount = 0;

            foreach (var item in series)
            {
                if (!item.IsBarSeries()) continue;

                if (item.IsColumnSeries())
                {
                    if (!item.IsStackedSeries())
                    {
                        columnPositions[item] = columnsCount++;
                        continue;
                    }

                    if (!stackColumnPositions.ContainsKey(item.GetStackGroup()))
                        stackColumnPositions[item.GetStackGroup()] = stackedColumnsCount++;

                    continue;
                }

                if (item.IsRowSeries())
                {
                    if (!item.IsRowSeries())
                    {
                        rowPositions[item] = rowsCount++;
                        continue;
                    }

                    if (!stackRowsPositions.ContainsKey(item.GetStackGroup()))
                        stackRowsPositions[item.GetStackGroup()] = stackedRowsCount++;

                    continue;
                }
            }

            areBarsIndexed = true;
        }

        #endregion

        #region stacked

        /// <summary>
        /// Gets the stack position.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="stackGroup">The stack group.</param>
        /// <returns></returns>
        public StackPosition<TDrawingContext>? GetStackPosition(IDrawableSeries<TDrawingContext> series, int stackGroup)
        {
            if (!series.IsStackedSeries()) return null;

            var s = GetStacker(series, stackGroup);

            return s == null
                ? null
                : new StackPosition<TDrawingContext>
            {
                Stacker = s,
                Position = s.GetSeriesStackPosition(series)
            };
        }

        private Stacker<TDrawingContext> GetStacker(IDrawableSeries<TDrawingContext> series, int stackGroup)
        {
            var key = $"{series.SeriesProperties}.{stackGroup}";

            if (!stackers.TryGetValue(key, out var stacker))
            {
                stacker = new Stacker<TDrawingContext>();
                stackers.Add(key, stacker);
            }

            return stacker;
        }

        #endregion
    }
}
