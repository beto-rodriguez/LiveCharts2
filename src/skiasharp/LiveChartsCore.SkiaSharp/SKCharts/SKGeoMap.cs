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
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <inheritdoc cref="IGeoMapView{SkiaSharpDrawingContext}"/>
public class SKGeoMap : InMemorySkiaSharpChart, IGeoMapView<SkiaSharpDrawingContext>
{
    private readonly GeoMap<SkiaSharpDrawingContext> _core;
    private object? _viewCommand;
    private IPaint<SkiaSharpDrawingContext>? _stroke = new SolidColorPaint(new SKColor(255, 255, 255, 255)) { IsStroke = true };
    private IPaint<SkiaSharpDrawingContext>? _fill = new SolidColorPaint(new SKColor(240, 240, 240, 255)) { IsFill = true };

    /// <summary>
    /// Initializes a new instance of the <see cref="SKGeoMap"/> class.
    /// </summary>
    public SKGeoMap()
    {
        _core = new GeoMap<SkiaSharpDrawingContext>(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SKGeoMap"/> class.
    /// </summary>
    /// <param name="mapView">The map view.</param>
    public SKGeoMap(IGeoMapView<SkiaSharpDrawingContext> mapView) : this()
    {
        MapProjection = mapView.MapProjection;
        Stroke = mapView.Stroke;
        Fill = mapView.Fill;
        Series = mapView.Series;
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.SyncContext" />
    public object SyncContext { get; set; } = new();

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.DesignerMode" />
    public bool DesignerMode { get; set; } = false;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ActiveMap"/>
    public CoreMap<SkiaSharpDrawingContext> ActiveMap { get; set; } = Maps.GetWorldMap<SkiaSharpDrawingContext>();

    float IGeoMapView<SkiaSharpDrawingContext>.Width => Width;

    float IGeoMapView<SkiaSharpDrawingContext>.Height => Height;

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Canvas"/>
    public MotionCanvas<SkiaSharpDrawingContext> Canvas { get; } = new();

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.MapProjection"/>
    public MapProjection MapProjection { get; set; }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Stroke"/>
    public IPaint<SkiaSharpDrawingContext>? Stroke
    {
        get => _stroke;
        set
        {
            if (value is not null) value.IsStroke = true;
            _stroke = value;
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Fill"/>
    public IPaint<SkiaSharpDrawingContext>? Fill
    {
        get => _fill;
        set
        {
            if (value is not null) value.IsStroke = false;
            _fill = value;
        }
    }

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Series"/>
    public IEnumerable<IGeoSeries> Series { get; set; } = Array.Empty<IGeoSeries>();

    /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ViewCommand"/>
    public object? ViewCommand
    {
        get => _viewCommand;
        set
        {
            _viewCommand = value;
            if (value is not null) _core.ViewTo(value);
        }
    }

    /// <inheritdoc cref="InMemorySkiaSharpChart.DrawOnCanvas(SKCanvas, SKSurface?, bool)"/>
    public override void DrawOnCanvas(SKCanvas canvas, SKSurface? surface, bool clearCanvasOnBeginDraw = false)
    {
        Canvas.DisableAnimations = true;

        _core.Measure();

        Canvas.DrawFrame(
            new SkiaSharpDrawingContext(
                Canvas,
                new SKImageInfo(Width, Height),
                surface!,
                canvas,
                Background,
                clearCanvasOnBeginDraw));

        _core.Unload();
    }

    void IGeoMapView<SkiaSharpDrawingContext>.InvokeOnUIThread(Action action)
    {
        action();
    }
}
