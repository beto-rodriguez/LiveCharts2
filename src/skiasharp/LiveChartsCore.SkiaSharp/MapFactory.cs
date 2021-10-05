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
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView
{
    /// <summary>
    /// Defines a map builder.
    /// </summary>
    public class MapFactory : IMapFactory<SkiaSharpDrawingContext>
    {
        /// <inheritdoc cref="IMapFactory{TDrawingContext}.FetchMapElements(MapContext{TDrawingContext})"/>
        public IEnumerable<IMapElement> FetchMapElements(MapContext<SkiaSharpDrawingContext> context)
        {
            foreach (var shape in context.View.Shapes)
                yield return shape;
        }

        /// <inheritdoc cref="IMapFactory{TDrawingContext}.UpdateLands(MapContext{TDrawingContext})"/>
        public void UpdateLands(MapContext<SkiaSharpDrawingContext> context)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IMapFactory{TDrawingContext}.ViewTo(GeoMap{TDrawingContext}, object)"/>
        public void ViewTo(GeoMap<SkiaSharpDrawingContext> sender, object command)
        {
            // not implemented yet.
        }

        /// <inheritdoc cref="IMapFactory{TDrawingContext}.Pan(GeoMap{TDrawingContext}, LvcPoint)"/>
        public void Pan(GeoMap<SkiaSharpDrawingContext> sender, LvcPoint delta)
        {
            // not implemented yet.
        }
    }
}
