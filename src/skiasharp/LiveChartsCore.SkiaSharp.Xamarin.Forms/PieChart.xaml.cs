using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PieChart : ContentView, IPieChartView<SkiaSharpDrawingContext>
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
            mouseMoveThrottler = new ActionThrottler(TimeSpan.FromMilliseconds(10));
            mouseMoveThrottler.Unlocked += MouseMoveThrottlerUnlocked;

            seriesObserver = new CollectionDeepObserver<ISeries>(
               (object sender, NotifyCollectionChangedEventArgs e) =>
               {
                   if (core == null) return;
                   MainThread.BeginInvokeOnMainThread(core.Update);
               },
               (object sender, PropertyChangedEventArgs e) =>
               {
                   if (core == null) return;
                   MainThread.BeginInvokeOnMainThread(core.Update);
               });
        }

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core => (PieChart<SkiaSharpDrawingContext>)core;

        public static readonly BindableProperty SeriesProperty =
          BindableProperty.Create(
              nameof(Series), typeof(IEnumerable<ISeries>), typeof(CartesianChart), new List<ISeries>(), BindingMode.Default, null,
              (BindableObject o, object oldValue, object newValue) =>
              {
                  var seriesObserver = ((PieChart)o).seriesObserver;
                  seriesObserver.Dispose((IEnumerable<ISeries>)oldValue);
                  seriesObserver.Initialize((IEnumerable<ISeries>)newValue);
              });

        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
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

        public LegendPosition LegendPosition { get; set; }

        public LegendOrientation LegendOrientation { get; set; }

        public IChartLegend<SkiaSharpDrawingContext> Legend => null;

        public Margin DrawMargin { get; set; }

        public TimeSpan AnimationsSpeed { get; set; } = TimeSpan.FromMilliseconds(500);

        public Func<float, float> EasingFunction { get; set; } = EasingFunctions.Lineal;

        public TooltipPosition TooltipPosition { get; set; }

        public TooltipFindingStrategy TooltipFindingStrategy { get; set; }

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
            MainThread.BeginInvokeOnMainThread(core.Update);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            // show tooltip ??
            // core.Update();
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(core.Update);
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }
}