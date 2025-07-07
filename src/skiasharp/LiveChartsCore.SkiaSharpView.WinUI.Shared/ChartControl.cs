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
using System.Runtime.InteropServices;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Observers;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.Motion;
using LiveChartsCore.Themes;
using LiveChartsCore.VisualElements;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <inheritdoc cref="IChartView"/>
public abstract partial class ChartControl : UserControl, IChartView
{
    private readonly ThemeListener _themeListener;
    private static bool _isWebAssembly = RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartControl"/> class.
    /// </summary>
    public ChartControl()
    {
        LiveCharts.Configure(config => config.UseDefaults());

        Content = new MotionCanvas();
        CoreChart = CreateCoreChart();

        CoreChart.Measuring += OnCoreMeasuring;
        CoreChart.UpdateStarted += OnCoreUpdateStarted;
        CoreChart.UpdateFinished += OnCoreUpdateFinished;

        SizeChanged += (s, e) =>
            CoreChart.Update();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        SizeChanged += (s, e) =>
        CoreChart.Update();

        _themeListener = new(CoreChart.ApplyTheme, DispatcherQueue);

        // We use the behaviours assembly to share support for Uno
        var chartBehaviour = new ChartBehaviour();

        chartBehaviour.Pressed += OnPressed;
        chartBehaviour.Moved += OnMoved;
        chartBehaviour.Released += OnReleased;
        chartBehaviour.Scrolled += OnScrolled;
        chartBehaviour.Pinched += OnPinched;
        chartBehaviour.Exited += OnExited;

        chartBehaviour.On(this);

        Observe = new ChartObserver(() => CoreChart?.Update(), AddUIElement, RemoveUIElement)
            .Collection(nameof(Series))
            .Collection(nameof(VisualElements))
            .Property(nameof(Title));

        Observe.Add(
             nameof(SeriesSource),
             new SeriesSourceObserver(
                 InflateSeriesTemplate,
                 GetSeriesSource,
                 () => SeriesSource is not null && SeriesTemplate is not null));
    }

    /// <summary>
    /// Gets the canvas view.
    /// </summary>
    public MotionCanvas CanvasView => (MotionCanvas)Content;

    /// <summary>
    /// Gets the core chart.
    /// </summary>
    public Chart CoreChart { get; }

    /// <summary>
    /// Gets the chart observer.
    /// </summary>
    protected ChartObserver Observe { get; }

    bool IChartView.DesignerMode => false;
    bool IChartView.IsDarkMode => Application.Current?.RequestedTheme == ApplicationTheme.Dark;
    LvcColor IChartView.BackColor
    {
        get => Background is not SolidColorBrush b
            ? new LvcColor()
            : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        set => SetValue(BackgroundProperty, new SolidColorBrush(Windows.UI.Color.FromArgb(value.A, value.R, value.G, value.B)));
    }
    LvcSize IChartView.ControlSize => new() { Width = (float)CanvasView.ActualWidth, Height = (float)CanvasView.ActualHeight };

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

    /// <summary>
    /// Creates the core chart instance for rendering and manipulation.
    /// </summary>
    /// <remarks>This method is abstract and must be implemented by derived classes to provide     a specific
    /// chart type. The returned <see cref="Chart"/> object represents the     foundational chart structure, which can
    /// be further customized or populated     with data.</remarks>
    /// <returns>A <see cref="Chart"/> object that serves as the base chart instance.</returns>
    protected abstract Chart CreateCoreChart();

    private void OnCoreUpdateFinished(IChartView chart) =>
        UpdateFinished?.Invoke(this);

    private void OnCoreUpdateStarted(IChartView chart)
    {
        if (UpdateStartedCommand is not null)
        {
            var args = new ChartCommandArgs(this);
            if (UpdateStartedCommand.CanExecute(args))
                UpdateStartedCommand.Execute(args);
        }

        UpdateStarted?.Invoke(this);
    }

    private void OnCoreMeasuring(IChartView chart) =>
        Measuring?.Invoke(this);

    private void OnLoaded(object sender, RoutedEventArgs e) => CoreChart.Load();

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _themeListener.Dispose();
        Observe.Dispose();
        CoreChart.Unload();
    }

    private void AddUIElement(object item)
    {
        if (CanvasView is null || item is not UIElement uiElement) return;
        CanvasView.Children.Add(uiElement);
    }

    private void RemoveUIElement(object item)
    {
        if (CanvasView is null || item is not UIElement uiElement) return;
        _ = CanvasView.Children.Remove(uiElement);
    }

    private void OnPressed(object? sender, Behaviours.Events.PressedEventArgs args)
    {
        // is this working on all platforms?
        //if (args.KeyModifiers > 0) return;

        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (PointerPressedCommand?.CanExecute(cArgs) == true)
            PointerPressedCommand.Execute(cArgs);

        CoreChart?.InvokePointerDown(args.Location, args.IsSecondaryPress);
    }

    private void OnMoved(object? sender, Behaviours.Events.ScreenEventArgs args)
    {
        var location = args.Location;

        var cArgs = new PointerCommandArgs(this, new(location.X, location.Y), args.OriginalEvent);
        if (PointerMoveCommand?.CanExecute(cArgs) == true)
            PointerMoveCommand.Execute(cArgs);

        CoreChart?.InvokePointerMove(location);
    }

    private void OnReleased(object? sender, Behaviours.Events.PressedEventArgs args)
    {
        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (PointerReleasedCommand?.CanExecute(cArgs) == true)
            PointerReleasedCommand.Execute(cArgs);

        CoreChart?.InvokePointerUp(args.Location, args.IsSecondaryPress);
    }

    private void OnExited(object? sender, Behaviours.Events.EventArgs args) =>
        CoreChart?.InvokePointerLeft();

    /// <summary>
    /// Invoked when a scroll event occurs, allowing derived classes to handle or respond to the event.
    /// </summary>
    /// <remarks>This method is designed to be overridden in derived classes to provide custom handling for
    /// scroll events. If not overridden, the base implementation does nothing.</remarks>
    /// <param name="sender">The source of the scroll event. This can be <see langword="null"/> if the event is not associated with a
    /// specific sender.</param>
    /// <param name="args">The event data containing details about the scroll action, such as scroll direction and position.</param>
    protected virtual void OnScrolled(object? sender, Behaviours.Events.ScrollEventArgs args) { }

    /// <summary>
    /// Handles the pinch gesture event, allowing zooming functionality in the chart.
    /// </summary>
    /// <remarks>This method is invoked when a pinch gesture is detected. It enables zooming in or out of the chart
    /// based on the scroll delta provided in the event arguments. Override this method in a derived class to customize the
    /// behavior of pinch gestures.</remarks>
    /// <param name="sender">The source of the event. This can be <see langword="null"/>.</param>
    /// <param name="args">The event data containing information about the pinch gesture, including its location and scroll delta.</param>
    protected virtual void OnPinched(object? sender, Behaviours.Events.PinchEventArgs args) { }

    /// <inheritdoc cref="IChartView.GetPointsAt(LvcPointD, FindingStrategy, FindPointFor)"/>
    public IEnumerable<ChartPoint> GetPointsAt(
        LvcPointD point, FindingStrategy strategy = FindingStrategy.Automatic, FindPointFor findPointFor = FindPointFor.HoverEvent)
            => CoreChart.GetPointsAt(point, strategy, findPointFor);

    /// <inheritdoc cref="IChartView.GetVisualsAt(LvcPointD)"/>
    public IEnumerable<IChartElement> GetVisualsAt(LvcPointD point)
        => CoreChart.GetVisualsAt(point);

    private ISeries InflateSeriesTemplate(object item)
    {
        var content = (FrameworkElement)SeriesTemplate.LoadContent();

        if (content is not ISeries series)
            throw new InvalidOperationException("The template must be a valid series.");

        content.DataContext = item;

        return series;
    }

    private static object GetSeriesSource(ISeries series) =>
        ((FrameworkElement)series).DataContext!;

    void IChartView.InvokeOnUIThread(Action action)
    {
        if (_isWebAssembly)
        {
            // IF UNO WASM, just run the action directly.
            // is this required on wasm isnt this already implemented in net 9?
            action();
            return;
        }

        _ = DispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () => action());
    }

    void IChartView.Invalidate() => CoreCanvas.Invalidate();

    void IChartView.OnDataPointerDown(IEnumerable<ChartPoint> points, LvcPoint pointer)
    {
        DataPointerDown?.Invoke(this, points);
        if (DataPointerDownCommand is not null && DataPointerDownCommand.CanExecute(points))
            DataPointerDownCommand.Execute(points);

        ChartPointPointerDown?.Invoke(this, points.FindClosestTo(pointer));
#pragma warning disable CS0618 // Type or member is obsolete
        ChartPointPointerDownCommand?.Execute(points.FindClosestTo(pointer));
#pragma warning restore CS0618 // Type or member is obsolete
    }

    void IChartView.OnHoveredPointsChanged(IEnumerable<ChartPoint>? newPoints, IEnumerable<ChartPoint>? oldPoints)
    {
        HoveredPointsChanged?.Invoke(this, newPoints, oldPoints);

        var args = new HoverCommandArgs(this, newPoints, oldPoints);
        if (HoveredPointsChangedCommand is not null && HoveredPointsChangedCommand.CanExecute(args))
            HoveredPointsChangedCommand.Execute(args);
    }

    void IChartView.OnVisualElementPointerDown(
        IEnumerable<IInteractable> visualElements, LvcPoint pointer)
    {
        var args = new VisualElementsEventArgs(CoreChart, visualElements, pointer);

        VisualElementsPointerDown?.Invoke(this, args);
        if (VisualElementsPointerDownCommand is not null && VisualElementsPointerDownCommand.CanExecute(args))
            VisualElementsPointerDownCommand.Execute(args);
    }
}
