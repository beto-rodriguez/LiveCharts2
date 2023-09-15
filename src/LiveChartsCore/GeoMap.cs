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
using System.Threading.Tasks;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;

namespace LiveChartsCore;

/// <summary>
/// Defines a geo map chart.
/// </summary>
/// <typeparam name="TDrawingContext"></typeparam>
public class GeoMap<TDrawingContext>
    where TDrawingContext : DrawingContext
{
    private readonly HashSet<IGeoSeries<TDrawingContext>> _everMeasuredSeries = new();
    private readonly ActionThrottler _updateThrottler;
    private readonly ActionThrottler _panningThrottler;
    private bool _isHeatInCanvas = false;
    private IPaint<TDrawingContext> _heatPaint;
    private IPaint<TDrawingContext>? _previousStroke;
    private IPaint<TDrawingContext>? _previousFill;
    private LvcPoint _pointerPanningPosition = new(-10, -10);
    private LvcPoint _pointerPreviousPanningPosition = new(-10, -10);
    private bool _isPanning = false;
    private IMapFactory<TDrawingContext> _mapFactory;
    private CoreMap<TDrawingContext>? _activeMap;
    private bool _isUnloaded = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoMap{TDrawingContext}"/> class.
    /// </summary>
    /// <param name="mapView"></param>
    public GeoMap(IGeoMapView<TDrawingContext> mapView)
    {
        View = mapView;
        _updateThrottler = mapView.DesignerMode
                ? new ActionThrottler(() => Task.CompletedTask, TimeSpan.FromMilliseconds(50))
                : new ActionThrottler(UpdateThrottlerUnlocked, TimeSpan.FromMilliseconds(100));
        _heatPaint = LiveCharts.DefaultSettings.GetProvider<TDrawingContext>().GetSolidColorPaint();
        _mapFactory = LiveCharts.DefaultSettings.GetProvider<TDrawingContext>().GetDefaultMapFactory();

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

    /// <summary>
    /// Gets the chart view.
    /// </summary>
    public IGeoMapView<TDrawingContext> View { get; private set; }

    /// <inheritdoc cref="IMapFactory{TDrawingContext}.ViewTo(GeoMap{TDrawingContext}, object)"/>
    public virtual void ViewTo(object? command)
    {
        _mapFactory.ViewTo(this, command);
    }

    /// <inheritdoc cref="IMapFactory{TDrawingContext}.Pan(GeoMap{TDrawingContext}, LvcPoint)"/>
    public virtual void Pan(LvcPoint delta)
    {
        _mapFactory.Pan(this, delta);
    }

    /// <summary>
    /// Queues a measure request to update the chart.
    /// </summary>
    /// <param name="chartUpdateParams"></param>
    public virtual void Update(ChartUpdateParams? chartUpdateParams = null)
    {
        chartUpdateParams ??= new ChartUpdateParams();

        if (chartUpdateParams.IsAutomaticUpdate && !View.AutoUpdateEnabled) return;

        if (!chartUpdateParams.Throttling)
        {
            _updateThrottler.ForceCall();
            return;
        }

        _updateThrottler.Call();
    }

    /// <summary>
    /// Unload the map resources.
    /// </summary>
    public void Unload()
    {
        if (View.Stroke is not null) View.Canvas.RemovePaintTask(View.Stroke);
        if (View.Fill is not null) View.Canvas.RemovePaintTask(View.Fill);

        _everMeasuredSeries.Clear();
        _heatPaint = null!;
        _previousStroke = null!;
        _previousFill = null!;
        _isUnloaded = true;
        _mapFactory.Dispose();
        _activeMap?.Dispose();

        _activeMap = null!;
        _mapFactory = null!;

        View.Canvas.Dispose();
    }

    internal void InvokePointerDown(LvcPoint point)
    {
        PointerDown?.Invoke(point);
    }

    internal void InvokePointerMove(LvcPoint point)
    {
        PointerMove?.Invoke(point);
    }

    internal void InvokePointerUp(LvcPoint point)
    {
        PointerUp?.Invoke(point);
    }

    internal void InvokePointerLeft()
    {
        PointerLeft?.Invoke();
    }

    /// <summary>
    /// Called to measure the chart.
    /// </summary>
    /// <returns>The update task.</returns>
    protected virtual Task UpdateThrottlerUnlocked()
    {
        return Task.Run(() =>
        {
            View.InvokeOnUIThread(() =>
            {
                lock (View.Canvas.Sync)
                {
                    if (_isUnloaded) return;
                    Measure();
                }
            });
        });
    }

    /// <summary>
    /// Measures the chart.
    /// </summary>
    internal void Measure()
    {
        if (_activeMap is not null && _activeMap != View.ActiveMap)
        {
            _previousStroke?.ClearGeometriesFromPaintTask(View.Canvas);
            _previousFill?.ClearGeometriesFromPaintTask(View.Canvas);

            _previousFill = null;
            _previousStroke = null;

            View.Canvas.Clear();
        }
        _activeMap = View.ActiveMap;

        if (!_isHeatInCanvas)
        {
            View.Canvas.AddDrawableTask(_heatPaint);
            _isHeatInCanvas = true;
        }

        if (_previousStroke != View.Stroke)
        {
            if (_previousStroke is not null)
                View.Canvas.RemovePaintTask(_previousStroke);

            if (View.Stroke is not null)
            {
                if (View.Stroke.ZIndex == 0) View.Stroke.ZIndex = 2;
                View.Stroke.IsStroke = true; // ToDo: why both properties? IsStroke? IsFill?
                View.Stroke.IsFill = false;
                View.Canvas.AddDrawableTask(View.Stroke);
            }

            _previousStroke = View.Stroke;
        }

        if (_previousFill != View.Fill)
        {
            if (_previousFill is not null)
                View.Canvas.RemovePaintTask(_previousFill);

            if (View.Fill is not null)
            {
                View.Fill.IsStroke = false; // ToDo: why both properties? IsStroke? IsFill?
                View.Fill.IsFill = true;
                View.Canvas.AddDrawableTask(View.Fill);
            }

            _previousFill = View.Fill;
        }

        var i = _previousFill?.ZIndex ?? 0;
        _heatPaint.ZIndex = i + 1;

        var context = new MapContext<TDrawingContext>(
            this, View, View.ActiveMap,
            Maps.BuildProjector(View.MapProjection, new[] { View.Width, View.Height }));

        _mapFactory.GenerateLands(context);

        var toDeleteSeries = new HashSet<IGeoSeries<TDrawingContext>>(_everMeasuredSeries);
        foreach (var series in View.Series.Cast<IGeoSeries<TDrawingContext>>())
        {
            series.Measure(context);
            _ = _everMeasuredSeries.Add(series);
            _ = toDeleteSeries.Remove(series);
        }

        foreach (var series in toDeleteSeries)
        {
            series.Delete(context);
            _ = _everMeasuredSeries.Remove(series);
        }

        View.Canvas.Invalidate();
    }

    private Task PanningThrottlerUnlocked()
    {
        return Task.Run(() =>
            View.InvokeOnUIThread(() =>
            {
                lock (View.Canvas.Sync)
                {
                    Pan(
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
        if (!_isPanning) return;
        _pointerPanningPosition = pointerPosition;
        _panningThrottler.Call();
    }

    private void Chart_PointerLeft()
    {
        // ...?
    }

    private void Chart_PointerUp(LvcPoint pointerPosition)
    {
        if (!_isPanning) return;
        _isPanning = false;
        _panningThrottler.Call();
    }
}
