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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using LiveChartsCore.Kernel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Collections.ObjectModel;
using Avalonia.Media;
using A = Avalonia;
using Avalonia.Input;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Templates;
using System.Linq;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.SkiaSharpView.Avalonia
{
    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}" />
    public class CartesianChart : UserControl, ICartesianChartView<SkiaSharpDrawingContext>, IAvaloniaChart
    {
        #region fields

        /// <summary>
        /// The core
        /// </summary>
        protected Chart<SkiaSharpDrawingContext>? core;

        /// <summary>
        /// The legend
        /// </summary>
        protected IChartLegend<SkiaSharpDrawingContext>? legend;

        /// <summary>
        /// The tool tip
        /// </summary>
        protected IChartTooltip<SkiaSharpDrawingContext>? tooltip;

        private MotionCanvas? _avaloniaCanvas;
        private readonly CollectionDeepObserver<ISeries> _seriesObserver;
        private readonly CollectionDeepObserver<IAxis> _xObserver;
        private readonly CollectionDeepObserver<IAxis> _yObserver;
        private readonly CollectionDeepObserver<Section<SkiaSharpDrawingContext>> _sectionsObserver;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChart"/> class.
        /// </summary>
        /// <exception cref="Exception">Default colors are not valid</exception>
        public CartesianChart()
        {
            InitializeComponent();

            // workaround to detect mouse events.
            // Avalonia do not seem to detect pointer events if background is not set.
            ((IChartView)this).BackColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);

            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            InitializeCore();

            _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _xObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _yObserver = new CollectionDeepObserver<IAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _sectionsObserver = new CollectionDeepObserver<Section<SkiaSharpDrawingContext>>(
                OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            XAxes = new List<IAxis>() { LiveCharts.CurrentSettings.AxisProvider() };
            YAxes = new List<IAxis>() { LiveCharts.CurrentSettings.AxisProvider() };
            Series = new ObservableCollection<ISeries>();

            PointerWheelChanged += CartesianChart_PointerWheelChanged;
            PointerPressed += CartesianChart_PointerPressed;
            PointerMoved += CartesianChart_PointerMoved;

            PointerLeave += CartesianChart_PointerLeave;
        }

        #region avalonia/dependency properties

        /// <summary>
        /// The draw margin property
        /// </summary>
        public static readonly AvaloniaProperty<Margin?> DrawMarginProperty =
           AvaloniaProperty.Register<CartesianChart, Margin?>(nameof(DrawMargin), null, inherits: true);

        /// <summary>
        /// The series property
        /// </summary>
        public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
           AvaloniaProperty.Register<CartesianChart, IEnumerable<ISeries>>(nameof(Series), Enumerable.Empty<ISeries>(), inherits: true);

        /// <summary>
        /// The x axes property
        /// </summary>
        public static readonly AvaloniaProperty<IEnumerable<IAxis>> XAxesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<IAxis>>(nameof(XAxes), Enumerable.Empty<IAxis>(), inherits: true);

        /// <summary>
        /// The y axes property
        /// </summary>
        public static readonly AvaloniaProperty<IEnumerable<IAxis>> YAxesProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<IAxis>>(nameof(YAxes), Enumerable.Empty<IAxis>(), inherits: true);

        /// <summary>
        /// The sections property
        /// </summary>
        public static readonly AvaloniaProperty<IEnumerable<Section<SkiaSharpDrawingContext>>> SectionsProperty =
            AvaloniaProperty.Register<CartesianChart, IEnumerable<Section<SkiaSharpDrawingContext>>>(
                nameof(Sections), Enumerable.Empty<Section<SkiaSharpDrawingContext>>(), inherits: true);

        /// <summary>
        /// The draw margin frame property
        /// </summary>
        public static readonly AvaloniaProperty<DrawMarginFrame<SkiaSharpDrawingContext>?> DrawMarginFrameProperty =
            AvaloniaProperty.Register<CartesianChart, DrawMarginFrame<SkiaSharpDrawingContext>?>(
                nameof(DrawMarginFrame), null, inherits: true);

        /// <summary>
        /// The zoom mode property
        /// </summary>
        public static readonly AvaloniaProperty<ZoomAndPanMode> ZoomModeProperty =
            AvaloniaProperty.Register<CartesianChart, ZoomAndPanMode>(
                nameof(ZoomMode), LiveCharts.CurrentSettings.DefaultZoomMode, inherits: true);

        /// <summary>
        /// The zooming speed property
        /// </summary>
        public static readonly AvaloniaProperty<double> ZoomingSpeedProperty =
            AvaloniaProperty.Register<CartesianChart, double>(
                nameof(ZoomingSpeed), LiveCharts.CurrentSettings.DefaultZoomSpeed, inherits: true);

        /// <summary>
        /// The animations speed property
        /// </summary>
        public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
            AvaloniaProperty.Register<CartesianChart, TimeSpan>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultAnimationsSpeed, inherits: true);

        /// <summary>
        /// The easing function property
        /// </summary>
        public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
            AvaloniaProperty.Register<CartesianChart, Func<float, float>>(
                nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultEasingFunction, inherits: true);

        /// <summary>
        /// The tool tip template property
        /// </summary>
        public static readonly AvaloniaProperty<DataTemplate?> TooltipTemplateProperty =
            AvaloniaProperty.Register<CartesianChart, DataTemplate?>(nameof(TooltipTemplate), null, inherits: true);

        /// <summary>
        /// The tool tip position property
        /// </summary>
        public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipPosition>(
                nameof(TooltipPosition), LiveCharts.CurrentSettings.DefaultTooltipPosition, inherits: true);

        /// <summary>
        /// The tool tip finding strategy property
        /// </summary>
        public static readonly AvaloniaProperty<TooltipFindingStrategy> TooltipFindingStrategyProperty =
            AvaloniaProperty.Register<CartesianChart, TooltipFindingStrategy>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy, inherits: true);

        /// <summary>
        /// The tool tip font family property
        /// </summary>
        public static readonly AvaloniaProperty<A.Media.FontFamily> TooltipFontFamilyProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontFamily>(
                nameof(TooltipFontFamily), new A.Media.FontFamily("Arial"), inherits: true);

        /// <summary>
        /// The tool tip font size property
        /// </summary>
        public static readonly AvaloniaProperty<double> TooltipFontSizeProperty =
            AvaloniaProperty.Register<CartesianChart, double>(nameof(TooltipFontSize), 13d, inherits: true);

        /// <summary>
        /// The tool tip font weight property
        /// </summary>
        public static readonly AvaloniaProperty<FontWeight> TooltipFontWeightProperty =
            AvaloniaProperty.Register<CartesianChart, FontWeight>(nameof(TooltipFontWeight), FontWeight.Normal, inherits: true);

        /// <summary>
        /// The tool tip font style property
        /// </summary>
        public static readonly AvaloniaProperty<A.Media.FontStyle> TooltipFontStyleProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontStyle>(
                nameof(TooltipFontStyle), A.Media.FontStyle.Normal, inherits: true);

        /// <summary>
        /// The tool tip text brush property
        /// </summary>
        public static readonly AvaloniaProperty<SolidColorBrush> TooltipTextBrushProperty =
            AvaloniaProperty.Register<CartesianChart, SolidColorBrush>(
                nameof(TooltipTextBrush), new SolidColorBrush(new A.Media.Color(255, 35, 35, 35)), inherits: true);

        /// <summary>
        /// The tool tip background property
        /// </summary>
        public static readonly AvaloniaProperty<IBrush> TooltipBackgroundProperty =
            AvaloniaProperty.Register<CartesianChart, IBrush>(nameof(TooltipBackground),
                new SolidColorBrush(new A.Media.Color(255, 250, 250, 250)), inherits: true);

        /// <summary>
        /// The legend position property
        /// </summary>
        public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
            AvaloniaProperty.Register<CartesianChart, LegendPosition>(
                nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultLegendPosition, inherits: true);

        /// <summary>
        /// The legend orientation property
        /// </summary>
        public static readonly AvaloniaProperty<LegendOrientation> LegendOrientationProperty =
            AvaloniaProperty.Register<CartesianChart, LegendOrientation>(
                nameof(LegendOrientation), LiveCharts.CurrentSettings.DefaultLegendOrientation, inherits: true);

        /// <summary>
        /// The legend template property
        /// </summary>
        public static readonly AvaloniaProperty<DataTemplate?> LegendTemplateProperty =
            AvaloniaProperty.Register<CartesianChart, DataTemplate?>(nameof(LegendTemplate), null, inherits: true);

        /// <summary>
        /// The legend font family property
        /// </summary>
        public static readonly AvaloniaProperty<A.Media.FontFamily> LegendFontFamilyProperty =
           AvaloniaProperty.Register<CartesianChart, A.Media.FontFamily>(
               nameof(LegendFontFamily), new A.Media.FontFamily("Arial"), inherits: true);

        /// <summary>
        /// The legend font size property
        /// </summary>
        public static readonly AvaloniaProperty<double> LegendFontSizeProperty =
            AvaloniaProperty.Register<CartesianChart, double>(nameof(LegendFontSize), 13d, inherits: true);

        /// <summary>
        /// The legend font weight property
        /// </summary>
        public static readonly AvaloniaProperty<FontWeight> LegendFontWeightProperty =
            AvaloniaProperty.Register<CartesianChart, FontWeight>(nameof(LegendFontWeight), FontWeight.Normal, inherits: true);

        /// <summary>
        /// The legend font style property
        /// </summary>
        public static readonly AvaloniaProperty<A.Media.FontStyle> LegendFontStyleProperty =
            AvaloniaProperty.Register<CartesianChart, A.Media.FontStyle>(
                nameof(LegendFontStyle), A.Media.FontStyle.Normal, inherits: true);

        /// <summary>
        /// The legend text brush property
        /// </summary>
        public static readonly AvaloniaProperty<SolidColorBrush> LegendTextBrushProperty =
            AvaloniaProperty.Register<CartesianChart, SolidColorBrush>(
                nameof(LegendTextBrush), new SolidColorBrush(new A.Media.Color(255, 35, 35, 35)), inherits: true);

        /// <summary>
        /// The legend background property
        /// </summary>
        public static readonly AvaloniaProperty<IBrush> LegendBackgroundProperty =
            AvaloniaProperty.Register<CartesianChart, IBrush>(nameof(LegendBackground),
                new SolidColorBrush(new A.Media.Color(255, 250, 250, 250)), inherits: true);

        #endregion

        #region properties

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

        #endregion

        #region properties

        /// <inheritdoc cref="IChartView.CoreChart" />
        public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => core is null ? throw new Exception("core not found") : core.Canvas;

        CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core =>
            core is null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)core;

        System.Drawing.Color IChartView.BackColor
        {
            get => Background is not ISolidColorBrush b
                    ? new System.Drawing.Color()
                    : System.Drawing.Color.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
            set
            {
                Background = new SolidColorBrush(new A.Media.Color(value.R, value.G, value.B, value.A));
                var canvas = this.FindControl<MotionCanvas>("canvas");
                canvas.BackColor = new SkiaSharp.SKColor(value.R, value.G, value.B, value.A);
            }
        }

        SizeF IChartView.ControlSize => _avaloniaCanvas is null
                    ? new()
                    : new()
                    {
                        Width = (float)_avaloniaCanvas.Bounds.Width,
                        Height = (float)_avaloniaCanvas.Bounds.Height
                    };

        /// <inheritdoc cref="IChartView.DrawMargin" />
        public Margin? DrawMargin
        {
            get => (Margin?)GetValue(DrawMarginProperty);
            set => SetValue(DrawMarginProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series" />
        public IEnumerable<ISeries> Series
        {
            get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes" />
        public IEnumerable<IAxis> XAxes
        {
            get => (IEnumerable<IAxis>)GetValue(XAxesProperty);
            set => SetValue(XAxesProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes" />
        public IEnumerable<IAxis> YAxes
        {
            get => (IEnumerable<IAxis>)GetValue(YAxesProperty);
            set => SetValue(YAxesProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Sections" />
        public IEnumerable<Section<SkiaSharpDrawingContext>> Sections
        {
            get => (IEnumerable<Section<SkiaSharpDrawingContext>>)GetValue(SectionsProperty);
            set => SetValue(SectionsProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.DrawMarginFrame" />
        public DrawMarginFrame<SkiaSharpDrawingContext>? DrawMarginFrame
        {
            get => (DrawMarginFrame<SkiaSharpDrawingContext>)GetValue(DrawMarginFrameProperty);
            set => SetValue(DrawMarginFrameProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomMode" />
        public ZoomAndPanMode ZoomMode
        {
            get => (ZoomAndPanMode)GetValue(ZoomModeProperty);
            set => SetValue(ZoomModeProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ZoomingSpeed" />
        public double ZoomingSpeed
        {
            get => (double)GetValue(ZoomingSpeedProperty);
            set => SetValue(ZoomingSpeedProperty, value);
        }

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        public TimeSpan AnimationsSpeed
        {
            get => (TimeSpan)GetValue(AnimationsSpeedProperty);
            set => SetValue(AnimationsSpeedProperty, value);
        }

        /// <inheritdoc cref="IChartView.EasingFunction" />
        public Func<float, float>? EasingFunction
        {
            get => (Func<float, float>)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        /// <inheritdoc cref="IChartView.TooltipPosition" />
        public TooltipPosition TooltipPosition
        {
            get => (TooltipPosition)GetValue(TooltipPositionProperty);
            set => SetValue(TooltipPositionProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.TooltipFindingStrategy" />
        public TooltipFindingStrategy TooltipFindingStrategy
        {
            get => (TooltipFindingStrategy)GetValue(TooltipFindingStrategyProperty);
            set => SetValue(TooltipFindingStrategyProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip data template.
        /// </summary>
        /// <value>
        /// The tool tip template.
        /// </value>
        public DataTemplate TooltipTemplate
        {
            get => (DataTemplate)GetValue(TooltipTemplateProperty);
            set => SetValue(TooltipTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip default font family.
        /// </summary>
        /// <value>
        /// The tool tip font family.
        /// </value>
        public A.Media.FontFamily TooltipFontFamily
        {
            get => (A.Media.FontFamily)GetValue(TooltipFontFamilyProperty);
            set => SetValue(TooltipFontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the default size of the tool tip font.
        /// </summary>
        /// <value>
        /// The size of the tool tip font.
        /// </value>
        public double TooltipFontSize
        {
            get => (double)GetValue(TooltipFontSizeProperty);
            set => SetValue(TooltipFontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip default font weight.
        /// </summary>
        /// <value>
        /// The tool tip font weight.
        /// </value>
        public FontWeight TooltipFontWeight
        {
            get => (FontWeight)GetValue(TooltipFontWeightProperty);
            set => SetValue(TooltipFontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip default font style.
        /// </summary>
        /// <value>
        /// The tool tip font style.
        /// </value>
        public A.Media.FontStyle TooltipFontStyle
        {
            get => (A.Media.FontStyle)GetValue(TooltipFontStyleProperty);
            set => SetValue(TooltipFontStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip default text brush.
        /// </summary>
        /// <value>
        /// The tool tip text brush.
        /// </value>
        public SolidColorBrush TooltipTextBrush
        {
            get => (SolidColorBrush)GetValue(TooltipTextBrushProperty);
            set => SetValue(TooltipTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip default background.
        /// </summary>
        /// <value>
        /// The tool tip background.
        /// </value>
        public IBrush TooltipBackground
        {
            get => (IBrush)GetValue(TooltipBackgroundProperty);
            set => SetValue(TooltipBackgroundProperty, value);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        /// <inheritdoc cref="IChartView.LegendPosition" />
        public LegendPosition LegendPosition
        {
            get => (LegendPosition)GetValue(LegendPositionProperty);
            set => SetValue(LegendPositionProperty, value);
        }

        /// <inheritdoc cref="IChartView.LegendOrientation" />
        public LegendOrientation LegendOrientation
        {
            get => (LegendOrientation)GetValue(LegendOrientationProperty);
            set => SetValue(LegendOrientationProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend template.
        /// </summary>
        /// <value>
        /// The legend template.
        /// </value>
        public DataTemplate LegendTemplate
        {
            get => (DataTemplate)GetValue(LegendTemplateProperty);
            set => SetValue(LegendTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend default font family.
        /// </summary>
        /// <value>
        /// The legend font family.
        /// </value>
        public A.Media.FontFamily LegendFontFamily
        {
            get => (A.Media.FontFamily)GetValue(LegendFontFamilyProperty);
            set => SetValue(LegendFontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the size of the legend default font.
        /// </summary>
        /// <value>
        /// The size of the legend font.
        /// </value>
        public double LegendFontSize
        {
            get => (double)GetValue(LegendFontSizeProperty);
            set => SetValue(LegendFontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend default font weight.
        /// </summary>
        /// <value>
        /// The legend font weight.
        /// </value>
        public FontWeight LegendFontWeight
        {
            get => (FontWeight)GetValue(LegendFontWeightProperty);
            set => SetValue(LegendFontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend default font style.
        /// </summary>
        /// <value>
        /// The legend font style.
        /// </value>
        public A.Media.FontStyle LegendFontStyle
        {
            get => (A.Media.FontStyle)GetValue(LegendFontStyleProperty);
            set => SetValue(LegendFontStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend default text brush.
        /// </summary>
        /// <value>
        /// The legend text brush.
        /// </value>
        public SolidColorBrush LegendTextBrush
        {
            get => (SolidColorBrush)GetValue(LegendTextBrushProperty);
            set => SetValue(LegendTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend default background.
        /// </summary>
        /// <value>
        /// The legend background.
        /// </value>
        public IBrush LegendBackground
        {
            get => (IBrush)GetValue(LegendBackgroundProperty);
            set => SetValue(LegendBackgroundProperty, value);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
        public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

        /// <inheritdoc cref="IChartView{TDrawingContext}.PointStates" />
        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <inheritdoc cref="IChartView.UpdaterThrottler" />
        public TimeSpan UpdaterThrottler
        {
            get => core?.UpdaterThrottler ?? throw new Exception("core not set yet.");
            set
            {
                if (core is null) throw new Exception("core not set yet.");
                core.UpdaterThrottler = value;
            }
        }

        #endregion

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(PointF, int, int)" />
        public double[] ScaleUIPoint(PointF point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            if (core is null) throw new Exception("core not found");
            var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)core;
            return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{TooltipPoint})"/>
        public void ShowTooltip(IEnumerable<TooltipPoint> points)
        {
            if (tooltip is null || core is null) return;

            tooltip.Show(points, core);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
        public void HideTooltip()
        {
            if (tooltip is null || core is null) return;

            foreach (var state in PointStates.GetStates())
            {
                if (!state.IsHoverState) continue;
                if (state.Fill is not null) state.Fill.ClearGeometriesFromPaintTask(core.Canvas);
                if (state.Stroke is not null) state.Stroke.ClearGeometriesFromPaintTask(core.Canvas);
            }

            tooltip.Hide();
        }

        /// <inheritdoc cref="IChartView.SetTooltipStyle(System.Drawing.Color, System.Drawing.Color)"/>
        public void SetTooltipStyle(System.Drawing.Color background, System.Drawing.Color textColor)
        {
            TooltipBackground = new SolidColorBrush(new A.Media.Color(background.A, background.R, background.G, background.B));
            TooltipTextBrush = new SolidColorBrush(new A.Media.Color(textColor.A, textColor.R, textColor.G, textColor.B));
        }

        /// <summary>
        /// Initializes the core.
        /// </summary>
        /// <returns></returns>
        protected void InitializeCore()
        {
            var canvas = this.FindControl<MotionCanvas>("canvas");
            _avaloniaCanvas = canvas;
            core = new CartesianChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);

            core.Measuring += OnCoreMeasuring;
            core.UpdateStarted += OnCoreUpdateStarted;
            core.UpdateFinished += OnCoreUpdateFinished;

            legend = this.FindControl<DefaultLegend>("legend");
            tooltip = this.FindControl<DefaultTooltip>("tooltip");

            core.Update();
        }

        /// <inheritdoc cref="OnPropertyChanged{T}(AvaloniaPropertyChangedEventArgs{T})" />
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (core is null || change.Property.Name == nameof(IsPointerOver)) return;

            if (change.Property.Name == nameof(Series))
            {
                _seriesObserver.Dispose((IEnumerable<ISeries>)change.OldValue.Value);
                _seriesObserver.Initialize((IEnumerable<ISeries>)change.NewValue.Value);
            }

            if (change.Property.Name == nameof(XAxes))
            {
                _xObserver.Dispose((IEnumerable<IAxis>)change.OldValue.Value);
                _xObserver.Initialize((IEnumerable<IAxis>)change.NewValue.Value);
            }

            if (change.Property.Name == nameof(YAxes))
            {
                _yObserver.Dispose((IEnumerable<IAxis>)change.OldValue.Value);
                _yObserver.Initialize((IEnumerable<IAxis>)change.NewValue.Value);
            }

            if (change.Property.Name == nameof(Sections))
            {
                _sectionsObserver.Dispose((IEnumerable<Section<SkiaSharpDrawingContext>>)change.OldValue.Value);
                _sectionsObserver.Initialize((IEnumerable<Section<SkiaSharpDrawingContext>>)change.NewValue.Value);
            }

            if (change.Property.Name == nameof(Background))
            {
                var canvas = this.FindControl<MotionCanvas>("canvas");
                var color = Background is not ISolidColorBrush b
                    ? new System.Drawing.Color()
                    : System.Drawing.Color.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
                canvas.BackColor = new SkiaSharp.SKColor(color.R, color.G, color.B, color.A);
            }

            core.Update();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (core is null) return;

            core.Update();
        }

        private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (core is null) return;

            core.Update();
        }

        private void CartesianChart_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            if (core is null) return;

            var c = (CartesianChart<SkiaSharpDrawingContext>)core;
            var p = e.GetPosition(this);

            c.Zoom(new PointF((float)p.X, (float)p.Y), e.Delta.Y > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
        }

        private void CartesianChart_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // only IClassicDesktopStyleApplicationLifetime supported for now.
            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            var p = e.GetPosition(this);
            foreach (var w in desktop.Windows) w.PointerReleased += Window_PointerReleased;
            core?.InvokePointerDown(new PointF((float)p.X, (float)p.Y));
        }

        private void CartesianChart_PointerMoved(object? sender, PointerEventArgs e)
        {
            var p = e.GetPosition(this);
            core?.InvokePointerMove(new PointF((float)p.X, (float)p.Y));
        }

        private void Window_PointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            foreach (var w in desktop.Windows) w.PointerReleased -= Window_PointerReleased;
            var p = e.GetPosition(this);
            core?.InvokePointerUp(new PointF((float)p.X, (float)p.Y));
        }

        private void OnCoreUpdateFinished(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateFinished?.Invoke(this);
        }

        private void OnCoreUpdateStarted(IChartView<SkiaSharpDrawingContext> chart)
        {
            UpdateStarted?.Invoke(this);
        }

        private void OnCoreMeasuring(IChartView<SkiaSharpDrawingContext> chart)
        {
            Measuring?.Invoke(this);
        }

        private void CartesianChart_PointerLeave(object? sender, PointerEventArgs e)
        {
            _ = Dispatcher.UIThread.InvokeAsync(HideTooltip, DispatcherPriority.Background);
            core?.InvokePointerLeft();
        }
    }
}
