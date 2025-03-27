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
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Paint = LiveChartsCore.Painting.Paint;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <inheritdoc cref="ICartesianChartView"/>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CartesianChart : ChartView, ICartesianChartView
{
    #region fields

    private Chart? _core;
    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _xObserver;
    private readonly CollectionDeepObserver<ICartesianAxis> _yObserver;
    private readonly CollectionDeepObserver<CoreSection> _sectionsObserver;
    private readonly CollectionDeepObserver<ChartElement> _visualsObserver;
    private IChartLegend? _legend;
    private IChartTooltip? _tooltip;
    private bool _matchAxesScreenDataRatio;
    private Theme? _chartTheme;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CartesianChart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    public CartesianChart()
    {
        InitializeComponent();

        LiveCharts.Configure(config => config.UseDefaults());

        InitializeCore();
        SizeChanged += OnSizeChanged;

        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _xObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _yObserver = new CollectionDeepObserver<ICartesianAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _sectionsObserver = new CollectionDeepObserver<CoreSection>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _visualsObserver = new CollectionDeepObserver<ChartElement>(
            OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        SetValue(XAxesProperty, new ObservableCollection<ICartesianAxis>());
        SetValue(YAxesProperty, new List<ICartesianAxis>());
        SetValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetValue(VisualElementsProperty, new ObservableCollection<ChartElement>());
        SetValue(SyncContextProperty, new object());

        if (_core is null) throw new Exception("Core not found!");
        _core.Measuring += OnCoreMeasuring;
        _core.UpdateStarted += OnCoreUpdateStarted;
        _core.UpdateFinished += OnCoreUpdateFinished;

        if (Application.Current is not null)
            Application.Current.RequestedThemeChanged += (sender, args) => _core?.ApplyTheme();
    }

    #region bindable properties

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly BindableProperty SyncContextProperty =
        BindableProperty.Create(
            nameof(SyncContext), typeof(object), typeof(CartesianChart), null, BindingMode.Default, null,
            (BindableObject bo, object o, object n) =>
            {
                var chart = (CartesianChart)bo;
                chart.CoreCanvas.Sync = n;
                PropertyHandlers<CartesianChart>.OnChanged(bo, o, n);
            });

    /// <summary>
    /// The title property.
    /// </summary>
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title), typeof(IChartElement), typeof(CartesianChart), null, BindingMode.Default, null,
            PropertyHandlers<CartesianChart>.OnUIElementChanged);

    /// <summary>
    /// The series property.
    /// </summary>
    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(
            nameof(Series), typeof(ICollection<ISeries>), typeof(CartesianChart), null, BindingMode.Default, null,
            PropertyHandlers<CartesianChart>.OnUIElementsCollectionChanged(c => c._seriesObserver));

    /// <summary>
    /// The x axes property
    /// </summary>
    public static readonly BindableProperty XAxesProperty =
        BindableProperty.Create(
            nameof(XAxes), typeof(ICollection<ICartesianAxis>), typeof(CartesianChart), null, BindingMode.Default, null,
            PropertyHandlers<CartesianChart>.OnUIElementsCollectionChanged(c => c._xObserver));

    /// <summary>
    /// The y axes property.
    /// </summary>
    public static readonly BindableProperty YAxesProperty =
        BindableProperty.Create(
            nameof(YAxes), typeof(IEnumerable<ICartesianAxis>), typeof(CartesianChart), null, BindingMode.Default, null,
            PropertyHandlers<CartesianChart>.OnUIElementsCollectionChanged(c => c._yObserver));

    /// <summary>
    /// The sections property.
    /// </summary>
    public static readonly BindableProperty SectionsProperty =
        BindableProperty.Create(
            nameof(Sections), typeof(IEnumerable<CoreSection>), typeof(CartesianChart), null, BindingMode.Default, null,
            PropertyHandlers<CartesianChart>.OnUIElementsCollectionChanged(c => c._sectionsObserver));

    /// <summary>
    /// The visual elements property.
    /// </summary>
    public static readonly BindableProperty VisualElementsProperty =
        BindableProperty.Create(
            nameof(VisualElements), typeof(IEnumerable<ChartElement>), typeof(CartesianChart), null, BindingMode.Default, null,
            PropertyHandlers<CartesianChart>.OnUIElementsCollectionChanged(c => c._visualsObserver));

    /// <summary>
    /// The draw margin frame property.
    /// </summary>
    public static readonly BindableProperty DrawMarginFrameProperty =
        BindableProperty.Create(
            nameof(DrawMarginFrame), typeof(CoreDrawMarginFrame), typeof(CartesianChart), null,
            BindingMode.Default, null, PropertyHandlers<CartesianChart>.OnUIElementChanged);

    /// <summary>
    /// The draw margin property.
    /// </summary>
    public static readonly BindableProperty DrawMarginProperty =
        BindableProperty.Create(
            nameof(DrawMargin), typeof(Margin), typeof(CartesianChart), null, BindingMode.Default, null,
            PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The zoom mode property.
    /// </summary>
    public static readonly BindableProperty ZoomModeProperty =
        BindableProperty.Create(
            nameof(ZoomMode), typeof(ZoomAndPanMode), typeof(CartesianChart),
            LiveCharts.DefaultSettings.ZoomMode, BindingMode.Default, null);

    /// <summary>
    /// The zooming speed property.
    /// </summary>
    public static readonly BindableProperty ZoomingSpeedProperty =
        BindableProperty.Create(
            nameof(ZoomingSpeed), typeof(double), typeof(CartesianChart),
            LiveCharts.DefaultSettings.ZoomSpeed, BindingMode.Default, null);

    /// <summary>
    /// The animations speed property.
    /// </summary>
    public static readonly BindableProperty AnimationsSpeedProperty =
       BindableProperty.Create(
           nameof(AnimationsSpeed), typeof(TimeSpan), typeof(CartesianChart), LiveCharts.DefaultSettings.AnimationsSpeed);

    /// <summary>
    /// The easing function property.
    /// </summary>
    public static readonly BindableProperty EasingFunctionProperty =
        BindableProperty.Create(
            nameof(EasingFunction), typeof(Func<float, float>), typeof(CartesianChart),
            LiveCharts.DefaultSettings.EasingFunction);

    /// <summary>
    /// The legend position property.
    /// </summary>
    public static readonly BindableProperty LegendPositionProperty =
        BindableProperty.Create(
            nameof(LegendPosition), typeof(LegendPosition), typeof(CartesianChart),
            LiveCharts.DefaultSettings.LegendPosition, propertyChanged: PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The legend background property.
    /// </summary>
    public static readonly BindableProperty LegendBackgroundPaintProperty =
        BindableProperty.Create(
            nameof(LegendBackgroundPaint), typeof(Paint), typeof(CartesianChart),
            LiveCharts.DefaultSettings.LegendBackgroundPaint, propertyChanged: PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The legend text paint property.
    /// </summary>
    public static readonly BindableProperty LegendTextPaintProperty =
        BindableProperty.Create(
            nameof(LegendTextPaint), typeof(Paint), typeof(CartesianChart),
            LiveCharts.DefaultSettings.LegendTextPaint, propertyChanged: PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The legend text size property.
    /// </summary>
    public static readonly BindableProperty LegendTextSizeProperty =
        BindableProperty.Create(
            nameof(LegendTextSize), typeof(double), typeof(CartesianChart),
            LiveCharts.DefaultSettings.LegendTextSize, propertyChanged: PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The tool tip position property.
    /// </summary>
    public static readonly BindableProperty TooltipPositionProperty =
       BindableProperty.Create(
           nameof(TooltipPosition), typeof(TooltipPosition), typeof(CartesianChart),
           LiveCharts.DefaultSettings.TooltipPosition, propertyChanged: PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The tool tip finding strategy property.
    /// </summary>
    public static readonly BindableProperty FindingStrategyProperty =
        BindableProperty.Create(
            nameof(FindingStrategy), typeof(FindingStrategy), typeof(CartesianChart),
            LiveCharts.DefaultSettings.FindingStrategy);

    /// <summary>
    /// The tooltip background property.
    /// </summary>
    public static readonly BindableProperty TooltipBackgroundPaintProperty =
        BindableProperty.Create(
            nameof(TooltipBackgroundPaint), typeof(Paint), typeof(CartesianChart),
            LiveCharts.DefaultSettings.TooltipBackgroundPaint, propertyChanged: PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The tooltip text paint property.
    /// </summary>
    public static readonly BindableProperty TooltipTextPaintProperty =
        BindableProperty.Create(
            nameof(TooltipTextPaint), typeof(Paint), typeof(CartesianChart),
            LiveCharts.DefaultSettings.TooltipTextPaint, propertyChanged: PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The tooltip text size property.
    /// </summary>
    public static readonly BindableProperty TooltipTextSizeProperty =
        BindableProperty.Create(
            nameof(TooltipTextSize), typeof(double), typeof(CartesianChart),
            LiveCharts.DefaultSettings.TooltipTextSize, propertyChanged: PropertyHandlers<CartesianChart>.OnChanged);

    /// <summary>
    /// The update started command.
    /// </summary>
    public static readonly BindableProperty UpdateStartedCommandProperty =
        BindableProperty.Create(
            nameof(UpdateStartedCommand), typeof(ICommand), typeof(CartesianChart), null);

    /// <summary>
    /// The pressed command.
    /// </summary>
    public static readonly BindableProperty PressedCommandProperty =
        BindableProperty.Create(
            nameof(PressedCommand), typeof(ICommand), typeof(CartesianChart), null);

    /// <summary>
    /// The released command.
    /// </summary>
    public static readonly BindableProperty ReleasedCommandProperty =
        BindableProperty.Create(
            nameof(ReleasedCommand), typeof(ICommand), typeof(CartesianChart), null);

    /// <summary>
    /// The pointer move command.
    /// </summary>
    public static readonly BindableProperty MovedCommandProperty =
        BindableProperty.Create(
            nameof(MovedCommand), typeof(ICommand), typeof(CartesianChart), null);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly BindableProperty DataPointerDownCommandProperty =
        BindableProperty.Create(
            nameof(DataPointerDownCommand), typeof(ICommand), typeof(CartesianChart), null);

    /// <summary>
    /// The hovered points chaanged command property
    /// </summary>
    public static readonly BindableProperty HoveredPointsChangedCommandProperty =
        BindableProperty.Create(
            nameof(HoveredPointsChangedCommand), typeof(ICommand), typeof(CartesianChart), null);

    /// <summary>
    /// The chart point pointer down command property
    /// </summary>
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public static readonly BindableProperty ChartPointPointerDownCommandProperty =
        BindableProperty.Create(
            nameof(ChartPointPointerDownCommand), typeof(ICommand), typeof(CartesianChart), null);

    /// <summary>
    /// The visual elements pointer down command property
    /// </summary>
    public static readonly BindableProperty VisualElementsPointerDownCommandProperty =
        BindableProperty.Create(
            nameof(VisualElementsPointerDownCommand), typeof(ICommand), typeof(CartesianChart), null);

    #endregion

    #region events

    /// <inheritdoc cref="IChartView.Measuring" />
    public event ChartEventHandler? Measuring;

    /// <inheritdoc cref="IChartView.UpdateStarted" />
    public event ChartEventHandler? UpdateStarted;

    /// <inheritdoc cref="IChartView.UpdateFinished" />
    public event ChartEventHandler? UpdateFinished;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.HoveredPointsChanged" />
    public event ChartPointHoverHandler? HoveredPointsChanged;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView.VisualElementsPointerDown"/>
    public event VisualElementsHandler? VisualElementsPointerDown;

    #endregion

    #region properties

    bool IChartView.DesignerMode => false;

    bool IChartView.IsDarkMode => Application.Current?.RequestedTheme == AppTheme.Dark;

    /// <inheritdoc cref="IChartView.ChartTheme" />
    public Theme? ChartTheme { get => _chartTheme; set { _chartTheme = value; _core?.Update(); } }

    /// <inheritdoc cref="IChartView.CoreChart" />
    public override Chart CoreChart => _core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor
    {
        get => Background is not SolidColorBrush b
            ? new LvcColor()
            : LvcColor.FromArgb(
                (byte)(b.Color.Alpha * 255), (byte)(b.Color.Red * 255),
                (byte)(b.Color.Green * 255), (byte)(b.Color.Blue * 255));
        set => Background = new SolidColorBrush(Color.FromRgba(value.R / 255, value.G / 255, value.B / 255, value.A / 255));
    }

    CartesianChartEngine ICartesianChartView.Core => _core is null ? throw new Exception("core not found") : (CartesianChartEngine)_core;

    LvcSize IChartView.ControlSize => new() { Width = (float)CanvasView.Width, Height = (float)CanvasView.Height };

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    public CoreMotionCanvas CoreCanvas => CanvasView.CanvasCore;

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin
    {
        get => (Margin)GetValue(DrawMarginProperty);
        set => SetValue(DrawMarginProperty, value);
    }

    /// <inheritdoc cref="IChartView.Title" />
    public IChartElement? Title
    {
        get => (IChartElement?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.Series" />
    public ICollection<ISeries> Series
    {
        get => (ICollection<ISeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    IEnumerable<ISeries> ICartesianChartView.Series
    {
        get => Series;
        set => Series = (ICollection<ISeries>)value;
    }

    /// <inheritdoc cref="ICartesianChartView.XAxes" />
    public ICollection<ICartesianAxis> XAxes
    {
        get => (ICollection<ICartesianAxis>)GetValue(XAxesProperty);
        set => SetValue(XAxesProperty, value);
    }

    IEnumerable<ICartesianAxis> ICartesianChartView.XAxes
    {
        get => XAxes;
        set => XAxes = (ICollection<ICartesianAxis>)value;
    }

    /// <inheritdoc cref="ICartesianChartView.YAxes" />
    public IEnumerable<ICartesianAxis> YAxes
    {
        get => (IEnumerable<ICartesianAxis>)GetValue(YAxesProperty);
        set => SetValue(YAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.Sections" />
    public IEnumerable<CoreSection> Sections
    {
        get => (IEnumerable<CoreSection>)GetValue(SectionsProperty);
        set => SetValue(SectionsProperty, value);
    }

    /// <inheritdoc cref="IChartView.VisualElements" />
    public IEnumerable<ChartElement> VisualElements
    {
        get => (IEnumerable<ChartElement>)GetValue(VisualElementsProperty);
        set => SetValue(VisualElementsProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.DrawMarginFrame" />
    public CoreDrawMarginFrame? DrawMarginFrame
    {
        get => (CoreDrawMarginFrame)GetValue(DrawMarginFrameProperty);
        set => SetValue(DrawMarginFrameProperty, value);
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

    /// <inheritdoc cref="ICartesianChartView.ZoomMode" />
    public ZoomAndPanMode ZoomMode
    {
        get => (ZoomAndPanMode)GetValue(ZoomModeProperty);
        set => SetValue(ZoomModeProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.ZoomingSpeed" />
    public double ZoomingSpeed
    {
        get => (double)GetValue(ZoomingSpeedProperty);
        set => SetValue(ZoomingSpeedProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public LegendPosition LegendPosition
    {
        get => (LegendPosition)GetValue(LegendPositionProperty);
        set => SetValue(LegendPositionProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendBackgroundPaint" />
    public Paint? LegendBackgroundPaint
    {
        get => (Paint?)GetValue(LegendBackgroundPaintProperty);
        set => SetValue(LegendBackgroundPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendTextPaint" />
    public Paint? LegendTextPaint
    {
        get => (Paint?)GetValue(LegendTextPaintProperty);
        set => SetValue(LegendTextPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView.LegendTextSize" />
    public double LegendTextSize
    {
        get => (double?)GetValue(LegendTextSizeProperty) ?? LiveCharts.DefaultSettings.LegendTextSize;
        set => SetValue(LegendTextSizeProperty, value);
    }

    /// <inheritdoc cref="IChartView.Legend" />
    public IChartLegend? Legend { get => _legend; set { _legend = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.TooltipPosition" />
    public TooltipPosition TooltipPosition
    {
        get => (TooltipPosition)GetValue(TooltipPositionProperty);
        set => SetValue(TooltipPositionProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    [Obsolete($"Renamed to {nameof(FindingStrategy)}")]
    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => ((FindingStrategy)GetValue(FindingStrategyProperty)!).AsOld();
        set => SetValue(FindingStrategyProperty, value.AsNew());
    }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    public FindingStrategy FindingStrategy
    {
        get => (FindingStrategy)GetValue(FindingStrategyProperty);
        set => SetValue(FindingStrategyProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipBackgroundPaint" />
    public Paint? TooltipBackgroundPaint
    {
        get => (Paint?)GetValue(TooltipBackgroundPaintProperty);
        set => SetValue(TooltipBackgroundPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipTextPaint" />
    public Paint? TooltipTextPaint
    {
        get => (Paint?)GetValue(TooltipTextPaintProperty);
        set => SetValue(TooltipTextPaintProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipTextSize" />
    public double TooltipTextSize
    {
        get => (double?)GetValue(TooltipTextSizeProperty) ?? LiveCharts.DefaultSettings.TooltipTextSize;
        set => SetValue(TooltipTextSizeProperty, value);
    }

    /// <inheritdoc cref="IChartView.Tooltip" />
    public IChartTooltip? Tooltip { get => _tooltip; set { _tooltip = value; OnPropertyChanged(); } }

    /// <inheritdoc cref="IChartView.AutoUpdateEnabled" />
    public bool AutoUpdateEnabled { get; set; } = true;

    /// <inheritdoc cref="IChartView.UpdaterThrottler" />
    public TimeSpan UpdaterThrottler { get; set; } = LiveCharts.DefaultSettings.UpdateThrottlingTimeout;

    /// <summary>
    /// Gets or sets a command to execute when the chart update started.
    /// </summary>
    public ICommand? UpdateStartedCommand
    {
        get => (ICommand?)GetValue(UpdateStartedCommandProperty);
        set => SetValue(UpdateStartedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the prressed the chart.
    /// </summary>
    public ICommand? PressedCommand
    {
        get => (ICommand?)GetValue(PressedCommandProperty);
        set => SetValue(PressedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the users released thhe press on the chart.
    /// </summary>
    public ICommand? ReleasedCommand
    {
        get => (ICommand?)GetValue(ReleasedCommandProperty);
        set => SetValue(ReleasedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer/finger moves over the chart.
    /// </summary>
    public ICommand? MovedCommand
    {
        get => (ICommand?)GetValue(MovedCommandProperty);
        set => SetValue(MovedCommandProperty, value);
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
    /// Gets or sets a command to execute when the hovered points change.
    /// </summary>
    public ICommand? HoveredPointsChangedCommand
    {
        get => (ICommand?)GetValue(HoveredPointsChangedCommandProperty);
        set => SetValue(HoveredPointsChangedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a chart point.
    /// </summary>
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public ICommand? ChartPointPointerDownCommand
    {
        get => (ICommand?)GetValue(ChartPointPointerDownCommandProperty);
        set => SetValue(ChartPointPointerDownCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a chart point.
    /// </summary>
    public ICommand? VisualElementsPointerDownCommand
    {
        get => (ICommand?)GetValue(VisualElementsPointerDownCommandProperty);
        set => SetValue(VisualElementsPointerDownCommandProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.MatchAxesScreenDataRatio" />
    public bool MatchAxesScreenDataRatio
    {
        get => _matchAxesScreenDataRatio;
        set
        {
            _matchAxesScreenDataRatio = value;

            if (value) SharedAxes.MatchAxesScreenDataRatio(this);
            else SharedAxes.DisposeMatchAxesScreenDataRatio(this);
        }
    }

    #endregion

    /// <inheritdoc cref="ICartesianChartView.ScalePixelsToData(LvcPointD, int, int)"/>
    public LvcPointD ScalePixelsToData(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (_core is not CartesianChartEngine cc) throw new Exception("core not found");
        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToChartValues(point.X), Y = yScaler.ToChartValues(point.Y) };
    }

    /// <inheritdoc cref="ICartesianChartView.ScaleDataToPixels(LvcPointD, int, int)"/>
    public LvcPointD ScaleDataToPixels(LvcPointD point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (_core is not CartesianChartEngine cc) throw new Exception("core not found");

        var xScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.XAxes[xAxisIndex]);
        var yScaler = new Scaler(cc.DrawMarginLocation, cc.DrawMarginSize, cc.YAxes[yAxisIndex]);

        return new LvcPointD { X = xScaler.ToPixels(point.X), Y = yScaler.ToPixels(point.Y) };
    }

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public IEnumerable<ChartPoint> GetPointsAt(LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
    {
        if (_core is not CartesianChartEngine cc) throw new Exception("core not found");

        if (strategy == FindingStrategy.Automatic)
            strategy = cc.Series.GetFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, new(point), strategy, FindPointFor.HoverEvent));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
    {
        return _core is not CartesianChartEngine cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((LiveChartsCore.VisualElements.VisualElement)visual).IsHitBy(_core, new(point)));
    }

    void IChartView.InvokeOnUIThread(Action action) =>
        _ = MainThread.InvokeOnMainThreadAsync(action);

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <returns></returns>
    protected void InitializeCore()
    {
        _core = new CartesianChartEngine(
            this, config => config.UseDefaults(), CanvasView.CanvasCore);

        _core.Update();
    }

    /// <inheritdoc cref="NavigableElement.OnParentSet"/>
    protected override void OnParentSet()
    {
        base.OnParentSet();

        if (Parent == null)
        {
            _core?.Unload();
            return;
        }

        _core?.Load();
    }

    internal override void OnPressed(object? sender, Behaviours.Events.PressedEventArgs args)
    {
        // not implemented yet?
        // https://github.com/dotnet/maui/issues/16202
        //if (Keyboard.Modifiers > 0) return;

        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (PressedCommand?.CanExecute(cArgs) == true) PressedCommand.Execute(cArgs);

        _core?.InvokePointerDown(args.Location, args.IsSecondaryPress);
    }

    internal override void OnMoved(object? sender, Behaviours.Events.ScreenEventArgs args)
    {
        var location = args.Location;

        var cArgs = new PointerCommandArgs(this, new(location.X, location.Y), args.OriginalEvent);
        if (MovedCommand?.CanExecute(cArgs) == true) MovedCommand.Execute(cArgs);

        _core?.InvokePointerMove(location);
    }

    internal override void OnReleased(object? sender, Behaviours.Events.PressedEventArgs args)
    {
        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (ReleasedCommand?.CanExecute(cArgs) == true) ReleasedCommand.Execute(cArgs);

        _core?.InvokePointerUp(args.Location, args.IsSecondaryPress);
    }

    internal override void OnScrolled(object? sender, Behaviours.Events.ScrollEventArgs args)
    {
        if (_core is null) throw new Exception("core not found");
        var c = (CartesianChartEngine)_core;
        c.Zoom(args.Location, args.ScrollDelta > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    internal override void OnPinched(object? sender, Behaviours.Events.PinchEventArgs args)
    {
        if (_core is null) return;

        var c = (CartesianChartEngine)_core;
        var p = args.PinchStart;
        var s = c.ControlSize;
        var pivot = new LvcPoint((float)(p.X * s.Width), (float)(p.Y * s.Height));
        c.Zoom(pivot, ZoomDirection.DefinedByScaleFactor, args.Scale, true);
    }

    internal override void OnExited(object? sender, Behaviours.Events.EventArgs args) =>
        _core?.InvokePointerLeft();

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        _core?.Update();

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        _core?.Update();

    private void OnSizeChanged(object? sender, EventArgs e) =>
        _core?.Update();

    private void OnCoreUpdateFinished(IChartView chart) =>
        UpdateFinished?.Invoke(this);

    private void OnCoreUpdateStarted(IChartView chart)
    {
        if (UpdateStartedCommand is not null)
        {
            var args = new ChartCommandArgs(this);
            if (UpdateStartedCommand.CanExecute(args)) UpdateStartedCommand.Execute(args);
        }

        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView chart) => Measuring?.Invoke(this);

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        if (DataPointerDownCommand is not null && DataPointerDownCommand.CanExecute(points)) DataPointerDownCommand.Execute(points);

        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
#pragma warning disable CS0618 // Type or member is obsolete
        ChartPointPointerDownCommand?.Execute(points.FindClosestTo(pointer));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    void IChartView.OnVisualElementPointerDown(
        IEnumerable<IInteractable> visualElements, LvcPoint pointer)
    {
        var args = new VisualElementsEventArgs(CoreChart, visualElements, pointer);

        VisualElementsPointerDown?.Invoke(this, args);
        if (VisualElementsPointerDownCommand is not null && VisualElementsPointerDownCommand.CanExecute(args))
            VisualElementsPointerDownCommand.Execute(args);
    }

    void IChartView.OnHoveredPointsChanged(IEnumerable<ChartPoint>? newPoints, IEnumerable<ChartPoint>? oldPoints)
    {
        HoveredPointsChanged?.Invoke(this, newPoints, oldPoints);

        var args = new HoverCommandArgs(this, newPoints, oldPoints);
        if (HoveredPointsChangedCommand is not null && HoveredPointsChangedCommand.CanExecute(args)) HoveredPointsChangedCommand.Execute(args);
    }

    void IChartView.Invalidate() => CoreCanvas.Invalidate();
}
