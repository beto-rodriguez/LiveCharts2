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
using System.Threading.Tasks;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.Drawing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SkiaSharp.Views.Windows;

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <summary>
/// The motion canvas control for winui.
/// </summary>
/// <seealso cref="UserControl" />
/// <seealso cref="Microsoft.UI.Xaml.Markup.IComponentConnector" />
public class MotionCanvas : Canvas
{
    private readonly SKXamlCanvas? _skiaElement;
    private bool _isDrawingLoopRunning;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        _skiaElement = new();
        Children.Add(_skiaElement);
        SetLeft(_skiaElement, 0);
        SetTop(_skiaElement, 0);

        SizeChanged += OnSizeChanged;
        _skiaElement.PaintSurface += OnPaintSurface;
    }

    /// <summary>
    /// Gets the canvas core.
    /// </summary>
    /// <value>
    /// The canvas core.
    /// </value>
    public CoreMotionCanvas CanvasCore { get; } = new();

    private void OnLoaded(object sender, RoutedEventArgs e) =>
        CanvasCore.Invalidated += OnCanvasCoreInvalidated;

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
    {
#if HAS_UNO_WINUI
        var scale = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().LogicalDpi / 96.0f;
        args.Surface.Canvas.Scale((float)scale, (float)scale);
        CanvasCore.DrawFrame(
            new SkiaSharpDrawingContext(CanvasCore, args.Info, args.Surface, args.Surface.Canvas));
#else
        var scaleFactor = XamlRoot.RasterizationScale;
        args.Surface.Canvas.Scale((float)scaleFactor, (float)scaleFactor);
        CanvasCore.DrawFrame(new
            SkiaSharpDrawingContext(CanvasCore, args.Info, args.Surface, args.Surface.Canvas));
#endif
    }

    private async void RunDrawingLoop()
    {
        if (_isDrawingLoopRunning || _skiaElement == null) return;
        _isDrawingLoopRunning = true;

        var ts = TimeSpan.FromSeconds(1 / LiveCharts.MaxFps);

        while (!CanvasCore.IsValid)
        {
            _skiaElement?.Invalidate();
            await Task.Delay(ts);
        }

        _isDrawingLoopRunning = false;
    }

    private void OnCanvasCoreInvalidated(CoreMotionCanvas sender) =>
        RunDrawingLoop();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_skiaElement == null) return;
        _skiaElement.Width = e.NewSize.Width;
        _skiaElement.Height = e.NewSize.Height;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        SizeChanged -= OnSizeChanged;
        CanvasCore.Invalidated -= OnCanvasCoreInvalidated;
        CanvasCore.Dispose();
    }
}
