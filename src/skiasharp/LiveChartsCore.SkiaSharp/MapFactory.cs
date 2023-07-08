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
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Segments;

namespace LiveChartsCore.SkiaSharpView;

/// <summary>
/// Defines a map builder.
/// </summary>
public class MapFactory : IMapFactory<SkiaSharpDrawingContext>
{
    private readonly HashSet<HeatPathShape> _usedPathShapes = new();
    private readonly HashSet<IPaint<SkiaSharpDrawingContext>> _usedPaints = new();
    private readonly HashSet<string> _usedLayers = new();
    private IGeoMapView<SkiaSharpDrawingContext>? _mapView;

    /// <inheritdoc cref="IMapFactory{TDrawingContext}.GenerateLands(MapContext{TDrawingContext})"/>
    public void GenerateLands(MapContext<SkiaSharpDrawingContext> context)
    {
        var projector = context.Projector;

        var toRemoveLayers = new HashSet<string>(_usedLayers);
        var toRemovePathShapes = new HashSet<HeatPathShape>(_usedPathShapes);
        var toRemovePaints = new HashSet<IPaint<SkiaSharpDrawingContext>>(_usedPaints);

        var layersQuery = context.View.ActiveMap.Layers.Values
            .Where(x => x.IsVisible)
            .OrderByDescending(x => x.ProcessIndex);

        _mapView = context.View;

        foreach (var layer in layersQuery)
        {
            var stroke = context.View.Stroke ?? layer.Stroke;
            var fill = context.View.Fill ?? layer.Fill;

            if (fill is not null)
            {
                context.View.Canvas.AddDrawableTask(fill);
                _ = _usedPaints.Add(fill);
                _ = toRemovePaints.Remove(fill);
            }
            if (stroke is not null)
            {
                context.View.Canvas.AddDrawableTask(stroke);
                _ = _usedPaints.Add(stroke);
                _ = toRemovePaints.Remove(stroke);
            }

            _ = _usedLayers.Add(layer.Name);
            _ = toRemoveLayers.Remove(layer.Name);

            foreach (var landDefinition in layer.Lands.Values)
            {
                foreach (var landData in landDefinition.Data)
                {
                    HeatPathShape shape;

                    if (landData.Shape is null)
                    {
                        landData.Shape = shape = new HeatPathShape { IsClosed = true };
                        shape.Animate(EasingFunctions.ExponentialOut, TimeSpan.FromMilliseconds(800));
                    }
                    else
                    {
                        shape = (HeatPathShape)landData.Shape;
                    }

                    _ = _usedPathShapes.Add(shape);
                    _ = toRemovePathShapes.Remove(shape);

                    stroke?.AddGeometryToPaintTask(context.View.Canvas, shape);
                    fill?.AddGeometryToPaintTask(context.View.Canvas, shape);

                    shape.ClearCommands();

                    var isFirst = true;

                    foreach (var point in landData.Coordinates)
                    {
                        var p = projector.ToMap(new double[] { point.X, point.Y });

                        var x = p[0];
                        var y = p[1];

                        if (isFirst)
                        {
                            _ = shape.AddLast(new MoveToPathCommand { X = x, Y = y });
                            isFirst = false;
                            continue;
                        }

                        _ = shape.AddLast(new LineSegment { X = x, Y = y });
                    }
                }
            }

            foreach (var shape in toRemovePathShapes)
            {
                stroke?.RemoveGeometryFromPainTask(context.View.Canvas, shape);
                fill?.RemoveGeometryFromPainTask(context.View.Canvas, shape);

                shape.ClearCommands();

                _ = _usedPathShapes.Remove(shape);
            }
        }

        foreach (var paint in toRemovePaints)
        {
            _ = _usedPaints.Remove(paint);
            context.View.Canvas.RemovePaintTask(paint);
        }

        foreach (var layerName in toRemoveLayers)
        {
            _ = context.MapFile.Layers.Remove(layerName);
            _ = _usedLayers.Remove(layerName);
        }
    }

    /// <inheritdoc cref="IMapFactory{TDrawingContext}.ViewTo(GeoMap{TDrawingContext}, object)"/>
    public void ViewTo(GeoMap<SkiaSharpDrawingContext> sender, object? command) { }

    /// <inheritdoc cref="IMapFactory{TDrawingContext}.Pan(GeoMap{TDrawingContext}, LvcPoint)"/>
    public void Pan(GeoMap<SkiaSharpDrawingContext> sender, LvcPoint delta) { }

    /// <summary>
    /// Disposes the map factory.
    /// </summary>
    public void Dispose()
    {
        if (_mapView is not null)
        {
            var layersQuery = _mapView.ActiveMap.Layers.Values
               .Where(x => x.IsVisible)
               .OrderByDescending(x => x.ProcessIndex);

            foreach (var layer in layersQuery)
            {
                var stroke = _mapView.Stroke ?? layer.Stroke;
                var fill = _mapView.Fill ?? layer.Fill;

                foreach (var landDefinition in layer.Lands.Values)
                {
                    foreach (var landData in landDefinition.Data)
                    {
                        var shape = (IDrawable<SkiaSharpDrawingContext>?)landData.Shape;
                        if (shape is null) continue;

                        stroke?.RemoveGeometryFromPainTask(_mapView.Canvas, shape);
                        fill?.AddGeometryToPaintTask(_mapView.Canvas, shape);

                        landData.Shape = null;
                    }
                }
                foreach (var paint in _usedPaints)
                {
                    _mapView.Canvas.RemovePaintTask(paint);
                    paint.ClearGeometriesFromPaintTask(_mapView.Canvas);
                }

                if (stroke is not null) _mapView.Canvas.RemovePaintTask(stroke);
                if (fill is not null) _mapView.Canvas.RemovePaintTask(fill);
            }
        }

        _usedPathShapes.Clear();
        _usedLayers.Clear();
        _usedPaints.Clear();
    }
}
