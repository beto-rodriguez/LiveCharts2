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
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    /// <seealso cref="Control" />
    public class GeoMap : Control, IGeoMap
    {
        private static GeoJsonFile? s_map = null;
        private int _heatKnownLength = 0;
        private List<Tuple<double, System.Drawing.Color>> _heatStops = new();

        static GeoMap()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeoMap), new FrameworkPropertyMetadata(typeof(GeoMap)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap"/> class.
        /// </summary>
        public GeoMap()
        {
            SetCurrentValue(ValuesProperty, new Dictionary<string, double>());
            SizeChanged += GeoMap_SizeChanged;
        }

        /// <summary>
        /// The projection property
        /// </summary>
        public static readonly DependencyProperty ProjectionProperty =
            DependencyProperty.Register(nameof(Projection), typeof(Projection), typeof(GeoMap), new PropertyMetadata(Projection.Default));

        /// <summary>
        /// The heat map property
        /// </summary>
        public static readonly DependencyProperty HeatMapProperty =
            DependencyProperty.Register(
                nameof(HeatMap), typeof(System.Windows.Media.Color[]), typeof(GeoMap),
                new PropertyMetadata(
                    new[]
                    {
                        System.Windows.Media.Color.FromArgb(255, 179, 229, 252), // cold (min value)
                        System.Windows.Media.Color.FromArgb(255, 2, 136, 209) // hot (max value)
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
                nameof(StrokeColor), typeof(System.Windows.Media.Color), typeof(GeoMap), new PropertyMetadata(System.Windows.Media.Color.FromRgb(224, 224, 224)));

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
                nameof(FillColor), typeof(System.Windows.Media.Color), typeof(GeoMap), new PropertyMetadata(System.Windows.Media.Color.FromRgb(250, 250, 250)));

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
        public System.Windows.Media.Color[] HeatMap
        {
            get => (System.Windows.Media.Color[])GetValue(HeatMapProperty);
            set => SetValue(HeatMapProperty, value);
        }

        System.Drawing.Color[] IGeoMap.HeatMap
        {
            get => HeatMap.Select(x => System.Drawing.Color.FromArgb(x.A, x.R, x.G, x.B)).ToArray();
            set => HeatMap = value.Select(x => System.Windows.Media.Color.FromArgb(x.A, x.R, x.G, x.B)).ToArray();
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
        public System.Windows.Media.Color StrokeColor
        {
            get => (System.Windows.Media.Color)GetValue(StrokeColorProperty);
            set => SetValue(StrokeColorProperty, value);
        }

        System.Drawing.Color IGeoMap.StrokeColor
        {
            get => System.Drawing.Color.FromArgb(StrokeColor.A, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            set => StrokeColor = System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B);
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
        public System.Windows.Media.Color FillColor
        {
            get => (System.Windows.Media.Color)GetValue(FillColorProperty);
            set => SetValue(FillColorProperty, value);
        }

        System.Drawing.Color IGeoMap.FillColor
        {
            get => System.Drawing.Color.FromArgb(FillColor.A, FillColor.R, FillColor.G, FillColor.B);
            set => FillColor = System.Windows.Media.Color.FromArgb(value.A, value.R, value.G, value.B);
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
            if (Template.FindName("canvas", this) is not MotionCanvas canvas)
                throw new Exception(
                    $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'");

            var paint = new SolidColorPaintTask();

            var thickness = (float)StrokeThickness;
            var stroke = System.Drawing.Color.FromArgb(255, StrokeColor.R, StrokeColor.G, StrokeColor.B);
            var fill = System.Drawing.Color.FromArgb(255, FillColor.R, FillColor.G, FillColor.B);

            var hm = HeatMap.Select(x => System.Drawing.Color.FromArgb(x.A, x.R, x.G, x.B)).ToArray();

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
        }
    }
}
