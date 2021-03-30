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

namespace LiveChartsCore.SkiaSharp.Avalonia
{
    public class CartesianChart : UserControl, ICartesianChartView<SkiaSharpDrawingContext>
    {
        protected Chart<SkiaSharpDrawingContext> core;
        //protected MotionCanvas motionCanvas;
        protected IChartLegend<SkiaSharpDrawingContext> legend;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();
        private CollectionDeepObserver<ISeries> seriesObserver;

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

            seriesObserver = new CollectionDeepObserver<ISeries>(
                (object? sender, NotifyCollectionChangedEventArgs e) =>
                {
                    if (core == null) return;
                    Dispatcher.UIThread.InvokeAsync(core.Update, DispatcherPriority.Background);
                },
                (object? sender, PropertyChangedEventArgs e) =>
                {
                    if (core == null) return;
                    Dispatcher.UIThread.InvokeAsync(core.Update, DispatcherPriority.Background);
                },
                true);

            XAxes = new List<IAxis>() { new Axis() };
            YAxes = new List<IAxis>() { new Axis() };
            Series = new ObservableCollection<ISeries>();
        }

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core => (CartesianChart<SkiaSharpDrawingContext>)core;

        public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<ISeries>>(nameof(Series), null, inherits: true);

        public static readonly AvaloniaProperty<IEnumerable<IAxis>> XAxesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<IAxis>>(nameof(XAxes), null, inherits: true);

        public static readonly AvaloniaProperty<IEnumerable<IAxis>> YAxesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<IAxis>>(nameof(YAxes), null, inherits: true);

        public static readonly AvaloniaProperty<ZoomMode> ZoomModeProperty =
            AvaloniaProperty.Register<CartesianChart, ZoomMode>(nameof(ZoomMode), ZoomMode.Both, inherits: true);

        public static readonly AvaloniaProperty<double> ZoomingSpeedProperty =
            AvaloniaProperty.Register<CartesianChart, double>(nameof(ZoomingSpeed), 0.5d, inherits: true);

        public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
            AvaloniaProperty.Register<CartesianChart, TimeSpan>(nameof(AnimationsSpeed), TimeSpan.FromMilliseconds(500), inherits: true);

        public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
            AvaloniaProperty.Register<CartesianChart, Func<float, float>>(nameof(AnimationsSpeed), EasingFunctions.SinOut, inherits: true);

        public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
            AvaloniaProperty.Register<CartesianChart, LegendPosition>(nameof(LegendPosition), LegendPosition.Hidden, inherits: true);

        public static readonly AvaloniaProperty<LegendOrientation> LegendOrientationProperty =
            AvaloniaProperty.Register<CartesianChart, LegendOrientation>(nameof(LegendOrientation), LegendOrientation.Auto, inherits: true);

        public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipPosition>(nameof(TooltipPosition), TooltipPosition.Hidden, inherits: true);

        public static readonly AvaloniaProperty<TooltipFindingStrategy> TooltipFindingStrategyProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipFindingStrategy>(
                nameof(LegendPosition), TooltipFindingStrategy.CompareOnlyX, inherits: true);

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

        public ZoomMode ZoomMode
        {
            get { return (ZoomMode)GetValue(ZoomModeProperty); }
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
            //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            Dispatcher.UIThread.InvokeAsync(core.Update, DispatcherPriority.Background);
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
                Dispatcher.UIThread.InvokeAsync(core.Update, DispatcherPriority.Background);
                return;
            }

            if (change.Property.Name == nameof(XAxes) || change.Property.Name == nameof(YAxes))
            {
                Dispatcher.UIThread.InvokeAsync(core.Update, DispatcherPriority.Background);
            }

            // is this how the size event is handled?
            // https://github.com/AvaloniaUI/Avalonia/issues/3237
            if (change.Property.Name != nameof(Bounds)) return;
            Dispatcher.UIThread.InvokeAsync(core.Update, DispatcherPriority.Background);
        }
    }
}
