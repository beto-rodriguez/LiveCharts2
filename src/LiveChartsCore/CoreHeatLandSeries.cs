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
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;

namespace LiveChartsCore;

/// <summary>
/// Defines the heat land series class.
/// </summary>
/// <typeparam name="TDrawingContext"></typeparam>
public class CoreHeatLandSeries<TDrawingContext> : IGeoSeries<TDrawingContext>, INotifyPropertyChanged
    where TDrawingContext : DrawingContext
{
    private IPaint<TDrawingContext>? _heatPaint;
    private bool _isHeatInCanvas = false;
    private LvcColor[] _heatMap = Array.Empty<LvcColor>();
    private double[]? _colorStops;
    private IEnumerable<IWeigthedMapLand>? _lands;
    private bool _isVisible;
    private readonly HashSet<GeoMap<TDrawingContext>> _subscribedTo = new();
    private readonly CollectionDeepObserver<IWeigthedMapLand> _observer;
    private readonly HashSet<LandDefinition> _everUsed = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreHeatLandSeries{TDrawingContext}"/> class.
    /// </summary>
    public CoreHeatLandSeries()
    {
        _observer = new CollectionDeepObserver<IWeigthedMapLand>(
            (sender, e) => NotifySubscribers(),
            (sender, e) => NotifySubscribers());
    }

    /// <summary>
    /// Called when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the heat map.
    /// </summary>
    public LvcColor[] HeatMap { get => _heatMap; set { _heatMap = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the color stops.
    /// </summary>
    public double[]? ColorStops { get => _colorStops; set { _colorStops = value; OnPropertyChanged(); } }

    /// <summary>
    /// Gets or sets the lands.
    /// </summary>
    public IEnumerable<IWeigthedMapLand>? Lands
    {
        get => _lands;
        set
        {
            _observer?.Dispose(_lands);
            _observer?.Initialize(value);
            _lands = value;
            OnPropertyChanged();
        }
    }

    /// <inheritdoc cref="IGeoSeries.IsVisible"/>
    public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IGeoSeries{TDrawingContext}.Measure(MapContext{TDrawingContext})"/>
    public void Measure(MapContext<TDrawingContext> context)
    {
        _ = _subscribedTo.Add(context.CoreMap);

        if (_heatPaint is null) throw new Exception("Default paint not found");

        if (!_isHeatInCanvas)
        {
            context.View.Canvas.AddDrawableTask(_heatPaint);
            _isHeatInCanvas = true;
        }

        var i = context.View.Fill?.ZIndex ?? 0;
        _heatPaint.ZIndex = i + 1;

        var bounds = new Bounds();
        foreach (var shape in Lands ?? Enumerable.Empty<IWeigthedMapLand>())
        {
            bounds.AppendValue(shape.Value);
        }

        var heatStops = HeatFunctions.BuildColorStops(HeatMap, ColorStops);
        var shapeContext = new MapShapeContext<TDrawingContext>(context.View, _heatPaint, heatStops, bounds);
        var toRemove = new HashSet<LandDefinition>(_everUsed);

        foreach (var land in Lands ?? Enumerable.Empty<IWeigthedMapLand>())
        {
            var projector = Maps.BuildProjector(
                context.View.MapProjection, new[] { context.View.Width, context.View.Height });

            var heat = HeatFunctions.InterpolateColor((float)land.Value, bounds, HeatMap, heatStops);

            var mapLand = context.View.ActiveMap.FindLand(land.Name);
            if (mapLand is null) return;

            var shapesQuery = mapLand.Data
                .Select(x => x.Shape)
                .Where(x => x is not null)
                .Cast<IHeatPathShape>();

            foreach (var pathShape in shapesQuery)
            {
                pathShape.FillColor = heat;
            }

            _ = _everUsed.Add(mapLand);
            _ = toRemove.Remove(mapLand);
        }

        ClearHeat(toRemove);
    }

    /// <inheritdoc cref="IGeoSeries{TDrawingContext}.Delete(MapContext{TDrawingContext})"/>
    public void Delete(MapContext<TDrawingContext> context)
    {
        ClearHeat(_everUsed);
        _ = _subscribedTo.Remove(context.CoreMap);
    }

    /// <summary>
    /// Initializes the series.
    /// </summary>
    protected void IntitializeSeries(IPaint<TDrawingContext> heatPaint)
    {
        _heatPaint = heatPaint;
    }

    /// <summary>
    /// Called to invoke the property changed event.
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void NotifySubscribers()
    {
        foreach (var chart in _subscribedTo) chart.Update();
    }

    private void ClearHeat(IEnumerable<LandDefinition> toRemove)
    {
        foreach (var mapLand in toRemove)
        {
            var shapesQuery = mapLand.Data
                .Select(x => x.Shape)
                .Where(x => x is not null)
                .Cast<IHeatPathShape>();

            foreach (var pathShape in shapesQuery)
            {
                pathShape.FillColor = LvcColor.Empty;
            }

            _ = _everUsed.Remove(mapLand);
        }
    }
}
