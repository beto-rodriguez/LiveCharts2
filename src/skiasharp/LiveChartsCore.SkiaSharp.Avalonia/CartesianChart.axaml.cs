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
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Styling;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Generators;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Painting;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="ICartesianChartView" />
public partial class CartesianChart : UserControl, ICartesianChartView, ICustomHitTest
{
    #region fields

    /// <summary>
    /// The legend
    /// </summary>
    protected IChartLegend? legend;

    /// <summary>
    /// The tool tip
    /// </summary>
    protected IChartTooltip? tooltip;

    private Chart? _core;
    private MotionCanvas? _avaloniaCanvas;
    private readonly ChartObserver _observe;

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

        AttachedToVisualTree += CartesianChart_AttachedToVisualTree;

        _observe = new ChartObserver(UpdateCore, AddUIElement, RemoveUIElement)
            .Collection(nameof(Series))
            .Collection(nameof(XAxes))
            .Collection(nameof(YAxes))
            .Collection(nameof(Sections))
            .Collection(nameof(VisualElements))
            .Property(nameof(Title))
            .Property(nameof(DrawMarginFrame));

        _observe.Add(
            nameof(SeriesSource),
            new SeriesSourceObserver(
                InflateSeriesTemplate,
                GetSeriesSource,
                () => SeriesSource is not null && SeriesTemplate is not null));

        XAxes = new ObservableCollection<ICartesianAxis>();
        YAxes = new ObservableCollection<ICartesianAxis>();
        Series = new ObservableCollection<ISeries>();
        Sections = new ObservableCollection<IChartElement>();
        VisualElements = new ObservableCollection<IChartElement>();

        PointerPressed += CartesianChart_PointerPressed;
        PointerMoved += CartesianChart_PointerMoved;
        PointerReleased += CartesianChart_PointerReleased;
        PointerWheelChanged += CartesianChart_PointerWheelChanged;
        PointerExited += CartesianChart_PointerLeave;

        var pinchGesture = new PinchGestureRecognizer();
        GestureRecognizers.Add(pinchGesture);
        AddHandler(Gestures.PinchEvent, CartesianChart_Pinched);

        DetachedFromVisualTree += CartesianChart_DetachedFromVisualTree;
    }

    #region Generated Bindable Properties

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0052 // Remove unread private member

    private static readonly XamlProperty<IEnumerable<object>> seriesSource = new(onChanged: OnSeriesSourceChanged);
    private static readonly XamlProperty<DataTemplate> seriesTemplate = new(onChanged: OnSeriesSourceChanged);

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0052 // Remove unread private members

    #endregion

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
    /// The title property.
    /// </summary>
    public static readonly AvaloniaProperty<IChartElement?> TitleProperty =
       AvaloniaProperty.Register<CartesianChart, IChartElement?>(nameof(Title), null, inherits: true);

    /// <summary>
    /// The series property
    /// </summary>
    public static readonly AvaloniaProperty<ICollection<ISeries>> SeriesProperty =
       AvaloniaProperty.Register<CartesianChart, ICollection<ISeries>>(nameof(Series), [], inherits: true);

    /// <summary>
    /// The x axes property
    /// </summary>
    public static readonly AvaloniaProperty<ICollection<ICartesianAxis>> XAxesProperty =
        AvaloniaProperty.Register<CartesianChart, ICollection<ICartesianAxis>>(nameof(XAxes), [], inherits: true);

    /// <summary>
    /// The y axes property
    /// </summary>
    public static readonly AvaloniaProperty<ICollection<ICartesianAxis>> YAxesProperty =
        AvaloniaProperty.Register<CartesianChart, ICollection<ICartesianAxis>>(nameof(YAxes), [], inherits: true);

    /// <summary>
    /// The sections property
    /// </summary>
    public static readonly AvaloniaProperty<ICollection<IChartElement>> SectionsProperty =
        AvaloniaProperty.Register<CartesianChart, ICollection<IChartElement>>(
            nameof(Sections), [], inherits: true);

    /// <summary>
    /// The visual elements property
    /// </summary>
    public static readonly AvaloniaProperty<ICollection<IChartElement>> VisualElementsProperty =
        AvaloniaProperty.Register<CartesianChart, ICollection<IChartElement>>(
            nameof(VisualElements), [], inherits: true);

    /// <summary>
    /// The draw margin frame property
    /// </summary>
    public static readonly AvaloniaProperty<IChartElement?> DrawMarginFrameProperty =
        AvaloniaProperty.Register<CartesianChart, IChartElement?>(
            nameof(DrawMarginFrame), null, inherits: true);

    /// <summary>
    /// The zoom mode property
    /// </summary>
    public static readonly AvaloniaProperty<ZoomAndPanMode> ZoomModeProperty =
        AvaloniaProperty.Register<CartesianChart, ZoomAndPanMode>(
            nameof(ZoomMode), LiveCharts.DefaultSettings.ZoomMode, inherits: true);

    /// <summary>
    /// The zooming speed property
    /// </summary>
    public static readonly AvaloniaProperty<double> ZoomingSpeedProperty =
        AvaloniaProperty.Register<CartesianChart, double>(
            nameof(ZoomingSpeed), LiveCharts.DefaultSettings.ZoomSpeed, inherits: true);

    /// <summary>
    /// The animations speed property
    /// </summary>
    public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
        AvaloniaProperty.Register<CartesianChart, TimeSpan>(
            nameof(AnimationsSpeed), LiveCharts.DefaultSettings.AnimationsSpeed, inherits: true);

    /// <summary>
    /// The easing function property
    /// </summary>
    public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
        AvaloniaProperty.Register<CartesianChart, Func<float, float>>(
            nameof(EasingFunction), LiveCharts.DefaultSettings.EasingFunction, inherits: true);

    /// <summary>
    /// The tool tip position property
    /// </summary>
    public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
        AvaloniaProperty.Register<CartesianChart, TooltipPosition>(
            nameof(TooltipPosition), LiveCharts.DefaultSettings.TooltipPosition, inherits: true);

    /// <summary>
    /// The tool tip finding strategy property
    /// </summary>
    public static readonly AvaloniaProperty<FindingStrategy> FindingStrategyProperty =
        AvaloniaProperty.Register<CartesianChart, FindingStrategy>(
            nameof(FindingStrategy), LiveCharts.DefaultSettings.FindingStrategy, inherits: true);

    /// <summary>
    /// The tooltip background paint property
    /// </summary>
    public static readonly AvaloniaProperty<Paint?> TooltipBackgroundPaintProperty =
        AvaloniaProperty.Register<CartesianChart, Paint?>(
            nameof(TooltipBackgroundPaint),
            (Paint?)LiveCharts.DefaultSettings.TooltipBackgroundPaint, inherits: true);

    /// <summary>
    /// The tooltip text paint property
    /// </summary>
    public static readonly AvaloniaProperty<Paint?> TooltipTextPaintProperty =
        AvaloniaProperty.Register<CartesianChart, Paint?>(
            nameof(TooltipTextPaint),
            (Paint?)LiveCharts.DefaultSettings.TooltipTextPaint, inherits: true);

    /// <summary>
    /// The tooltip text size property
    /// </summary>
    public static readonly AvaloniaProperty<double> TooltipTextSizeProperty =
        AvaloniaProperty.Register<CartesianChart, double>(
            nameof(TooltipTextSize), LiveCharts.DefaultSettings.TooltipTextSize, inherits: true);

    /// <summary>
    /// The legend position property
    /// </summary>
    public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
        AvaloniaProperty.Register<CartesianChart, LegendPosition>(
            nameof(LegendPosition), LiveCharts.DefaultSettings.LegendPosition, inherits: true);

    /// <summary>
    /// The legend background paint property
    /// </summary>
    public static readonly AvaloniaProperty<Paint?> LegendBackgroundPaintProperty =
        AvaloniaProperty.Register<CartesianChart, Paint?>(
            nameof(LegendBackgroundPaint),
            (Paint?)LiveCharts.DefaultSettings.LegendBackgroundPaint, inherits: true);

    /// <summary>
    /// The legend text paint property
    /// </summary>
    public static readonly AvaloniaProperty<Paint?> LegendTextPaintProperty =
        AvaloniaProperty.Register<CartesianChart, Paint?>(
            nameof(LegendTextPaint),
            (Paint?)LiveCharts.DefaultSettings.LegendTextPaint, inherits: true);

    /// <summary>
    /// The legend text size property
    /// </summary>
    public static readonly AvaloniaProperty<double> LegendTextSizeProperty =
        AvaloniaProperty.Register<CartesianChart, double>(
            nameof(LegendTextSize), LiveCharts.DefaultSettings.LegendTextSize, inherits: true);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> UpdateStartedCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(UpdateStartedCommand), null, inherits: true);

    /// <summary>
    /// The pointer pressed command.
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> PointerPressedCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(PointerPressedCommand), null, inherits: true);

    /// <summary>
    /// The pointer released command.
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> PointerReleasedCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(PointerReleasedCommand), null, inherits: true);

    /// <summary>
    /// The pointer move command.
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> PointerMoveCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(PointerMoveCommand), null, inherits: true);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> DataPointerDownCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(DataPointerDownCommand), null, inherits: true);

    /// <summary>
    /// The hovered points chaanged command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> HoveredPointsChangedCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(HoveredPointsChangedCommand), null, inherits: true);

    /// <summary>
    /// The <see cref="ChartPoint"/> pointer down command property
    /// </summary>
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public static readonly AvaloniaProperty<ICommand?> ChartPointPointerDownCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(ChartPointPointerDownCommand), null, inherits: true);

    /// <summary>
    /// The <see cref="VisualElement"/> pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> VisualElementsPointerDownCommandProperty =
        AvaloniaProperty.Register<CartesianChart, ICommand?>(nameof(VisualElementsPointerDownCommand), null, inherits: true);

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

    bool IChartView.DesignerMode => Design.IsDesignMode;

    bool IChartView.IsDarkMode => Application.Current?.ActualThemeVariant == ThemeVariant.Dark;

    /// <inheritdoc cref="IChartView.ChartTheme" />
    public Theme? ChartTheme { get => _chartTheme; set { _chartTheme = value; _core?.Update(); } }

    /// <inheritdoc cref="IChartView.CoreChart" />
    public Chart CoreChart => _core ?? throw new Exception("Core not set yet.");

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    public CoreMotionCanvas CoreCanvas => _core is null ? throw new Exception("core not found") : _core.Canvas;

    CartesianChartEngine ICartesianChartView.Core =>
        _core is null ? throw new Exception("core not found") : (CartesianChartEngine)_core;

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
        get => GetValue(SyncContextProperty)!;
        set => SetValue(SyncContextProperty, value);
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
        get => (ICollection<ISeries>)GetValue(SeriesProperty)!;
        set => SetValue(SeriesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.XAxes" />
    public ICollection<ICartesianAxis> XAxes
    {
        get => (ICollection<ICartesianAxis>)GetValue(XAxesProperty)!;
        set => SetValue(XAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.YAxes" />
    public ICollection<ICartesianAxis> YAxes
    {
        get => (ICollection<ICartesianAxis>)GetValue(YAxesProperty)!;
        set => SetValue(YAxesProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.Sections" />
    public ICollection<IChartElement> Sections
    {
        get => (ICollection<IChartElement>)GetValue(SectionsProperty)!;
        set => SetValue(SectionsProperty, value);
    }

    /// <inheritdoc cref="IChartView.VisualElements" />
    public ICollection<IChartElement> VisualElements
    {
        get => (ICollection<IChartElement>)GetValue(VisualElementsProperty)!;
        set => SetValue(VisualElementsProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.DrawMarginFrame" />
    public IChartElement? DrawMarginFrame
    {
        get => (IChartElement?)GetValue(DrawMarginFrameProperty);
        set => SetValue(DrawMarginFrameProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.ZoomMode" />
    public ZoomAndPanMode ZoomMode
    {
        get => (ZoomAndPanMode)GetValue(ZoomModeProperty)!;
        set => SetValue(ZoomModeProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.ZoomingSpeed" />
    public double ZoomingSpeed
    {
        get => (double)GetValue(ZoomingSpeedProperty)!;
        set => SetValue(ZoomingSpeedProperty, value);
    }

    /// <inheritdoc cref="IChartView.AnimationsSpeed" />
    public TimeSpan AnimationsSpeed
    {
        get => (TimeSpan)GetValue(AnimationsSpeedProperty)!;
        set => SetValue(AnimationsSpeedProperty, value);
    }

    /// <inheritdoc cref="IChartView.EasingFunction" />
    public Func<float, float>? EasingFunction
    {
        get => (Func<float, float>?)GetValue(EasingFunctionProperty);
        set => SetValue(EasingFunctionProperty, value);
    }

    /// <inheritdoc cref="IChartView.TooltipPosition" />
    public TooltipPosition TooltipPosition
    {
        get => (TooltipPosition)GetValue(TooltipPositionProperty)!;
        set => SetValue(TooltipPositionProperty, value);
    }

    /// <inheritdoc cref="IChartView.Tooltip" />
    public IChartTooltip? Tooltip { get => tooltip; set => tooltip = value; }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    public FindingStrategy FindingStrategy
    {
        get => (FindingStrategy)GetValue(FindingStrategyProperty)!;
        set => SetValue(FindingStrategyProperty, value);
    }

    /// <inheritdoc cref="ICartesianChartView.FindingStrategy" />
    [Obsolete($"Renamed to {nameof(FindingStrategy)}")]
    public TooltipFindingStrategy TooltipFindingStrategy
    {
        get => ((FindingStrategy)GetValue(FindingStrategyProperty)!).AsOld();
        set => SetValue(FindingStrategyProperty, value.AsNew());
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

    /// <inheritdoc cref="IChartView.LegendPosition" />
    public LegendPosition LegendPosition
    {
        get => (LegendPosition)GetValue(LegendPositionProperty)!;
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
    public IChartLegend? Legend { get => legend; set => legend = value; }

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
    /// Gets or sets a command to execute when the pointer is pressed on the chart.
    /// </summary>
    public ICommand? PointerPressedCommand
    {
        get => (ICommand?)GetValue(PointerPressedCommandProperty);
        set => SetValue(PointerPressedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer is released on the chart.
    /// </summary>
    public ICommand? PointerReleasedCommand
    {
        get => (ICommand?)GetValue(PointerReleasedCommandProperty);
        set => SetValue(PointerReleasedCommandProperty, value);
    }

    /// <summary>
    /// Gets or sets a command to execute when the pointer moves over the chart.
    /// </summary>
    public ICommand? PointerMoveCommand
    {
        get => (ICommand?)GetValue(PointerMoveCommandProperty);
        set => SetValue(PointerMoveCommandProperty, value);
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
    /// Gets or sets a command to execute when the pointer goes down on a visual element(s).
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

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, new(point), strategy, findPointFor));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
    {
        return _core is not CartesianChartEngine cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((VisualElement)visual).IsHitBy(cc, new(point)));
    }

    void IChartView.InvokeOnUIThread(Action action) =>
        Dispatcher.UIThread.Post(action);

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <returns></returns>
    protected void InitializeCore()
    {
        var canvas = this.FindControl<MotionCanvas>("canvas")!;

        _avaloniaCanvas = canvas;
        _core = new CartesianChartEngine(
            this, config => config.UseDefaults(), canvas.CanvasCore);

        _core.Measuring += OnCoreMeasuring;
        _core.UpdateStarted += OnCoreUpdateStarted;
        _core.UpdateFinished += OnCoreUpdateFinished;

        _core.Update();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="change"></param>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (_core is null || change.Property.Name == nameof(IsPointerOver)) return;

        OnXamlPropertyChanged(change);

        if (change.Property.Name == nameof(SyncContext))
        {
            CoreCanvas.Sync = change.NewValue ?? new object();
        }

        if (_observe.TryGetValue(change.Property.Name, out var observer))
        {
            observer.Initialize(change.NewValue);
        }

        if (change.Property.Name == nameof(ActualThemeVariant))
            _core.ApplyTheme();

        _core.Update();
    }

    private static void OnSeriesSourceChanged(CartesianChart chart, object o, object n)
    {
        var seriesObserver = (SeriesSourceObserver)chart._observe[nameof(SeriesSource)];
        seriesObserver.Initialize(chart.SeriesSource);

        if (seriesObserver.Series is not null)
            chart.Series = seriesObserver.Series;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void UpdateCore() => _core?.Update();

    private void AddUIElement(object item)
    {
        if (_avaloniaCanvas is null || item is not ILogical logical) return;
        _avaloniaCanvas.Children.Add(logical);
    }

    private void RemoveUIElement(object item)
    {
        if (_avaloniaCanvas is null || item is not ILogical logical) return;
        _ = _avaloniaCanvas.Children.Remove(logical);
    }

    private DateTime _lastPresed;
    private readonly int _tolearance = 50;
    private void CartesianChart_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.KeyModifiers > 0) return;
        var p = e.GetPosition(this);

        if (PointerPressedCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerPressedCommand.CanExecute(args)) PointerPressedCommand.Execute(args);
        }

        var isSecondary =
            e.GetCurrentPoint(this).Properties.IsRightButtonPressed ||
            (DateTime.Now - _lastPresed).TotalMilliseconds < 500;

        _core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y), isSecondary);
        _lastPresed = DateTime.Now;
    }

    private void CartesianChart_PointerMoved(object? sender, PointerEventArgs e)
    {
        if ((DateTime.Now - _lastPresed).TotalMilliseconds < _tolearance) return;
        var p = e.GetPosition(_avaloniaCanvas);

        if (PointerMoveCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerMoveCommand.CanExecute(args)) PointerMoveCommand.Execute(args);
        }

        _core?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void CartesianChart_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if ((DateTime.Now - _lastPresed).TotalMilliseconds < _tolearance) return;
        var p = e.GetPosition(this);

        if (PointerReleasedCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerReleasedCommand.CanExecute(args)) PointerReleasedCommand.Execute(args);
        }

        _core?.InvokePointerUp(new LvcPoint((float)p.X, (float)p.Y), e.GetCurrentPoint(this).Properties.IsRightButtonPressed);
    }

    private void CartesianChart_PointerLeave(object? sender, PointerEventArgs e) => _core?.InvokePointerLeft();

    private void CartesianChart_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_core is null) return;

        e.Handled = true;

        var c = (CartesianChartEngine)_core;
        var p = e.GetPosition(this);

        c.Zoom(new LvcPoint((float)p.X, (float)p.Y), e.Delta.Y > 0 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    private float _previousScale = 1;
    private bool _matchAxesScreenDataRatio;
    private Theme? _chartTheme;

    private void CartesianChart_Pinched(object? sender, PinchEventArgs e)
    {
        if (_core is null) return;

        var c = (CartesianChartEngine)_core;
        var p = e.ScaleOrigin;
        var s = c.ControlSize;
        var pivot = new LvcPoint((float)(p.X * s.Width), (float)(p.Y * s.Height));

        var scale = (float)e.Scale;
        var delta = _previousScale - scale;
        if (Math.Abs(delta) > 0.05) delta = 0; // ignore the first call.
        _previousScale = scale;

        c.Zoom(pivot, ZoomDirection.DefinedByScaleFactor, 1 - delta, true);
    }

    private void OnCoreUpdateFinished(IChartView chart) => UpdateFinished?.Invoke(this);

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

    private void CartesianChart_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e) => _core?.Load();

    private void CartesianChart_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        _observe.Dispose();
        _core?.Unload();
    }

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        if (DataPointerDownCommand is not null && DataPointerDownCommand.CanExecute(points)) DataPointerDownCommand.Execute(points);

        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
#pragma warning disable CS0618 // Type or member is obsolete
        ChartPointPointerDownCommand?.Execute(points.FindClosestTo(pointer));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    void IChartView.OnHoveredPointsChanged(IEnumerable<ChartPoint>? newPoints, IEnumerable<ChartPoint>? oldPoints)
    {
        HoveredPointsChanged?.Invoke(this, newPoints, oldPoints);

        var args = new HoverCommandArgs(this, newPoints, oldPoints);
        if (HoveredPointsChangedCommand is not null && HoveredPointsChangedCommand.CanExecute(args)) HoveredPointsChangedCommand.Execute(args);
    }

    void IChartView.OnVisualElementPointerDown(
        IEnumerable<IInteractable> visualElements, LvcPoint pointer)
    {
        var args = new VisualElementsEventArgs(CoreChart, visualElements, pointer);

        VisualElementsPointerDown?.Invoke(this, args);
        if (VisualElementsPointerDownCommand is not null && VisualElementsPointerDownCommand.CanExecute(args))
            VisualElementsPointerDownCommand.Execute(args);
    }

    void IChartView.Invalidate() => CoreCanvas.Invalidate();

    bool ICustomHitTest.HitTest(Point point) => new Rect(Bounds.Size).Contains(point);

    private ISeries InflateSeriesTemplate(object item)
    {
        var control = SeriesTemplate.Build(item);

        if (control is not ISeries series)
            throw new InvalidOperationException("The template must be a valid series.");

        control.DataContext = item;

        return series;
    }

    private static object GetSeriesSource(ISeries series) =>
        ((StyledElement)series).DataContext!;
}
