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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace LiveChartsCore.SkiaSharpView.UWP
{
    /// <inheritdoc cref="IPolarChartView{TDrawingContext}"/>
    public sealed partial class PolarChart : UserControl, IPolarChartView<SkiaSharpDrawingContext>, IUwpChart
    {
        #region fields

        private Chart<SkiaSharpDrawingContext> _core;
        private MotionCanvas _canvas;
        private readonly CollectionDeepObserver<ISeries> _seriesObserver;
        private readonly CollectionDeepObserver<IPolarAxis> _angleObserver;
        private readonly CollectionDeepObserver<IPolarAxis> _radiusObserver;
        private readonly CollectionDeepObserver<Section<SkiaSharpDrawingContext>> _sectionsObserver;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PolarChart"/> class.
        /// </summary>
        public PolarChart()
        {
            if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

            var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
            var initializer = stylesBuilder.GetVisualsInitializer();
            if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
                throw new Exception("Default colors are not valid");
            initializer.ApplyStyleToChart(this);

            InitializeComponent();

            _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _angleObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _radiusObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
            _sectionsObserver = new CollectionDeepObserver<Section<SkiaSharpDrawingContext>>(
                OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            SetValue(AngleAxesProperty, new ObservableCollection<IPolarAxis>() { LiveCharts.CurrentSettings.PolarAxisProvider() });
            SetValue(RadiusAxesProperty, new ObservableCollection<IPolarAxis>() { LiveCharts.CurrentSettings.PolarAxisProvider() });
            SetValue(SeriesProperty, new ObservableCollection<ISeries>());
        }

        #region dependency properties

        /// <summary>
        /// The fit to bounds property.
        /// </summary>
        public static readonly DependencyProperty FitToBoundsProperty =
            DependencyProperty.Register(
                nameof(FitToBounds), typeof(double), typeof(PolarChart), new PropertyMetadata(false, OnDependencyPropertyChanged));

        /// <summary>
        /// The inner radius property.
        /// </summary>
        public static readonly DependencyProperty TotalAngleProperty =
            DependencyProperty.Register(
                nameof(TotalAngle), typeof(double), typeof(PolarChart), new PropertyMetadata(360d, OnDependencyPropertyChanged));

        /// <summary>
        /// The inner radius property.
        /// </summary>
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register(
                nameof(InnerRadius), typeof(double), typeof(PolarChart), new PropertyMetadata(0d, OnDependencyPropertyChanged));

        /// <summary>
        /// The initial rotation property.
        /// </summary>
        public static readonly DependencyProperty InitialRotationProperty =
            DependencyProperty.Register(
                nameof(InitialRotation), typeof(double), typeof(PolarChart), new PropertyMetadata(0d, OnDependencyPropertyChanged));

        /// <summary>
        /// The series property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty =
            DependencyProperty.Register(
                nameof(Series), typeof(IEnumerable<ISeries>), typeof(PolarChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (PolarChart)o;
                        var seriesObserver = chart._seriesObserver;
                        seriesObserver.Dispose((IEnumerable<ISeries>)args.OldValue);
                        seriesObserver.Initialize((IEnumerable<ISeries>)args.NewValue);
                        if (chart._core == null) return;
                        chart._core.Update();
                    }));

        /// <summary>
        /// The x axes property.
        /// </summary>
        public static readonly DependencyProperty AngleAxesProperty =
            DependencyProperty.Register(
                nameof(AngleAxes), typeof(IEnumerable<IPolarAxis>), typeof(PolarChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (PolarChart)o;
                        var observer = chart._angleObserver;
                        observer.Dispose((IEnumerable<IPolarAxis>)args.OldValue);
                        observer.Initialize((IEnumerable<IPolarAxis>)args.NewValue);
                        if (chart._core == null) return;
                        chart._core.Update();
                    }));

        /// <summary>
        /// The y axes property.
        /// </summary>
        public static readonly DependencyProperty RadiusAxesProperty =
            DependencyProperty.Register(
                nameof(RadiusAxes), typeof(IEnumerable<IPolarAxis>), typeof(PolarChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (PolarChart)o;
                        var observer = chart._radiusObserver;
                        observer.Dispose((IEnumerable<IPolarAxis>)args.OldValue);
                        observer.Initialize((IEnumerable<IPolarAxis>)args.NewValue);
                        if (chart._core == null) return;
                        chart._core.Update();
                    }));

        /// <summary>
        /// The sync context property.
        /// </summary>
        public static readonly DependencyProperty SyncContextProperty =
            DependencyProperty.Register(
                nameof(SyncContext), typeof(object), typeof(PolarChart), new PropertyMetadata(null,
                    (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                    {
                        var chart = (PolarChart)o;
                        if (chart._canvas != null) chart.CoreCanvas.Sync = args.NewValue;
                        if (chart._core == null) return;
                        chart._core.Update();
                    }));

        /// <summary>
        /// The tool tip finding strategy property.
        /// </summary>
        public static readonly DependencyProperty TooltipFindingStrategyProperty =
            DependencyProperty.Register(
                nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(PolarChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy, OnDependencyPropertyChanged));

        /// <summary>
        /// The animations speed property.
        /// </summary>
        public static readonly DependencyProperty AnimationsSpeedProperty =
            DependencyProperty.Register(
                nameof(AnimationsSpeed), typeof(TimeSpan), typeof(PolarChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultAnimationsSpeed, OnDependencyPropertyChanged));

        /// <summary>
        /// The easing function property.
        /// </summary>
        public static readonly DependencyProperty EasingFunctionProperty =
            DependencyProperty.Register(
                nameof(EasingFunction), typeof(Func<float, float>), typeof(PolarChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultEasingFunction, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend position property.
        /// </summary>
        public static readonly DependencyProperty LegendPositionProperty =
            DependencyProperty.Register(
                nameof(LegendPosition), typeof(LegendPosition), typeof(PolarChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendPosition, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend orientation property.
        /// </summary>
        public static readonly DependencyProperty LegendOrientationProperty =
            DependencyProperty.Register(
                nameof(LegendOrientation), typeof(LegendOrientation), typeof(PolarChart),
                new PropertyMetadata(LiveCharts.CurrentSettings.DefaultLegendOrientation, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip position property.
        /// </summary>
        public static readonly DependencyProperty TooltipPositionProperty =
           DependencyProperty.Register(
               nameof(TooltipPosition), typeof(TooltipPosition), typeof(PolarChart),
               new PropertyMetadata(LiveCharts.CurrentSettings.DefaultTooltipPosition, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip background property.
        /// </summary>
        public static readonly DependencyProperty TooltipBackgroundProperty =
           DependencyProperty.Register(
               nameof(TooltipBackground), typeof(SolidColorBrush), typeof(PolarChart),
               new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 250, 250, 250)), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font family property.
        /// </summary>
        public static readonly DependencyProperty TooltipFontFamilyProperty =
           DependencyProperty.Register(
               nameof(TooltipFontFamily), typeof(FontFamily), typeof(PolarChart),
               new PropertyMetadata(new FontFamily("Trebuchet MS"), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip text color property.
        /// </summary>
        public static readonly DependencyProperty TooltipTextBrushProperty =
           DependencyProperty.Register(
               nameof(TooltipTextBrush), typeof(Brush), typeof(PolarChart),
               new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 35, 35, 35)), OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font size property.
        /// </summary>
        public static readonly DependencyProperty TooltipFontSizeProperty =
           DependencyProperty.Register(
               nameof(TooltipFontSize), typeof(double), typeof(PolarChart), new PropertyMetadata(13d, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font weight property.
        /// </summary>
        public static readonly DependencyProperty TooltipFontWeightProperty =
           DependencyProperty.Register(
               nameof(TooltipFontWeight), typeof(FontWeight), typeof(PolarChart),
               new PropertyMetadata(FontWeights.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font stretch property.
        /// </summary>
        public static readonly DependencyProperty TooltipFontStretchProperty =
           DependencyProperty.Register(
               nameof(TooltipFontStretch), typeof(FontStretch), typeof(PolarChart),
               new PropertyMetadata(FontStretch.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip font style property.
        /// </summary>
        public static readonly DependencyProperty TooltipFontStyleProperty =
           DependencyProperty.Register(
               nameof(TooltipFontStyle), typeof(FontStyle), typeof(PolarChart),
               new PropertyMetadata(FontStyle.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The tool tip template property.
        /// </summary>
        public static readonly DependencyProperty TooltipTemplateProperty =
            DependencyProperty.Register(
                nameof(TooltipTemplate), typeof(DataTemplate), typeof(PolarChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font family property.
        /// </summary>
        public static readonly DependencyProperty LegendFontFamilyProperty =
           DependencyProperty.Register(
               nameof(LegendFontFamily), typeof(FontFamily), typeof(PolarChart),
               new PropertyMetadata(new FontFamily("Trebuchet MS"), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend text color property.
        /// </summary>
        public static readonly DependencyProperty LegendTextBrushProperty =
           DependencyProperty.Register(
               nameof(LegendTextBrush), typeof(Brush), typeof(PolarChart),
               new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 35, 35, 35)), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend background property.
        /// </summary>
        public static readonly DependencyProperty LegendBackgroundProperty =
           DependencyProperty.Register(
               nameof(LegendBackground), typeof(Brush), typeof(PolarChart),
               new PropertyMetadata(new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255)), OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font size property.
        /// </summary>
        public static readonly DependencyProperty LegendFontSizeProperty =
           DependencyProperty.Register(
               nameof(LegendFontSize), typeof(double), typeof(PolarChart), new PropertyMetadata(13d, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font weight property.
        /// </summary>
        public static readonly DependencyProperty LegendFontWeightProperty =
           DependencyProperty.Register(
               nameof(LegendFontWeight), typeof(FontWeight), typeof(PolarChart),
               new PropertyMetadata(FontWeights.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font stretch property.
        /// </summary>
        public static readonly DependencyProperty LegendFontStretchProperty =
           DependencyProperty.Register(
               nameof(LegendFontStretch), typeof(FontStretch), typeof(PolarChart),
               new PropertyMetadata(FontStretch.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend font style property.
        /// </summary>
        public static readonly DependencyProperty LegendFontStyleProperty =
           DependencyProperty.Register(
               nameof(LegendFontStyle), typeof(FontStyle), typeof(PolarChart),
               new PropertyMetadata(FontStyle.Normal, OnDependencyPropertyChanged));

        /// <summary>
        /// The legend template property.
        /// </summary>
        public static readonly DependencyProperty LegendTemplateProperty =
            DependencyProperty.Register(
                nameof(LegendTemplate), typeof(DataTemplate), typeof(PolarChart), new PropertyMetadata(null, OnDependencyPropertyChanged));

        #endregion

        #region events

        /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
        public event ChartEventHandler<SkiaSharpDrawingContext> Measuring;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
        public event ChartEventHandler<SkiaSharpDrawingContext> UpdateStarted;

        /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
        public event ChartEventHandler<SkiaSharpDrawingContext> UpdateFinished;

        #endregion

        #region properties

        Grid IUwpChart.LayoutGrid => grid;
        FrameworkElement IUwpChart.Canvas => motionCanvas;
        FrameworkElement IUwpChart.Legend => legend;

        /// <inheritdoc cref="IChartView.DesignerMode" />
        bool IChartView.DesignerMode => Windows.ApplicationModel.DesignMode.DesignModeEnabled;

        /// <inheritdoc cref="IChartView.CoreChart" />
        public IChart CoreChart => _core ?? throw new Exception("Core not set yet.");

        /// <inheritdoc cref="IChartView.SyncContext" />
        public object SyncContext
        {
            get => GetValue(SyncContextProperty);
            set => SetValue(SyncContextProperty, value);
        }

        LvcColor IChartView.BackColor
        {
            get => Background is not SolidColorBrush b
                ? new LvcColor()
                : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
            set => SetValue(BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(value.A, value.R, value.G, value.B)));
        }

        /// <inheritdoc cref="IChartView.DrawMargin" />
        public Margin DrawMargin
        {
            get => null;
            set => throw new NotImplementedException();
        }

        Margin IChartView.DrawMargin
        {
            get => DrawMargin;
            set => throw new NotImplementedException();
        }

        LvcSize IChartView.ControlSize => _canvas == null
            ? throw new Exception("Canvas not found")
            : (new() { Width = (float)_canvas.ActualWidth, Height = (float)_canvas.ActualHeight });

        /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
        public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => _canvas == null ? throw new Exception("Canvas not found") : _canvas.CanvasCore;

        PolarChart<SkiaSharpDrawingContext> IPolarChartView<SkiaSharpDrawingContext>.Core =>
            _core == null ? throw new Exception("core not found") : (PolarChart<SkiaSharpDrawingContext>)_core;

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.FitToBounds" />
        public bool FitToBounds
        {
            get => (bool)GetValue(FitToBoundsProperty);
            set => SetValue(FitToBoundsProperty, value);
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.TotalAngle" />
        public double TotalAngle
        {
            get => (double)GetValue(TotalAngleProperty);
            set => SetValue(TotalAngleProperty, value);
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.InnerRadius" />
        public double InnerRadius
        {
            get => (double)GetValue(InnerRadiusProperty);
            set => SetValue(InnerRadiusProperty, value);
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.InitialRotation" />
        public double InitialRotation
        {
            get => (double)GetValue(InitialRotationProperty);
            set => SetValue(InitialRotationProperty, value);
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.Series" />
        public IEnumerable<ISeries> Series
        {
            get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.AngleAxes" />
        public IEnumerable<IPolarAxis> AngleAxes
        {
            get => (IEnumerable<IPolarAxis>)GetValue(AngleAxesProperty);
            set => SetValue(AngleAxesProperty, value);
        }

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.RadiusAxes" />
        public IEnumerable<IPolarAxis> RadiusAxes
        {
            get => (IEnumerable<IPolarAxis>)GetValue(RadiusAxesProperty);
            set => SetValue(RadiusAxesProperty, value);
        }

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

        Func<float, float> IChartView.EasingFunction
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
        public DataTemplate TooltipTemplate
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
        public IChartTooltip<SkiaSharpDrawingContext> Tooltip => tooltip;

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
        public IChartLegend<SkiaSharpDrawingContext> Legend => legend;

        /// <inheritdoc cref="IChartView{TDrawingContext}.PointStates" />
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

        /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)" />
        public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
        {
            return new double[0];
            // if (_core == null) throw new Exception("core not found");
            //var cartesianCore = (PolarChart<SkiaSharpDrawingContext>)_core;
            //return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
        }

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
            CoreApplication.MainView.CoreWindow.Dispatcher
                .RunAsync(CoreDispatcherPriority.Normal, () => action())
                .AsTask()
                .GetAwaiter()
                .GetResult();
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
            var canvas = (MotionCanvas)FindName("motionCanvas");
            _canvas = canvas;

            if (_core is null)
            {
                _core = new PolarChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);

                if (SyncContext != null)
                    _canvas.CanvasCore.Sync = SyncContext;

                if (_core == null) throw new Exception("Core not found!");
                _core.Measuring += OnCoreMeasuring;
                _core.UpdateStarted += OnCoreUpdateStarted;
                _core.UpdateFinished += OnCoreUpdateFinished;

                PointerWheelChanged += OnWheelChanged;
                PointerPressed += OnPointerPressed;
                PointerReleased += OnPointerReleased;
                SizeChanged += OnSizeChanged;
                PointerMoved += OnPointerMoved;
                PointerExited += OnPointerExited;
            }

            _core.Load();
            _core.Update();
        }

        private void OnDeepCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_core == null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
            _core.Update();
        }

        private void OnDeepCollectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_core == null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
            _core.Update();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_core == null) throw new Exception("Core not found!");
            _core.Update();
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
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

        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            HideTooltip();
            _core?.InvokePointerLeft();
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(this);
            _core?.InvokePointerUp(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
            ReleasePointerCapture(e.Pointer);
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _ = CapturePointer(e.Pointer);
            var p = e.GetCurrentPoint(this);
            _core?.InvokePointerDown(new LvcPoint((float)p.Position.X, (float)p.Position.Y));
        }

        private void OnWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            //if (_core == null) throw new Exception("core not found");
            //var c = (PolarChart<SkiaSharpDrawingContext>)_core;
            //var p = e.GetCurrentPoint(this);

            //c.Zoom(
            //    new System.Drawing.PointF(
            //        (float)p.Position.X, (float)p.Position.Y),
            //        p.Properties.MouseWheelDelta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _core?.Unload();
        }

        private static void OnDependencyPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            var chart = (PolarChart)o;
            if (chart._core == null) return;

            chart._core.Update();
        }
    }
}
