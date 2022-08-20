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
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel.Providers;

/// <summary>
/// Defines the <see cref="ChartEngine{TDrawingContext}"/> class.
/// </summary>
/// <typeparam name="TDrawingContext">The type of the drawing context.</typeparam>
public abstract class ChartEngine<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <summary>
    /// Gets a new instance of the default data factory.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <returns></returns>
    public virtual DataFactory<TModel, TDrawingContext> GetDefaultDataFactory<TModel>()
    {
        return new DataFactory<TModel, TDrawingContext>();
    }

    /// <summary>
    /// Gets a new instance of the default map factory.
    /// </summary>
    /// <returns></returns>
    public abstract IMapFactory<TDrawingContext> GetDefaultMapFactory();

    /// <summary>
    /// Gets a new instance of the default Cartesian axis.
    /// </summary>
    /// <returns></returns>
    public abstract ICartesianAxis GetDefaultCartesianAxis();

    /// <summary>
    /// Gets a new instance of the default polar axis.
    /// </summary>
    /// <returns></returns>
    public abstract IPolarAxis GetDefaultPolarAxis();

    /// <summary>
    /// Gets a new paint of the given color.
    /// </summary>
    /// <returns></returns>
    public abstract IPaint<TDrawingContext> GetSolidColorPaint(LvcColor color = new());
}
