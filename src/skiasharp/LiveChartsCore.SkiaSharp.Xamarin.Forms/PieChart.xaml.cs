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
        #region fields

        private readonly CollectionDeepObserver<ISeries> seriesObserver;
        protected Chart<SkiaSharpDrawingContext>? core;
        private readonly ActionThrottler mouseMoveThrottler;
        private PointF mousePosition = new();

        #endregion

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

        #region bindable properties

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

        public static readonly BindableProperty DrawMarginProperty =
            BindableProperty.Create(
                nameof(DrawMargin), typeof(Margin), typeof(CartesianChart), null, BindingMode.Default, null, OnBindablePropertyChanged);

        public static readonly BindableProperty AnimationsSpeedProperty =
          BindableProperty.Create(
              nameof(AnimationsSpeed), typeof(TimeSpan), typeof(PieChart), LiveCharts.CurrentSettings.DefaultAnimationsSpeed);

        public static readonly BindableProperty EasingFunctionProperty =
            BindableProperty.Create(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(PieChart), LiveCharts.CurrentSettings.DefaultEasingFunction);

        public static readonly BindableProperty LegendPositionProperty =
            BindableProperty.Create(
                nameof(LegendPosition), typeof(LegendPosition), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultLegendPosition, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendOrientationProperty =
            BindableProperty.Create(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultLegendOrientation, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendTemplateProperty =
            BindableProperty.Create(
                nameof(LegendTemplate), typeof(DataTemplate), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendFontFamilyProperty =
            BindableProperty.Create(
                nameof(LegendFontFamily), typeof(string), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendFontSizeProperty =
            BindableProperty.Create(
                nameof(LegendFontSize), typeof(double), typeof(CartesianChart), 13d, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendTextColorProperty =
            BindableProperty.Create(
                nameof(LegendTextColor), typeof(c), typeof(CartesianChart),
                new c(35 / 255d, 35 / 255d, 35 / 255d), propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendBackgroundProperty =
            BindableProperty.Create(
                nameof(LegendTextColor), typeof(c), typeof(CartesianChart),
                new c(250 / 255d, 250 / 255d, 250 / 255d), propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty LegendFontAttributesProperty =
            BindableProperty.Create(
                nameof(LegendFontAttributes), typeof(FontAttributes), typeof(CartesianChart),
                FontAttributes.None, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipPositionProperty =
           BindableProperty.Create(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(CartesianChart),
               LiveCharts.CurrentSettings.DefaultTooltipPosition, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipFindingStrategyProperty =
            BindableProperty.Create(
                nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy);

        public static readonly BindableProperty TooltipTemplateProperty =
            BindableProperty.Create(
                nameof(TooltipTemplate), typeof(DataTemplate), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipFontFamilyProperty =
            BindableProperty.Create(
                nameof(TooltipFontFamily), typeof(string), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipFontSizeProperty =
            BindableProperty.Create(
                nameof(TooltipFontSize), typeof(double), typeof(CartesianChart), 13d, propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipTextColorProperty =
            BindableProperty.Create(
                nameof(TooltipTextColor), typeof(c), typeof(CartesianChart),
                new c(35 / 255d, 35 / 255d, 35 / 255d), propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipBackgroundProperty =
            BindableProperty.Create(
                nameof(TooltipTextColor), typeof(c), typeof(CartesianChart),
                new c(250 / 255d, 250 / 255d, 250 / 255d), propertyChanged: OnBindablePropertyChanged);

        public static readonly BindableProperty TooltipFontAttributesProperty =
            BindableProperty.Create(
                nameof(TooltipFontAttributes), typeof(FontAttributes), typeof(CartesianChart),
                FontAttributes.None, propertyChanged: OnBindablePropertyChanged);

        #endregion

        #region properties

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core
        {
            get
            {
                if (core == null || core == null) throw new Exception("core not found");
                return (PieChart<SkiaSharpDrawingContext>)core;
            }
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

        public Margin? DrawMargin
        {
            get { return (Margin)GetValue(DrawMarginProperty); }
            set { SetValue(DrawMarginProperty, value); }
        }

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

        public DataTemplate LegendTemplate
        {
            get { return (DataTemplate)GetValue(LegendTemplateProperty); }
            set { SetValue(LegendTemplateProperty, value); }
        }

        public string LegendFontFamily
        {
            get { return (string)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
        }

        public double LegendFontSize
        {
            get { return (double)GetValue(LegendFontSizeProperty); }
            set { SetValue(LegendFontSizeProperty, value); }
        }

        public c LegendTextColor
        {
            get { return (c)GetValue(LegendTextColorProperty); }
            set { SetValue(LegendTextColorProperty, value); }
        }

        public c LegendBackgroundColor
        {
            get { return (c)GetValue(LegendBackgroundProperty); }
            set { SetValue(LegendBackgroundProperty, value); }
        }

        public FontAttributes LegendFontAttributes
        {
            get { return (FontAttributes)GetValue(LegendFontAttributesProperty); }
            set { SetValue(LegendFontAttributesProperty, value); }
        }

        public IChartLegend<SkiaSharpDrawingContext>? Legend => null;

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

        public string TooltipFontFamily
        {
            get { return (string)GetValue(TooltipFontFamilyProperty); }
            set { SetValue(TooltipFontFamilyProperty, value); }
        }

        public double TooltipFontSize
        {
            get { return (double)GetValue(TooltipFontSizeProperty); }
            set { SetValue(TooltipFontSizeProperty, value); }
        }

        public c TooltipTextColor
        {
            get { return (c)GetValue(TooltipTextColorProperty); }
            set { SetValue(TooltipTextColorProperty, value); }
        }

        public c TooltipBackgroundColor
        {
            get { return (c)GetValue(TooltipBackgroundProperty); }
            set { SetValue(TooltipBackgroundProperty, value); }
        }

        public FontAttributes TooltipFontAttributes
        {
            get { return (FontAttributes)GetValue(TooltipFontAttributesProperty); }
            set { SetValue(TooltipFontAttributesProperty, value); }
        }

        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        #endregion

        protected void InitializeCore()
        {
            core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        protected static void OnBindablePropertyChanged(BindableObject o, object oldValue, object newValue)
        {
            var chart = (PieChart)o;
            if (chart.core == null) return;
            MainThread.BeginInvokeOnMainThread(() => chart.core.Update());
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            if (core == null) return;
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        private void MouseMoveThrottlerUnlocked()
        {
            if (TooltipPosition == TooltipPosition.Hidden || core == null) return;
            tooltip.Show(core.FindPointsNearTo(mousePosition), core);
        }
    }
}