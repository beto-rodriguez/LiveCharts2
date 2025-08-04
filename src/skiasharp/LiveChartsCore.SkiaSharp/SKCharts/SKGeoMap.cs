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
using LiveChartsCore.Geo;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts;

/// <inheritdoc cref="IGeoMapView"/>
public class SKGeoMap : InMemorySkiaSharpChart, IGeoMapView
{
    private readonly GeoMapChart _core;

    /// <summary>
    /// Initializes a new instance of the <see cref="SKGeoMap"/> class.
    /// </summary>
    public SKGeoMap()
    {
        _core = new GeoMapChart(this);
        ActiveMap = Maps.GetWorldMap();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SKGeoMap"/> class.
    /// </summary>
    /// <param name="mapView">The map view.</param>
    public SKGeoMap(IGeoMapView mapView) : this()
    {
        MapProjection = mapView.MapProjection;
        Stroke = mapView.Stroke;
        Fill = mapView.Fill;
        Series = mapView.Series;
    }

    /// <inheritdoc cref="IGeoMapView.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IGeoMapView.SyncContext" />
    public object SyncContext { get; set; } = new();

    /// <inheritdoc cref="IGeoMapView.DesignerMode" />
    public bool DesignerMode { get; set; } = false;

    /// <inheritdoc cref="IGeoMapView.ActiveMap"/>
    public DrawnMap ActiveMap { get; set; }

    float IGeoMapView.Width => Width;

    float IGeoMapView.Height => Height;

    /// <inheritdoc cref="IGeoMapView.Canvas"/>
    public CoreMotionCanvas Canvas { get; } = new();

    /// <inheritdoc cref="IGeoMapView.MapProjection"/>
    public MapProjection MapProjection { get; set; }

    /// <inheritdoc cref="IGeoMapView.Stroke"/>
    public Paint? Stroke
    {
        get;
        set
        {
            if (value is not null) value.PaintStyle = PaintStyle.Stroke;
            field = value;
        }
    } = new SolidColorPaint(new SKColor(255, 255, 255, 255)) { PaintStyle = PaintStyle.Stroke };

    /// <inheritdoc cref="IGeoMapView.Fill"/>
    public Paint? Fill
    {
        get;
        set
        {
            if (value is not null) value.PaintStyle = PaintStyle.Fill;
            field = value;
        }
    } = new SolidColorPaint(new SKColor(240, 240, 240, 255)) { PaintStyle = PaintStyle.Fill };

    /// <inheritdoc cref="IGeoMapView.Series"/>
    public IEnumerable<IGeoSeries> Series { get; set; } = [];

    /// <inheritdoc cref="IGeoMapView.ViewCommand"/>
    public object? ViewCommand
    {
        get;
        set
        {
            field = value;
            if (value is not null) _core.ViewTo(value);
        }
    }

    /// <inheritdoc cref="InMemorySkiaSharpChart.DrawOnCanvas(SKCanvas, bool)"/>
    public override void DrawOnCanvas(SKCanvas canvas, bool clearCanvasOnBeginDraw = false)
    {
        Canvas.DisableAnimations = true;

        _core.Measure();

        Canvas.DrawFrame(
            new SkiaSharpDrawingContext(
                Canvas,
                new SKImageInfo(Width, Height),
                canvas,
                Background,
                clearCanvasOnBeginDraw));

        _core.Unload();
    }

    void IGeoMapView.InvokeOnUIThread(Action action) =>
        action();
}
