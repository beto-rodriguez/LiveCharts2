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

// ==============================================================================
// 
// this file contains the WinUI/UNO specific code for the ChartControl class,
// the rest of the code can be found in the _Shared project.
// 
// ==============================================================================

using System;
using System.Runtime.InteropServices;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Native;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <inheritdoc cref="IChartView"/>
public abstract partial class ChartControl : UserControl, IChartView
{
    private DateTime _lastPresed;
    private readonly ThemeListener _themeListener;
    private readonly PointerController _pointerController;
    private static readonly bool s_isWebAssembly = RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartControl"/> class.
    /// </summary>
    public ChartControl()
    {
        Content = new MotionCanvas();

        SizeChanged += (s, e) =>
            CoreChart.Update();

        InitializeChartControl();
        InitializeObservedProperties();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        _themeListener = new(CoreChart.ApplyTheme, DispatcherQueue);

        _pointerController = new PointerController();

        _pointerController.Pressed += OnPressed;
        _pointerController.Moved += OnMoved;
        _pointerController.Released += OnReleased;
        _pointerController.Scrolled += OnScrolled;
        _pointerController.Pinched += OnPinched;
        _pointerController.Exited += OnExited;
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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _themeListener.Listen();
        _pointerController.InitializeController(this);
        StartObserving();
        CoreChart.Load();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _themeListener.Dispose();
        _pointerController.DisposeController(this);
        StopObserving();
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

    private void OnPressed(object? sender, Native.Events.PressedEventArgs args)
    {
        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (PointerPressedCommand?.CanExecute(cArgs) == true)
            PointerPressedCommand.Execute(cArgs);

        var isSecondary = (DateTime.Now - _lastPresed).TotalMilliseconds < 500;

        CoreChart?.InvokePointerDown(args.Location, args.IsSecondaryPress || isSecondary);
        _lastPresed = DateTime.Now;
    }

    private void OnMoved(object? sender, Native.Events.ScreenEventArgs args)
    {
        var location = args.Location;

        var cArgs = new PointerCommandArgs(this, new(location.X, location.Y), args.OriginalEvent);
        if (PointerMoveCommand?.CanExecute(cArgs) == true)
            PointerMoveCommand.Execute(cArgs);

        CoreChart?.InvokePointerMove(location);
    }

    private void OnReleased(object? sender, Native.Events.PressedEventArgs args)
    {
        var cArgs = new PointerCommandArgs(this, new(args.Location.X, args.Location.Y), args);
        if (PointerReleasedCommand?.CanExecute(cArgs) == true)
            PointerReleasedCommand.Execute(cArgs);

        CoreChart?.InvokePointerUp(args.Location, args.IsSecondaryPress);
    }

    private void OnExited(object? sender, Native.Events.EventArgs args) =>
        CoreChart?.InvokePointerLeft();

    internal virtual void OnScrolled(object? sender, Native.Events.ScrollEventArgs args) { }

    internal virtual void OnPinched(object? sender, Native.Events.PinchEventArgs args) { }

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
            action();
            return;
        }

        _ = DispatcherQueue.TryEnqueue(
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () => action());
    }
}
