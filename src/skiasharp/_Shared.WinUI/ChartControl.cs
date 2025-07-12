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
using System.Runtime.InteropServices;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <inheritdoc cref="IChartView"/>
public abstract partial class ChartControl : UserControl, IChartView
{
    private readonly ThemeListener _themeListener;
    private readonly ChartBehaviour _chartBehaviour;
    private static readonly bool s_isWebAssembly = RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartControl"/> class.
    /// </summary>
    public ChartControl()
    {
        LiveCharts.Configure(config => config.UseDefaults());

        Content = new MotionCanvas();

        InitializeCoreChart();
        InitializeObservers();

        SizeChanged += (s, e) =>
            CoreChart.Update();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        SizeChanged += (s, e) =>
            CoreChart.Update();

        _themeListener = new(CoreChart.ApplyTheme, DispatcherQueue);

        _chartBehaviour = new ChartBehaviour();

        _chartBehaviour.Pressed += OnPressed;
        _chartBehaviour.Moved += OnMoved;
        _chartBehaviour.Released += OnReleased;
        _chartBehaviour.Scrolled += OnScrolled;
        _chartBehaviour.Pinched += OnPinched;
        _chartBehaviour.Exited += OnExited;

        _chartBehaviour.On(this);
    }

    /// <summary>
    /// Gets the canvas view.
    /// </summary>
    public MotionCanvas CanvasView => (MotionCanvas)Content;

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

    private void OnLoaded(object sender, RoutedEventArgs e) =>
        CoreChart.Load();

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _themeListener.Dispose();
        _chartBehaviour.Off(this);
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

    private ISeries InflateSeriesTemplate(object item)
    {
        var content = (FrameworkElement?)SeriesTemplate.LoadContent();

        if (content is not ISeries series)
            throw new InvalidOperationException("The template must be a valid series.");

        content.DataContext = item;

        return series;
    }

    private static object GetSeriesSource(ISeries series) =>
        ((FrameworkElement)series).DataContext!;

    void IChartView.InvokeOnUIThread(Action action)
    {
        if (s_isWebAssembly)
        {
            // IF UNO WASM, just run the action directly.
            // is this required on wasm isnt this already implemented in net 9?
            action();
            return;
        }

        _ = DispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () => action());
    }
}
