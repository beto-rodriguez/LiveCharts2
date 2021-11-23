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

using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Measure;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines the heat land series class.
    /// </summary>
    /// <typeparam name="TDrawingContext"></typeparam>
    public class HeatLandSeries<TDrawingContext> : IGeoSeries<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly IPaint<TDrawingContext> _heatPaint;
        private bool _isHeatInCanvas = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatLandSeries{TDrawingContext}"/> class.
        /// </summary>
        public HeatLandSeries()
        {
            _heatPaint = LiveCharts.CurrentSettings.GetProvider<TDrawingContext>().GetSolidColorPaint();
        }

        /// <summary>
        /// Gets or sets the heat map.
        /// </summary>
        public LvcColor[] HeatMap { get; set; } = new LvcColor[0];

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public double[]? ColorStops { get; set; }

        /// <summary>
        /// Gets or sets the lands.
        /// </summary>
        public IWeigthedMapShape[]? Lands { get; set; }

        /// <inheritdoc cref="IGeoSeries{TDrawingContext}.IsVisible"/>
        public bool IsVisible { get; set; }

        /// <inheritdoc cref="IGeoSeries{TDrawingContext}.Measure(MapContext{TDrawingContext})"/>
        public void Measure(MapContext<TDrawingContext> context)
        {
            if (!_isHeatInCanvas)
            {
                context.View.Canvas.AddDrawableTask(_heatPaint);
                _isHeatInCanvas = true;
            }

            var i = context.View.Fill?.ZIndex ?? 0;
            _heatPaint.ZIndex = i + 1;

            var bounds = new Bounds();
            foreach (var shape in Lands ?? Enumerable.Empty<IWeigthedMapShape>())
            {
                bounds.AppendValue(shape.Value);
            }

            var heatStops = HeatFunctions.BuildColorStops(HeatMap, ColorStops);
            var shapeContext = new MapShapeContext<TDrawingContext>(context.View, _heatPaint, heatStops, bounds);

            foreach (var land in Lands ?? Enumerable.Empty<IWeigthedMapShape>())
            {
                var projector = Maps.BuildProjector(
                    context.View.MapProjection, new[] { context.View.Width, context.View.Height });

                var heat = HeatFunctions.InterpolateColor((float)land.Value, bounds, HeatMap, heatStops);

                var mapLand = context.View.ActiveMap.FindLand(land.Name);
                if (mapLand is null) return;

                var shapesQuery = mapLand.Data
                    .Select(x => x.Shape)
                    .Where(x => x is not null)
                    .Cast<IHeatLandShape>();

                foreach (var pathShape in shapesQuery)
                {
                    pathShape.FillColor = heat;
                }
            }
        }

        /// <inheritdoc cref="IGeoSeries{TDrawingContext}.DeleteMapElement(MapContext{TDrawingContext}, IMapElement)"/>
        public void DeleteMapElement(MapContext<TDrawingContext> context, IMapElement mapElement)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IGeoSeries{TDrawingContext}.Delete(MapContext{TDrawingContext})"/>
        public void Delete(MapContext<TDrawingContext> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
