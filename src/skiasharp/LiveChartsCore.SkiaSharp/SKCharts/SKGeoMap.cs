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
using System.IO;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts
{
    /// <inheritdoc cref="IGeoMapView{SkiaSharpDrawingContext}"/>
    public class SKGeoMap : IGeoMapView<SkiaSharpDrawingContext>, ISkiaSharpChart
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

            // obsoletes, moved to series property.
            HeatMap = mapView.HeatMap;
            ColorStops = mapView.ColorStops;
            Shapes = mapView.Shapes;
        }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public SKColor Background { get; set; } = SKColors.White;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public int Height { get; set; } = 600;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public int Width { get; set; } = 900;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.AutoUpdateEnabled" />
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.SyncContext" />
        public object SyncContext { get; set; } = new object();

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

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.HeatMap"/>
        public LvcColor[] HeatMap { get; set; } = new[]
        {
            LvcColor.FromArgb(255, 179, 229, 252), // cold (min value)
            LvcColor.FromArgb(255, 2, 136, 209) // hot (max value)
        };

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ColorStops"/>
        public double[]? ColorStops { get; set; }

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

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Shapes"/>
        public IEnumerable<IMapElement> Shapes { get; set; } = Enumerable.Empty<MapShape<SkiaSharpDrawingContext>>();

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Series"/>
        public IEnumerable<IGeoSeries<SkiaSharpDrawingContext>> Series { get; set; } = Array.Empty<IGeoSeries<SkiaSharpDrawingContext>>();

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

        /// <inheritdoc cref="ISkiaSharpChart.GetImage"/>
        public SKImage GetImage()
        {
            load();
            Canvas.DisableAnimations = true;

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

            var canvas = surface.Canvas;
            using var clearColor = new SKPaint { Color = Background };
            canvas.DrawRect(0, 0, Width, Height, clearColor);

            Canvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    Canvas,
                    new SKImageInfo(Height, Width),
                    surface,
                    canvas)
                {
                    ClearColor = Background
                });

            return surface.Snapshot();
        }

        /// <inheritdoc cref="ISkiaSharpChart.SaveImage(string, SKEncodedImageFormat, int)"/>
        public void SaveImage(string path, SKEncodedImageFormat format = SKEncodedImageFormat.Png, int quality = 80)
        {
            load();
            Canvas.DisableAnimations = true;

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

            var canvas = surface.Canvas;
            using var clearColor = new SKPaint { Color = Background };
            canvas.DrawRect(0, 0, Width, Height, clearColor);

            Canvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    Canvas,
                    new SKImageInfo(Height, Width),
                    surface,
                    canvas)
                {
                    ClearColor = Background
                });

            using var image = surface.Snapshot();
            using var data = image.Encode(format, quality);
            using var stream = File.OpenWrite(path);
            data.SaveTo(stream);
        }

        void IGeoMapView<SkiaSharpDrawingContext>.InvokeOnUIThread(Action action)
        {
            action();
        }

        private void load()
        {
            _core.Update(new Kernel.ChartUpdateParams { Throttling = false, IsAutomaticUpdate = false });
        }
    }
}
