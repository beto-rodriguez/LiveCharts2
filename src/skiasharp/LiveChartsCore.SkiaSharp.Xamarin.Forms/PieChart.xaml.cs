using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using c = Xamarin.Forms.Color;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PieChart : ContentView, IPieChartView<SkiaSharpDrawingContext>, IMobileChart
    {
        private CollectionDeepObserver<ISeries> seriesObserver;
        protected Chart<SkiaSharpDrawingContext> core;
        //protected MotionCanvas motionCanvas;
        protected IChartLegend<SkiaSharpDrawingContext> legend;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();

        public PieChart()
        {
            InitializeComponent();

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetStylesBuilder<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetInitializer();
            if (stylesBuilder.CurrentColors == null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ConstructChart(this);

            InitializeCore();
            SizeChanged += OnSizeChanged;
            mouseMoveThrottler = new ActionThrottler(MouseMoveThrottlerUnlocked, TimeSpan.FromMilliseconds(10));

            seriesObserver = new CollectionDeepObserver<ISeries>(
               (object sender, NotifyCollectionChangedEventArgs e) =>
               {
                   if (core == null) return;
                   MainThread.BeginInvokeOnMainThread(() => core.Update());
               },
               (object sender, PropertyChangedEventArgs e) =>
               {
                   if (core == null) return;
                   MainThread.BeginInvokeOnMainThread(() => core.Update());
               });

            Series = new ObservableCollection<ISeries>();
        }

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core => (PieChart<SkiaSharpDrawingContext>)core;

        public static readonly BindableProperty SeriesProperty =
          BindableProperty.Create(
              nameof(Series), typeof(IEnumerable<ISeries>), typeof(PieChart), new ObservableCollection<ISeries>(), BindingMode.Default, null,
              (BindableObject o, object oldValue, object newValue) =>
              {
                  var chart = (PieChart)o;
                  var seriesObserver = chart.seriesObserver;
                  seriesObserver.Dispose((IEnumerable<ISeries>)oldValue);
                  seriesObserver.Initialize((IEnumerable<ISeries>)newValue);
                  if (chart.core == null) return;
                  MainThread.BeginInvokeOnMainThread(() => chart.core.Update());
              });

        public static readonly BindableProperty AnimationsSpeedProperty =
          BindableProperty.Create(
              nameof(AnimationsSpeed), typeof(TimeSpan), typeof(PieChart), LiveCharts.CurrentSettings.DefaultAnimationsSpeed);

        public static readonly BindableProperty EasingFunctionProperty =
            BindableProperty.Create(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(PieChart), LiveCharts.CurrentSettings.DefaultEasingFunction);

        public static readonly BindableProperty LegendPositionProperty =
            BindableProperty.Create(
                nameof(LegendPosition), typeof(LegendPosition), typeof(PieChart), LiveCharts.CurrentSettings.DefaultLegendPosition);

        public static readonly BindableProperty LegendOrientationProperty =
            BindableProperty.Create(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(PieChart),
                LiveCharts.CurrentSettings.DefaultLegendOrientation);

        public static readonly BindableProperty TooltipPositionProperty =
           BindableProperty.Create(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(PieChart), LiveCharts.CurrentSettings.DefaultTooltipPosition);

        public static readonly BindableProperty TooltipFindingStrategyProperty =
            BindableProperty.Create(
                nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(PieChart),
                LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy);

        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
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
                return new SizeF
                {
                    Width = (float)(Width * DeviceDisplay.MainDisplayInfo.Density),
                    Height = (float)(Height * DeviceDisplay.MainDisplayInfo.Density)
                };
            }
        }

        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => canvas.CanvasCore;

        public IChartLegend<SkiaSharpDrawingContext> Legend => null;

        public Margin DrawMargin { get; set; }

        public DataTemplate TooltipTemplate { get; set; }

        public string TooltipFontFamily { get; set; } = null;

        public double TooltipFontSize { get; set; } = 12;

        public c TooltipTextColor { get; set; } = new c(35, 35, 35);

        public FontAttributes TooltipFontAttributes { get; set; }

        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => null;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; }

        protected void InitializeCore()
        {
            //if (!(FindByName("canvas") is MotionCanvas canvas))
            //    throw new Exception(
            //        $"SkiaElement not found. This was probably caused because the control {nameof(CartesianChart)} template was overridden, " +
            //        $"If you override the template please add an {nameof(MotionCanvas)} to the template and name it 'canvas'");

            core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            // show tooltip ??
            // () => core.Update()();
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }
}