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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Painting;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    /// <seealso cref="Control" />
    public class GeoMap : Control, IGeoMap<SkiaSharpDrawingContext>
    {
        private static GeoJsonFile? s_map = null;
        private int _heatKnownLength = 0;
        private List<Tuple<double, LvcColor>> _heatStops = new();
        private readonly CollectionDeepObserver<MapShape<SkiaSharpDrawingContext>> _shapesObserver;

        static GeoMap()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeoMap), new FrameworkPropertyMetadata(typeof(GeoMap)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap"/> class.
        /// </summary>
        public GeoMap()
        {
            _shapesObserver = new CollectionDeepObserver<MapShape<SkiaSharpDrawingContext>>(
                (object? sender, NotifyCollectionChangedEventArgs e) => Measure(),
                (object? sender, PropertyChangedEventArgs e) => Measure());
            SetCurrentValue(ValuesProperty, new Dictionary<string, double>());
            SetCurrentValue(ShapesProperty, Enumerable.Empty<MapShape<SkiaSharpDrawingContext>>());
            SizeChanged += GeoMap_SizeChanged;
        }

        #region dependency props

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
        /// The values property
        /// </summary>
        public static readonly DependencyProperty ShapesProperty =
            DependencyProperty.Register(nameof(Shapes), typeof(IEnumerable<MapShape<SkiaSharpDrawingContext>>),
                typeof(GeoMap), new PropertyMetadata(null, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (GeoMap)o;
                    var seriesObserver = chart._shapesObserver;
                    seriesObserver.Dispose((IEnumerable<MapShape<SkiaSharpDrawingContext>>)args.OldValue);
                    seriesObserver.Initialize((IEnumerable<MapShape<SkiaSharpDrawingContext>>)args.NewValue);
                    chart.Measure();
                }));

        /// <summary>
        /// The stroke color property
        /// </summary>
        public static readonly DependencyProperty StrokeColorProperty =
            DependencyProperty.Register(
                nameof(StrokeColor), typeof(System.Windows.Media.Color), typeof(GeoMap),
                new PropertyMetadata(System.Windows.Media.Color.FromRgb(224, 224, 224)));

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
                nameof(FillColor), typeof(System.Windows.Media.Color), typeof(GeoMap),
                new PropertyMetadata(System.Windows.Media.Color.FromRgb(250, 250, 250)));

        #endregion

        #region props

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.Measured"/>
        public event Action<IGeoMap<SkiaSharpDrawingContext>> Measured;

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.Canvas"/>
        public MotionCanvas<SkiaSharpDrawingContext> Canvas =>
            Template.FindName("canvas", this) is not MotionCanvas canvas
            ? throw new Exception(
                $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'")
            : canvas.CanvasCore;

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.ActiveMap"/>
        public GeoJsonFile ActiveMap { get => s_map ??= Maps.GetWorldMap(); set => throw new NotImplementedException(); }

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.Width"/>
        float IGeoMap<SkiaSharpDrawingContext>.Width => (float)ActualWidth;

        /// <inheritdoc cref="IGeoMap{TDrawingContext}.Height"/>
        float IGeoMap<SkiaSharpDrawingContext>.Height => (float)ActualHeight;

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

        LvcColor[] IGeoMap<SkiaSharpDrawingContext>.HeatMap
        {
            get => HeatMap.Select(x => LvcColor.FromArgb(x.A, x.R, x.G, x.B)).ToArray();
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

        LvcColor IGeoMap<SkiaSharpDrawingContext>.StrokeColor
        {
            get => LvcColor.FromArgb(StrokeColor.A, StrokeColor.R, StrokeColor.G, StrokeColor.B);
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

        LvcColor IGeoMap<SkiaSharpDrawingContext>.FillColor
        {
            get => LvcColor.FromArgb(FillColor.A, FillColor.R, FillColor.G, FillColor.B);
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

        /// <summary>
        /// Gets or sets the shapes.
        /// </summary>
        public IEnumerable<MapShape<SkiaSharpDrawingContext>> Shapes
        {
            get => (IEnumerable<MapShape<SkiaSharpDrawingContext>>)GetValue(ShapesProperty);
            set => SetValue(ShapesProperty, value);
        }

        #endregion

        private void GeoMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Measure();
        }

        private readonly SolidColorPaint _defaultPaint = new();

        private void Measure()
        {
            if (Template is null) return;

            if (Template.FindName("canvas", this) is not MotionCanvas canvas)
                throw new Exception(
                    $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                    $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'");

            var bounds = new Dictionary<int, Bounds>();
            foreach (var shape in Shapes)
            {
                if (shape is not IWeigthedMapShape wShape) continue;

                if (!bounds.TryGetValue(wShape.WeigthedAt, out var weightBounds))
                {
                    weightBounds = new Bounds();
                    bounds.Add(wShape.WeigthedAt, weightBounds);
                }
                weightBounds.AppendValue(wShape.Value);
            }

            var paint = _defaultPaint;

            var igeo = (IGeoMap<SkiaSharpDrawingContext>)this;
            var thickness = (float)StrokeThickness;
            var stroke = igeo.StrokeColor;
            var fill = igeo.FillColor;
            var hm = igeo.HeatMap;

            if (_heatKnownLength != HeatMap.Length)
            {
                _heatStops = HeatFunctions.BuildColorStops(hm, ColorStops);
                _heatKnownLength = HeatMap.Length;
            }

            var map = ActiveMap;
            var projector = Maps.BuildProjector(Projection, new[] { (float)ActualWidth, (float)ActualHeight });
            var shapes = map.AsMapShapes(hm, _heatStops, stroke, fill, thickness, projector);

            foreach (var shape in shapes)
                paint.AddGeometryToPaintTask(canvas.CanvasCore, shape);
            canvas.CanvasCore.AddDrawableTask(paint);

            var context = new MapShapeContext<SkiaSharpDrawingContext>(this, paint, _heatStops, bounds);

            foreach (var shape in Shapes)
            {
                shape.Measure(context);
            }

            Measured?.Invoke(this);
        }
    }
}
