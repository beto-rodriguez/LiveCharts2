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

using LiveChartsCore.Native;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.WinUI.Rendering;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

#pragma warning disable IDE0028 // Simplify collection initialization

namespace LiveChartsCore.SkiaSharpView.WinUI;

/// <summary>
/// The motion canvas control for WinUI and Uno Platform.
/// </summary>
public partial class MotionCanvas : Canvas
{
    private readonly CanvasRenderSettings<CPURenderMode, GPURenderMode, NativeFrameTicker> _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas()
    {
#if __UNO_SKIA__ || DESKTOP
        // then force the skiarendermode.
        _settings = new(new SkiaRenderMode());
#else
        _settings = new();
#endif

        Children.Add((UIElement)_settings.RenderMode);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        SizeChanged += OnSizeChanged;
    }

    /// <inheritdoc cref="CoreMotionCanvas"/>
    public CoreMotionCanvas CanvasCore { get; } = new();

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var fe = (FrameworkElement)_settings.RenderMode;

        fe.Width = e.NewSize.Width;
        fe.Height = e.NewSize.Height;
    }

    private void OnLoaded(object sender, RoutedEventArgs e) =>
        _settings.Initialize(CanvasCore);

    private void OnUnloaded(object sender, RoutedEventArgs e) =>
        _settings.Dispose(CanvasCore);
}
