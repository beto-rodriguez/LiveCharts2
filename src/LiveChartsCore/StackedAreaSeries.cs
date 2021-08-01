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

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the stacked area series class.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TVisual">The type of the visual.</typeparam>
    /// <typeparam name="TLabel">The type of the label.</typeparam>
    /// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
    /// <typeparam name="TPathGeometry">The type of the path geometry.</typeparam>
    /// <typeparam name="TLineSegment">The type of the line segment.</typeparam>
    /// <typeparam name="TBezierSegment">The type of the bezier segment.</typeparam>
    /// <typeparam name="TMoveToCommand">The type of the move to command.</typeparam>
    /// <typeparam name="TPathArgs">The type of the path arguments.</typeparam>
    /// <seealso cref="LineSeries{TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs}" />
    public class StackedAreaSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs>
        : LineSeries<TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs>
        where TPathGeometry : IPathGeometry<TDrawingContext, TPathArgs>, new()
        where TLineSegment : ILinePathSegment<TPathArgs>, new()
        where TBezierSegment : IBezierSegment<TPathArgs>, new()
        where TMoveToCommand : IMoveToPathCommand<TPathArgs>, new()
        where TVisual : class, ISizedVisualChartPoint<TDrawingContext>, new()
        where TLabel : class, ILabelGeometry<TDrawingContext>, new()
        where TDrawingContext : DrawingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StackedAreaSeries{TModel, TVisual, TLabel, TDrawingContext, TPathGeometry, TLineSegment, TBezierSegment, TMoveToCommand, TPathArgs}"/> class.
        /// </summary>
        public StackedAreaSeries()
            : base(true)
        {
            GeometryFill = null;
            GeometryStroke = null;
            GeometrySize = 0;
        }
    }
}
