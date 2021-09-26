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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Geo;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <inheritdoc cref="IGeoMapView{TDrawingContext}"/>
    public partial class GeoMap : UserControl, IGeoMapView<SkiaSharpDrawingContext>
    {
        private readonly CollectionDeepObserver<IMapElement> _shapesObserver;
        private readonly GeoMap<SkiaSharpDrawingContext> _core;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoMap"/> class.
        /// </summary>
        public GeoMap()
        {
            InitializeComponent();
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);
            _core = new GeoMap<SkiaSharpDrawingContext>(this);
            _shapesObserver = new CollectionDeepObserver<IMapElement>(
                (object? sender, NotifyCollectionChangedEventArgs e) => _core?.Update(),
                (object? sender, PropertyChangedEventArgs e) => _core?.Update(),
                true);

            PointerWheelChanged += OnPointerWheelChanged;
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerLeave += OnPointerLeave;

            //Shapes = Enumerable.Empty<MapShape<SkiaSharpDrawingContext>>();
            ActiveMap = Maps.GetWorldMap();
            SyncContext = new object();
        }

        #region dependency props

        /// <summary>
        /// The active map property.
        /// </summary>
        public static readonly AvaloniaProperty<GeoJsonFile> ActiveMapProperty =
           AvaloniaProperty.Register<CartesianChart, GeoJsonFile>(nameof(ActiveMap), null, inherits: true);

        /// <summary>
        /// The active map property.
        /// </summary>
        public static readonly AvaloniaProperty<object> SyncContextProperty =
           AvaloniaProperty.Register<CartesianChart, object>(nameof(SyncContext), null, inherits: true);

        /// <summary>
        /// The projection property.
        /// </summary>
        public static readonly AvaloniaProperty<MapProjection> MapProjectionProperty =
           AvaloniaProperty.Register<CartesianChart, MapProjection>(nameof(MapProjection), MapProjection.Default, inherits: true);

        /// <summary>
        /// The heat map property.
        /// </summary>
        public static readonly AvaloniaProperty<LvcColor[]> HeatMapProperty =
          AvaloniaProperty.Register<CartesianChart, LvcColor[]>(nameof(HeatMap),
              new[]
              {
                  LvcColor.FromArgb(255, 179, 229, 252), // cold (min value)
                  LvcColor.FromArgb(255, 2, 136, 209) // hot (max value)
              }, inherits: true);

        /// <summary>
        /// The color stops property.
        /// </summary>
        public static readonly AvaloniaProperty<double[]?> ColorStopsProperty =
          AvaloniaProperty.Register<CartesianChart, double[]?>(nameof(ColorStops), null, inherits: true);

        /// <summary>
        /// The shapes property.
        /// </summary>
        public static readonly AvaloniaProperty<IEnumerable<IMapElement>> ShapesProperty =
          AvaloniaProperty.Register<CartesianChart, IEnumerable<IMapElement>>(nameof(Shapes),
              Enumerable.Empty<IMapElement>(), inherits: true);

        /// <summary>
        /// The stroke property.
        /// </summary>
        public static readonly AvaloniaProperty<IPaint<SkiaSharpDrawingContext>> StrokeProperty =
          AvaloniaProperty.Register<CartesianChart, IPaint<SkiaSharpDrawingContext>>(nameof(Stroke),
              new SolidColorPaint(new SKColor(255, 224, 224, 224)) { IsStroke = true }, inherits: true);

        /// <summary>
        /// The fill color property.
        /// </summary>
        public static readonly AvaloniaProperty<IPaint<SkiaSharpDrawingContext>> FillProperty =
          AvaloniaProperty.Register<CartesianChart, IPaint<SkiaSharpDrawingContext>>(nameof(Fill),
               new SolidColorPaint(new SKColor(255, 250, 250, 250)) { IsFill = true }, inherits: true);

        #endregion

        #region props

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.AutoUpdateEnabled" />
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.DesignerMode" />
        bool IGeoMapView<SkiaSharpDrawingContext>.DesignerMode => Design.IsDesignMode;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.SyncContext" />
        public object SyncContext
        {
            get => GetValue(SyncContextProperty);
            set => SetValue(SyncContextProperty, value);
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Canvas"/>
        public MotionCanvas<SkiaSharpDrawingContext> Canvas
        {
            get
            {
                var canvas = this.FindControl<MotionCanvas>("canvas");
                return canvas.CanvasCore;
            }
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.ActiveMap"/>
        public GeoJsonFile ActiveMap
        {
            get => (GeoJsonFile)GetValue(ActiveMapProperty);
            set => SetValue(ActiveMapProperty, value);
        }

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Width"/>
        float IGeoMapView<SkiaSharpDrawingContext>.Width => (float)Bounds.Width;

        /// <inheritdoc cref="IGeoMapView{TDrawingContext}.Height"/>
        float IGeoMapView<SkiaSharpDrawingContext>.Height => (float)Bounds.Height;

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
        public IEnumerable<IMapElement> Shapes
        {
            get => (IEnumerable<IMapElement>)GetValue(ShapesProperty);
            set => SetValue(ShapesProperty, value);
        }

        #endregion

        void IGeoMapView<SkiaSharpDrawingContext>.InvokeOnUIThread(Action action)
        {
            _ = Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Normal);//.GetAwaiter().GetResult();
        }

        /// <inheritdoc cref="OnPropertyChanged{T}(AvaloniaPropertyChangedEventArgs{T})"/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(Shapes))
            {
                _shapesObserver.Dispose((IEnumerable<IMapElement>)change.OldValue.Value);
                _shapesObserver.Initialize((IEnumerable<IMapElement>)change.NewValue.Value);
            }

            _core?.Update();
        }

        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (_core is null) return;

            var p = e.GetPosition(this);

            _core.Zoom(new LvcPoint((float)p.X, (float)p.Y), e.Delta.Y > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            var p = e.GetPosition(this);
            foreach (var w in desktop.Windows) w.PointerReleased += OnWindowPointerReleased;
            _core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y));
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var p = e.GetPosition(this);
            _core?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
        }

        private void OnWindowPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            foreach (var w in desktop.Windows) w.PointerReleased -= OnWindowPointerReleased;
            var p = e.GetPosition(this);
            _core?.InvokePointerUp(new LvcPoint((float)p.X, (float)p.Y));
        }

        private void OnPointerLeave(object? sender, PointerEventArgs e)
        {
            _core?.InvokePointerLeft();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
