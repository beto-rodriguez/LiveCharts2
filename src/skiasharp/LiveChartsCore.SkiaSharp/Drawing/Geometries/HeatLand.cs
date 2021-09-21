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
using LiveChartsCore.Geo;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    /// <summary>
    /// Defines a heat lane.
    /// </summary>
    public class HeatLand : MapShape<SkiaSharpDrawingContext>, IWeigthedMapShape
    {
        private IEnumerable<PathShape>? _paths;
        private double _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatLand"/> class.
        /// </summary>
        public HeatLand()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeatLand"/> class.
        /// </summary>
        /// <param name="name">The name/</param>
        /// <param name="value">The value.</param>
        public HeatLand(string name, double value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public int WeigthedAt { get; set; }

        /// <summary>
        /// Gets or sets the land name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value { get => _value; set { _value = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="Measure(MapShapeContext{SkiaSharpDrawingContext})"/>
        public override void Measure(MapShapeContext<SkiaSharpDrawingContext> context)
        {
            if (_paths is null)
            {
                var projector = Maps.BuildProjector(context.Chart.Projection, new[] { context.Chart.Width, context.Chart.Height });
                _paths = context.Chart.ActiveMap.FindFeature(Name).AsPathShape(projector);
                foreach (var path in _paths) context.DefaultPaint.AddGeometryToPaintTask(context.Chart.Canvas, path);
            }

            var color = HeatFunctions.InterpolateColor(
                (float)Value, context.BoundsDictionary[WeigthedAt], context.Chart.HeatMap, context.HeatStops);

            foreach (var path in _paths)
            {
                path.FillColor = color;
            }
        }
    }
}
