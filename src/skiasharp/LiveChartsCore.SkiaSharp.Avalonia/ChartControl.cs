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
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Styling;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.Avalonia;

/// <inheritdoc cref="ICartesianChartView" />
public abstract partial class ChartControl : UserControl, IChartView, ICustomHitTest
{
    private DateTime _lastPresed;
    private readonly int _tolearance = 50;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartControl"/> class.
    /// </summary>
    protected ChartControl()
    {
        LiveCharts.Configure(config => config.UseDefaults());

        Content = new MotionCanvas();
        CoreChart = CreateCoreChart();

        CoreChart.Measuring += OnCoreMeasuring;
        CoreChart.UpdateStarted += OnCoreUpdateStarted;
        CoreChart.UpdateFinished += OnCoreUpdateFinished;

        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;

        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
        PointerExited += OnPointerLeave;

        SizeChanged += (s, e) =>
            CoreChart.Update();

        Observe = new ChartObserver(() => CoreChart?.Update(), AddUIElement, RemoveUIElement);
    }

    /// <summary>
    /// Gets the canvas view.
    /// </summary>
    public MotionCanvas CanvasView => (MotionCanvas)Content!;

    /// <summary>
    /// Gets the core chart.
    /// </summary>
    public Chart CoreChart { get; }

    /// <summary>
    /// Gets the chart observer.
    /// </summary>
    protected ChartObserver Observe { get; }

    bool IChartView.DesignerMode => Design.IsDesignMode;
    bool IChartView.IsDarkMode => Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
    LvcColor IChartView.BackColor
    {
        get => Background is not ISolidColorBrush b
                ? new LvcColor()
                : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        set => Background = new SolidColorBrush(new Color(value.R, value.G, value.B, value.A));
    }
    LvcSize IChartView.ControlSize => new() { Width = (float)CanvasView.Bounds.Width, Height = (float)CanvasView.Bounds.Height };

    /// <inheritdoc cref="IChartView.Tooltip" />
    public IChartTooltip? Tooltip { get; set; }

    /// <inheritdoc cref="IChartView.Legend" />
    public IChartLegend? Legend { get => field; set { field = value; CoreChart.Update(); } }

    /// <inheritdoc cref="IChartView.ChartTheme" />
    public Theme? ChartTheme { get; set { field = value; OnChartPropertyChanged(this); } }

    /// <inheritdoc cref="IChartView.CoreCanvas" />
    public CoreMotionCanvas CoreCanvas => CanvasView.CanvasCore;

    /// <inheritdoc cref="IChartView.Measuring" />
    public event ChartEventHandler? Measuring;

    /// <inheritdoc cref="IChartView.UpdateFinished" />
    public event ChartEventHandler? UpdateFinished;

    /// <inheritdoc cref="IChartView.UpdateStarted" />
    public event ChartEventHandler? UpdateStarted;

    /// <inheritdoc cref="IChartView.DataPointerDown" />
    public event ChartPointsHandler? DataPointerDown;

    /// <inheritdoc cref="IChartView.HoveredPointsChanged" />
    public event ChartPointHoverHandler? HoveredPointsChanged;

    /// <inheritdoc cref="IChartView.ChartPointPointerDown" />
    [Obsolete($"Use the {nameof(DataPointerDown)} event instead with a {nameof(FindingStrategy)} that used TakeClosest.")]
    public event ChartPointHandler? ChartPointPointerDown;

    /// <inheritdoc cref="IChartView.VisualElementsPointerDown"/>
    public event VisualElementsHandler? VisualElementsPointerDown;

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e) =>
        CoreChart?.Load();

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Observe.Dispose();
        CoreChart?.Unload();
    }

    /// <summary>
    /// Creates the core chart instance for rendering and manipulation.
    /// </summary>
    /// <remarks>This method is abstract and must be implemented by derived classes to provide     a specific
    /// chart type. The returned <see cref="Chart"/> object represents the     foundational chart structure, which can
    /// be further customized or populated     with data.</remarks>
    /// <returns>A <see cref="Chart"/> object that serves as the base chart instance.</returns>
    protected abstract Chart CreateCoreChart();

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public IEnumerable<ChartPoint> GetPointsAt(
        LvcPointD point,
        FindingStrategy strategy = FindingStrategy.Automatic,
        FindPointFor findPointFor = FindPointFor.HoverEvent)
    {
        if (strategy == FindingStrategy.Automatic)
            strategy = CoreChart.Series.GetFindingStrategy();

        return CoreChart.Series.SelectMany(series =>
            series.FindHitPoints(CoreChart, new(point), strategy, FindPointFor.HoverEvent));
    }

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point) =>
        CoreChart.VisualElements.SelectMany(visual =>
            ((VisualElement)visual).IsHitBy(CoreChart, new(point)));

    void IChartView.InvokeOnUIThread(Action action) =>
        Dispatcher.UIThread.Post(action);

    private void OnCoreMeasuring(IChartView chart) => Measuring?.Invoke(this);

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

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.KeyModifiers > 0) return;
        var p = e.GetPosition(this);

        if (PointerPressedCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerPressedCommand.CanExecute(args))
                PointerPressedCommand.Execute(args);
        }

        var isSecondary =
            e.GetCurrentPoint(this).Properties.IsRightButtonPressed ||
            (DateTime.Now - _lastPresed).TotalMilliseconds < 500;

        CoreChart?.InvokePointerDown(new LvcPoint((float)p.X, (float)p.Y), isSecondary);
        _lastPresed = DateTime.Now;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if ((DateTime.Now - _lastPresed).TotalMilliseconds < _tolearance) return;
        var p = e.GetPosition(CanvasView);

        if (PointerMoveCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerMoveCommand.CanExecute(args))
                PointerMoveCommand.Execute(args);
        }

        CoreChart?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if ((DateTime.Now - _lastPresed).TotalMilliseconds < _tolearance) return;
        var p = e.GetPosition(this);

        if (PointerReleasedCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerReleasedCommand.CanExecute(args))
                PointerReleasedCommand.Execute(args);
        }

        CoreChart?.InvokePointerUp(new LvcPoint((float)p.X, (float)p.Y), e.GetCurrentPoint(this).Properties.IsRightButtonPressed);
    }

    private void OnPointerLeave(object? sender, PointerEventArgs e) =>
        CoreChart?.InvokePointerLeft();

    private void AddUIElement(object item)
    {
        if (CanvasView is null || item is not ILogical logical) return;
        CanvasView.Children.Add(logical);
    }

    private void RemoveUIElement(object item)
    {
        if (CanvasView is null || item is not ILogical logical) return;
        _ = CanvasView.Children.Remove(logical);
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
}
