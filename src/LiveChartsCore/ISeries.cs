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

using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System.Collections.Generic;

namespace LiveChartsCore
{
    public interface ISeries
    {
        SeriesProperties SeriesProperties { get; }
        string Name { get; set; }
        int ScalesXAt { get; set; }
        int ScalesYAt { get; set; }
    }

    public interface ISeries<TDrawingContext>: ISeries
        where TDrawingContext: DrawingContext
    {
        IDrawableTask<TDrawingContext> Stroke { get; }
        IDrawableTask<TDrawingContext> Fill { get; }
        IDrawableTask<TDrawingContext> HighlightStroke { get; }
        IDrawableTask<TDrawingContext> HighlightFill { get; }

        PaintContext<TDrawingContext> DefaultPaintContext { get; }

        IEnumerable<ICartesianCoordinate> Fetch(CartesianChartCore<TDrawingContext> chart);

        CartesianBounds GetBounds(CartesianChartCore<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y);

        void Measure(CartesianChartCore<TDrawingContext> chart, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y);

        int GetStackGroup();
    }

    public delegate void TransitionsSetterDelegate<T>(T visual, Animation chartAnimation);
}
