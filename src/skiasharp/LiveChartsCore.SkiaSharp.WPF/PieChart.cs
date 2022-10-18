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
using System.Windows;
using System.Windows.Input;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView.Drawing;

namespace LiveChartsCore.SkiaSharpView.WPF;

/// <inheritdoc cref="IPieChartView{TDrawingContext}" />
public class PieChart : Chart, IPieChartView<SkiaSharpDrawingContext>
{
    private readonly CollectionDeepObserver<ISeries> _seriesObserver;
    private readonly CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>> _visualsObserver;

    static PieChart()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PieChart), new FrameworkPropertyMetadata(typeof(PieChart)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    public PieChart()
    {
        _seriesObserver = new CollectionDeepObserver<ISeries>(
            (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                if (core is null ||(sender is ChartElement<SkiaSharpDrawingContext> stop && stop._isInternalSet)) return;
                core.Update();
            },
            (object? sender, PropertyChangedEventArgs e) =>
            {
                if (core is null ||(sender is ChartElement<SkiaSharpDrawingContext> stop && stop._isInternalSet)) return;
                core.Update();
            }, true);
        _visualsObserver = new CollectionDeepObserver<ChartElement<SkiaSharpDrawingContext>>(
            (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                if (core is null ||(sender is ChartElement<SkiaSharpDrawingContext> stop && stop._isInternalSet)) return;
                core.Update();
            },
            (object? sender, PropertyChangedEventArgs e) =>
            {
                if (core is null ||(sender is ChartElement<SkiaSharpDrawingContext> stop && stop._isInternalSet)) return;
                core.Update();
            }, true);

        SetCurrentValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetCurrentValue(VisualElementsProperty, new ObservableCollection<ChartElement<SkiaSharpDrawingContext>>());
        MouseDown += OnMouseDown;
    }

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
                    seriesObserver?.Dispose((IEnumerable<ISeries>)args.OldValue);
                    seriesObserver?.Initialize((IEnumerable<ISeries>)args.NewValue);
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ISeries> ? value : new ObservableCollection<ISeries>();
                }));

    /// <summary>
    /// The visual elements property
    /// </summary>
    public static readonly DependencyProperty VisualElementsProperty =
        DependencyProperty.Register(
            nameof(VisualElements), typeof(IEnumerable<ChartElement<SkiaSharpDrawingContext>>), typeof(PieChart), new PropertyMetadata(null,
                (DependencyObject o, DependencyPropertyChangedEventArgs args) =>
                {
                    var chart = (PieChart)o;
                    var observer = chart._visualsObserver;
                    observer?.Dispose((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)args.OldValue);
                    observer?.Initialize((IEnumerable<ChartElement<SkiaSharpDrawingContext>>)args.NewValue);
                    if (chart.core is null) return;
                    chart.core.Update();
                },
                (DependencyObject o, object value) =>
                {
                    return value is IEnumerable<ChartElement<SkiaSharpDrawingContext>>
                    ? value
                    : new List<ChartElement<SkiaSharpDrawingContext>>();
                }));

    /// <summary>
    /// The isClockwise property
    /// </summary>
    public static readonly DependencyProperty IsClockwiseProperty =
        DependencyProperty.Register(
            nameof(IsClockwise), typeof(bool), typeof(PieChart), new PropertyMetadata(true, OnDependencyPropertyChanged));

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

    PieChart<SkiaSharpDrawingContext> IPieChartView<SkiaSharpDrawingContext>.Core => core is null ? throw new Exception("core not found") : (PieChart<SkiaSharpDrawingContext>)core;

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.Series" />
    public IEnumerable<ISeries> Series
    {
        get => (IEnumerable<ISeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    /// <inheritdoc cref="IPieChartView{TDrawingContext}.VisualElements" />
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

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <exception cref="Exception">canvas not found</exception>
    protected override void InitializeCore()
    {
        if (canvas is null) throw new Exception("canvas not found");
        core = new PieChart<SkiaSharpDrawingContext>(this, LiveChartsSkiaSharp.DefaultPlatformBuilder, canvas.CanvasCore);
        legend = Template.FindName("legend", this) as IChartLegend<SkiaSharpDrawingContext>;
        tooltip = Template.FindName("tooltip", this) as IChartTooltip<SkiaSharpDrawingContext>;
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnUnloaded"/>
    protected override void OnUnloaded()
    {
        core?.Unload();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var p = e.GetPosition(this);
        core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y), e.ChangedButton == MouseButton.Right);
    }
}
