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
using System.Collections.Generic;
using LiveChartsCore.Measure;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using System.Threading.Tasks;
using System;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a geo map chart.
    /// </summary>
    /// <typeparam name="TDrawingContext"></typeparam>
    /// <typeparam name="TPathGeometry"></typeparam>
    /// <typeparam name="TLineSegment"></typeparam>
    /// <typeparam name="TMoveToCommand"></typeparam>
    /// <typeparam name="TPathArgs"></typeparam>
    public class GeoMap<TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs>
        where TPathGeometry : IPathGeometry<TDrawingContext, TPathArgs>, new()
        where TLineSegment : ILinePathSegment<TPathArgs>, new()
        where TMoveToCommand : IMoveToPathCommand<TPathArgs>, new()
        where TDrawingContext : DrawingContext
    {
        private readonly IGeoMapView<TDrawingContext> _chartView;
        private readonly ActionThrottler _updateThrottler;
        private readonly IPaint<TDrawingContext> _heatPaint;
        private bool _isHeatInCanvas = false;
        private IPaint<TDrawingContext>? _previousStroke;
        private IPaint<TDrawingContext>? _previousFill;
        private int _heatKnownLength = 0;
        private List<Tuple<double, LvcColor>> _heatStops = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap{TDrawingContext, TPathGeometry, TLineSegment, TMoveToCommand, TPathArgs}"/> class.
        /// </summary>
        /// <param name="mapView"></param>
        public GeoMap(IGeoMapView<TDrawingContext> mapView)
        {
            _chartView = mapView;
            _updateThrottler = mapView.DesignerMode
                    ? new ActionThrottler(() => Task.CompletedTask, TimeSpan.FromMilliseconds(50))
                    : new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(50));
            _heatPaint = LiveCharts.CurrentSettings.GetProvider<TDrawingContext>().GetSolidColorPaint();
        }

        /// <summary>
        /// Queues a measure request to update the chart.
        /// </summary>
        /// <param name="chartUpdateParams"></param>
        public virtual void Update(ChartUpdateParams? chartUpdateParams = null)
        {
            chartUpdateParams ??= new ChartUpdateParams();

            if (chartUpdateParams.IsAutomaticUpdate && !_chartView.AutoUpdateEnabled) return;

            if (!chartUpdateParams.Throttling)
            {
                _updateThrottler.ForceCall();
                return;
            }

            _updateThrottler.Call();
        }

        /// <summary>
        /// Called to measure the chart.
        /// </summary>
        /// <returns>The update task.</returns>
        protected virtual Task UpdateThrottlerUnlocked()
        {
            return Task.Run(() =>
            {
                _chartView.InvokeOnUIThread(() =>
                {
                    lock (_chartView.Canvas.Sync)
                    {
                        Measure();
                    }
                });
            });
        }

        /// <summary>
        /// Measures the chart.
        /// </summary>
        private void Measure()
        {
            if (!_isHeatInCanvas)
            {
                _chartView.Canvas.AddDrawableTask(_heatPaint);
                _isHeatInCanvas = true;
            }

            if (_previousFill != _chartView.Fill)
            {
                if (_previousFill is not null)
                    _chartView.Canvas.RemovePaintTask(_previousFill);

                if (_chartView.Fill is not null)
                    _chartView.Canvas.AddDrawableTask(_chartView.Fill);

                _previousFill = _chartView.Fill;
            }

            if (_previousStroke != _chartView.Stroke)
            {
                if (_previousStroke is not null)
                    _chartView.Canvas.RemovePaintTask(_previousStroke);

                if (_chartView.Stroke is not null)
                    _chartView.Canvas.AddDrawableTask(_chartView.Stroke);

                _previousStroke = _chartView.Stroke;
            }

            var i = Math.Max(_previousStroke?.ZIndex ?? 0, _previousFill?.ZIndex ?? 0);
            _heatPaint.ZIndex = i + 1;

            var bounds = new Dictionary<int, Bounds>();
            foreach (var shape in _chartView.Shapes)
            {
                if (shape is not IWeigthedMapShape wShape) continue;

                if (!bounds.TryGetValue(wShape.WeigthedAt, out var weightBounds))
                {
                    weightBounds = new Bounds();
                    bounds.Add(wShape.WeigthedAt, weightBounds);
                }

                weightBounds.AppendValue(wShape.Value);
            }

            var hm = _chartView.HeatMap;

            if (_heatKnownLength != _chartView.HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(hm, _chartView.ColorStops);
                _heatKnownLength = _chartView.HeatMap.Length;
            }

            var map = _chartView.ActiveMap;
            var projector = Maps.BuildProjector(_chartView.MapProjection, new[] { _chartView.Width, _chartView.Height });

            if (_chartView.Stroke is not null)
                _chartView.Stroke.ClearGeometriesFromPaintTask(_chartView.Canvas);

            if (_chartView.Fill is not null)
                _chartView.Fill.ClearGeometriesFromPaintTask(_chartView.Canvas);

            foreach (var feature in map.Features ?? new GeoJsonFeature[0])
            {
                var pathShapes = AsPathShape(feature, projector);

                foreach (var shapeGeometry in pathShapes)
                {
                    if (_chartView.Stroke is not null)
                        _chartView.Stroke.AddGeometryToPaintTask(_chartView.Canvas, shapeGeometry);

                    if (_chartView.Fill is not null)
                        _chartView.Fill.AddGeometryToPaintTask(_chartView.Canvas, shapeGeometry);
                }
            }

            var context = new MapShapeContext<TDrawingContext>(_chartView, _heatPaint, _heatStops, bounds);

            _heatPaint.ClearGeometriesFromPaintTask(_chartView.Canvas);
            foreach (var shape in _chartView.Shapes)
            {
                shape.Measure(context);
            }

            _chartView.Canvas.Invalidate();
        }

        private static IEnumerable<TPathGeometry> AsPathShape(
            GeoJsonFeature feature,
            MapProjector projector)
        {
            var paths = new List<TPathGeometry>();
            var d = new double[0][][][];

            foreach (var geometry in feature.Geometry?.Coordinates ?? d)
            {
                foreach (var segment in geometry)
                {
                    var path = new TPathGeometry { IsClosed = true };

                    var isFirst = true;
                    foreach (var point in segment)
                    {
                        var p = projector.ToMap(point);

                        if (isFirst)
                        {
                            isFirst = false;
                            path.AddCommand(new TMoveToCommand { X = p[0], Y = p[1] });
                            continue;
                        }

                        path.AddCommand(new TLineSegment { X = p[0], Y = p[1] });
                    }

                    paths.Add(path);
                }
            }

            return paths;
        }
    }
}

