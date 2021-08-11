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
using System.Windows.Forms;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;

namespace LiveChartsCore.SkiaSharpView.WinForms
{
    /// <summary>
    /// The geo map control.
    /// </summary>
    /// <seealso cref="UserControl" />
    public partial class GeoMap : UserControl, IGeoMap
    {
        private static GeoJsonFile? s_map = null;
        private int _heatKnownLength = 0;
        private List<Tuple<double, Color>> _heatStops = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap"/> class.
        /// </summary>
        public GeoMap()
        {
            InitializeComponent();

            Resize += GeoMap_Resize;
        }

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        public Projection Projection { get; set; } = Projection.Default;

        /// <summary>
        /// Gets or sets the heat map.
        /// </summary>
        public Color[] HeatMap { get; set; } = new[]
        {
            Color.FromArgb(255, 179, 229, 252), // cold (min value)
            Color.FromArgb(255, 2, 136, 209) // hot (max value)
        };

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public double[]? ColorStops { get; set; } = null;

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public Color StrokeColor { get; set; } = Color.FromArgb(255, 224, 224, 224);

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public double StrokeThickness { get; set; } = 1d;

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public Color FillColor { get; set; } = Color.FromArgb(255, 250, 250, 250);

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public Dictionary<string, double> Values { get; set; } = new Dictionary<string, double>();

        private void GeoMap_Resize(object sender, EventArgs e)
        {
            load();
        }

        private void load()
        {
            var canvas = motionCanvas1;

            var paint = new SolidColorPaint();

            var thickness = (float)StrokeThickness;
            var stroke = Color.FromArgb(255, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            var fill = Color.FromArgb(255, FillColor.R, FillColor.G, FillColor.B);

            if (_heatKnownLength != HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(HeatMap, ColorStops);
                _heatKnownLength = HeatMap.Length;
            }

            var worldMap = s_map ??= Maps.GetWorldMap();
            var projector = Maps.BuildProjector(Projection, new float[] { Width, Height });
            var shapes = worldMap.AsHeatMapShapes(Values, HeatMap, _heatStops, stroke, fill, thickness, projector);

            canvas.PaintTasks = new List<PaintSchedule<SkiaSharpDrawingContext>>
            {
                 new PaintSchedule<SkiaSharpDrawingContext>(
                    paint,
                    new HashSet<IDrawable<SkiaSharpDrawingContext>>(shapes))
            };
        }
    }
}
