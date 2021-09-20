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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeoMap : ContentView, IGeoMap<SkiaSharpDrawingContext>
    {
        private static GeoJsonFile? s_map = null;
        private int _heatKnownLength = 0;
        private List<Tuple<double, LvcColor>> _heatStops = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap"/> class.
        /// </summary>
        public GeoMap()
        {
            InitializeComponent();
            SizeChanged += GeoMap_SizeChanged;
        }

        /// <summary>
        /// The projection property
        /// </summary>
        public static readonly BindableProperty ProjectionProperty =
            BindableProperty.Create(
                nameof(Projection), typeof(Projection), typeof(GeoMap), Projection.Default, BindingMode.Default, null);

        /// <summary>
        /// The heat map property
        /// </summary>
        public static readonly BindableProperty HeatMapProperty =
            BindableProperty.Create(
                nameof(HeatMap), typeof(Color[]), typeof(GeoMap),
                new[]
                {
                  Color.FromRgba(179, 229, 252, 255), // cold (min value)
                  Color.FromRgba(2, 136, 209, 255) // hot (max value)
                }, BindingMode.Default, null);

        /// <summary>
        /// The color stops property
        /// </summary>
        public static readonly BindableProperty ColorStopsProperty =
            BindableProperty.Create(
                nameof(ColorStops), typeof(double[]), typeof(GeoMap), null, BindingMode.Default, null);

        /// <summary>
        /// The stroke color property
        /// </summary>
        public static readonly BindableProperty StrokeColorProperty =
            BindableProperty.Create(
                nameof(StrokeColor), typeof(Color), typeof(GeoMap), Color.FromRgba(224, 224, 224, 255), BindingMode.Default, null);

        /// <summary>
        /// The stroke thickness property
        /// </summary>
        public static readonly BindableProperty StrokeThicknessProperty =
            BindableProperty.Create(
                nameof(StrokeThickness), typeof(double), typeof(GeoMap), 1d, BindingMode.Default, null);

        /// <summary>
        /// The fill color property
        /// </summary>
        public static readonly BindableProperty FillColorProperty =
           BindableProperty.Create(
               nameof(FillColor), typeof(Color), typeof(GeoMap), Color.FromRgba(250, 250, 250, 255), BindingMode.Default, null);

        /// <summary>
        /// The values property
        /// </summary>
        public static readonly BindableProperty ValuesProperty =
           BindableProperty.Create(
               nameof(Values), typeof(Dictionary<string, double>), typeof(GeoMap), new Dictionary<string, double>(), BindingMode.Default, null);

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.Measured"/>
        public event Action<IGeoMap<SkiaSharpDrawingContext>> Measured;

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.Canvas"/>
        public MotionCanvas<SkiaSharpDrawingContext> Canvas => canvas.CanvasCore;

        /// <summary>
        /// Gets or sets the projection.
        /// </summary>
        public Projection Projection
        {
            get => (Projection)GetValue(ProjectionProperty);
            set => SetValue(ProjectionProperty, value);
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
            get => HeatMap.Select(x => LvcColor.FromArgb((byte)(x.A * 255), (byte)(x.R * 255), (byte)(x.G * 255), (byte)(x.B * 255))).ToArray();
            set => HeatMap = value.Select(x => new Color(x.R / 255d, x.G / 255d, x.B / 255d, x.A / 255d)).ToArray();
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
            get => LvcColor.FromArgb((byte)(StrokeColor.A * 255), (byte)(StrokeColor.R * 255), (byte)(StrokeColor.G * 255), (byte)(StrokeColor.B * 255));
            set => StrokeColor = new Color(value.R / 255d, value.G / 255d, value.B / 255d, value.A / 255d);
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
            get => LvcColor.FromArgb((byte)(FillColor.A * 255), (byte)(FillColor.R * 255), (byte)(FillColor.G * 255), (byte)(FillColor.B * 255));
            set => FillColor = new Color(value.R / 255d, value.G / 255d, value.B / 255d, value.A / 255d);
        }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public Dictionary<string, double> Values
        {
            get => (Dictionary<string, double>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }

        private void GeoMap_SizeChanged(object sender, EventArgs e)
        {
            load();
        }

        private void load()
        {
            var paint = new SolidColorPaint();

            var thickness = (float)StrokeThickness;
            var stroke = LvcColor.FromArgb(255, (byte)(StrokeColor.R * 255), (byte)(StrokeColor.G * 255), (byte)(StrokeColor.B * 255));
            var fill = LvcColor.FromArgb(255, (byte)(FillColor.R * 255), (byte)(FillColor.G * 255), (byte)(FillColor.B * 255));

            var hm = HeatMap.Select(x => LvcColor.FromArgb((byte)(255 * x.A), (byte)(255 * x.R), (byte)(255 * x.G), (byte)(255 * x.B))).ToArray();

            if (_heatKnownLength != HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(hm, ColorStops);
                _heatKnownLength = HeatMap.Length;
            }

            var worldMap = s_map ??= Maps.GetWorldMap();
            var projector = Maps.BuildProjector(
                Projection, new[]
                {
                    (float)(Bounds.Width * DeviceDisplay.MainDisplayInfo.Density),
                    (float)(Bounds.Height * DeviceDisplay.MainDisplayInfo.Density)
                });
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
