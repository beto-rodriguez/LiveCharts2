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
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.WPF
{
    /// <summary>
    /// Defines a geographic map.
    /// </summary>
    /// <seealso cref="Control" />
    public class GeoMap : Control, IGeoMapView<SkiaSharpDrawingContext>
    {
        private readonly CollectionDeepObserver<IMapShape> _shapesObserver;
        private readonly GeoMap<SkiaSharpDrawingContext, PathGeometry, LineSegment, MoveToPathCommand, SKPath> _core;

        static GeoMap()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GeoMap), new FrameworkPropertyMetadata(typeof(GeoMap)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap"/> class.
        /// </summary>
        public GeoMap()
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);
            _core = new GeoMap<SkiaSharpDrawingContext, PathGeometry, LineSegment, MoveToPathCommand, SKPath>(this);
            _shapesObserver = new CollectionDeepObserver<IMapShape>(
                (object? sender, NotifyCollectionChangedEventArgs e) => Measure(),
                (object? sender, PropertyChangedEventArgs e) => Measure());
            SetCurrentValue(ShapesProperty, Enumerable.Empty<MapShape<SkiaSharpDrawingContext>>());
            SetCurrentValue(ActiveMapProperty, Maps.GetWorldMap());
            SetCurrentValue(SyncContextProperty, new object());
            SizeChanged += GeoMap_SizeChanged;
        }

        #region dependency props

        /// <summary>
        /// The active map property
        /// </summary>
        public static readonly DependencyProperty ActiveMapProperty =
            DependencyProperty.Register(nameof(ActiveMap), typeof(GeoJsonFile), typeof(GeoMap),
                new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The sync context property.
        /// </summary>
        public static readonly DependencyProperty SyncContextProperty =
           DependencyProperty.Register(
               nameof(SyncContext), typeof(object), typeof(GeoMap), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The projection property
        /// </summary>
        public static readonly DependencyProperty MapProjectionProperty =
            DependencyProperty.Register(nameof(MapProjection), typeof(MapProjection), typeof(GeoMap),
                new PropertyMetadata(MapProjection.Default, OnDependencyPropertyChanged));

        /// <summary>
        /// The heat map property
        /// </summary>
        public static readonly DependencyProperty HeatMapProperty =
            DependencyProperty.Register(
                nameof(HeatMap), typeof(LvcColor[]), typeof(GeoMap),
                new PropertyMetadata(
                    new[]
                    {
                        LvcColor.FromArgb(255, 179, 229, 252), // cold (min value)
                        LvcColor.FromArgb(255, 2, 136, 209) // hot (max value)
                    }, OnDependencyPropertyChanged));

        /// <summary>
        /// The color stops property
        /// </summary>
        public static readonly DependencyProperty ColorStopsProperty =
            DependencyProperty.Register(nameof(ColorStops), typeof(double[]), typeof(GeoMap),
                new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The values property
        /// </summary>
        public static readonly DependencyProperty ShapesProperty =
            DependencyProperty.Register(nameof(Shapes), typeof(IEnumerable<IMapShape>),
                typeof(GeoMap), new PropertyMetadata(null, (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (GeoMap)o;
                    var seriesObserver = chart._shapesObserver;
                    seriesObserver.Dispose((IEnumerable<MapShape<SkiaSharpDrawingContext>>)args.OldValue);
                    seriesObserver.Initialize((IEnumerable<MapShape<SkiaSharpDrawingContext>>)args.NewValue);
                    chart._core.Update();
                }));

        /// <summary>
        /// The stroke color property
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(
                nameof(Stroke), typeof(IPaint<SkiaSharpDrawingContext>), typeof(GeoMap),
                new PropertyMetadata(new SolidColorPaint(new SKColor(224, 224, 224)) { IsStroke = true }, OnDependencyPropertyChanged));

        /// <summary>
        /// The fill color property
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(
                nameof(Fill), typeof(IPaint<SkiaSharpDrawingContext>), typeof(GeoMap),
                new PropertyMetadata(new SolidColorPaint(new SKColor(250, 250, 250)) { IsFill = true }, OnDependencyPropertyChanged));

        #endregion

        #region props

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.AutoUpdateEnabled" />
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Measured"/>
        public event Action<IGeoMapView<SkiaSharpDrawingContext>>? Measured;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.DesignerMode" />
        bool IGeoMapView<SkiaSharpDrawingContext>.DesignerMode => DesignerProperties.GetIsInDesignMode(this);

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.SyncContext" />
        public object SyncContext
        {
            get => GetValue(SyncContextProperty);
            set => SetValue(SyncContextProperty, value);
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Canvas"/>
        public MotionCanvas<SkiaSharpDrawingContext> Canvas =>
            Template.FindName("canvas", this) is not MotionCanvas canvas
            ? throw new Exception(
                $"{nameof(MotionCanvas)} not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
                $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'")
            : canvas.CanvasCore;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ActiveMap"/>
        public GeoJsonFile ActiveMap
        {
            get => (GeoJsonFile)GetValue(ActiveMapProperty);
            set => SetValue(ActiveMapProperty, value);
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Width"/>
        float IGeoMapView<SkiaSharpDrawingContext>.Width => (float)ActualWidth;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Height"/>
        float IGeoMapView<SkiaSharpDrawingContext>.Height => (float)ActualHeight;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.MapProjection"/>
        public MapProjection MapProjection
        {
            get => (MapProjection)GetValue(MapProjectionProperty);
            set => SetValue(MapProjectionProperty, value);
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.HeatMap"/>
        public LvcColor[] HeatMap
        {
            get => (LvcColor[])GetValue(HeatMapProperty);
            set => SetValue(HeatMapProperty, value);
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ColorStops"/>
        public double[]? ColorStops
        {
            get => (double[])GetValue(ColorStopsProperty);
            set => SetValue(ColorStopsProperty, value);
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Stroke"/>
        public IPaint<SkiaSharpDrawingContext>? Stroke
        {
            get => (IPaint<SkiaSharpDrawingContext>)GetValue(StrokeProperty);
            set
            {
                if (value is not null) value.IsStroke = true;
                SetValue(StrokeProperty, value);
            }
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Fill"/>
        public IPaint<SkiaSharpDrawingContext>? Fill
        {
            get => (IPaint<SkiaSharpDrawingContext>)GetValue(FillProperty);
            set
            {
                if (value is not null) value.IsFill = true;
                SetValue(FillProperty, value);
            }
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Shapes"/>
        public IEnumerable<IMapShape> Shapes
        {
            get => (IEnumerable<IMapShape>)GetValue(ShapesProperty);
            set => SetValue(ShapesProperty, value);
        }

        #endregion

        void IGeoMapView<SkiaSharpDrawingContext>.InvokeOnUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        private void GeoMap_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _core.Update();
        }

        private readonly SolidColorPaint _defaultPaint = new();

        private void Measure()
        {
            if (Template is null) return;

            _core.Update();
            Measured?.Invoke(this);
        }

        private static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var chart = (GeoMap)o;
            chart._core.Update();
        }
    }
}
