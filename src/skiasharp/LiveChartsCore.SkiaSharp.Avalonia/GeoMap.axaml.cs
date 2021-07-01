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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;
using LiveChartsCore.SkiaSharpView.Painting;

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    public partial class GeoMap : UserControl
    {
        private static GeoJsonFile? s_map = null;
        private int _heatKnownLength = 0;
        private List<Tuple<double, System.Drawing.Color>> _heatStops = new();
        private Bounds _weightBounds = new();

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
        public static readonly AvaloniaProperty<System.Drawing.Color[]> HeatMapProperty =
          AvaloniaProperty.Register<CartesianChart, System.Drawing.Color[]>(nameof(HeatMap),
              new[]
              {
                  System.Drawing.Color.FromArgb(255, 255, 205, 210), // cold (min value)
                  System.Drawing.Color.FromArgb(255, 244, 67, 54) // hot (max value)
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
          AvaloniaProperty.Register<CartesianChart, Dictionary<string, double>>(nameof(ColorStops), new Dictionary<string, double>(), inherits: true);

        /// <summary>
        /// The stroke color property.
        /// </summary>
        public static readonly AvaloniaProperty<Color> StrokeColorProperty =
          AvaloniaProperty.Register<CartesianChart, Color>(nameof(ColorStops), Color.FromArgb(255, 236, 239, 241), inherits: true);

        /// <summary>
        /// The stroke thickness property.
        /// </summary>
        public static readonly AvaloniaProperty<double> StrokeThicknessProperty =
          AvaloniaProperty.Register<CartesianChart, double>(nameof(ColorStops), 1d, inherits: true);

        /// <summary>
        /// The fill color property.
        /// </summary>
        public static readonly AvaloniaProperty<Color> FillColorProperty =
          AvaloniaProperty.Register<CartesianChart, Color>(nameof(ColorStops), Color.FromArgb(255, 250, 250, 250), inherits: true);

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
        public System.Drawing.Color[] HeatMap
        {
            get => (System.Drawing.Color[])GetValue(HeatMapProperty);
            set => SetValue(HeatMapProperty, value);
        }

        /// <summary>
        /// Gets or sets the color stops.
        /// </summary>
        public double[] ColorStops
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

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
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

            var paint = new SolidColorPaintTask();

            var thickness = (float)StrokeThickness;
            var stroke = System.Drawing.Color.FromArgb(255, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            var fill = System.Drawing.Color.FromArgb(255, FillColor.R, FillColor.G, FillColor.B);

            Values = new Dictionary<string, double>
            {
                ["mex"] = 10,
                ["usa"] = 15,
                ["can"] = 8,
                ["ind"] = 12,
                ["deu"] = 13,
                ["chn"] = 14,
                ["rus"] = 11,
                ["fra"] = 8,
                ["esp"] = 7,
                ["kor"] = 10,
                ["zaf"] = 12,
                ["bra"] = 13,
                ["are"] = 13
            };

            var worldMap = s_map ??= Maps.GetWorldMap();

            var d = new double[0][][][];
            var paths = new List<PathShape>();
            var projector = Maps.BuildProjector(Projection, new[] { (float)Bounds.Width, (float)Bounds.Height });

            if (_heatKnownLength != HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(HeatMap, ColorStops);
                _heatKnownLength = HeatMap.Length;
            }

            _weightBounds = new Bounds();
            foreach (var value in Values)
            {
                _weightBounds.AppendValue(value.Value);
            }

            foreach (var feature in worldMap.Features ?? new GeoJsonFeature[0])
            {
                var name = feature.Properties != null ? feature.Properties["shortName"] : "";
                System.Drawing.Color? baseColor = Values.TryGetValue(name, out var weight)
                    ? HeatFunctions.InterpolateColor((float)weight, _weightBounds, HeatMap, _heatStops)
                    : null;

                foreach (var geometry in feature.Geometry?.Coordinates ?? d)
                {
                    foreach (var segment in geometry)
                    {
                        var path = new PathShape
                        {
                            StrokeColor = stroke,
                            FillColor = baseColor ?? fill,
                            StrokeThickness = thickness,
                            IsClosed = true
                        };
                        var isFirst = true;
                        foreach (var point in segment)
                        {
                            var p = projector.ToMap(point);

                            if (isFirst)
                            {
                                isFirst = false;
                                path.AddCommand(new MoveToPathCommand { X = p[0], Y = p[1] });
                                continue;
                            }

                            path.AddCommand(new Drawing.Geometries.Segments.LineSegment { X = p[0], Y = p[1] });
                        }
                        paths.Add(path);
                    }
                }
            }

            canvas.PaintTasks = new List<PaintSchedule<SkiaSharpDrawingContext>>
            {
                 new PaintSchedule<SkiaSharpDrawingContext>(
                    paint,
                    new HashSet<IDrawable<SkiaSharpDrawingContext>>(paths))
            };
        }
    }
}
