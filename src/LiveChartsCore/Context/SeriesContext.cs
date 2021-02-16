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
        private readonly IEnumerable<ISeries<TDrawingContext>> series;

        private int columnsCount = 0;
        private int rowsCount = 0;
        private int stackedColumnsCount = 0;
        private int stackedRowsCount = 0;
        private bool isCounted = false;
        private Dictionary<ISeries<TDrawingContext>, int> columnPositions = new Dictionary<ISeries<TDrawingContext>, int>();
        private Dictionary<ISeries<TDrawingContext>, int> rowPositions = new Dictionary<ISeries<TDrawingContext>, int>();
        private Dictionary<int, int> stackColumnPositions = new Dictionary<int, int>();
        private Dictionary<int, int> stackRowsPositions = new Dictionary<int, int>();

        private Dictionary<string, Stacker<TDrawingContext>> stackers = new Dictionary<string, Stacker<TDrawingContext>>();

        public SeriesContext(IEnumerable<ISeries<TDrawingContext>> series)
        {
            this.series = series;
        }

        #region columns and rows

        public int GetColumnPostion(ISeries<TDrawingContext> series)
        {
            if (isCounted) return columnPositions[series];
            IndexRowsColumns();
            return columnPositions[series];
        }

        public int GetColumnSeriesCount()
        {
            if (isCounted) return columnsCount;
            IndexRowsColumns();
            return columnsCount;
        }

        public int GetRowPostion(ISeries<TDrawingContext> series)
        {
            if (isCounted) return rowPositions[series];
            IndexRowsColumns();
            return rowPositions[series];
        }

        public int GetRowSeriesCount()
        {
            if (isCounted) return rowsCount;
            IndexRowsColumns();
            return rowsCount;
        }

        public int GetStackedColumnPostion(ISeries<TDrawingContext> series)
        {
            if (isCounted) return stackColumnPositions[series.GetStackGroup()];
            IndexRowsColumns();
            return stackColumnPositions[series.GetStackGroup()];
        }

        public int GetStackedColumnSeriesCount()
        {
            if (isCounted) return stackedColumnsCount;
            IndexRowsColumns();
            return stackedColumnsCount;
        }

        public int GetStackedRowPostion(ISeries<TDrawingContext> series)
        {
            if (isCounted) return stackRowsPositions[series.GetStackGroup()];
            IndexRowsColumns();
            return stackRowsPositions[series.GetStackGroup()];
        }

        public int GetStackedRowSeriesCount()
        {
            if (isCounted) return stackedRowsCount;
            IndexRowsColumns();
            return stackedRowsCount;
        }

        private void IndexRowsColumns()
        {
            columnsCount = 0;
            rowsCount = 0;
            stackedColumnsCount = 0;
            stackedRowsCount = 0;

            foreach (var item in series)
            {
                if (!item.IsColumnOrRow) continue;
                if (item.SeriesType == SeriesType.Column) columnPositions[item] = columnsCount++;
                if (item.SeriesType == SeriesType.Row) rowPositions[item] = rowsCount++;
                if (item.SeriesType == SeriesType.StackedColumn && !stackColumnPositions.ContainsKey(item.GetStackGroup()))
                    stackColumnPositions[item.GetStackGroup()] = stackedColumnsCount++;
                if (item.SeriesType == SeriesType.StackedRow && !stackRowsPositions.ContainsKey(item.GetStackGroup()))
                    stackRowsPositions[item.GetStackGroup()] = stackedRowsCount++;
            }

            isCounted = true;
        }

        #endregion

        #region stacked

        public StackPosition<TDrawingContext> GetStackPosition(ISeries<TDrawingContext> series, int stackGroup)
        {
            var s = GetStacker(series, stackGroup);

            if (s == null) return null;

            return new StackPosition<TDrawingContext>
            {
                Stacker = s,
                Position = s.GetSeriesStackPosition(series)
            };
        }

        private Stacker<TDrawingContext> GetStacker(ISeries<TDrawingContext> series, int stackGroup)
        {
            var key = $"{series.SeriesType}.{series.Direction}.{stackGroup}";

            if (!stackers.TryGetValue(key, out var stacker))
            {
                stacker = new Stacker<TDrawingContext>(series.Direction);
                stackers.Add(key, stacker);
            }

            return stacker;
        }

        #endregion
    }
}
