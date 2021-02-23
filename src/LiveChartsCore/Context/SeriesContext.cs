// The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using System.Collections.Generic;

namespace LiveChartsCore.Context
{
    public class SeriesContext<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly IEnumerable<IDataSeries<TDrawingContext>> series;

        private int columnsCount = 0;
        private int rowsCount = 0;
        private int stackedColumnsCount = 0;
        private int stackedRowsCount = 0;
        private bool areBarsIndexed = false;

        private Dictionary<IDataSeries<TDrawingContext>, int> columnPositions = new Dictionary<IDataSeries<TDrawingContext>, int>();
        private Dictionary<IDataSeries<TDrawingContext>, int> rowPositions = new Dictionary<IDataSeries<TDrawingContext>, int>();
        private Dictionary<int, int> stackColumnPositions = new Dictionary<int, int>();
        private Dictionary<int, int> stackRowsPositions = new Dictionary<int, int>();

        private Dictionary<string, Stacker<TDrawingContext>> stackers = new Dictionary<string, Stacker<TDrawingContext>>();

        public SeriesContext(IEnumerable<IDataSeries<TDrawingContext>> series)
        {
            this.series = series;
        }

        #region columns and rows

        public int GetColumnPostion(IDataSeries<TDrawingContext> series)
        {
            if (areBarsIndexed) return columnPositions[series];
            IndexBars();
            return columnPositions[series];
        }

        public int GetColumnSeriesCount()
        {
            if (areBarsIndexed) return columnsCount;
            IndexBars();
            return columnsCount;
        }

        public int GetRowPostion(IDataSeries<TDrawingContext> series)
        {
            if (areBarsIndexed) return rowPositions[series];
            IndexBars();
            return rowPositions[series];
        }

        public int GetRowSeriesCount()
        {
            if (areBarsIndexed) return rowsCount;
            IndexBars();
            return rowsCount;
        }

        public int GetStackedColumnPostion(IDataSeries<TDrawingContext> series)
        {
            if (areBarsIndexed) return stackColumnPositions[series.GetStackGroup()];
            IndexBars();
            return stackColumnPositions[series.GetStackGroup()];
        }

        public int GetStackedColumnSeriesCount()
        {
            if (areBarsIndexed) return stackedColumnsCount;
            IndexBars();
            return stackedColumnsCount;
        }

        public int GetStackedRowPostion(IDataSeries<TDrawingContext> series)
        {
            if (areBarsIndexed) return stackRowsPositions[series.GetStackGroup()];
            IndexBars();
            return stackRowsPositions[series.GetStackGroup()];
        }

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

        public StackPosition<TDrawingContext>? GetStackPosition(IDataSeries<TDrawingContext> series, int stackGroup)
        {
            if (!series.IsStackedSeries()) return null;

            var s = GetStacker(series, stackGroup);

            if (s == null) return null;

            return new StackPosition<TDrawingContext>
            {
                Stacker = s,
                Position = s.GetSeriesStackPosition(series)
            };
        }

        private Stacker<TDrawingContext> GetStacker(IDataSeries<TDrawingContext> series, int stackGroup)
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
