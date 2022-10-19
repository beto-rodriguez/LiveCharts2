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
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="IPieChartView{TDrawingContext}" />
public class PieChart : UserControl, IPieChartView<SkiaSharpDrawingContext>, IAvaloniaChart
{
    #region fields

    /// <summary>
    /// The legend
    /// </summary>
    protected IChartLegend<SkiaSharpDrawingContext>? legend;

    /// <summary>
    /// The tooltip
    /// </summary>
    protected IChartTooltip<SkiaSharpDrawingContext>? tooltip;

    private Chart<SkiaSharpDrawingContext>? _core;
    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;
    private MotionCanvas? _avaloniaCanvas;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    public PieChart()
    {
        InitializeComponent();

        // workaround to detect mouse events.
        // Avalonia do not seem to detect events if background is not set.
        Background = new SolidColorBrush(Colors.Transparent);

        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
        var initializer = stylesBuilder.GetVisualsInitializer();
        if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        initializer.ApplyStyleToChart(this);

        InitializeCore();

        AttachedToVisualTree += OnAttachedToVisualTree;

        _seriesObserver = new CollectionDeepObserver<ISeries>(
           (object? sender, NotifyCollectionChangedEventArgs e) =>
           {
               if (_core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
               _core.Update();
           },
           (object? sender, PropertyChangedEventArgs e) =>
           {
               if (_core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
               _core.Update();
           }, true);
        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
          (object? sender, NotifyCollectionChangedEventArgs e) =>
          {
              if (_core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
              _core.Update();
          },
          (object? sender, PropertyChangedEventArgs e) =>
          {
              if (_core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
              _core.Update();
          }, true);

        Series = new ObservableCollection<ISeries>();
        VisualElements = new ObservableCollection<ChartElement<SkiaSharpDrawingContext>>();
        PointerLeave += Chart_PointerLeave;

        PointerMoved += Chart_PointerMoved;
        PointerPressed += Chart_PointerPressed;
        DetachedFromVisualTree += PieChart_DetachedFromVisualTree;
    }

    #region avalonia/dependency properties

    /// <summary>
    /// The draw margin property
    /// </summary>
    public static readonly AvaloniaProperty<Margin?> DrawMarginProperty =
       AvaloniaProperty.Register<PieChart, Margin?>(nameof(DrawMargin), null, inherits: true);

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly AvaloniaProperty<object> SyncContextProperty =
       AvaloniaProperty.Register<PieChart, object>(nameof(SyncContext), new object(), inherits: true);

    /// <summary>
    /// The title property.
    /// </summary>
    public static readonly AvaloniaProperty<VisualElement<SkiaSharpDrawingContext>?> TitleProperty =
       AvaloniaProperty.Register<PieChart, VisualElement<SkiaSharpDrawingContext>?>(nameof(Title), null, inherits: true);

    /// <summary>
    /// The series property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ISeries>> SeriesProperty =
        AvaloniaProperty.Register<PieChart, IEnumerable<ISeries>>(nameof(Series), new List<ISeries>(), inherits: true);

    /// <summary>
    /// The visual elements property
    /// </summary>
    public static readonly AvaloniaProperty<IEnumerable<ChartElement<SkiaSharpDrawingContext>>> VisualElementsProperty =
        AvaloniaProperty.Register<PieChart, IEnumerable<ChartElement<SkiaSharpDrawingContext>>>(
            nameof(VisualElements), Enumerable.Empty<ChartElement<SkiaSharpDrawingContext>>(), inherits: true);

    /// <summary>
    /// The IsClockwise property
    /// </summary>
    public static readonly AvaloniaProperty<bool> IsClockwiseProperty =
        AvaloniaProperty.Register<PieChart, bool>(nameof(IsClockwise), true, inherits: true);

    /// <summary>
    /// The initial rotation property
    /// </summary>
    public static readonly AvaloniaProperty<double> InitialRotationProperty =
        AvaloniaProperty.Register<PieChart, double>(nameof(InitialRotation), 0d, inherits: true);

    /// <summary>
    /// The maximum angle property
    /// </summary>
    public static readonly AvaloniaProperty<double> MaxAngleProperty =
        AvaloniaProperty.Register<PieChart, double>(nameof(MaxAngle), 360d, inherits: true);

    /// <summary>
    /// The total property
    /// </summary>
    public static readonly AvaloniaProperty<double?> TotalProperty =
        AvaloniaProperty.Register<PieChart, double?>(nameof(Total), null, inherits: true);

    /// <summary>
    /// The animations speed property
    /// </summary>
    public static readonly AvaloniaProperty<TimeSpan> AnimationsSpeedProperty =
        AvaloniaProperty.Register<PieChart, TimeSpan>(
            nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultAnimationsSpeed, inherits: true);

    /// <summary>
    /// The easing function property
    /// </summary>
    public static readonly AvaloniaProperty<Func<float, float>> EasingFunctionProperty =
        AvaloniaProperty.Register<PieChart, Func<float, float>>(
            nameof(AnimationsSpeed), LiveCharts.CurrentSettings.DefaultEasingFunction, inherits: true);

    /// <summary>
    /// The tool tip template property
    /// </summary>
    public static readonly AvaloniaProperty<DataTemplate?> TooltipTemplateProperty =
        AvaloniaProperty.Register<PieChart, DataTemplate?>(nameof(TooltipTemplate), null, inherits: true);

    /// <summary>
    /// The tool tip position property
    /// </summary>
    public static readonly AvaloniaProperty<TooltipPosition> TooltipPositionProperty =
        AvaloniaProperty.Register<PieChart, TooltipPosition>(
            nameof(TooltipPosition), LiveCharts.CurrentSettings.DefaultTooltipPosition, inherits: true);

    /// <summary>
    /// The tool tip font family property
    /// </summary>
    public static readonly AvaloniaProperty<FontFamily> TooltipFontFamilyProperty =
        AvaloniaProperty.Register<PieChart, FontFamily>(
            nameof(TooltipFontFamily), new FontFamily("Arial"), inherits: true);

    /// <summary>
    /// The tool tip font size property
    /// </summary>
    public static readonly AvaloniaProperty<double> TooltipFontSizeProperty =
        AvaloniaProperty.Register<PieChart, double>(nameof(TooltipFontSize), 13d, inherits: true);

    /// <summary>
    /// The tool tip font weight property
    /// </summary>
    public static readonly AvaloniaProperty<FontWeight> TooltipFontWeightProperty =
        AvaloniaProperty.Register<PieChart, FontWeight>(nameof(TooltipFontWeight), FontWeight.Normal, inherits: true);

    /// <summary>
    /// The tool tip font style property
    /// </summary>
    public static readonly AvaloniaProperty<FontStyle> TooltipFontStyleProperty =
        AvaloniaProperty.Register<PieChart, FontStyle>(
            nameof(TooltipFontStyle), FontStyle.Normal, inherits: true);

    /// <summary>
    /// The tool tip text brush property
    /// </summary>
    public static readonly AvaloniaProperty<SolidColorBrush> TooltipTextBrushProperty =
        AvaloniaProperty.Register<PieChart, SolidColorBrush>(
            nameof(TooltipTextBrush), new SolidColorBrush(new Color(255, 35, 35, 35)), inherits: true);

    /// <summary>
    /// The tool tip background property
    /// </summary>
    public static readonly AvaloniaProperty<IBrush> TooltipBackgroundProperty =
        AvaloniaProperty.Register<PieChart, IBrush>(nameof(TooltipBackground),
            new SolidColorBrush(new Color(255, 250, 250, 250)), inherits: true);

    /// <summary>
    /// The legend position property
    /// </summary>
    public static readonly AvaloniaProperty<LegendPosition> LegendPositionProperty =
        AvaloniaProperty.Register<PieChart, LegendPosition>(
            nameof(LegendPosition), LiveCharts.CurrentSettings.DefaultLegendPosition, inherits: true);

    /// <summary>
    /// The legend orientation property
    /// </summary>
    public static readonly AvaloniaProperty<LegendOrientation> LegendOrientationProperty =
        AvaloniaProperty.Register<PieChart, LegendOrientation>(
            nameof(LegendOrientation), LiveCharts.CurrentSettings.DefaultLegendOrientation, inherits: true);

    /// <summary>
    /// The legend template property
    /// </summary>
    public static readonly AvaloniaProperty<DataTemplate?> LegendTemplateProperty =
        AvaloniaProperty.Register<PieChart, DataTemplate?>(nameof(LegendTemplate), null, inherits: true);

    /// <summary>
    /// The legend font family property
    /// </summary>
    public static readonly AvaloniaProperty<FontFamily> LegendFontFamilyProperty =
       AvaloniaProperty.Register<PieChart, FontFamily>(
           nameof(LegendFontFamily), new FontFamily("Arial"), inherits: true);

    /// <summary>
    /// The legend font size property
    /// </summary>
    public static readonly AvaloniaProperty<double> LegendFontSizeProperty =
        AvaloniaProperty.Register<PieChart, double>(nameof(LegendFontSize), 13d, inherits: true);

    /// <summary>
    /// The legend font weight property
    /// </summary>
    public static readonly AvaloniaProperty<FontWeight> LegendFontWeightProperty =
        AvaloniaProperty.Register<PieChart, FontWeight>(nameof(LegendFontWeight), FontWeight.Normal, inherits: true);

    /// <summary>
    /// The legend font style property
    /// </summary>
    public static readonly AvaloniaProperty<FontStyle> LegendFontStyleProperty =
        AvaloniaProperty.Register<PieChart, FontStyle>(
            nameof(LegendFontStyle), FontStyle.Normal, inherits: true);

    /// <summary>
    /// The legend text brush property
    /// </summary>
    public static readonly AvaloniaProperty<SolidColorBrush> LegendTextBrushProperty =
        AvaloniaProperty.Register<PieChart, SolidColorBrush>(
            nameof(LegendTextBrush), new SolidColorBrush(new Color(255, 35, 35, 35)), inherits: true);

    /// <summary>
    /// The legend background property
    /// </summary>
    public static readonly AvaloniaProperty<IBrush> LegendBackgroundProperty =
        AvaloniaProperty.Register<PieChart, IBrush>(nameof(LegendBackground),
            new SolidColorBrush(new Color(255, 255, 255, 255)), inherits: true);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> DataPointerDownCommandProperty =
        AvaloniaProperty.Register<PieChart, ICommand?>(nameof(DataPointerDownCommand), null, inherits: true);

    /// <summary>
    /// The chart point pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> ChartPointPointerDownCommandProperty =
        AvaloniaProperty.Register<PieChart, ICommand?>(nameof(ChartPointPointerDownCommand), null, inherits: true);

    /// <summary>
    /// The <see cref="VisualElement{TDrawingContext}"/> pointer down command property
    /// </summary>
    public static readonly AvaloniaProperty<ICommand?> VisualElementsPointerDownCommandProperty =
        AvaloniaProperty.Register<PieChart, ICommand?>(nameof(VisualElementsPointerDownCommand), null, inherits: true);

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

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElementsPointerDown"/>
    public event VisualElementHandler<SkiaSharpDrawingContext>? VisualElementsPointerDown;

    #endregion

    #region properties

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => Design.IsDesignMode;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => _core ?? throw new Exception("Core not set yet.");

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

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => _core is null ? throw new Exception("core not found") : _core.Canvas;

    PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core => _core is null ? throw new Exception("core not found") : (PieChart<SkiaSharpDrawingContext>)_core;

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin
    {
        get => (Margin?)GetValue(DrawMarginProperty);
        set => SetValue(DrawMarginProperty, value);
    }

    /// <inheritdoc cref="IChartView{SkiaSharpDrawingContext}.Title" />
    public VisualElement<SkiaSharpDrawingContext>? Title
    {
        get => (VisualElement<SkiaSharpDrawingContext>)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Series" />
    public IEnumerable<ISeries> Series
    {
        get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.VisualElements" />
    public IEnumerable<ChartElement<SkiaSharpDrawingContext>> VisualElements
    {
        get => (IEnumerable<ChartElement<SkiaSharpDrawingContext>>)GetValue(VisualElementsProperty);
        set => SetValue(VisualElementsProperty, value);
    }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.IsClockwise" />
    public bool IsClockwise
    {
        get => (bool)GetValue(IsClockwiseProperty);
        set => SetValue(IsClockwiseProperty, value);
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

    /// <summary>
    /// Gets or sets the tool data tip template.
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

    /// <summary>
    /// Gets or sets the default tool tip text brush.
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
    /// Gets or sets the default tool tip background.
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

    /// <summary>
    /// Gets or sets the default legend text brush.
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
    /// Gets or sets the default legend background.
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

    /// <summary>
    /// Gets or sets a command to execute when the pointer goes down on a visual element(s).
    /// </summary>
    public ICommand? VisualElementsPointerDownCommand
    {
        get => (ICommand?)GetValue(VisualElementsPointerDownCommandProperty);
        set => SetValue(VisualElementsPointerDownCommandProperty, value);
    }

    #endregion

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
        _avaloniaCanvas = canvas;
        _core = new PieChart<SkiaSharpDrawingContext>(
            this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);

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

        if (_core is null) return;

        if (change.Property.Name == nameof(SyncContext))
        {
            CoreCanvas.Sync = change.NewValue;
        }

        if (change.Property.Name == nameof(Series))
        {
            _seriesObserver?.Dispose((IEnumerable<ISeries>)change.OldValue.Value);
            _seriesObserver?.Initialize((IEnumerable<ISeries>)change.NewValue.Value);
            return;
        }

        if (change.Property.Name == nameof(VisualElements))
        {
            _visualsObserver?.Dispose((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)change.OldValue.Value);
            _visualsObserver?.Initialize((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)change.NewValue.Value);
            return;
        }

        _core.Update();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Chart_PointerMoved(object? sender, PointerEventArgs e)
    {
        var p = e.GetPosition(_avaloniaCanvas);
        _core?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void Chart_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        var p = e.GetPosition(this);
        _core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y), false);
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

    private void Chart_PointerLeave(object? sender, PointerEventArgs e)
    {
        _ = Dispatcher.UIThread.InvokeAsync(HideTooltip, DispatcherPriority.Background);
        _core?.InvokePointerLeft();
    }

    private void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
    {
        _core?.Load();
    }

    private void PieChart_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
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

    void IChartView<SkiaSharpDrawingContext>.OnVisualElementPointerDown(
        IEnumerable<VisualElement<SkiaSharpDrawingContext>> visualElements, LvcPoint pointer)
    {
        VisualElementsPointerDown?.Invoke(this, visualElements);
        if (VisualElementsPointerDownCommand is not null && VisualElementsPointerDownCommand.CanExecute(visualElements))
            VisualElementsPointerDownCommand.Execute(visualElements);
    }

    void IChartView.Invalidate()
    {
        CoreCanvas.Invalidate();
    }
}
