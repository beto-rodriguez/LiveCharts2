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
using System.Text;
using System.Threading.Tasks;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Graphics;

namespace LiveChartsCore.SkiaSharpView.Maui
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GeoMap : ContentView, IGeoMap
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
            PropertyChanged += GeoMap_PropertyChanged;
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

        LvcColor[] IGeoMap.HeatMap
        {
            get => HeatMap.Select(x => LvcColor.FromArgb(
                (byte)(x.Alpha * 255), (byte)(x.Red * 255), (byte)(x.Green * 255), (byte)(x.Blue * 255))).ToArray();
            set => HeatMap = value.Select(x => new Color(x.R / 255f, x.G / 255f, x.B / 255f, x.A / 255f)).ToArray();
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

        LvcColor IGeoMap.StrokeColor
        {
            get => LvcColor.FromArgb(
                (byte)(StrokeColor.Alpha * 255), (byte)(StrokeColor.Red * 255), (byte)(StrokeColor.Green * 255), (byte)(StrokeColor.Blue * 255));
            set => StrokeColor = new Color(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
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

        LvcColor IGeoMap.FillColor
        {
            get => LvcColor.FromArgb(
                (byte)(FillColor.Alpha * 255), (byte)(FillColor.Red * 255), (byte)(FillColor.Green * 255), (byte)(FillColor.Blue * 255));
            set => FillColor = new Color(value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f);
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

        private void GeoMap_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var affectsPlot =
                e.PropertyName == nameof(Values) || e.PropertyName == nameof(Width) || e.PropertyName == nameof(Height);
            if (!affectsPlot) return;
            load();
        }

        private void load()
        {
            var paint = new SolidColorPaint();

            var thickness = (float)StrokeThickness;

            var igm = (IGeoMap)this;

            var stroke = igm.StrokeColor;
            var fill = igm.FillColor;
            var hm = igm.HeatMap;

            if (_heatKnownLength != HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(hm, ColorStops);
                _heatKnownLength = HeatMap.Length;
            }

            var worldMap = s_map ??= Maps.GetWorldMap();
            var projector = Maps.BuildProjector(
                Projection,
                new[]
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
        }
    }
}
