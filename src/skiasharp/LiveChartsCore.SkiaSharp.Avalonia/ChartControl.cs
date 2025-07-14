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
// this file contains the Avalonia specific code for the ChartControl class,
// the rest of the code can be found in the _Shared project.
// 
// ==============================================================================

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;
using Avalonia.Styling;
using Avalonia.Threading;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel.Events;
using LiveChartsCore.Kernel.Sketches;

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

        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;

        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;
        PointerExited += OnPointerLeave;

        SizeChanged += (s, e) =>
            CoreChart.Update();

        InitializeChartControl();
        InitializeObservedProperties();
    }

    /// <inheritdoc cref="IChartView.CoreCanvas"/>
    public MotionCanvas CanvasView => (MotionCanvas)Content!;

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

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        StartObserving();
        CoreChart?.Load();
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        StopObserving();
        CoreChart?.Unload();
    }

    void IChartView.InvokeOnUIThread(Action action) =>
        Dispatcher.UIThread.Post(action);

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

    private ISeries InflateSeriesTemplate(object item)
    {
        var control = SeriesTemplate.Build(item);

        if (control is not ISeries series)
            throw new InvalidOperationException("The template must be a valid series.");

        control.DataContext = item;

        return series;
    }

    private static object GetSeriesSource(ISeries series) =>
        ((Control)series).DataContext!;

    bool ICustomHitTest.HitTest(Point point) =>
        new Rect(Bounds.Size).Contains(point);
}
