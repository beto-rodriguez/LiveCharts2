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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <inheritdoc cref="IGeoMap"/>
    public partial class GeoMap : UserControl, IGeoMap
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
        }

        /// <summary>
        /// The projection property.
        /// </summary>
        public static readonly AvaloniaProperty<Projection> ProjectionProperty =
           AvaloniaProperty.Register<CartesianChart, Projection>(nameof(Projection), Projection.Default, inherits: true);

        /// <summary>
        /// The heat map property.
        /// </summary>
        public static readonly AvaloniaProperty<Color[]> HeatMapProperty =
          AvaloniaProperty.Register<CartesianChart, Color[]>(nameof(HeatMap),
              new[]
              {
                  Color.FromArgb(255, 179, 229, 252), // cold (min value)
                  Color.FromArgb(255, 2, 136, 209) // hot (max value)
              }, inherits: true);

        /// <summary>
        /// The color stops property.
        /// </summary>
        public static readonly AvaloniaProperty<double[]?> ColorStopsProperty =
          AvaloniaProperty.Register<CartesianChart, double[]?>(nameof(ColorStops), null, inherits: true);

        /// <summary>
        /// The values property.
        /// </summary>
        public static readonly AvaloniaProperty<Dictionary<string, double>> ValuesProperty =
          AvaloniaProperty.Register<CartesianChart, Dictionary<string, double>>(nameof(ValuesProperty), new Dictionary<string, double>(), inherits: true);

        /// <summary>
        /// The stroke color property.
        /// </summary>
        public static readonly AvaloniaProperty<Color> StrokeColorProperty =
          AvaloniaProperty.Register<CartesianChart, Color>(nameof(StrokeColor), Color.FromArgb(255, 224, 224, 224), inherits: true);

        /// <summary>
        /// The stroke thickness property.
        /// </summary>
        public static readonly AvaloniaProperty<double> StrokeThicknessProperty =
          AvaloniaProperty.Register<CartesianChart, double>(nameof(StrokeThickness), 1d, inherits: true);

        /// <summary>
        /// The fill color property.
        /// </summary>
        public static readonly AvaloniaProperty<Color> FillColorProperty =
          AvaloniaProperty.Register<CartesianChart, Color>(nameof(FillColor), Color.FromArgb(255, 250, 250, 250), inherits: true);

        /// <inheritdoc cref="IGeoMap.Projection"/>
        public Projection Projection
        {
            get => (Projection)GetValue(ProjectionProperty);
            set => SetValue(ProjectionProperty, value);
        }

        /// <inheritdoc cref="IGeoMap.HeatMap"/>
        public Color[] HeatMap
        {
            get => (Color[])GetValue(HeatMapProperty);
            set => SetValue(HeatMapProperty, value);
        }

        LvcColor[] IGeoMap.HeatMap
        {
            get => HeatMap.Select(x => LvcColor.FromArgb(x.A, x.R, x.G, x.B)).ToArray();
            set => HeatMap = value.Select(x => new Color(x.A, x.R, x.G, x.B)).ToArray();
        }

        /// <inheritdoc cref="IGeoMap.ColorStops"/>
        public double[]? ColorStops
        {
            get => (double[])GetValue(ColorStopsProperty);
            set => SetValue(ColorStopsProperty, value);
        }

        /// <inheritdoc cref="IGeoMap.StrokeColor"/>
        public Color StrokeColor
        {
            get => (Color)GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }

        LvcColor IGeoMap.StrokeColor
        {
            get => LvcColor.FromArgb(StrokeColor.A, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            set => StrokeColor = new Color(value.A, value.R, value.G, value.B);
        }

        /// <inheritdoc cref="IGeoMap.StrokeThickness"/>
        public double StrokeThickness
        {
            get => (double)GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        /// <inheritdoc cref="IGeoMap.FillColor"/>
        public Color FillColor
        {
            get => (Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        LvcColor IGeoMap.FillColor
        {
            get => LvcColor.FromArgb(FillColor.A, FillColor.R, FillColor.G, FillColor.B);
            set => FillColor = new Color(value.A, value.R, value.G, value.B);
        }

        /// <inheritdoc cref="IGeoMap.Values"/>
        public Dictionary<string, double> Values
        {
            get => (Dictionary<string, double>)GetValue(ValuesProperty);
            set => SetValue(ValuesProperty, value);
        }

        /// <inheritdoc cref="OnPropertyChanged{T}(AvaloniaPropertyChangedEventArgs{T})"/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(Bounds)) load();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void load()
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");

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
            var projector = Maps.BuildProjector(Projection, new[] { (float)Bounds.Width, (float)Bounds.Height });
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
