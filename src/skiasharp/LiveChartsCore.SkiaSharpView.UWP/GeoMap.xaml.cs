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
using LiveChartsCore.SkiaSharpView.Painting;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveChartsCore.SkiaSharpView.UWP
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    public sealed partial class GeoMap : UserControl, IGeoMap<SkiaSharpDrawingContext>
    {
        private static GeoJsonFile? s_map = null;
        private int _heatKnownLength = 0;
        private List<Tuple<double, LvcColor>> _heatStops = new();

        public GeoMap()
        {
            InitializeComponent();
            SetValue(ValuesProperty, new Dictionary<string, double>());
            SizeChanged += GeoMap_SizeChanged; ;
        }

        /// <summary>
        /// The projection property
        /// </summary>
        public static readonly DependencyProperty MapProjectionProperty =
            DependencyProperty.Register(nameof(Projection), typeof(Projection), typeof(GeoMap), new PropertyMetadata(Projection.Default));

        /// <summary>
        /// The heat map property
        /// </summary>
        public static readonly DependencyProperty HeatMapProperty =
            DependencyProperty.Register(
                nameof(HeatMap), typeof(Color[]), typeof(GeoMap),
                new PropertyMetadata(
                    new[]
                    {
                        Color.FromArgb(255, 179, 229, 252), // cold (min value)
                        Color.FromArgb(255, 2, 136, 209) // hot (max value)
                    }));

        /// <summary>
        /// The color stops property
        /// </summary>
        public static readonly DependencyProperty ColorStopsProperty =
            DependencyProperty.Register(nameof(ColorStops), typeof(double[]), typeof(GeoMap), new PropertyMetadata(null));

        /// <summary>
        /// The values property
        /// </summary>
        public static readonly DependencyProperty ValuesProperty =
            DependencyProperty.Register(nameof(Values), typeof(Dictionary<string, double>), typeof(GeoMap), new PropertyMetadata(null));

        /// <summary>
        /// The stroke color property
        /// </summary>
        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register(
                nameof(StrokeColor), typeof(Color), typeof(GeoMap),
                new PropertyMetadata(Color.FromArgb(255, 224, 224, 224)));

        /// <summary>
        /// The stroke thickness property
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(GeoMap), new PropertyMetadata(1d));

        /// <summary>
        /// The fill color property
        /// </summary>
        public static readonly DependencyProperty FillColorProperty =
            DependencyProperty.Register(
                nameof(FillColor), typeof(Color), typeof(GeoMap),
                new PropertyMetadata(Color.FromArgb(255, 250, 250, 250)));

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.Measured"/>
        public event Action<IGeoMap<SkiaSharpDrawingContext>> Measured;

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.Canvas"/>
        public MotionCanvas<SkiaSharpDrawingContext> Canvas => canvas.CanvasCore;

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        public new Projection Projection // <- hide the base property?? maybe we need to re name our property to MapProjection
        {
            get => (Projection)GetValue(MapProjectionProperty);
            set => SetValue(MapProjectionProperty, value);
        }

        /// <summary>
        /// Gets or sets the heat map.
        /// </summary>
        public Color[] HeatMap
        {
            get => (Color[])GetValue(HeatMapProperty);
            set => SetValue(HeatMapProperty, value);
        }

        LvcColor[] IGeoMap<SkiaSharpDrawingContext>.HeatMap
        {
            get => HeatMap.Select(x => LvcColor.FromArgb(x.A, x.R, x.G, x.B)).ToArray();
            set => HeatMap = value.Select(x => Color.FromArgb(x.A, x.R, x.G, x.B)).ToArray();
        }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public double[]? ColorStops
        {
            get => (double[])GetValue(ColorStopsProperty);
            set => SetValue(ColorStopsProperty, value);
        }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public Color StrokeColor
        {
            get => (Color)GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }

        LvcColor IGeoMap<SkiaSharpDrawingContext>.StrokeColor
        {
            get => LvcColor.FromArgb(StrokeColor.A, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            set => StrokeColor = Color.FromArgb(value.A, value.R, value.G, value.B);
        }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        LvcColor IGeoMap<SkiaSharpDrawingContext>.FillColor
        {
            get => LvcColor.FromArgb(FillColor.A, FillColor.R, FillColor.G, FillColor.B);
            set => FillColor = Color.FromArgb(value.A, value.R, value.G, value.B);
        }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public Dictionary<string, double> Values
        {
            get => (Dictionary<string, double>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }

        private void GeoMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var paint = new SolidColorPaint();

            var thickness = (float)StrokeThickness;
            var stroke = LvcColor.FromArgb(255, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            var fill = LvcColor.FromArgb(255, FillColor.R, FillColor.G, FillColor.B);

            var hm = HeatMap.Select(x => LvcColor.FromArgb(x.A, x.R, x.G, x.B)).ToArray();

            if (_heatKnownLength != HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(hm, ColorStops);
                _heatKnownLength = HeatMap.Length;
            }

            var worldMap = s_map ??= Maps.GetWorldMap();
            var projector = Maps.BuildProjector(Projection, new[] { (float)ActualWidth, (float)ActualHeight });
            var shapes = worldMap.AsHeatMapShapes(Values, hm, _heatStops, stroke, fill, thickness, projector);

            canvas.PaintTasks = new List<PaintSchedule<SkiaSharpDrawingContext>>
            {
                 new PaintSchedule<SkiaSharpDrawingContext>(
                    paint,
                    new HashSet<IDrawable<SkiaSharpDrawingContext>>(shapes))
            };

            Measured?.Invoke(this);
        }
    }
}
