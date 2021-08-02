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
using System.Drawing;
using System.IO;
using System.Linq;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.SKCharts
{
    /// <inheritdoc cref="IGeoMap"/>
    public class SKGeoMap : IGeoMap, ISkiaSharpChart
    {
        private readonly MotionCanvas<SkiaSharpDrawingContext> _motionCanvas = new();
        private static GeoJsonFile? s_map = null;
        private int _heatKnownLength = 0;
        private List<Tuple<double, Color>> _heatStops = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SKGeoMap"/> class.
        /// </summary>
        public SKGeoMap()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SKGeoMap"/> class.
        /// </summary>
        /// <param name="mapView">The map view.</param>
        public SKGeoMap(IGeoMap mapView)
        {
            Projection = mapView.Projection;
            HeatMap = mapView.HeatMap;
            ColorStops = mapView.ColorStops;
            StrokeColor = mapView.StrokeColor;
            StrokeThickness = mapView.StrokeThickness;
            FillColor = mapView.FillColor;
            Values = mapView.Values;
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

        /// <inheritdoc cref="IGeoMap.Projection"/>
        public Projection Projection { get; set; }

        /// <inheritdoc cref="IGeoMap.HeatMap"/>
        public Color[] HeatMap { get; set; } = new[]
        {
            Color.FromArgb(255, 179, 229, 252), // cold (min value)
            Color.FromArgb(255, 2, 136, 209) // hot (max value)
        };

        /// <inheritdoc cref="IGeoMap.ColorStops"/>
        public double[]? ColorStops { get; set; }

        /// <inheritdoc cref="IGeoMap.StrokeColor"/>
        public Color StrokeColor { get; set; } = Color.FromArgb(255, 224, 224, 224);

        /// <inheritdoc cref="IGeoMap.StrokeThickness"/>
        public double StrokeThickness { get; set; } = 1;

        /// <inheritdoc cref="IGeoMap.FillColor"/>
        public Color FillColor { get; set; } = Color.FromArgb(255, 250, 250, 250);

        /// <inheritdoc cref="IGeoMap.Values"/>
        public Dictionary<string, double> Values { get; set; } = new Dictionary<string, double>();

        /// <inheritdoc cref="ISkiaSharpChart.GetImage"/>
        public SKImage GetImage()
        {
            load();
            _motionCanvas.DisableAnimations = true;

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

            var canvas = surface.Canvas;
            using var clearColor = new SKPaint { Color = Background };
            canvas.DrawRect(0, 0, Width, Height, clearColor);

            _motionCanvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    _motionCanvas,
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
            _motionCanvas.DisableAnimations = true;

            using var surface = SKSurface.Create(new SKImageInfo(Width, Height));

            var canvas = surface.Canvas;
            using var clearColor = new SKPaint { Color = Background };
            canvas.DrawRect(0, 0, Width, Height, clearColor);

            _motionCanvas.DrawFrame(
                new SkiaSharpDrawingContext(
                    _motionCanvas,
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

        private void load()
        {
            var paint = new SolidColorPaintTask();

            var thickness = (float)StrokeThickness;
            var stroke = Color.FromArgb(255, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            var fill = Color.FromArgb(255, FillColor.R, FillColor.G, FillColor.B);

            var hm = HeatMap.Select(x => Color.FromArgb(x.A, x.R, x.G, x.B)).ToArray();

            if (_heatKnownLength != HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(hm, ColorStops);
                _heatKnownLength = HeatMap.Length;
            }

            var worldMap = s_map ??= Maps.GetWorldMap();
            var projector = Maps.BuildProjector(Projection, new float[] { Width, Height });
            var shapes = worldMap.AsHeatMapShapes(Values, hm, _heatStops, stroke, fill, thickness, projector);
            paint.SetGeometries(_motionCanvas, new HashSet<IDrawable<SkiaSharpDrawingContext>>(shapes));
            var tasks = new HashSet<IPaintTask<SkiaSharpDrawingContext>> { paint };
            _motionCanvas.SetPaintTasks(tasks);
        }
    }
}
