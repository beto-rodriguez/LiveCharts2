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
        private readonly IChartView<TDrawingContext> chartView;
        private readonly IEnumerable<ISeries<TDrawingContext>> series;

        private int columnsCount = 0;
        private Dictionary<ISeries<TDrawingContext>, int> columnPositions;

        private Stacker<TDrawingContext> columnsStacker = new Stacker<TDrawingContext>(AxisOrientation.Y);

        public SeriesContext(IChartView<TDrawingContext> chartView, IEnumerable<ISeries<TDrawingContext>> series)
        {
            this.series = series;
            this.chartView = chartView;
        }

        #region columns and rows

        public int GetColumnPostion(ISeries<TDrawingContext> series)
        {
            if (columnPositions != null) return columnPositions[series];
            IndexColumns();
            return columnPositions[series];
        }

        public int GetColumnSeriesCount()
        {
            if (columnPositions != null) return columnsCount;
            IndexColumns();
            return columnsCount;
        }

        private void IndexColumns()
        {
            columnPositions = new Dictionary<ISeries<TDrawingContext>, int>();
            columnsCount = 0;
            foreach (var item in this.series)
            {
                if (item.SeriesType != SeriesType.Column) continue;
                columnPositions[item] = columnsCount++;
            }
        }

        #endregion

        #region stacked

        public StackPosition<TDrawingContext> GetStackPosition(ISeries<TDrawingContext> series)
        {
            var s = GetStacker(series);

            if (s == null) return null;

            return new StackPosition<TDrawingContext>
            {
                Stacker = s,
                Position = s.GetSeriesStackPosition(series)
            };
        }

        private Stacker<TDrawingContext> GetStacker(ISeries<TDrawingContext> series)
        {
            Stacker<TDrawingContext> stacker = null;

            switch (series.SeriesType)
            {
                case SeriesType.Column:
                    break;
                case SeriesType.StackedColumn:
                    stacker = columnsStacker;
                    break;
                case SeriesType.Row:
                    break;
                case SeriesType.StackedRow:
                    break;
                case SeriesType.HorizontalLine:
                    break;
                case SeriesType.VerticalLine:
                    break;
                default:
                    return null;
            }

            return stacker;
        }

        #endregion
    }
}
