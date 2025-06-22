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
using System.Windows;
using System.Windows.Input;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.WPF;

/// <inheritdoc cref="IPieChartView" />
public class PieChart : Chart, IPieChartView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PieChart"/> class.
    /// </summary>
    public PieChart()
    {
        _ = Observe
            .Collection(nameof(Series));

        SetCurrentValue(SeriesProperty, new ObservableCollection<ISeries>());
        SetCurrentValue(VisualElementsProperty, new ObservableCollection<IChartElement>());
        SetCurrentValue(SyncContextProperty, new object());

        MouseDown += OnMouseDown;
    }

    /// <summary>
    /// The series property
    /// </summary>
    public static readonly DependencyProperty SeriesProperty =
        DependencyProperty.Register(
            nameof(Series), typeof(IEnumerable<ISeries>), typeof(PieChart),
            new PropertyMetadata(null, InitializeObserver(nameof(Series))));

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
    public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register(
            nameof(MaxValue), typeof(double), typeof(PieChart), new PropertyMetadata(double.NaN, OnDependencyPropertyChanged));

    /// <summary>
    /// The start property
    /// </summary>
    public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register(
            nameof(MinValue), typeof(double), typeof(PieChart), new PropertyMetadata(0d, OnDependencyPropertyChanged));

    PieChartEngine IPieChartView.Core => core is null ? throw new Exception("core not found") : (PieChartEngine)core;

    /// <inheritdoc cref="IPieChartView.Series" />
    public override ICollection<ISeries> Series
    {
        get => (ICollection<ISeries>)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.IsClockwise" />
    public bool IsClockwise
    {
        get => (bool)GetValue(IsClockwiseProperty);
        set => SetValue(IsClockwiseProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.InitialRotation" />
    public double InitialRotation
    {
        get => (double)GetValue(InitialRotationProperty);
        set => SetValue(InitialRotationProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.MaxAngle" />
    public double MaxAngle
    {
        get => (double)GetValue(MaxAngleProperty);
        set => SetValue(MaxAngleProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.MaxValue" />
    public double MaxValue
    {
        get => (double)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    /// <inheritdoc cref="IPieChartView.MinValue" />
    public double MinValue
    {
        get => (double)GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public override IEnumerable<ChartPoint> GetPointsAt(LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
    {
        if (core is not PieChartEngine cc) throw new Exception("core not found");

        if (strategy == FindingStrategy.Automatic)
            strategy = cc.Series.GetFindingStrategy();

        return cc.Series.SelectMany(series => series.FindHitPoints(cc, new(point), strategy, findPointFor));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public override IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
    {
        return core is not PieChartEngine cc
            ? throw new Exception("core not found")
            : cc.VisualElements.SelectMany(visual => ((VisualElement)visual).IsHitBy(core, new(point)));
    }

    /// <summary>
    /// Initializes the core.
    /// </summary>
    /// <exception cref="Exception">canvas not found</exception>
    protected override void InitializeCore()
    {
        if (canvas is null) throw new Exception("canvas not found");
        core = new PieChartEngine(this, config => config.UseDefaults(), canvas.CanvasCore);
        core.Update();
    }

    /// <inheritdoc cref="Chart.OnUnloaded"/>
    protected override void OnUnloaded()
    {
        Observe.Dispose();
        core?.Unload();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var p = e.GetPosition(this);
        core?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y), e.ChangedButton == MouseButton.Right);
    }
}
