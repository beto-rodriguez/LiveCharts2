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
using LiveChartsCore.Kernel.Events;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines a geo map chart.
    /// </summary>
    /// <typeparam name="TDrawingContext"></typeparam>
    public class GeoMap<TDrawingContext>
        where TDrawingContext : DrawingContext
    {
        private readonly IMapFactory<TDrawingContext> _mapFactory;
        private readonly HashSet<IMapElement> _everMeasuredShapes = new();
        private readonly IGeoMapView<TDrawingContext> _chartView;
        private readonly ActionThrottler _updateThrottler;
        private readonly ActionThrottler _panningThrottler;
        private readonly IPaint<TDrawingContext> _heatPaint;
        private bool _isHeatInCanvas = false;
        private IPaint<TDrawingContext>? _previousStroke;
        private IPaint<TDrawingContext>? _previousFill;
        private int _heatKnownLength = 0;
        private List<Tuple<double, LvcColor>> _heatStops = new();
        private LvcPoint _pointerPosition = new(-10, -10);
        private LvcPoint _pointerPanningPosition = new(-10, -10);
        private LvcPoint _pointerPreviousPanningPosition = new(-10, -10);
        private bool _isPanning = false;
        private bool _isPointerIn = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap{TDrawingContext}"/> class.
        /// </summary>
        /// <param name="mapView"></param>
        public GeoMap(IGeoMapView<TDrawingContext> mapView)
        {
            _chartView = mapView;
            _updateThrottler = mapView.DesignerMode
                    ? new ActionThrottler(() => Task.CompletedTask, TimeSpan.FromMilliseconds(50))
                    : new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(50));
            _heatPaint = LiveCharts.CurrentSettings.GetProvider<TDrawingContext>().GetSolidColorPaint();
            _mapFactory = LiveCharts.CurrentSettings.GetProvider<TDrawingContext>().GetDefaultMapFactory();

            PointerDown += Chart_PointerDown;
            PointerMove += Chart_PointerMove;
            PointerUp += Chart_PointerUp;
            PointerLeft += Chart_PointerLeft;

            _panningThrottler = new ActionThrottler(PanningThrottlerUnlocked, TimeSpan.FromMilliseconds(30));
        }

        internal event Action<LvcPoint> PointerDown;

        internal event Action<LvcPoint> PointerMove;

        internal event Action<LvcPoint> PointerUp;

        internal event Action PointerLeft;

        internal event Action<PanGestureEventArgs>? PanGesture;

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
            foreach (var shape in _mapFactory.FetchMapElements(_chartView))
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

            foreach (var feature in _mapFactory.FetchFeatures(map, projector))
            {
                var pathShapes = _mapFactory.ConvertToPathShape(map, feature, projector);

                foreach (var shapeGeometry in pathShapes)
                {
                    if (_chartView.Stroke is not null)
                        _chartView.Stroke.AddGeometryToPaintTask(_chartView.Canvas, shapeGeometry);

                    if (_chartView.Fill is not null)
                        _chartView.Fill.AddGeometryToPaintTask(_chartView.Canvas, shapeGeometry);
                }
            }

            var toDeleteShapes = new HashSet<IMapElement>(_everMeasuredShapes);
            var context = new MapShapeContext<TDrawingContext>(_chartView, _heatPaint, _heatStops, bounds);

            foreach (var shape in _mapFactory.FetchMapElements(_chartView))
            {
                _ = _everMeasuredShapes.Add(shape);
                //shape.Measure(context);
                _ = toDeleteShapes.Remove(shape);
            }

            foreach (var shape in toDeleteShapes)
            {
                shape.RemoveFromUI(context);
                _ = _everMeasuredShapes.Remove(shape);
            }

            _chartView.Canvas.Invalidate();
        }

        private Task PanningThrottlerUnlocked()
        {
            return Task.Run(() =>
                _chartView.InvokeOnUIThread(() =>
                {
                    if (this is not CartesianChart<TDrawingContext> cartesianChart) return;

                    lock (_chartView.Canvas.Sync)
                    {
                        cartesianChart.Pan(
                        new LvcPoint(
                            (float)(_pointerPanningPosition.X - _pointerPreviousPanningPosition.X),
                            (float)(_pointerPanningPosition.Y - _pointerPreviousPanningPosition.Y)));
                        _pointerPreviousPanningPosition = new LvcPoint(_pointerPanningPosition.X, _pointerPanningPosition.Y);
                    }
                }));
        }

        private void Chart_PointerDown(LvcPoint pointerPosition)
        {
            _isPanning = true;
            _pointerPreviousPanningPosition = pointerPosition;
        }

        private void Chart_PointerMove(LvcPoint pointerPosition)
        {
            _pointerPosition = pointerPosition;
            _isPointerIn = true;
            if (!_isPanning) return;
            _pointerPanningPosition = pointerPosition;
            _panningThrottler.Call();
        }

        private void Chart_PointerLeft()
        {
            _isPointerIn = false;
        }

        private void Chart_PointerUp(LvcPoint pointerPosition)
        {
            if (!_isPanning) return;
            _isPanning = false;
        }
    }
}

