﻿// The MIT License(MIT)
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
using System.Windows.Input;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using SkiaSharp.Views.Maui;

namespace LiveChartsCore.SkiaSharpView.Maui;

/// <inheritdoc cref="IPolarChartView{TDrawingContext}"/>
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class PolarChart : ContentView, IPolarChartView<SkiaSharpDrawingContext>, IMauiChart
{
    #region fields

    /// <summary>
    /// The core
    /// </summary>
    protected Chart<SkiaSharpDrawingContext>? core;

    private CollectionDeepObserver<ISeries> _seriesObserver;
    private CollectionDeepObserver<IPolarAxis> _angleObserver;
    private CollectionDeepObserver<IPolarAxis> _radiusObserver;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="PolarChart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    public PolarChart()
    {
        InitializeComponent();

        if (!LiveCharts.IsConfigured) LiveCharts.Configure(LiveChartsSkiaSharp.DefaultPlatformBuilder);

        var stylesBuilder = LiveCharts.CurrentSettings.GetTheme<SkiaSharpDrawingContext>();
        var initializer = stylesBuilder.GetVisualsInitializer();
        if (stylesBuilder.CurrentColors is null || stylesBuilder.CurrentColors.Length == 0)
            throw new Exception("Default colors are not valid");
        initializer.ApplyStyleToChart(this);

        InitializeCore();
        SizeChanged += OnSizeChanged;

        _seriesObserver = new CollectionDeepObserver<ISeries>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _angleObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);
        _radiusObserver = new CollectionDeepObserver<IPolarAxis>(OnDeepCollectionChanged, OnDeepCollectionPropertyChanged, true);

        AngleAxes = new List<IPolarAxis>()
            {
                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
            };
        RadiusAxes = new List<IPolarAxis>()
            {
                LiveCharts.CurrentSettings.GetProvider<SkiaSharpDrawingContext>().GetDefaultPolarAxis()
            };
        Series = new ObservableCollection<ISeries>();

        canvas.SkCanvasView.EnableTouchEvents = true;
        canvas.SkCanvasView.Touch += OnSkCanvasTouched;

        if (core is null) throw new Exception("Core not found!");
        core.Measuring += OnCoreMeasuring;
        core.UpdateStarted += OnCoreUpdateStarted;
        core.UpdateFinished += OnCoreUpdateFinished;
    }

    #region bindable properties 

    /// <summary>
    /// The sync context property.
    /// </summary>
    public static readonly BindableProperty SyncContextProperty =
        BindableProperty.Create(
            nameof(SyncContext), typeof(object), typeof(PolarChart), new ObservableCollection<ISeries>(), BindingMode.Default, null,
            (BindableObject o, object oldValue, object newValue) =>
            {
                var chart = (PolarChart)o;
                chart.CoreCanvas.Sync = newValue;
                if (chart.core is null) return;
                chart.core.Update();
            });

    /// <summary>
    /// The fit to bounds property.
    /// </summary>
    public static readonly BindableProperty FitToBoundsProperty =
       BindableProperty.Create(nameof(FitToBounds), typeof(bool), typeof(PolarChart), false,
           propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The total angle property.
    /// </summary>
    public static readonly BindableProperty TotalAngleProperty =
       BindableProperty.Create(nameof(TotalAngle), typeof(double), typeof(PolarChart), 360d,
           propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The Inner radius property.
    /// </summary>
    public static readonly BindableProperty InnerRadiusProperty =
       BindableProperty.Create(nameof(InnerRadius), typeof(double), typeof(PolarChart), 0d,
           propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The Initial rotation property.
    /// </summary>
    public static readonly BindableProperty InitialRotationProperty =
       BindableProperty.Create(nameof(InitialRotation), typeof(double), typeof(PolarChart), LiveCharts.CurrentSettings.PolarInitialRotation,
           propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The series property.
    /// </summary>
    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(
            nameof(Series), typeof(IEnumerable<ISeries>), typeof(PolarChart), new ObservableCollection<ISeries>(), BindingMode.Default, null,
            (BindableObject o, object oldValue, object newValue) =>
            {
                var chart = (PolarChart)o;
                var seriesObserver = chart._seriesObserver;
                seriesObserver?.Dispose((IEnumerable<ISeries>)oldValue);
                seriesObserver?.Initialize((IEnumerable<ISeries>)newValue);
                if (chart.core is null) return;
                chart.core.Update();
            });

    /// <summary>
    /// The x axes property.
    /// </summary>
    public static readonly BindableProperty AngleAxesProperty =
        BindableProperty.Create(
            nameof(AngleAxes), typeof(IEnumerable<IPolarAxis>), typeof(PolarChart), new List<IPolarAxis>() { new PolarAxis() },
            BindingMode.Default, null, (BindableObject o, object oldValue, object newValue) =>
            {
                var chart = (PolarChart)o;
                var observer = chart._angleObserver;
                observer?.Dispose((IEnumerable<IPolarAxis>)oldValue);
                observer?.Initialize((IEnumerable<IPolarAxis>)newValue);
                if (chart.core is null) return;
                chart.core.Update();
            });

    /// <summary>
    /// The y axes property.
    /// </summary>
    public static readonly BindableProperty RadiusAxesProperty =
        BindableProperty.Create(
            nameof(RadiusAxes), typeof(IEnumerable<IPolarAxis>), typeof(PolarChart), new List<IPolarAxis>() { new PolarAxis() },
            BindingMode.Default, null, (BindableObject o, object oldValue, object newValue) =>
            {
                var chart = (PolarChart)o;
                var observer = chart._radiusObserver;
                observer?.Dispose((IEnumerable<IPolarAxis>)oldValue);
                observer?.Initialize((IEnumerable<IPolarAxis>)newValue);
                if (chart.core is null) return;
                chart.core.Update();
            });

    /// <summary>
    /// The animations speed property.
    /// </summary>
    public static readonly BindableProperty AnimationsSpeedProperty =
       BindableProperty.Create(
           nameof(AnimationsSpeed), typeof(TimeSpan), typeof(PolarChart), LiveCharts.CurrentSettings.DefaultAnimationsSpeed);

    /// <summary>
    /// The easing function property.
    /// </summary>
    public static readonly BindableProperty EasingFunctionProperty =
        BindableProperty.Create(
            nameof(EasingFunction), typeof(Func<float, float>), typeof(PolarChart),
            LiveCharts.CurrentSettings.DefaultEasingFunction);

    /// <summary>
    /// The legend position property.
    /// </summary>
    public static readonly BindableProperty LegendPositionProperty =
        BindableProperty.Create(
            nameof(LegendPosition), typeof(LegendPosition), typeof(PolarChart),
            LiveCharts.CurrentSettings.DefaultLegendPosition, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend orientation property.
    /// </summary>
    public static readonly BindableProperty LegendOrientationProperty =
        BindableProperty.Create(
            nameof(LegendOrientation), typeof(LegendOrientation), typeof(PolarChart),
            LiveCharts.CurrentSettings.DefaultLegendOrientation, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend template property.
    /// </summary>
    public static readonly BindableProperty LegendTemplateProperty =
        BindableProperty.Create(
            nameof(LegendTemplate), typeof(DataTemplate), typeof(PolarChart), null, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend font family property.
    /// </summary>
    public static readonly BindableProperty LegendFontFamilyProperty =
        BindableProperty.Create(
            nameof(LegendFontFamily), typeof(string), typeof(PolarChart), null, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend font size property.
    /// </summary>
    public static readonly BindableProperty LegendFontSizeProperty =
        BindableProperty.Create(
            nameof(LegendFontSize), typeof(double), typeof(PolarChart), 13d, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend text color property.
    /// </summary>
    public static readonly BindableProperty LegendTextBrushProperty =
        BindableProperty.Create(
            nameof(LegendTextBrush), typeof(Color), typeof(PolarChart),
            Color.FromRgb(35 / 255d, 35 / 255d, 35 / 255d), propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend background property.
    /// </summary>
    public static readonly BindableProperty LegendBackgroundProperty =
        BindableProperty.Create(
            nameof(LegendBackground), typeof(Color), typeof(PolarChart),
            Color.FromRgb(255 / 255d, 255 / 255d, 255 / 255d), propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The legend font attributes property.
    /// </summary>
    public static readonly BindableProperty LegendFontAttributesProperty =
        BindableProperty.Create(
            nameof(LegendFontAttributes), typeof(FontAttributes), typeof(PolarChart),
            FontAttributes.None, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tool tip position property.
    /// </summary>
    public static readonly BindableProperty TooltipPositionProperty =
       BindableProperty.Create(
           nameof(TooltipPosition), typeof(TooltipPosition), typeof(PolarChart),
           LiveCharts.CurrentSettings.DefaultTooltipPosition, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The too ltip finding strategy property.
    /// </summary>
    public static readonly BindableProperty TooltipFindingStrategyProperty =
        BindableProperty.Create(
            nameof(TooltipFindingStrategy), typeof(TooltipFindingStrategy), typeof(PolarChart),
            LiveCharts.CurrentSettings.DefaultTooltipFindingStrategy);

    /// <summary>
    /// The tool tip template property.
    /// </summary>
    public static readonly BindableProperty TooltipTemplateProperty =
        BindableProperty.Create(
            nameof(TooltipTemplate), typeof(DataTemplate), typeof(PolarChart), null, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tool tip font family property.
    /// </summary>
    public static readonly BindableProperty TooltipFontFamilyProperty =
        BindableProperty.Create(
            nameof(TooltipFontFamily), typeof(string), typeof(PolarChart), null, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tool tip font size property.
    /// </summary>
    public static readonly BindableProperty TooltipFontSizeProperty =
        BindableProperty.Create(
            nameof(TooltipFontSize), typeof(double), typeof(PolarChart), 13d, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tool tip text color property.
    /// </summary>
    public static readonly BindableProperty TooltipTextBrushProperty =
        BindableProperty.Create(
            nameof(TooltipTextBrush), typeof(Color), typeof(PolarChart),
            Color.FromRgb(35 / 255d, 35 / 255d, 35 / 255d), propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tool tip background property.
    /// </summary>
    public static readonly BindableProperty TooltipBackgroundProperty =
        BindableProperty.Create(
            nameof(TooltipBackground), typeof(Color), typeof(PolarChart),
            Color.FromRgb(250 / 255d, 250 / 255d, 250 / 255d), propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The tool tip font attributes property.
    /// </summary>
    public static readonly BindableProperty TooltipFontAttributesProperty =
        BindableProperty.Create(
            nameof(TooltipFontAttributes), typeof(FontAttributes), typeof(PolarChart),
            FontAttributes.None, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The data pointer down command property
    /// </summary>
    public static readonly BindableProperty DataPointerDownCommandProperty =
        BindableProperty.Create(
            nameof(DataPointerDownCommand), typeof(ICommand), typeof(PolarChart),
            null, propertyChanged: OnBindablePropertyChanged);

    /// <summary>
    /// The chart point pointer down command property
    /// </summary>
    public static readonly BindableProperty ChartPointPointerDownCommandProperty =
        BindableProperty.Create(
            nameof(ChartPointPointerDownCommand), typeof(ICommand), typeof(PolarChart),
            null, propertyChanged: OnBindablePropertyChanged);

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

    /// <summary>
    /// Called when the chart is touched.
    /// </summary>
    public event EventHandler<SKTouchEventArgs>? Touched;

    #endregion

    #region properties

    Grid IMauiChart.LayoutGrid => grid;
    BindableObject IMauiChart.Canvas => canvas;
    BindableObject IMauiChart.Legend => legend;

    /// <inheritdoc cref="IChartView.DesignerMode" />
    bool IChartView.DesignerMode => DesignMode.IsDesignModeEnabled;

    /// <inheritdoc cref="IChartView.CoreChart" />
    public IChart CoreChart => core ?? throw new Exception("Core not set yet.");

    LvcColor IChartView.BackColor
    {
        get => Background is not SolidColorBrush b
            ? new LvcColor()
            : LvcColor.FromArgb(
                (byte)(b.Color.Alpha * 255), (byte)(b.Color.Red * 255), (byte)(b.Color.Green * 255), (byte)(b.Color.Blue * 255));
        set => Background = new SolidColorBrush(Color.FromRgba(value.R / 255, value.G / 255, value.B / 255, value.A / 255));
    }

    PolarChart<SkiaSharpDrawingContext> IPolarChartView<SkiaSharpDrawingContext>.Core
        => core is null ? throw new Exception("core not found") : (PolarChart<SkiaSharpDrawingContext>)core;

    LvcSize IChartView.ControlSize => new()
    {
        Width = (float)(canvas.Width * DeviceDisplay.MainDisplayInfo.Density),
        Height = (float)(canvas.Height * DeviceDisplay.MainDisplayInfo.Density)
    };

    /// <inheritdoc cref="IChartView{TDrawingContext}.CoreCanvas" />
    public MotionCanvas<SkiaSharpDrawingContext> CoreCanvas => canvas.CanvasCore;

    /// <inheritdoc cref="IChartView.SyncContext" />
    public object SyncContext
    {
        get => GetValue(SyncContextProperty);
        set => SetValue(SyncContextProperty, value);
    }

    /// <inheritdoc cref="IChartView.DrawMargin" />
    public Margin? DrawMargin
    {
        get => null;
        set => throw new NotImplementedException();
    }

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

    /// <inheritdoc cref="IChartView.EasingFunction" />
    public Func<float, float>? EasingFunction
    {
        get => (Func<float, float>)GetValue(EasingFunctionProperty);
        set => SetValue(EasingFunctionProperty, value);
    }

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
    public string LegendFontFamily
    {
        get => (string)GetValue(LegendFontFamilyProperty);
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
    /// Gets or sets the default color of the legend text.
    /// </summary>
    /// <value>
    /// The color of the legend text.
    /// </value>
    public Color LegendTextBrush
    {
        get => (Color)GetValue(LegendTextBrushProperty);
        set => SetValue(LegendTextBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the default color of the legend background.
    /// </summary>
    /// <value>
    /// The color of the legend background.
    /// </value>
    public Color LegendBackground
    {
        get => (Color)GetValue(LegendBackgroundProperty);
        set => SetValue(LegendBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the default legend font attributes.
    /// </summary>
    /// <value>
    /// The legend font attributes.
    /// </value>
    public FontAttributes LegendFontAttributes
    {
        get => (FontAttributes)GetValue(LegendFontAttributesProperty);
        set => SetValue(LegendFontAttributesProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Legend" />
    public IChartLegend<SkiaSharpDrawingContext>? Legend => legend;

    /// <inheritdoc cref="IChartView.TooltipPosition" />
    public TooltipPosition TooltipPosition
    {
        get => (TooltipPosition)GetValue(TooltipPositionProperty);
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
    /// Gets or sets the default tool tip font family.
    /// </summary>
    /// <value>
    /// The tool tip font family.
    /// </value>
    public string TooltipFontFamily
    {
        get => (string)GetValue(TooltipFontFamilyProperty);
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
    /// Gets or sets the default color of the tool tip text.
    /// </summary>
    /// <value>
    /// The color of the tool tip text.
    /// </value>
    public Color TooltipTextBrush
    {
        get => (Color)GetValue(TooltipTextBrushProperty);
        set => SetValue(TooltipTextBrushProperty, value);
    }

    /// <summary>
    /// Gets or sets the default color of the tool tip background.
    /// </summary>
    /// <value>
    /// The color of the tool tip background.
    /// </value>
    public Color TooltipBackground
    {
        get => (Color)GetValue(TooltipBackgroundProperty);
        set => SetValue(TooltipBackgroundProperty, value);
    }

    /// <summary>
    /// Gets or sets the default tool tip font attributes.
    /// </summary>
    /// <value>
    /// The tool tip font attributes.
    /// </value>
    public FontAttributes TooltipFontAttributes
    {
        get => (FontAttributes)GetValue(TooltipFontAttributesProperty);
        set => SetValue(TooltipFontAttributesProperty, value);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.Tooltip" />
    public IChartTooltip<SkiaSharpDrawingContext>? Tooltip => tooltip;

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

    /// <inheritdoc cref="IPolarChartView{TDrawingContext}.ScaleUIPoint(LvcPoint, int, int)" />
    public double[] ScaleUIPoint(LvcPoint point, int xAxisIndex = 0, int yAxisIndex = 0)
    {
        if (core is null) throw new Exception("core not found");
        var cartesianCore = (PolarChart<SkiaSharpDrawingContext>)core;
        return cartesianCore.ScaleUIPoint(point, xAxisIndex, yAxisIndex);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.ShowTooltip(IEnumerable{ChartPoint})"/>
    public void ShowTooltip(IEnumerable<ChartPoint> points)
    {
        if (tooltip is null || core is null) return;

        ((IChartTooltip<SkiaSharpDrawingContext>)tooltip).Show(points, core);
    }

    /// <inheritdoc cref="IChartView{TDrawingContext}.HideTooltip"/>
    public void HideTooltip()
    {
        if (tooltip is null || core is null) return;

        core.ClearTooltipData();
        ((IChartTooltip<SkiaSharpDrawingContext>)tooltip).Hide();
    }

    /// <inheritdoc cref="IMauiChart.GetCanvasPosition" />
    LvcPoint IMauiChart.GetCanvasPosition()
    {
        return new LvcPoint((float)canvas.X, (float)canvas.Y);
    }

    /// <inheritdoc cref="IChartView.SetTooltipStyle(LvcColor, LvcColor)"/>
    public void SetTooltipStyle(LvcColor background, LvcColor textColor)
    {
        TooltipBackground = Color.FromRgba(background.R, background.G, background.B, background.A);
        TooltipTextBrush = Color.FromRgba(textColor.R, textColor.G, textColor.B, textColor.A);
    }

    void IChartView.InvokeOnUIThread(Action action)
    {
        _ = MainThread.InvokeOnMainThreadAsync(action);
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <returns></returns>
    protected void InitializeCore()
    {
        core = new PolarChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
        core.Update();
    }

    /// <summary>
    /// Called when a bindable property changed.
    /// </summary>
    /// <param name="o">The o.</param>
    /// <param name="oldValue">The old value.</param>
    /// <param name="newValue">The new value.</param>
    /// <returns></returns>
    protected static void OnBindablePropertyChanged(BindableObject o, object oldValue, object newValue)
    {
        var chart = (PolarChart)o;
        if (chart.core is null) return;
        chart.core.Update();
    }

    /// <inheritdoc cref="NavigableElement.OnParentSet"/>
    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent == null)
        {
            core?.Unload();
            return;
        }

        core?.Load();
    }

    private void OnDeepCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
        core.Update();
    }

    private void OnDeepCollectionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (core is null || (sender is IStopNPC stop && !stop.IsNotifyingChanges)) return;
        core.Update();
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (core is null) return;
        core.Update();
    }

    private void PanGestureRecognizer_PanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        //if (core is null) return;
        //if (e.StatusType != GestureStatus.Running) return;

        //var c = (PolarChart<SkiaSharpDrawingContext>)core;
        //var delta = new PointF((float)e.TotalX, (float)e.TotalY);
        //var args = new PanGestureEventArgs(delta);
        //c.InvokePanGestrue(args);
        //if (!args.Handled) c.Pan(delta);
    }

    private void PinchGestureRecognizer_PinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        //if (e.Status != GestureStatus.Running || Math.Abs(e.Scale - 1) < 0.05 || core is null) return;

        //var c = (PolarChart<SkiaSharpDrawingContext>)core;
        //var p = e.ScaleOrigin;
        //var s = c.ControlSize;

        //c.Zoom(
        //    new PointF((float)(p.X * s.Width), (float)(p.Y * s.Height)),
        //    e.Scale > 1 ? ZoomDirection.ZoomIn : ZoomDirection.ZoomOut);
    }

    private void OnSkCanvasTouched(object? sender, SKTouchEventArgs e)
    {
        if (core is null) return;

        var location = new LvcPoint(e.Location.X, e.Location.Y);
        core.InvokePointerDown(location);
        core.InvokePointerMove(location);

        Touched?.Invoke(this, e);
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
