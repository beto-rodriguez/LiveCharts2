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

using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.Motion;
using LiveChartsCore.SkiaSharpView.WPF.Rendering;
namespace LiveChartsCore.SkiaSharpView.WPF;

/// <summary>
/// Defines the motion canvas control for WPF, <see cref="CoreMotionCanvas"/>.
/// </summary>
/// <seealso cref="Control" />
public class MotionCanvas : UserControl
{
    private readonly MotionCanvasComposer _composer;

    static MotionCanvas()
    {
        _ = LiveChartsSkiaSharp
            .EnsureInitialized()
            .HasRenderingFactory(
                (settings, forceGPU) =>
                {
                    IRenderMode renderMode = forceGPU || settings.UseGPU
                        ? new GPURenderMode()
                        : new CPURenderMode();

                    IFrameTicker ticker = settings.TryUseVSync
                        ? new CompositionTargetTicker()
                        : new AsyncLoopTicker();

                    return new MotionCanvasComposer(renderMode, ticker);
                });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MotionCanvas"/> class.
    /// </summary>
    public MotionCanvas(bool forceGPU)
    {
        _composer = LiveChartsSkiaSharp.MotionCanvasRenderingFactory(LiveCharts.RenderingSettings, forceGPU);

        Content = _composer.RenderMode;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    /// <inheritdoc cref="CoreMotionCanvas"/>
    public CoreMotionCanvas CanvasCore { get; } = new();

    internal void AddLogicalChild(DependencyObject child) =>
        base.AddLogicalChild(child);

    internal void RemoveLogicalChild(DependencyObject child) =>
        base.RemoveLogicalChild(child);

    private void OnLoaded(object sender, RoutedEventArgs e) =>
        _composer.Initialize(CanvasCore);

    private void OnUnloaded(object sender, RoutedEventArgs e) =>
        _composer.Dispose(CanvasCore);
}
