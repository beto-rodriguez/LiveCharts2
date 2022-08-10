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

using System.Collections.Generic;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.Kernel.Providers;

/// <summary>
/// Just a cleaner <see cref="DataFactory{TModel, TDrawingContext}"/> but optimized for <see cref="IChartEntity"/> objects.
/// </summary>
public class EntitiesDataFactory<TModel, TDrawingContext> : DataFactory<TModel, TDrawingContext>
    where TDrawingContext : DrawingContext
{
    /// <inheritdoc cref="DataFactory{TModel, TDrawingContext}.Fetch(ISeries{TModel}, IChart)"/>
    public override IEnumerable<ChartPoint> Fetch(ISeries<TModel> series, IChart chart)
    {
        if (series.Values is null) yield break;
        var index = 0;

        foreach (var value in series.Values)
        {
            if (value is not IChartEntity entity) continue;

            entity.ChartPoint ??= new ChartPoint(chart.View, series);
            entity.ChartPoint.Context.DataSource = entity;
            entity.ChartPoint.Context.Index = index;
            entity.EntityId = index;
            entity.ChartPoint.Coordinate = entity.Coordinate;

            yield return entity.ChartPoint;

            index++;
        }
    }
}
