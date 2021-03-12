using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveChartsCore.AvaloniaView.Drawing;
using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.Rx;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LiveChartsCore.AvaloniaView
{
    public class CartesianChart : UserControl, ICartesianChartView<AvaloniaDrawingContext>
    {
        protected Chart<AvaloniaDrawingContext> core;
        //protected MotionCanvas motionCanvas;
        protected IChartLegend<AvaloniaDrawingContext> legend;
        protected IChartTooltip<AvaloniaDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();

        public CartesianChart()
        {
            InitializeComponent();

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsAvalonia.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<AvaloniaDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            InitializeCore();

            mouseMoveThrottler = new ActionThrottler(TimeSpan.FromMilliseconds(10));
            mouseMoveThrottler.Unlocked += MouseMoveThrottlerUnlocked;
        }

        CartesianChart<AvaloniaDrawingContext> ICartesianChartView<AvaloniaDrawingContext>.Core => (CartesianChart<AvaloniaDrawingContext>)core;

        public static readonly AvaloniaProperty<IEnumerable<ICartesianSeries<AvaloniaDrawingContext>>> SeriesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<ICartesianSeries<AvaloniaDrawingContext>>>(
                nameof(Series), new List<ICartesianSeries<AvaloniaDrawingContext>>(), inherits: true);

        public static readonly AvaloniaProperty<IEnumerable<IAxis<AvaloniaDrawingContext>>> XAxesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<IAxis<AvaloniaDrawingContext>>>(
                nameof(XAxes), new List<IAxis<AvaloniaDrawingContext>>(), inherits: true);

        public static readonly AvaloniaProperty<IEnumerable<IAxis<AvaloniaDrawingContext>>> YAxesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<IAxis<AvaloniaDrawingContext>>>(
                nameof(YAxes),  new List<IAxis<AvaloniaDrawingContext>>(), inherits: true);

        public IEnumerable<ICartesianSeries<AvaloniaDrawingContext>> Series
        {
            get { return (IEnumerable<ICartesianSeries<AvaloniaDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        public IEnumerable<IAxis<AvaloniaDrawingContext>> XAxes
        {
            get { return (IEnumerable<IAxis<AvaloniaDrawingContext>>)GetValue(XAxesProperty); }
            set { SetValue(XAxesProperty, value); }
        }

        public IEnumerable<IAxis<AvaloniaDrawingContext>> YAxes
        {
            get { return (IEnumerable<IAxis<AvaloniaDrawingContext>>)GetValue(YAxesProperty); }
            set { SetValue(YAxesProperty, value); }
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

        public MotionCanvas<AvaloniaDrawingContext> CoreCanvas => core.Canvas;

        public LegendPosition LegendPosition { get; set; }

        public LegendOrientation LegendOrientation { get; set; }

        public IChartLegend<AvaloniaDrawingContext> Legend => null;

        public Margin DrawMargin { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

        public TooltipPosition TooltipPosition { get; set; }

        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

        public IChartTooltip<AvaloniaDrawingContext> Tooltip => null;

        public PointStatesDictionary<AvaloniaDrawingContext> PointStates { get; set; }

        protected void InitializeCore()
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");
            core = new CartesianChart<AvaloniaDrawingContext>(this, LiveChartsAvalonia.DefaultPlatformBuilder, canvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            core.Update();
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

            // is this how the size event is handled?
            // https://github.com/AvaloniaUI/Avalonia/issues/3237

            if (change.Property.Name != nameof(Bounds)) return;
            core.Update();
        }
    }
}
