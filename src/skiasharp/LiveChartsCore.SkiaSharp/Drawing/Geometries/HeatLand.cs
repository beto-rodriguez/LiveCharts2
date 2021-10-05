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

using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;

namespace LiveChartsCore.SkiaSharpView.Drawing.Geometries
{
    /// <summary>
    /// Defines a heat lane.
    /// </summary>
    public class HeatLand : MapShape<SkiaSharpDrawingContext>, IWeigthedMapShape
    {
        private double _value;
        private Tuple<HeatPathShape, IEnumerable<PathCommand>>[]? _paths;
        private int _weigthedAt;

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
        /// Gets or sets the land name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc cref="IWeigthedMapShape.WeigthedAt"/>
        public int WeigthedAt { get => _weigthedAt; set { _weigthedAt = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="IWeigthedMapShape.Value"/>
        public double Value { get => _value; set { _value = value; OnPropertyChanged(); } }

        /// <inheritdoc cref="Measure(MapShapeContext{SkiaSharpDrawingContext})"/>
        public override void Measure(MapShapeContext<SkiaSharpDrawingContext> context)
        {
            //var projector = Maps.BuildProjector(context.Chart.MapProjection, new[] { context.Chart.Width, context.Chart.Height });

            //var heat = HeatFunctions.InterpolateColor(
            //    (float)Value, context.BoundsDictionary[WeigthedAt], context.Chart.HeatMap, context.HeatStops);

            //if (_paths is null)
            //{
            //    _paths = GetPathCommands(context.Chart.ActiveMap.FindFeature(Name), projector).ToArray();

            //    foreach (var path in _paths)
            //    {
            //        path.Item1.FillColor = new LvcColor(heat.R, heat.G, heat.B, 0);

            //        _ = path.Item1
            //            .TransitionateProperties(nameof(HeatPathShape.FillColor))
            //            .WithAnimation(animation =>
            //                animation
            //                    .WithDuration(TimeSpan.FromMilliseconds(800))
            //                    .WithEasingFunction(EasingFunctions.Lineal))
            //            .CompleteCurrentTransitions();

            //        context.HeatPaint.AddGeometryToPaintTask(context.Chart.Canvas, path.Item1);
            //    }
            //}
            //else
            //{
            //    _paths = GetPathCommands(context.Chart.ActiveMap.FindFeature(Name), projector).ToArray();
            //}

            //foreach (var path in _paths)
            //{
            //    path.Item1.ClearCommands();

            //    foreach (var command in path.Item2)
            //    {
            //        path.Item1.AddLast(command);
            //    }

            //    path.Item1.FillColor = heat;
            //}
        }

        /// <inheritdoc cref="RemoveFromUI(MapShapeContext{SkiaSharpDrawingContext})"/>
        public override void RemoveFromUI(MapShapeContext<SkiaSharpDrawingContext> context)
        {
            if (_paths is null) return;

            foreach (var path in _paths)
                context.HeatPaint.RemoveGeometryFromPainTask(context.Chart.Canvas, path.Item1);

            _paths = null;
        }

        private IEnumerable<Tuple<HeatPathShape, IEnumerable<PathCommand>>> GetPathCommands(
            GeoJsonFeature feature,
            MapProjector projector)
        {
            var d = new double[0][][][];

            var i = 0;
            foreach (var geometry in feature.Geometry?.Coordinates ?? d)
            {
                foreach (var segment in geometry)
                {
                    var path = _paths is not null && i <= _paths.Length - 1
                        ? _paths[i].Item1
                        : new HeatPathShape { IsClosed = true };

                    yield return new Tuple<HeatPathShape, IEnumerable<PathCommand>>(
                        path, EnumerateCommands(segment, projector));

                    i++;
                }
            }
        }

        private IEnumerable<PathCommand> EnumerateCommands(double[][] segment, MapProjector projector)
        {
            var isFirst = true;
            foreach (var point in segment)
            {
                var p = projector.ToMap(point);

                if (isFirst)
                {
                    isFirst = false;
                    yield return new MoveToPathCommand { X = p[0], Y = p[1] };
                    continue;
                }

                yield return new LineSegment { X = p[0], Y = p[1] };
            }
        }
    }
}
