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

namespace LiveChartsCore.Context
{
    public interface IChartPoint: IOutOfContextChartPoint
    {
        ChartPointContext PointContext { get; set; }
    }

    public interface IOutOfContextChartPoint
    {
        /// <summary>
        /// Gets the primary value of the point.
        /// Normally the map goes as follows:
        /// For Horizontal Cartesian (<see cref="LineSeries{TModel, TVisual, TDrawingContext, TGeometryPath, TLineSegment, TBezierSegment, TMoveToCommand, TPathContext}"/>, <see cref="ColumnSeries{TModel, TVisual, TDrawingContext}"/>) series => Y coordinate.
        /// For Vertical Cartesian (VerticalLine, RowSeries) series => X coordinate.
        /// For <see cref="PieDataSeries{TModel, TVisual, TDrawingContext}"/> => the value of the slice.
        /// </summary>
        float PrimaryValue { get; }

        /// <summary>
        /// Gets the secondary value of the point.
        /// For Horizontal Cartesian (<see cref="LineSeries{TModel, TVisual, TDrawingContext, TGeometryPath, TLineSegment, TBezierSegment, TMoveToCommand, TPathContext}"/>, <see cref="ColumnSeries{TModel, TVisual, TDrawingContext}"/>) series => X coordinate.
        /// For Vertical Cartesian (VerticalLine, RowSeries) series => Y coordinate.
        /// For <see cref="PieDataSeries{TModel, TVisual, TDrawingContext}"/> => *ignored*.
        /// </summary>
        float SecondaryValue { get; }
    }
}
