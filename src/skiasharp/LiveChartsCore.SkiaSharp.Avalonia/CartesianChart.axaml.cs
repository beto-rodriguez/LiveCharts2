 // The MIT License(MIT)

// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Collections.ObjectModel;
using Avalonia.Media;
using a = Avalonia;
using Avalonia.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Templates;

namespace LiveChartsCore.SkiaSharp.Avalonia
{
    public class CartesianChart : UserControl, ICartesianChartView<SkiaSharpDrawingContext>, IAvaloniaChart
    {
        protected Chart<SkiaSharpDrawingContext> core;
        //protected MotionCanvas motionCanvas;
        protected IChartLegend<SkiaSharpDrawingContext> legend;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();
        private readonly CollectionDeepObserver<ISeries> seriesObserver;
        private readonly CollectionDeepObserver<IAxis> xObserver;
        private readonly CollectionDeepObserver<IAxis> yObserver;
        private readonly ActionThrottler panningThrottler;
        private a.Point? previous;
        private a.Point? current;
        private bool isPanning = false;

        public CartesianChart()
        {
            InitializeComponent();

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            InitializeCore();

            mouseMoveThrottler = new ActionThrottler(MouseMoveThrottlerUnlocked, TimeSpan.FromMilliseconds(10));

            seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            xObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            yObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            XAxes = new List<IAxis>() { new Axis() };
            YAxes = new List<IAxis>() { new Axis() };
            Series = new ObservableCollection<ISeries>();

            // workaround to deteck mouse events.
            // avalonia do not seem to detect events if brackground is not set.
            Background = new SolidColorBrush(Colors.Transparent);

            PointerWheelChanged += CartesianChart_PointerWheelChanged;
            PointerPressed += CartesianChart_PointerPressed;
            PointerMoved += CartesianChart_PointerMoved;

            panningThrottler = new ActionThrottler(DoPan, TimeSpan.FromMilliseconds(30));
        }

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core => (CartesianChart<SkiaSharpDrawingContext>)core;

        public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<ISeries>>(nameof(Series), null, inherits: true);

        public static readonly AvaloniaProperty<IEnumerable<IAxis>> XAxesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<IAxis>>(nameof(XAxes), null, inherits: true);

        public static readonly AvaloniaProperty<IEnumerable<IAxis>> YAxesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<IAxis>>(nameof(YAxes), null, inherits: true);

        public static readonly AvaloniaProperty<ZoomAndPanMode> ZoomModeProperty =
            AvaloniaProperty.Register<CartesianChart, ZoomAndPanMode>(
                nameof(ZoomMode), LiveCharts.CurrentSettings.DefaultZoomMode, inherits: true);

        public static readonly AvaloniaProperty<double> ZoomingSpeedProperty =
            AvaloniaProperty.Register<CartesianChart, double>(
                nameof(ZoomingSpeed), LiveCharts.CurrentSettings.DefaultZoomSpeed, inherits: true);

        public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
            AvaloniaProperty.Register<CartesianChart, TimeSpan>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultAnimationsSpeed, inherits: true);

        public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
            AvaloniaProperty.Register<CartesianChart, Func<float, float>>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultEasingFunction, inherits: true);

        public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
            AvaloniaProperty.Register<CartesianChart, LegendPosition>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultLegendPosition, inherits: true);

        public static readonly AvaloniaProperty<LegendOrientation> LegendOrientationProperty =
            AvaloniaProperty.Register<CartesianChart, LegendOrientation>(
                nameof(LegendOrientation), LiveCharts.CurrentSettings.DefaultLegendOrientation, inherits: true);

        public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipPosition>(
                nameof(TooltipPosition), LiveCharts.CurrentSettings.DefaultTooltipPosition, inherits: true);

        public static readonly AvaloniaProperty<TooltipFindingStrategy> TooltipFindingStrategyProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipFindingStrategy>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy, inherits: true);

        public static readonly AvaloniaProperty<DataTemplate> TooltipTemplateProperty =
           AvaloniaProperty.Register<CartesianChart, DataTemplate>(nameof(TooltipTemplate), null, inherits: true);

        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public IEnumerable<IAxis> XAxes
        {
            get { return (IEnumerable<IAxis>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        public IEnumerable<IAxis> YAxes
        {
            get { return (IEnumerable<IAxis>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
        }

        public ZoomAndPanMode ZoomMode
        {
            get { return (ZoomAndPanMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }

        public double ZoomingSpeed
        {
            get { return (double)GetValue(ZoomingSpeedProperty); }
            set { SetValue(ZoomingSpeedProperty, value); }
        }

        public TimeSpan AnimationsSpeed
        {
            get { return (TimeSpan)GetValue(AnimationsSpeedProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public Func<float, float> EasingFunction
        {
            get { return (Func<float, float>)GetValue(EasingFunctionProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        public LegendOrientation LegendOrientation
        {
            get { return (LegendOrientation)GetValue(LegendOrientationProperty); }
            set { SetValue(LegendOrientationProperty, value); }
        }

        public TooltipPosition TooltipPosition
        {
            get { return (TooltipPosition)GetValue(TooltipPositionProperty); }
            set { SetValue(TooltipPositionProperty, value); }
        }

        public TooltipFindingStrategy TooltipFindingStrategy
        {
            get { return (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty); }
            set { SetValue(TooltipFindingStrategyProperty, value); }
        }

        public DataTemplate TooltipTemplate
        {
            get { return (DataTemplate)GetValue(TooltipTemplateProperty); }
            set { SetValue(TooltipTemplateProperty, value); }
        }

        SizeF IChartView.ControlSize
        {
            get
            {
                //Measure(new Avalonia.Size(double.PositiveInfinity, double.PositiveInfinity));
                return new SizeF
                {
                    Width = (float)Bounds.Width,
                    Height = (float)Bounds.Height
                };
            }
        }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => core.Canvas;

        public IChartLegend<SkiaSharpDrawingContext> Legend => null;

        public Margin DrawMargin { get; set; }

        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => null;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; }
        public a.Media.FontFamily TooltipFontFamily { get; set; }
        public double TooltipFontSize { get; set; } = 14;
        public FontWeight TooltipFontWeight { get; set; } = FontWeight.Normal;
        public a.Media.FontStyle TooltipFontStyle { get; set; } = a.Media.FontStyle.Normal;
        public SolidColorBrush TooltipTextBrush { get; set; } = new SolidColorBrush(new a.Media.Color(255, 30, 30, 30));

        public PointF ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
            return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

        protected void InitializeCore()
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");
            core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            tooltip = this.FindControl<DefaultTooltip>("tooltip");
            Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(Series))
            {
                seriesObserver.Dispose((IEnumerable<ISeries>)change.OldValue.Value);
                seriesObserver.Initialize((IEnumerable<ISeries>)change.NewValue.Value);
            }

            if (change.Property.Name == nameof(XAxes))
            {
                xObserver.Dispose((IEnumerable<IAxis>)change.OldValue.Value);
                xObserver.Initialize((IEnumerable<IAxis>)change.NewValue.Value);
            }

            if (change.Property.Name == nameof(YAxes))
            {
                yObserver.Dispose((IEnumerable<IAxis>)change.OldValue.Value);
                yObserver.Initialize((IEnumerable<IAxis>)change.NewValue.Value);
            }

            // is this how the size event is handled?
            // https://github.com/AvaloniaUI/Avalonia/issues/3237
            if (change.Property.Name != nameof(Bounds)) return;

            Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (core == null) return;

            Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (core == null) return;

            Dispatcher.UIThread.InvokeAsync(() => core.Update(), DispatcherPriority.Background);
        }

        private void CartesianChart_PointerWheelChanged(object? sender, global::Avalonia.Input.PointerWheelEventArgs e)
        {
            var c = (CartesianChart<SkiaSharpDrawingContext>)core;
            var p = e.GetPosition(this);

            c.Zoom(new PointF((float)p.X, (float)p.Y), e.Delta.Y > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
        }

        private void CartesianChart_PointerPressed(object? sender, global::Avalonia.Input.PointerPressedEventArgs e)
        {
            // only IClassicDesktopStyleApplicationLifetime supported for now.
            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

            isPanning = true;
            previous = e.GetPosition(this);

            foreach (var w in desktop.Windows) w.PointerReleased += Window_PointerReleased;
        }

        private void CartesianChart_PointerMoved(object? sender, global::Avalonia.Input.PointerEventArgs e)
        {
            var p = e.GetPosition(this);
            mousePosition = new PointF((float)p.X, (float)p.Y);
            mouseMoveThrottler.Call();

            if (!isPanning || previous == null) return;

            current = e.GetPosition(this);
            panningThrottler.Call();
        }

        private void Window_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;

            if (!isPanning) return;
            isPanning = false;
            previous = null;

            foreach (var w in desktop.Windows) w.PointerReleased -= Window_PointerReleased;
        }

        private void DoPan()
        {
            if (previous == null || current == null) return;

            var c = (CartesianChart<SkiaSharpDrawingContext>)core;

            c.Pan(
                new PointF(
                (float)(current.Value.X - previous.Value.X),
                (float)(current.Value.Y - previous.Value.Y)));

            previous = new a.Point(current.Value.X, current.Value.Y);
        }
    }
}
