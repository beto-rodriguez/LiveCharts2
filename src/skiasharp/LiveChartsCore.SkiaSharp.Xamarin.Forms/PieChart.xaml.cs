using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using LiveChartsCore.Rx;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiveChartsCore.SkiaSharpView.Xamarin.Forms
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PieChart : ContentView, IPieChartView<SkiaSharpDrawingContext>
    {
        protected Chart<SkiaSharpDrawingContext> core;
        //protected MotionCanvas motionCanvas;
        protected IChartLegend<SkiaSharpDrawingContext> legend;
        protected IChartTooltip<SkiaSharpDrawingContext> tooltip;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new PointF();

        public PieChart()
        {
            InitializeComponent();
        }

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core => (PieChart<SkiaSharpDrawingContext>)core;

        public static readonly BindableProperty SeriesProperty =
          BindableProperty.Create(
              nameof(Series), typeof(IEnumerable<IPieSeries<SkiaSharpDrawingContext>>), typeof(CartesianChart),
              new List<IPieSeries<SkiaSharpDrawingContext>>(), BindingMode.Default, null);

        public IEnumerable<IPieSeries<SkiaSharpDrawingContext>> Series
        {
            get { return (IEnumerable<IPieSeries<SkiaSharpDrawingContext>>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        SizeF IChartView.ControlSize
        {
            get
            {
                var i = DeviceDisplay.MainDisplayInfo;
                return new SizeF
                {
                    Width = (float)i.Width,
                    Height = (float)i.Height
                };
            }
        }

        public Canvas<SkiaSharpDrawingContext> CoreCanvas => canvas.CanvasCore;

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

            core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSK.DefaultPlatformBuilder, canvas.CanvasCore);
            //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
            //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
            core.Update();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            // show tooltip ??
            core.Update();
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            core.Update();
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }
}