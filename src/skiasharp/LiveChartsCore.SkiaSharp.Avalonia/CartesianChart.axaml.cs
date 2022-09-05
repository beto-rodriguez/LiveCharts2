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
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="ICartesianChartView{TDrawingContext}" />
public class CartesianChart : UserControl, ICartesianChartView<SkiaSharpDrawingContext>, IAvaloniaChart
{
    #region fields

    /// <summary>
    /// The legend
    /// </summary>
    protected IChartLegend<SkiaSharpDrawingContext>? legend;

    /// <summary>
    /// The tool tip
    /// </summary>
    protected IChartTooltip<SkiaSharpDrawingContext>? tooltip;

    private Chart<SkiaSharpDrawingContext>? _core;
    private MotionCanvas? _avaloniaCanvas;
    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _xObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _yObserver;
    private readonly CollectionDeepObserver<Section<SkiaSharpDrawingContext>> _sectionsObserver;
    private readonly CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;

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
        ((IChartView)this).BackColor = LvcColor.FromArgb(0, 0, 0, 0);

        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
        var initializer = stylesBuilder.GetVisualsInitializer();
        if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        initializer.ApplyStyleToChart(this);

        InitializeCore();

        AttachedToVisualTree += CartesianChart_AttachedToVisualTree;

        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _xObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _yObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _sectionsObserver = new CollectionDeepObserver<Section<SkiaSharpDrawingContext>>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
           OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        XAxes = new List<ICartesianAxis>()
            {
                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
            };
        YAxes = new List<ICartesianAxis>()
            {
                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultCartesianAxis()
            };
        Series = new ObservableCollection<ISeries>();
        VisualElements = new ObservableCollection<ChartElement<SkiaSharpDrawingContext>>();

        PointerPressed += CartesianChart_PointerPressed;
        PointerMoved += CartesianChart_PointerMoved;
        // .. special case in avalonia for pointer released... he handle our own pointer capture.
        PointerWheelChanged += CartesianChart_PointerWheelChanged;
        PointerLeave += CartesianChart_PointerLeave;

        DetachedFromVisualTree += CartesianChart_DetachedFromVisualTree;
    }

    #region avalonia/dependency properties

    /// <summary>
    /// The draw margin property
    /// </summary>
    public static readonly AvaloniaProperty<Margin?> DrawMarginProperty =
       AvaloniaProperty.Register<CartesianChart, Margin?>(nameof(DrawMargin), null, inherits: true);

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly AvaloniaProperty<object> SyncContextProperty =
       AvaloniaProperty.Register<CartesianChart, object>(nameof(SyncContext), new object(), inherits: true);

    /// <summary>
    /// The series property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
       AvaloniaProperty.Register<CartesianChart, IEnumerable<ISeries>>(nameof(Series), Enumerable.Empty<ISeries>(), inherits: true);

    /// <summary>
    /// The x axes property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ICartesianAxis>> XAxesProperty =
        AvaloniaProperty.Register<CartesianChart, IEnumerable<ICartesianAxis>>(nameof(XAxes), Enumerable.Empty<ICartesianAxis>(), inherits: true);

    /// <summary>
    /// The y axes property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ICartesianAxis>> YAxesProperty =
        AvaloniaProperty.Register<CartesianChart, IEnumerable<ICartesianAxis>>(nameof(YAxes), Enumerable.Empty<ICartesianAxis>(), inherits: true);

    /// <summary>
    /// The sections property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<Section<SkiaSharpDrawingContext>>> SectionsProperty =
        AvaloniaProperty.Register<CartesianChart, IEnumerable<Section<SkiaSharpDrawingContext>>>(
            nameof(Sections), Enumerable.Empty<Section<SkiaSharpDrawingContext>>(), inherits: true);

    /// <summary>
    /// The visual elements property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ChartElement<SkiaSharpDrawingContext>>> VisualElementsProperty =
        AvaloniaProperty.Register<CartesianChart, IEnumerable<ChartElement<SkiaSharpDrawingContext>>>(
            nameof(VisualElements), Enumerable.Empty<ChartElement<SkiaSharpDrawingContext>>(), inherits: true);

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
    public static readonly AvaloniaProperty<FontFamily> TooltipFontFamilyProperty =
        AvaloniaProperty.Register<CartesianChart, FontFamily>(
            nameof(TooltipFontFamily), new FontFamily("Arial"), inherits: true);

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
    public static readonly AvaloniaProperty<FontStyle> TooltipFontStyleProperty =
        AvaloniaProperty.Register<CartesianChart, FontStyle>(
            nameof(TooltipFontStyle), FontStyle.Normal, inherits: true);

    /// <summary>
    /// The tool tip text brush property
    /// </summary>
    public static readonly AvaloniaProperty<SolidColorBrush> TooltipTextBrushProperty =
        AvaloniaProperty.Register<CartesianChart, SolidColorBrush>(
            nameof(TooltipTextBrush), new SolidColorBrush(new Color(255, 35, 35, 35)), inherits: true);

    /// <summary>
    /// The tool tip background property
    /// </summary>
    public static readonly AvaloniaProperty<IBrush> TooltipBackgroundProperty =
        AvaloniaProperty.Register<CartesianChart, IBrush>(nameof(TooltipBackground),
            new SolidColorBrush(new Color(255, 250, 250, 250)), inherits: true);

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
    public static readonly AvaloniaProperty<FontFamily> LegendFontFamilyProperty =
       AvaloniaProperty.Register<CartesianChart, FontFamily>(
           nameof(LegendFontFamily), new FontFamily("Arial"), inherits: true);

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
    public static readonly AvaloniaProperty<FontStyle> LegendFontStyleProperty =
        AvaloniaProperty.Register<CartesianChart, FontStyle>(
            nameof(LegendFontStyle), FontStyle.Normal, inherits: true);

    /// <summary>
    /// The legend text brush property
    /// </summary>
    public static readonly AvaloniaProperty<SolidColorBrush> LegendTextBrushProperty =
        AvaloniaProperty.Register<CartesianChart, SolidColorBrush>(
            nameof(LegendTextBrush), new SolidColorBrush(new Color(255, 35, 35, 35)), inherits: true);

    /// <summary>
    /// The legend background property
    /// </summary>
    public static readonly AvaloniaProperty<IBrush> LegendBackgroundProperty =
        AvaloniaProperty.Register<CartesianChart, IBrush>(nameof(LegendBackground),
            new SolidColorBrush(new Color(255, 255, 255, 255)), inherits: true);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> DataPointerDownCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(DataPointerDownCommand), null, inherits: true);

    /// <summary>
    /// The <see cref="ChartPoint"/> pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> ChartPointPointerDownCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(ChartPointPointerDownCommand), null, inherits: true);

    #endregion

    #region events

    /// <inheritdoc cref="IChartView{TDrawingContext}.Measuring" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? Measuring;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateStarted" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateStarted;

    /// <inheritdoc cref="IChartView{TDrawingContext}.UpdateFinished" />
    public event ChartEventHandler<SkiaSharpDrawingContext>? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    public event ChartPointHandler? ChartPointPointerDown;

    #endregion

    #region properties

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => Design.IsDesignMode;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => _core ?? throw new Exception("Core not set yet.");

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => _core is null ? throw new Exception("core not found") : _core.Canvas;

    CartesianChart<SkiaSharpDrawingContext> ICartesianChartView<SkiaSharpDrawingContext>.Core =>
        _core is null ? throw new Exception("core not found") : (CartesianChart<SkiaSharpDrawingContext>)_core;

    LvcColor IChartView.BackColor
    {
        get => Background is not ISolidColorBrush b
                ? new LvcColor()
                : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        set => Background = new SolidColorBrush(new Color(value.R, value.G, value.B, value.A));
    }

    LvcSize IChartView.ControlSize => _avaloniaCanvas is null
        ? new LvcSize()
        : new LvcSize
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

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.XAxes" />
    public IEnumerable<ICartesianAxis> XAxes
    {
        get => (IEnumerable<ICartesianAxis>)GetValue(XAxesProperty);
        set => SetValue(XAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.YAxes" />
    public IEnumerable<ICartesianAxis> YAxes
    {
        get => (IEnumerable<ICartesianAxis>)GetValue(YAxesProperty);
        set => SetValue(YAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.Sections" />
    public IEnumerable<Section<SkiaSharpDrawingContext>> Sections
    {
        get => (IEnumerable<Section<SkiaSharpDrawingContext>>)GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.VisualElements" />
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements
    {
        get => (IEnumerable<ChartElement<SkiaSharpDrawingContext>>)GetValue(VisualElementsProperty);
        set => SetValue(VisualElementsProperty, value);
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
    public FontFamily TooltipFontFamily
    {
        get => (FontFamily)GetValue(TooltipFontFamilyProperty);
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
    public FontStyle TooltipFontStyle
    {
        get => (FontStyle)GetValue(TooltipFontStyleProperty);
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
    public FontFamily LegendFontFamily
    {
        get => (FontFamily)GetValue(LegendFontFamilyProperty);
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
    public FontStyle LegendFontStyle
    {
        get => (FontStyle)GetValue(LegendFontStyleProperty);
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

    /// <inheritdoc cref="IChartView{TDrawingContext}.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    public TimeSpan UpdaterThrottler
    {
        get => _core?.UpdaterThrottler ?? throw new Exception("core not set yet.");
        set
        {
            if (_core is null) throw new Exception("core not set yet.");
            _core.UpdaterThrottler = value;
        }
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a data or data points.
    /// </summary>
    public ICommand? DataPointerDownCommand
    {
        get => (ICommand?)GetValue(DataPointerDownCommandProperty);
        set => SetValue(DataPointerDownCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a chart point.
    /// </summary>
    public ICommand? ChartPointPointerDownCommand
    {
        get => (ICommand?)GetValue(ChartPointPointerDownCommandProperty);
        set => SetValue(ChartPointPointerDownCommandProperty, value);
    }

    #endregion

    /// <inheritdoc cref="ICartesianChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)" />
    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (_core is null) throw new Exception("core not found");
        var cartesianCore = (CartesianChart<SkiaSharpDrawingContext>)_core;
        return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{ChartPoint})"/>
    public void ShowTooltip(IEnumerable<ChartPoint> points)
    {
        if (tooltip is null || _core is null) return;

        tooltip.Show(points, _core);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
    public void HideTooltip()
    {
        if (tooltip is null || _core is null) return;

        _core.ClearTooltipData();
        tooltip.Hide();
    }

    /// <inheritdoc cref="IAvaloniaChart.GetCanvasPosition"/>
    Point IAvaloniaChart.GetCanvasPosition()
    {
        var p = _avaloniaCanvas.TranslatePoint(new Point(0, 0), this);
        return _avaloniaCanvas is null || p is null ? throw new Exception("Canvas not found") : p.Value;
    }

    /// <inheritdoc cref="IChartView.SetTooltipStyle(LvcColor, LvcColor)"/>
    public void SetTooltipStyle(LvcColor background, LvcColor textColor)
    {
        TooltipBackground = new SolidColorBrush(new Color(background.A, background.R, background.G, background.B));
        TooltipTextBrush = new SolidColorBrush(new Color(textColor.A, textColor.R, textColor.G, textColor.B));
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        Dispatcher.UIThread.Post(action);
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <returns></returns>
    protected void InitializeCore()
    {
        var canvas = this.FindControl<MotionCanvas>("canvas");

        var zoomingSection = new Drawing.Geometries.RectangleGeometry();
        var zoomingSectionPaint = new SolidColorPaint
        {
            IsFill = true,
            Color = new SkiaSharp.SKColor(33, 150, 243, 50),
            ZIndex = int.MaxValue
        };
        zoomingSectionPaint.AddGeometryToPaintTask(canvas.CanvasCore, zoomingSection);
        canvas.CanvasCore.AddDrawableTask(zoomingSectionPaint);

        _avaloniaCanvas = canvas;
        _core = new CartesianChart<SkiaSharpDrawingContext>(
            this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore, zoomingSection);

        _core.Measuring += OnCoreMeasuring;
        _core.UpdateStarted += OnCoreUpdateStarted;
        _core.UpdateFinished += OnCoreUpdateFinished;

        legend = this.FindControl<DefaultLegend>("legend");
        tooltip = this.FindControl<DefaultTooltip>("tooltip");

        _core.Update();
    }

    /// <inheritdoc cref="OnPropertyChanged{T}(AvaloniaPropertyChangedEventArgs{T})" />
    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);

        if (_core is null || change.Property.Name == nameof(IsPointerOver)) return;

        if (change.Property.Name == nameof(SyncContext))
        {
            CoreCanvas.Sync = change.NewValue;
        }

        if (change.Property.Name == nameof(Series))
        {
            _seriesObserver?.Dispose((IEnumerable<ISeries>)change.OldValue.Value);
            _seriesObserver?.Initialize((IEnumerable<ISeries>)change.NewValue.Value);
        }

        if (change.Property.Name == nameof(XAxes))
        {
            _xObserver?.Dispose((IEnumerable<ICartesianAxis>)change.OldValue.Value);
            _xObserver?.Initialize((IEnumerable<ICartesianAxis>)change.NewValue.Value);
        }

        if (change.Property.Name == nameof(YAxes))
        {
            _yObserver?.Dispose((IEnumerable<ICartesianAxis>)change.OldValue.Value);
            _yObserver?.Initialize((IEnumerable<ICartesianAxis>)change.NewValue.Value);
        }

        if (change.Property.Name == nameof(Sections))
        {
            _sectionsObserver?.Dispose((IEnumerable<Section<SkiaSharpDrawingContext>>)change.OldValue.Value);
            _sectionsObserver?.Initialize((IEnumerable<Section<SkiaSharpDrawingContext>>)change.NewValue.Value);
        }

        if (change.Property.Name == nameof(VisualElementsProperty))
        {
            _visualsObserver?.Dispose((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)change.OldValue.Value);
            _visualsObserver?.Initialize((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)change.NewValue.Value);
        }

        _core.Update();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;

        _core.Update();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;

        _core.Update();
    }

    private void CartesianChart_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        var p = e.GetPosition(this);
        foreach (var w in desktop.Windows) w.PointerReleased += Window_PointerReleased;
        _core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y), e.GetCurrentPoint(this).Properties.IsRightButtonPressed);
    }

    private void CartesianChart_PointerMoved(object? sender, PointerEventArgs e)
    {
        var p = e.GetPosition(_avaloniaCanvas);
        _core?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void Window_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
        foreach (var w in desktop.Windows) w.PointerReleased -= Window_PointerReleased;
        var p = e.GetPosition(this);
        _core?.InvokePointerUp(new LvcPoint((float)p.X, (float)p.Y), e.GetCurrentPoint(this).Properties.IsRightButtonPressed);
    }

    private void CartesianChart_PointerLeave(object? sender, PointerEventArgs e)
    {
        _ = Dispatcher.UIThread.InvokeAsync(HideTooltip, DispatcherPriority.Background);
        _core?.InvokePointerLeft();
    }

    private void CartesianChart_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_core is null) return;

        var c = (CartesianChart<SkiaSharpDrawingContext>)_core;
        var p = e.GetPosition(this);

        c.Zoom(new LvcPoint((float)p.X, (float)p.Y), e.Delta.Y > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
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

    private void CartesianChart_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        _core?.Load();
    }

    private void CartesianChart_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        _core?.Unload();
    }

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        if (DataPointerDownCommand is not null && DataPointerDownCommand.CanExecute(points)) DataPointerDownCommand.Execute(points);

        var closest = points.FindClosestTo(pointer);
        ChartPointPointerDown?.Invoke(this, closest);
        if (ChartPointPointerDownCommand is not null && ChartPointPointerDownCommand.CanExecute(closest)) ChartPointPointerDownCommand.Execute(closest);
    }

    void IChartView.Invalidate()
    {
        CoreCanvas.Invalidate();
    }
}
