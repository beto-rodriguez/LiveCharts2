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
    /// <inheritdoc cref="IPieChartView{TDrawingContext}" />
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PieChart : ContentView, IPieChartView<SkiaSharpDrawingContext>, IMobileChart
    {
        #region fields

        /// <summary>
        /// The core
        /// </summary>
        protected Chart<SkiaSharpDrawingContext>? core;
        private readonly CollectionDeepObserver<ISeries> seriesObserver;
        private Grid? grid;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PieChart"/> class.
        /// </summary>
        /// <exception cref="Exception">Default colors are not valid</exception>
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

            canvas.SkCanvasView.EnableTouchEvents = true;
            canvas.SkCanvasView.Touch += OnSkCanvasTouched;
        }

        #region bindable properties

        /// <summary>
        /// The series property
        /// </summary>
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

        /// <summary>
        /// The draw margin property
        /// </summary>
        public static readonly BindableProperty DrawMarginProperty =
            BindableProperty.Create(
                nameof(DrawMargin), typeof(Margin), typeof(CartesianChart), null, BindingMode.Default, null, OnBindablePropertyChanged);

        /// <summary>
        /// The animations speed property
        /// </summary>
        public static readonly BindableProperty AnimationsSpeedProperty =
          BindableProperty.Create(
              nameof(AnimationsSpeed), typeof(TimeSpan), typeof(PieChart), LiveCharts.CurrentSettings.DefaultAnimationsSpeed);

        /// <summary>
        /// The easing function property
        /// </summary>
        public static readonly BindableProperty EasingFunctionProperty =
            BindableProperty.Create(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(PieChart), LiveCharts.CurrentSettings.DefaultEasingFunction);

        /// <summary>
        /// The legend position property
        /// </summary>
        public static readonly BindableProperty LegendPositionProperty =
            BindableProperty.Create(
                nameof(LegendPosition), typeof(LegendPosition), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultLegendPosition, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The legend orientation property
        /// </summary>
        public static readonly BindableProperty LegendOrientationProperty =
            BindableProperty.Create(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultLegendOrientation, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The legend template property
        /// </summary>
        public static readonly BindableProperty LegendTemplateProperty =
            BindableProperty.Create(
                nameof(LegendTemplate), typeof(DataTemplate), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The legend font family property
        /// </summary>
        public static readonly BindableProperty LegendFontFamilyProperty =
            BindableProperty.Create(
                nameof(LegendFontFamily), typeof(string), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The legend font size property
        /// </summary>
        public static readonly BindableProperty LegendFontSizeProperty =
            BindableProperty.Create(
                nameof(LegendFontSize), typeof(double), typeof(CartesianChart), 13d, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The legend text color property
        /// </summary>
        public static readonly BindableProperty LegendTextColorProperty =
            BindableProperty.Create(
                nameof(LegendTextColor), typeof(c), typeof(CartesianChart),
                new c(35 / 255d, 35 / 255d, 35 / 255d), propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The legend background property
        /// </summary>
        public static readonly BindableProperty LegendBackgroundProperty =
            BindableProperty.Create(
                nameof(LegendTextColor), typeof(c), typeof(CartesianChart),
                new c(250 / 255d, 250 / 255d, 250 / 255d), propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The legend font attributes property
        /// </summary>
        public static readonly BindableProperty LegendFontAttributesProperty =
            BindableProperty.Create(
                nameof(LegendFontAttributes), typeof(FontAttributes), typeof(CartesianChart),
                FontAttributes.None, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The tool tip position property;
        /// </summary>
        public static readonly BindableProperty TooltipPositionProperty =
           BindableProperty.Create(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(CartesianChart),
               LiveCharts.CurrentSettings.DefaultTooltipPosition, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The tool tip finding strategy property
        /// </summary>
        public static readonly BindableProperty TooltipFindingStrategyProperty =
            BindableProperty.Create(
                nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(CartesianChart),
                LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy);

        /// <summary>
        /// The tool tip template property
        /// </summary>
        public static readonly BindableProperty TooltipTemplateProperty =
            BindableProperty.Create(
                nameof(TooltipTemplate), typeof(DataTemplate), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The tool tip font family property
        /// </summary>
        public static readonly BindableProperty TooltipFontFamilyProperty =
            BindableProperty.Create(
                nameof(TooltipFontFamily), typeof(string), typeof(CartesianChart), null, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The tool tip font size property
        /// </summary>
        public static readonly BindableProperty TooltipFontSizeProperty =
            BindableProperty.Create(
                nameof(TooltipFontSize), typeof(double), typeof(CartesianChart), 13d, propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The tool tip text color property
        /// </summary>
        public static readonly BindableProperty TooltipTextColorProperty =
            BindableProperty.Create(
                nameof(TooltipTextColor), typeof(c), typeof(CartesianChart),
                new c(35 / 255d, 35 / 255d, 35 / 255d), propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The tool tip background property
        /// </summary>
        public static readonly BindableProperty TooltipBackgroundProperty =
            BindableProperty.Create(
                nameof(TooltipTextColor), typeof(c), typeof(CartesianChart),
                new c(250 / 255d, 250 / 255d, 250 / 255d), propertyChanged: OnBindablePropertyChanged);

        /// <summary>
        /// The tool tip font attributes property
        /// </summary>
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

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => canvas.CanvasCore;

        BindableObject IMobileChart.Canvas => canvas;

        BindableObject IMobileChart.Legend => legend;

        Grid IMobileChart.LayoutGrid => grid ??= this.FindByName<Grid>("gridLayout");

        /// <inheritdoc cref="IChartView.DrawMargin" />
        public Margin? DrawMargin
        {
            get { return (Margin)GetValue(DrawMarginProperty); }
            set { SetValue(DrawMarginProperty, value); }
        }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.Series" />
        public IEnumerable<ISeries> Series
        {
            get { return (IEnumerable<ISeries>)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        public TimeSpan AnimationsSpeed
        {
            get { return (TimeSpan)GetValue(AnimationsSpeedProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        /// <inheritdoc cref="IChartView.EasingFunction" />
        public Func<float, float> EasingFunction
        {
            get { return (Func<float, float>)GetValue(EasingFunctionProperty); }
            set { SetValue(AnimationsSpeedProperty, value); }
        }

        /// <inheritdoc cref="IChartView.LegendPosition" />
        public LegendPosition LegendPosition
        {
            get { return (LegendPosition)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        /// <inheritdoc cref="IChartView.LegendOrientation" />
        public LegendOrientation LegendOrientation
        {
            get { return (LegendOrientation)GetValue(LegendOrientationProperty); }
            set { SetValue(LegendOrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the legend template.
        /// </summary>
        /// <value>
        /// The legend template.
        /// </value>
        public DataTemplate LegendTemplate
        {
            get { return (DataTemplate)GetValue(LegendTemplateProperty); }
            set { SetValue(LegendTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the legend font family.
        /// </summary>
        /// <value>
        /// The legend font family.
        /// </value>
        public string LegendFontFamily
        {
            get { return (string)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the legend font.
        /// </summary>
        /// <value>
        /// The size of the legend font.
        /// </value>
        public double LegendFontSize
        {
            get { return (double)GetValue(LegendFontSizeProperty); }
            set { SetValue(LegendFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the legend text.
        /// </summary>
        /// <value>
        /// The color of the legend text.
        /// </value>
        public c LegendTextColor
        {
            get { return (c)GetValue(LegendTextColorProperty); }
            set { SetValue(LegendTextColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the legend background.
        /// </summary>
        /// <value>
        /// The color of the legend background.
        /// </value>
        public c LegendBackgroundColor
        {
            get { return (c)GetValue(LegendBackgroundProperty); }
            set { SetValue(LegendBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the legend font attributes.
        /// </summary>
        /// <value>
        /// The legend font attributes.
        /// </value>
        public FontAttributes LegendFontAttributes
        {
            get { return (FontAttributes)GetValue(LegendFontAttributesProperty); }
            set { SetValue(LegendFontAttributesProperty, value); }
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
        public IChartLegend<SkiaSharpDrawingContext>? Legend => null;

        /// <inheritdoc cref="IChartView.TooltipPosition" />
        public TooltipPosition TooltipPosition
        {
            get { return (TooltipPosition)GetValue(TooltipPositionProperty); }
            set { SetValue(TooltipPositionProperty, value); }
        }

        /// <inheritdoc cref="IChartView.TooltipFindingStrategy" />
        public TooltipFindingStrategy TooltipFindingStrategy
        {
            get { return (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty); }
            set { SetValue(TooltipFindingStrategyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>
        /// The tool tip template.
        /// </value>
        public DataTemplate TooltipTemplate
        {
            get { return (DataTemplate)GetValue(TooltipTemplateProperty); }
            set { SetValue(TooltipTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tool tip font family.
        /// </summary>
        /// <value>
        /// The tool tip font family.
        /// </value>
        public string TooltipFontFamily
        {
            get { return (string)GetValue(TooltipFontFamilyProperty); }
            set { SetValue(TooltipFontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the tool tip font.
        /// </summary>
        /// <value>
        /// The size of the tool tip font.
        /// </value>
        public double TooltipFontSize
        {
            get { return (double)GetValue(TooltipFontSizeProperty); }
            set { SetValue(TooltipFontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the tool tip text.
        /// </summary>
        /// <value>
        /// The color of the tool tip text.
        /// </value>
        public c TooltipTextColor
        {
            get { return (c)GetValue(TooltipTextColorProperty); }
            set { SetValue(TooltipTextColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the color of the tool tip background.
        /// </summary>
        /// <value>
        /// The color of the tool tip background.
        /// </value>
        public c TooltipBackgroundColor
        {
            get { return (c)GetValue(TooltipBackgroundProperty); }
            set { SetValue(TooltipBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the tool tip font attributes.
        /// </summary>
        /// <value>
        /// The tool tip font attributes.
        /// </value>
        public FontAttributes TooltipFontAttributes
        {
            get { return (FontAttributes)GetValue(TooltipFontAttributesProperty); }
            set { SetValue(TooltipFontAttributesProperty, value); }
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        /// <inheritdoc cref="IChartView{TDrawingContext}.PointStates" />
        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        #endregion

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <returns></returns>
        protected void InitializeCore()
        {
            core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
            MainThread.BeginInvokeOnMainThread(() => core.Update());
        }

        /// <summary>
        /// Called when a bindable property changes.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns></returns>
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

        private void OnSkCanvasTouched(object? sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            if (TooltipPosition == TooltipPosition.Hidden || core == null) return;
            var location = new PointF(e.Location.X, e.Location.Y);
            ((IChartTooltip<SkiaSharpDrawingContext>)tooltip).Show(core.FindPointsNearTo(location), core);
        }
    }
}