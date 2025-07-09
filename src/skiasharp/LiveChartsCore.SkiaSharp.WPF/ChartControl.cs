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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;

namespace LiveChartsCore.SkiaSharpView.WPF;

/// <inheritdoc cref="IChartView" />
public abstract partial class ChartControl : UserControl, IChartView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Chart"/> class.
    /// </summary>
    /// <exception cref="Exception">Default colors are not valid</exception>
    protected ChartControl()
    {
        LiveCharts.Configure(config => config.UseDefaults());

        Content = new MotionCanvas();

        InitializeCoreChart();
        InitializeObservers();

        SizeChanged += (s, e) =>
            CoreChart.Update();

        MouseDown += Chart_MouseDown;
        MouseMove += OnMouseMove;
        MouseUp += Chart_MouseUp;
        MouseLeave += OnMouseLeave;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        //    // hack hack #251201 for:
        //    // https://github.com/beto-rodriguez/LiveCharts2/issues/1383
        //    // when the chart starts with Visibility.Collapsed
        //    // the OnApplyTemplate() is not called, BUT the Loaded event is called...
        //    // this result in the core not being loaded, and the chart not updating.
        //    // so in this case, we load the core here.
        //    if (core is not null && !core.IsLoaded)
        //        core.Load();
    }

    /// <summary>
    /// Gets the canvas view.
    /// </summary>
    public MotionCanvas CanvasView => (MotionCanvas)Content;

    bool IChartView.DesignerMode => DesignerProperties.GetIsInDesignMode(this);
    bool IChartView.IsDarkMode => false;
    LvcColor IChartView.BackColor
    {
        get => Background is not SolidColorBrush b
            ? new LvcColor()
            : LvcColor.FromArgb(b.Color.A, b.Color.R, b.Color.G, b.Color.B);
        set => SetValue(BackgroundProperty, new SolidColorBrush(Color.FromArgb(value.A, value.R, value.G, value.B)));
    }
    LvcSize IChartView.ControlSize => new() { Width = (float)CanvasView.ActualWidth, Height = (float)CanvasView.ActualHeight };

    private void OnLoaded(object sender, RoutedEventArgs e) =>
        CoreChart.Load();

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Observe.Dispose();
        CoreChart?.Unload();
    }

    private void AddUIElement(object item)
    {
        if (CanvasView is null || item is not FrameworkElement view) return;
        CanvasView.AddLogicalChild(view);
    }

    private void RemoveUIElement(object item)
    {
        if (CanvasView is null || item is not FrameworkElement view) return;
        CanvasView.RemoveLogicalChild(view);
    }

    private void Chart_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (Keyboard.Modifiers > 0) return;
        _ = CaptureMouse();

        var p = e.GetPosition(this);
        var cArgs = new PointerCommandArgs(this, new(p.X, p.Y), e);
        if (PointerPressedCommand?.CanExecute(cArgs) == true)
            PointerPressedCommand.Execute(cArgs);

        CoreChart?.InvokePointerDown(new(p.X, p.Y), e.ChangedButton == MouseButton.Right);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var p = e.GetPosition(this);

        if (PointerMoveCommand is not null)
        {
            var args = new PointerCommandArgs(this, new(p.X, p.Y), e);
            if (PointerMoveCommand.CanExecute(args))
                PointerMoveCommand.Execute(args);
        }

        CoreChart?.InvokePointerMove(new LvcPoint((float)p.X, (float)p.Y));
    }

    private void Chart_MouseUp(object sender, MouseButtonEventArgs e)
    {
        var p = e.GetPosition(this);

        var cArgs = new PointerCommandArgs(this, new(p.X, p.Y), e);
        if (PointerReleasedCommand?.CanExecute(cArgs) == true)
            PointerReleasedCommand.Execute(cArgs);

        CoreChart?.InvokePointerUp(new(p.X, p.Y), e.ChangedButton == MouseButton.Right);
        ReleaseMouseCapture();
    }

    private void OnMouseLeave(object sender, MouseEventArgs e) =>
        CoreChart?.InvokePointerLeft();

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

    void IChartView.InvokeOnUIThread(Action action) =>
        Dispatcher.Invoke(action);
}
