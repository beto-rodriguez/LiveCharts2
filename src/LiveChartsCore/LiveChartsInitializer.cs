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

using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using System.Drawing;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines an object that must set the default Fil, Stroke and series name.
    /// </summary>
    public abstract class LiveChartsInitializer<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        public abstract void ConstructChart(IChartView<TDrawingContext> chart);

        public abstract void ConstructSeries(IDrawableSeries<TDrawingContext> series);

        public abstract void ConstructAxis(IAxis<TDrawingContext> axis);

        public abstract void ResolveSeriesDefaults(Color[] colors, IDrawableSeries<TDrawingContext> series);

        public abstract void ResolveAxisDefaults(IAxis<TDrawingContext> axis);
    }
}
