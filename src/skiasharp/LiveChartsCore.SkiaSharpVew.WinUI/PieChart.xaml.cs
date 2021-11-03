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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Text;

namespace LiveChartsCore.SkiaSharpView.WinUI
{
    /// <inheritdoc cref="IChartView{TDrawingContext}" />
    public sealed partial class PieChart : UserControl, IPieChartView<SkiaSharpDrawingContext>, IWinUIChart
    {
        private Chart<SkiaSharpDrawingContext>? _core;
        private MotionCanvas? _canvas;
        private readonly CollectionDeepObserver<ISeries> _seriesObserver;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartesianChart"/> class.
        /// </summary>
        public PieChart()
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            InitializeComponent();

            _seriesObserver = new CollectionDeepObserver<ISeries>(
                (object? sender, NotifyCollectionChangedEventArgs e) =>
                {
                    if (_core == null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
                    _core.Update();
                },
                (object? sender, PropertyChangedEventArgs e) =>
                {
                    if (_core == null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
                    _core.Update();
                });

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #region dependency properties

        /// <summary>
        /// The series property
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries>), typeof(PieChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (PieChart)o;
                        var seriesObserver = chart._seriesObserver;
                        seriesObserver.Dispose((IEnumerable<ISeries>)args.OldValue);
                        seriesObserver.Initialize((IEnumerable<ISeries>)args.NewValue);
                        if (chart._core == null) return;
                        chart._core.Update();
                    }));

        /// <summary>
        /// The sync context property
        /// </summary>
        public static readonly DependencyProperty SyncContextProperty =
            DependencyProperty.Register(
                nameof(SyncContext), typeof(object), typeof(PieChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (PieChart)o;
                        chart.CoreCanvas.Sync = args.NewValue;
                        if (chart._core == null) return;
                        chart._core.Update();
                    }));

        /// <summary>
        /// The initial rotation property
        /// </summary>
        public static readonly DependencyProperty InitialRotationProperty =
            DependencyProperty.Register(
                nameof(InitialRotation), typeof(double), typeof(PieChart), new PropertyMetadata(0d, OnDependencyPropertyChanged));

        /// <summary>
        /// The maximum angle property
        /// </summary>
        public static readonly DependencyProperty MaxAngleProperty =
            DependencyProperty.Register(
                nameof(MaxAngle), typeof(double), typeof(PieChart), new PropertyMetadata(360d, OnDependencyPropertyChanged));

        /// <summary>
        /// The total property
        /// </summary>
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register(
                nameof(Total), typeof(double?), typeof(PieChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The draw margin property
        /// </summary>
        public static readonly DependencyProperty DrawMarginProperty =
           DependencyProperty.Register(
               nameof(DrawMargin), typeof(Margin), typeof(PieChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The animations speed property
        /// </summary>
        public static readonly DependencyProperty AnimationsSpeedProperty =
            DependencyProperty.Register(
                nameof(AnimationsSpeed), typeof(TimeSpan), typeof(PieChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultAnimationsSpeed, OnDependencyPropertyChanged));

        /// <summary>
        /// The easing function property
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(PieChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultEasingFunction, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend position property
        /// </summary>
        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register(
                nameof(LegendPosition), typeof(LegendPosition), typeof(PieChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendPosition, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend orientation property
        /// </summary>
        public static readonly DependencyProperty LegendOrientationProperty =
            DependencyProperty.Register(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(PieChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendOrientation, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip position property
        /// </summary>
        public static readonly DependencyProperty TooltipPositionProperty =
           DependencyProperty.Register(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(PieChart),
               new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipPosition, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip background property
        /// </summary>
        public static readonly DependencyProperty TooltipBackgroundProperty =
           DependencyProperty.Register(
               nameof(TooltipBackground), typeof(SolidColorBrush), typeof(PieChart),
               new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 250, 250, 250)), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font family property
        /// </summary>
        public static readonly DependencyProperty TooltipFontFamilyProperty =
           DependencyProperty.Register(
               nameof(TooltipFontFamily), typeof(FontFamily), typeof(PieChart),
               new PropertyMetadata(new FontFamily("Trebuchet MS"), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip text color property
        /// </summary>
        public static readonly DependencyProperty TooltipTextBrushProperty =
           DependencyProperty.Register(
               nameof(TooltipTextBrush), typeof(Brush), typeof(PieChart),
               new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 35, 35, 35)), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font size property
        /// </summary>
        public static readonly DependencyProperty TooltipFontSizeProperty =
           DependencyProperty.Register(
               nameof(TooltipFontSize), typeof(double), typeof(PieChart), new PropertyMetadata(13d, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font weight property
        /// </summary>
        public static readonly DependencyProperty TooltipFontWeightProperty =
           DependencyProperty.Register(
               nameof(TooltipFontWeight), typeof(FontWeight), typeof(PieChart),
               new PropertyMetadata(FontWeights.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font stretch property
        /// </summary>
        public static readonly DependencyProperty TooltipFontStretchProperty =
           DependencyProperty.Register(
               nameof(TooltipFontStretch), typeof(FontStretch), typeof(PieChart),
               new PropertyMetadata(FontStretch.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font style property
        /// </summary>
        public static readonly DependencyProperty TooltipFontStyleProperty =
           DependencyProperty.Register(
               nameof(TooltipFontStyle), typeof(FontStyle), typeof(PieChart),
               new PropertyMetadata(FontStyle.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip template property
        /// </summary>
        public static readonly DependencyProperty TooltipTemplateProperty =
            DependencyProperty.Register(
                nameof(TooltipTemplate), typeof(DataTemplate), typeof(PieChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font family property
        /// </summary>
        public static readonly DependencyProperty LegendFontFamilyProperty =
           DependencyProperty.Register(
               nameof(LegendFontFamily), typeof(FontFamily), typeof(PieChart),
               new PropertyMetadata(new FontFamily("Trebuchet MS"), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend text color property
        /// </summary>
        public static readonly DependencyProperty LegendTextBrushProperty =
           DependencyProperty.Register(
               nameof(LegendTextBrush), typeof(Brush), typeof(PieChart),
               new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 35, 35, 35)), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend background property
        /// </summary>
        public static readonly DependencyProperty LegendBackgroundProperty =
           DependencyProperty.Register(
               nameof(LegendBackground), typeof(Brush), typeof(PieChart),
               new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font size property
        /// </summary>
        public static readonly DependencyProperty LegendFontSizeProperty =
           DependencyProperty.Register(
               nameof(LegendFontSize), typeof(double), typeof(PieChart), new PropertyMetadata(13d, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font weight property
        /// </summary>
        public static readonly DependencyProperty LegendFontWeightProperty =
           DependencyProperty.Register(
               nameof(LegendFontWeight), typeof(FontWeight), typeof(PieChart),
               new PropertyMetadata(FontWeights.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font stretch property
        /// </summary>
        public static readonly DependencyProperty LegendFontStretchProperty =
           DependencyProperty.Register(
               nameof(LegendFontStretch), typeof(FontStretch), typeof(PieChart),
               new PropertyMetadata(FontStretch.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font style property
        /// </summary>
        public static readonly DependencyProperty LegendFontStyleProperty =
           DependencyProperty.Register(
               nameof(LegendFontStyle), typeof(FontStyle), typeof(PieChart),
               new PropertyMetadata(FontStyle.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend template property
        /// </summary>
        public static readonly DependencyProperty LegendTemplateProperty =
            DependencyProperty.Register(
                nameof(LegendTemplate), typeof(DataTemplate), typeof(PieChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        #endregion

        #region events

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

        #endregion

        #region properties

        Grid IWinUIChart.LayoutGrid => grid;
        FrameworkElement IWinUIChart.Canvas => motionCanvas;
        FrameworkElement IWinUIChart.Legend => legend;

        /// <inheritdoc cref="IChartView.DesignerMode" />
        bool IChartView.DesignerMode => Windows.ApplicationModel.DesignMode.DesignModeEnabled;

        /// <inheritdoc cref="IChartView.CoreChart" />
        public IChart CoreChart => _core ?? throw new Exception("Core not set yet.");

        PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core
            => _core == null ? throw new Exception("core not found") : (PieChart<SkiaSharpDrawingContext>)_core;

        LvcColor IChartView.BackColor
        {
            get => Background is not SolidColorBrush b
                ? new LvcColor()
                : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
            set => SetValue(BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(value.A, value.R, value.G, value.B)));
        }

        /// <inheritdoc cref="IChartView.SyncContext" />
        public object SyncContext
        {
            get => GetValue(SyncContextProperty);
            set => SetValue(SyncContextProperty, value);
        }

        /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Series" />
        public IEnumerable<ISeries> Series
        {
            get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.InitialRotation" />
        public double InitialRotation
        {
            get => (double)GetValue(InitialRotationProperty);
            set => SetValue(InitialRotationProperty, value);
        }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.MaxAngle" />
        public double MaxAngle
        {
            get => (double)GetValue(MaxAngleProperty);
            set => SetValue(MaxAngleProperty, value);
        }

        /// <inheritdoc cref="IPieChartView{TDrawingContext}.Total" />
        public double? Total
        {
            get => (double?)GetValue(TotalProperty);
            set => SetValue(TotalProperty, value);
        }

        /// <inheritdoc cref="IChartView.DrawMargin" />
        public Margin? DrawMargin
        {
            get => (Margin)GetValue(DrawMarginProperty);
            set => SetValue(DrawMarginProperty, value);
        }

        Margin? IChartView.DrawMargin
        {
            get => DrawMargin;
            set => SetValue(DrawMarginProperty, value);
        }

        LvcSize IChartView.ControlSize => _canvas == null
                    ? throw new Exception("Canvas not found")
                    : (new() { Width = (float)_canvas.ActualWidth, Height = (float)_canvas.ActualHeight });

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => _canvas == null ? throw new Exception("Canvas not found") : _canvas.CanvasCore;

        /// <inheritdoc cref="IChartView.AnimationsSpeed" />
        public TimeSpan AnimationsSpeed
        {
            get => (TimeSpan)GetValue(AnimationsSpeedProperty);
            set => SetValue(AnimationsSpeedProperty, value);
        }

        TimeSpan IChartView.AnimationsSpeed
        {
            get => AnimationsSpeed;
            set => SetValue(AnimationsSpeedProperty, value);
        }

        /// <inheritdoc cref="IChartView.EasingFunction" />
        public Func<float, float> EasingFunction
        {
            get => (Func<float, float>)GetValue(EasingFunctionProperty);
            set => SetValue(EasingFunctionProperty, value);
        }

        Func<float, float>? IChartView.EasingFunction
        {
            get => EasingFunction;
            set => SetValue(EasingFunctionProperty, value);
        }

        /// <inheritdoc cref="IChartView.LegendPosition" />
        public LegendPosition LegendPosition
        {
            get => (LegendPosition)GetValue(LegendPositionProperty);
            set => SetValue(LegendPositionProperty, value);
        }

        LegendPosition IChartView.LegendPosition
        {
            get => LegendPosition;
            set => SetValue(LegendPositionProperty, value);
        }

        /// <inheritdoc cref="IChartView.LegendOrientation" />
        public LegendOrientation LegendOrientation
        {
            get => (LegendOrientation)GetValue(LegendOrientationProperty);
            set => SetValue(LegendOrientationProperty, value);
        }

        LegendOrientation IChartView.LegendOrientation
        {
            get => LegendOrientation;
            set => SetValue(LegendOrientationProperty, value);
        }

        /// <inheritdoc cref="IChartView.TooltipPosition" />
        public TooltipPosition TooltipPosition
        {
            get => (TooltipPosition)GetValue(TooltipPositionProperty);
            set => SetValue(TooltipPositionProperty, value);
        }

        TooltipPosition IChartView.TooltipPosition
        {
            get => TooltipPosition;
            set => SetValue(TooltipPositionProperty, value);
        }

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>
        /// The tool tip template.
        /// </value>
        public DataTemplate? TooltipTemplate
        {
            get => (DataTemplate)GetValue(TooltipTemplateProperty);
            set => SetValue(TooltipTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip background.
        /// </summary>
        /// <value>
        /// The tool tip background.
        /// </value>
        public Brush TooltipBackground
        {
            get => (Brush)GetValue(TooltipBackgroundProperty);
            set => SetValue(TooltipBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip font family.
        /// </summary>
        /// <value>
        /// The tool tip font family.
        /// </value>
        public FontFamily TooltipFontFamily
        {
            get => (FontFamily)GetValue(TooltipFontFamilyProperty);
            set => SetValue(TooltipFontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the default color of the tool tip text.
        /// </summary>
        /// <value>
        /// The color of the tool tip text.
        /// </value>
        public Brush TooltipTextBrush
        {
            get => (SolidColorBrush)GetValue(TooltipTextBrushProperty);
            set => SetValue(TooltipTextBrushProperty, value);
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
        /// Gets or sets the default tool tip font weight.
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
        /// Gets or sets the default tool tip font stretch.
        /// </summary>
        /// <value>
        /// The tool tip font stretch.
        /// </value>
        public FontStretch TooltipFontStretch
        {
            get => (FontStretch)GetValue(TooltipFontStretchProperty);
            set => SetValue(TooltipFontStretchProperty, value);
        }

        /// <summary>
        /// Gets or sets the default tool tip font style.
        /// </summary>
        /// <value>
        /// The tool tip font style.
        /// </value>
        public FontStyle TooltipFontStyle
        {
            get => (FontStyle)GetValue(TooltipFontStyleProperty);
            set => SetValue(TooltipFontStyleProperty, value);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
        public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

        /// <summary>
        /// Gets or sets the legend template.
        /// </summary>
        /// <value>
        /// The legend template.
        /// </value>
        public DataTemplate? LegendTemplate
        {
            get => (DataTemplate)GetValue(LegendTemplateProperty);
            set => SetValue(LegendTemplateProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend font family.
        /// </summary>
        /// <value>
        /// The legend font family.
        /// </value>
        public FontFamily LegendFontFamily
        {
            get => (FontFamily)GetValue(LegendFontFamilyProperty);
            set => SetValue(LegendFontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the default color of the legend text.
        /// </summary>
        /// <value>
        /// The color of the legend text.
        /// </value>
        public Brush LegendTextBrush
        {
            get => (SolidColorBrush)GetValue(LegendTextBrushProperty);
            set => SetValue(LegendTextBrushProperty, value);
        }

        /// <summary>
        /// Gets or sets the legend background.
        /// </summary>
        /// <value>
        /// The legend t background.
        /// </value>
        public Brush LegendBackground
        {
            get => (SolidColorBrush)GetValue(LegendBackgroundProperty);
            set => SetValue(LegendBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the default size of the legend font.
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
        /// Gets or sets the default legend font weight.
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
        /// Gets or sets the default legend font stretch.
        /// </summary>
        /// <value>
        /// The legend font stretch.
        /// </value>
        public FontStretch LegendFontStretch
        {
            get => (FontStretch)GetValue(LegendFontStretchProperty);
            set => SetValue(LegendFontStretchProperty, value);
        }

        /// <summary>
        /// Gets or sets the default legend font style.
        /// </summary>
        /// <value>
        /// The legend font style.
        /// </value>
        public FontStyle LegendFontStyle
        {
            get => (FontStyle)GetValue(LegendFontStyleProperty);
            set => SetValue(LegendFontStyleProperty, value);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
        public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

        /// <summary>
        /// Gets or sets the point states.
        /// </summary>
        public PointStatesDictionary<SkiaSharpDrawingContext> PointStates { get; set; } = new();

        /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
        public bool AutoUpdateEnabled { get; set; } = true;

        /// <inheritdoc cref="IChartView.UpdaterThrottler" />
        public TimeSpan UpdaterThrottler
        {
            get => _core?.UpdaterThrottler ?? throw new Exception("core not set yet.");
            set
            {
                if (_core == null) throw new Exception("core not set yet.");
                _core.UpdaterThrottler = value;
            }
        }

        #endregion

        /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{TooltipPoint})"/>
        public void ShowTooltip(IEnumerable<TooltipPoint> points)
        {
            if (tooltip == null || _core == null) return;

            ((IChartTooltip<SkiaSharpDrawingContext>)tooltip).Show(points, _core);
        }

        /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
        public void HideTooltip()
        {
            if (tooltip == null || _core == null) return;

            foreach (var state in PointStates.GetStates())
            {
                if (!state.IsHoverState) continue;
                if (state.Fill != null) state.Fill.ClearGeometriesFromPaintTask(_core.Canvas);
                if (state.Stroke != null) state.Stroke.ClearGeometriesFromPaintTask(_core.Canvas);
            }

            _core.ClearTooltipData();
            ((IChartTooltip<SkiaSharpDrawingContext>)tooltip).Hide();
        }

        /// <inheritdoc cref="IChartView.SetTooltipStyle(LvcColor, LvcColor)"/>
        public void SetTooltipStyle(LvcColor background, LvcColor textColor)
        {
            TooltipBackground = new SolidColorBrush(Windows.UI.Color.FromArgb(background.A, background.R, background.G, background.B));
            TooltipTextBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(textColor.A, textColor.R, textColor.G, textColor.B));
        }

        void IChartView.InvokeOnUIThread(Action action)
        {
            _ = DispatcherQueue.TryEnqueue(
               Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
               () => action());
        }

        /// <inheritdoc cref="IChartView.SyncAction(Action)"/>
        public void SyncAction(Action action)
        {
            lock (CoreCanvas.Sync)
            {
                action();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            var canvas = (MotionCanvas)FindName("motionCanvas");
            _canvas = canvas;

            if (_core is null)
            {
                _core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
                //legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
                //tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;

                if (SyncContext != null)
                    _canvas.CanvasCore.Sync = SyncContext;

                if (_core == null) throw new Exception("Core not found!");
                _core.Update();

                _core.Measuring += OnCoreMeasuring;
                _core.UpdateStarted += OnCoreUpdateStarted;
                _core.UpdateFinished += OnCoreUpdateFinished;

                SizeChanged += OnSizeChanged;
                PointerMoved += OnPointerMoved;
                PointerExited += OnPointerExited;
            }

            _core.Load();
            _core.Update();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_core == null) throw new Exception("Core not found!");
            _core.Update();
        }

        private void OnPointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(this);
            _core?.InvokePointerMove(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
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

        private void OnPointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            HideTooltip();
            _core?.InvokePointerLeft();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _core?.Unload();
        }

        private static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var chart = (PieChart)o;
            if (chart._core == null) return;

            chart._core.Update();
        }
    }
}
